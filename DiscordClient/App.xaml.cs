using Discord.Views;
using DiscordClient;
using DiscordClient.Views;
using DiscordCommon;
using Microsoft.QueryStringDotNET;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Discord
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(deviceFamilyVersion);
            WindowsBuildVersion = (version & 0x00000000FFFF0000L) >> 16;

            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;

            if (WindowsBuildVersion >= 14393)
            {
                this.EnteredBackground += App_EnteredBackground;
                this.LeavingBackground += App_LeavingBackground;
            }
        }

        public ulong WindowsBuildVersion { get; private set; }
        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame != null)
                {
                    if (GnarlyClient.Instance.Connected)
                    {
                        rootFrame.Navigate(typeof(MainPage));
                    }
                    else
                    {
                        if (TryConnect())
                        {
                            rootFrame.Navigate(typeof(MainPage));
                        }
                        else
                        {
                            rootFrame.Navigate(typeof(LoginSelection));
                        }
                    }
                }
                else
                {
                    throw new NullReferenceException("The current window content is invalid when attempting crash recovery.");
                }

                e.Handled = true;
            }
            catch (Exception exception)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Global Unhandled Exception triggered");
                sb.AppendLine(exception.StackTrace);
                ExceptionUploader.UploadException(exception.Message, sb.ToString());
                CoreApplication.Exit();
            }
        }

        public static void LogUnhandledError(Exception e)
        {
            ExceptionUploader.UploadException(e.Message, e.StackTrace);

            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
            {
                rootFrame.Navigate(typeof(CatastrophicError));
            }
            else // window not activated yet so exit out.
            {
                CoreApplication.Exit();
            }
        }

        public static void TryGracefulRelaunch()
        {
            try
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame != null)
                {
                    if (GnarlyClient.Instance.Connected)
                    {
                        rootFrame.Navigate(typeof(MainPage));
                    }
                    else
                    {
                        if (TryConnect())
                        {
                            rootFrame.Navigate(typeof(MainPage));
                        }
                        else
                        {
                            rootFrame.Navigate(typeof(LoginSelection));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionUploader.UploadException(exception.Message, exception.StackTrace);
                CoreApplication.Exit();
            }
        }

        public const string LicenseAgreementFile = "AgreedToLicense.txt";

        private async void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            //UnregisterTask(BackgroundClientTask);
            try
            {
                if (_appPreviouslyRunning)
                {

                    if (GnarlyClient.Instance.Connected == false && TryConnect() == false)
                    {
                        Frame rootFrame = Window.Current.Content as Frame;
                        var selection = rootFrame.Content as LoginSelection;
                        var userPassword = rootFrame.Content as Login;
                        var token = rootFrame.Content as TokenLogin;

                        if (selection == null &&
                            userPassword == null &&
                            token == null
                            )
                        {
                            rootFrame.Navigate(typeof(LoginSelection));
                        }
                    }

                    var frame = Window.Current.Content as Frame;
                    var mainWindow = frame.Content as MainPage;
                    if (mainWindow != null && mainWindow.ActiveChannel != null)
                    {
                        var newMessages = await mainWindow.ActiveChannel.DownloadMessages(MainPage.MaxMessageDownloadCount);
                        var unreadMessages = DiffMessages(_cachedMessages, newMessages);
                        mainWindow.ProcessMessages(unreadMessages.Reverse().ToArray());
                    }

                }
            }
            catch (TaskCanceledException)
            {
                TryGracefulRelaunch();
            }
            catch (OperationCanceledException)
            {
                TryGracefulRelaunch();
            }
            catch (Exception exception)
            {
                LogUnhandledError(exception);
            }
        }

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            try
            {
                //GNARLY_TODO: Save the visual state.            
                var frame = Window.Current.Content as Frame;
                var mainWindow = frame.Content as MainPage;
                if (mainWindow != null && mainWindow.ActiveChannel != null)
                {
                    _cachedMessages = await mainWindow.ActiveChannel.DownloadMessages(MainPage.MaxMessageDownloadCount);
                }
                _appPreviouslyRunning = true;
                GnarlyClient.Instance.Disconnect();
            }
            catch (TaskCanceledException)
            {
                CoreApplication.Exit();
            }
            catch (OperationCanceledException)
            {
                CoreApplication.Exit();
            }
            catch (HttpRequestException)
            {
                CoreApplication.Exit();
            }
            catch (Exception exception)
            {
                ExceptionUploader.UploadException(exception.Message, exception.StackTrace);
            }
            // RegisterTask(BackgroundClientTask, BackgroundClientTaskEntryPoint);
        }

        public static readonly string AppName = "Discord Client";
        public const string TokenPassword = "E3CA2C47-96DD-4ABB-BDAB-F3F653FC8213";

        protected async override void OnActivated(IActivatedEventArgs e)
        {
            try
            {
                // Get the root frame
                Frame rootFrame = Window.Current.Content as Frame;

                if (rootFrame == null)
                {
                    rootFrame = await Launch(false, e.PreviousExecutionState, string.Empty);
                }

                // Handle toast activation
                if (e is ToastNotificationActivatedEventArgs)
                {
                    var toastActivationArgs = e as ToastNotificationActivatedEventArgs;

                    // Parse the query string
                    QueryString args = QueryString.Parse(toastActivationArgs.Argument);
                    if (args.Count() > 0)
                    {
                        // See what action is being requested 
                        switch (args[ToastAssistant.LaunchAction])
                        {
                            // Open the image
                            case ToastAssistant.GoToTrello:
                                {
                                    Windows.System.Launcher.LaunchUriAsync(new Uri("https://trello.com/b/HgHREOjb"));
                                }
                                break;
                            case ToastAssistant.GoToDonationPage:
                                {
                                    if (rootFrame != null)
                                    {
                                        rootFrame.Navigate(typeof(TipPage));
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LogUnhandledError(exception);
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            try
            {
                await Launch(e.PrelaunchActivated, e.PreviousExecutionState, e.Arguments);
            }
            catch (TaskCanceledException)
            {
                TryGracefulRelaunch();
            }
            catch (OperationCanceledException)
            {
                TryGracefulRelaunch();
            }
            catch (Exception exception)
            {
                LogUnhandledError(exception);
            }
        }

        private async Task<Frame> Launch(bool prelaunchActivated, ApplicationExecutionState perviousExecutionState, string arguments)
        {
#if DELETE_PRIVACY_FILE
            DeletePrivacyPolicyFile();
#endif
            Frame rootFrame = Window.Current.Content as Frame;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                statusBar.BackgroundOpacity = 1.0;
                var color = Windows.UI.Color.FromArgb(255, 31, 31, 31);
                statusBar.BackgroundColor = color;
            }



            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (perviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                    _appPreviouslyRunning = true;
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

                SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested; ;
            }

            if (prelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    if (WindowsBuildVersion < 14393)
                    {
                        rootFrame.Navigate(typeof(UnsupportedOperatingSystem));
                    }
                    else
                    {
                        // When the navigation stack isn't restored navigate to the first page,
                        // configuring the new page by passing required information as a navigation
                        // parameter
                        if (GnarlyClient.Instance.Connected == false && TryConnect() == false)
                        {
                            rootFrame.Navigate(typeof(LoginSelection));
                        }
                        else
                        {
                            rootFrame.Navigate(typeof(MainPage), arguments);

                        }
                    }
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
            return rootFrame;
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            try
            {
                Frame rootFrame = Window.Current.Content as Frame;

                if (rootFrame.CurrentSourcePageType == typeof(MainPage)
                    || rootFrame.CurrentSourcePageType == typeof(CatastrophicError)
                    || rootFrame.CanGoBack == false)
                {
                    // ignore the event. We want the default system behavior
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                    rootFrame.GoBack();
                }
            }
            catch (Exception exception)
            {
                LogUnhandledError(exception);
            }
        }


        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            LogUnhandledError(new Exception("Failed to load Page " + e.SourcePageType.FullName));
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            try
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                //TODO: Save application state and stop any background activity
                deferral.Complete();
            }
            catch (Exception exception)
            {
                LogUnhandledError(exception);
            }
        }

        private static bool TryConnect()
        {
            try
            {
                //VersionFixer();
                PasswordVault valut = new PasswordVault();
#if FORCE_NEW_USER
            foreach (var c in valut.RetrieveAll())
            {
                valut.Remove(c);
            }
#endif
                var usernameCredentials = valut.RetrieveAll().FirstOrDefault();


                PasswordCredential passwordCredential = null;
                if (usernameCredentials != null)
                {
                    passwordCredential = valut.Retrieve(AppName, usernameCredentials.UserName);
                }

                if (passwordCredential != null)
                {
                    string dummy = string.Empty;
                    //This is pretty ugly but if any user uses this guid as a password I would be impressed.
                    if (passwordCredential.Password == TokenPassword)
                    {
                        GnarlyClient.Instance.Connect(passwordCredential.UserName, out dummy);
                    }
                    else
                    {
                        GnarlyClient.Instance.Connect(passwordCredential.UserName, passwordCredential.Password, out dummy);
                    }
                }
            }
            catch (Exception exception)
            {
                LogUnhandledError(exception);
            }


            return GnarlyClient.Instance.Connected;
        }

        /// <summary>
        /// Gets a diff between two message arrays
        /// </summary>
        /// <param name="oldMessages">Messages that exist during suspend.</param>
        /// <param name="newMessages">Messages that exist during resume.</param>
        /// <returns>Unread messages</returns>
        private Message[] DiffMessages(Message[] oldMessages, Message[] newMessages)
        {
            return newMessages.Except(oldMessages).ToArray();
        }

        private async void RegisterTask(string taskName, string entryPoint)
        {
            bool taskRegistered = false;

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    taskRegistered = true;
                    break;
                }
            }

            if (!taskRegistered)
            {
                var builder = new BackgroundTaskBuilder();
                var trigger = new ApplicationTrigger();
                builder.Name = taskName;
                builder.TaskEntryPoint = entryPoint;
                builder.SetTrigger(trigger);

                BackgroundTaskRegistration task = builder.Register();

                var result = await trigger.RequestAsync();
            }
        }

        private void UnregisterTask(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    task.Value.Unregister(true);
                    break;
                }
            }
        }

        //private async void VersionFixer()
        //{
        //    Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        //    var file = await storageFolder.TryGetItemAsync(VersionFile);

        //    if (file != null)
        //    {
        //        var readFile = file as StorageFile;
        //        string version = await FileIO.ReadTextAsync(readFile);
        //        if(version != Version)
        //        {
        //            RemoveUserCredentials();
        //            await FileIO.WriteTextAsync(readFile, Version);
        //        }
        //    }
        //    else
        //    {
        //        RemoveUserCredentials();
        //        StorageFile sampleFile = await storageFolder.CreateFileAsync(VersionFile,
        //        CreationCollisionOption.ReplaceExisting);
        //        await Windows.Storage.FileIO.WriteTextAsync(sampleFile, Version);
        //    }
        //}

        private void RemoveUserCredentials()
        {
            PasswordVault valut = new PasswordVault();
            foreach (var c in valut.RetrieveAll())
            {
                valut.Remove(c);
            }
        }

        private const string VersionFile = "VersionFile.txt";
        private const string BackgroundClientTask = "DiscordClientBackgroundTask";
        private const string BackgroundClientTaskEntryPoint = "DiscordClientBackground.DiscordClientBackgroundTask";
        Message[] _cachedMessages = null;
        bool _appPreviouslyRunning = false;
    }
}
