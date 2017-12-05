using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.QueryStringDotNET;
using Windows.Security.Credentials;

namespace DiscordClientBackground
{

    public sealed class NetworkChangeTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
        }
    }

    public sealed class PushNotifyTask : IBackgroundTask
    {
        
        public void Run(IBackgroundTaskInstance taskInstance)
        {
        }
    }

    
    //public sealed class DiscordClientBackgroundTask : IBackgroundTask
    //{
    //    public async void Run(IBackgroundTaskInstance taskInstance)
    //    {
    //        _deferral = taskInstance.GetDeferral();
    //        try
    //        {
    //            taskInstance.Canceled += TaskInstance_Canceled;
    //            DebugToast("Background Task Started");
    //            //GNARLY_TODO: move this into a shared project
    //            string AppName = "Discord Client";
    //            PasswordVault vault = new PasswordVault();
    //            var usernameCredentials = vault.RetrieveAll().FirstOrDefault();
    //            PasswordCredential passwordCredential = null;
    //            if (usernameCredentials != null)
    //            {
    //                passwordCredential = vault.Retrieve(AppName, usernameCredentials.UserName);
    //            }

    //            if (passwordCredential != null)
    //            {
    //                _client = new DiscordClient();
    //                await _client.Connect(passwordCredential.UserName, passwordCredential.Password);

    //                DebugToast("Login Success");
    //                _client.MessageReceived += MessageReceived;
    //            }
    //            else
    //            {
    //                throw new System.NullReferenceException("Could not retrieve username and password from PasswordVault");
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            await Disconnect(BackgroundTaskCancellationReason.Abort);
    //            DebugToast("Background Task Exception: " + e.Message);
    //        }

    //    }

    //    private async void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
    //    {
    //        await Disconnect(reason);
    //    }

    //    private void MessageReceived(object sender, MessageEventArgs e)
    //    {
    //        if (e.Message.IsAuthor == false)
    //        {
    //            string title = string.Format("{0} (#{1})", e.User.Name, e.Channel.Name);
    //            string content = e.Message.RawText;

    //            //Need to handle null url.
    //            string logo = e.User.AvatarUrl;

    //            // Construct the visuals of the toast
    //            ToastVisual visual = new ToastVisual()
    //            {
    //                BindingGeneric = new ToastBindingGeneric()
    //                {
    //                    Children =
    //                {
    //                new AdaptiveText()
    //                {
    //                Text = title
    //                },

    //                new AdaptiveText()
    //                {
    //                Text = content
    //                },
    //                },

    //                    AppLogoOverride = new ToastGenericAppLogo()
    //                    {
    //                        Source = logo,
    //                        HintCrop = ToastGenericAppLogoCrop.Circle
    //                    }
    //                }
    //            };


    //            //Look here to setup action
    //            https://blogs.msdn.microsoft.com/tiles_and_toasts/2015/07/08/quickstart-sending-a-local-toast-notification-and-handling-activations-from-it-windows-10/

    //            // Now we can construct the final toast content
    //            ToastContent toastContent = new ToastContent()
    //            {
    //                Visual = visual,
    //                //Actions = actions,

    //                //// Arguments when the user taps body of toast
    //                //Launch = new QueryString()
    //                //{
    //                //    { "action", "viewConversation" },
    //                //    { "conversationId", conversationId.ToString() }

    //                //}.ToString()
    //            };

    //            // And create the toast notification
    //            var toast = new ToastNotification(toastContent.GetXml());
    //            ToastNotificationManager.CreateToastNotifier().Show(toast);
    //        }
    //    }

    //    private async Task Disconnect(BackgroundTaskCancellationReason reason)
    //    {
    //        if (_deferral != null)
    //        {
    //            if (_client.State == ConnectionState.Connected)
    //            {
    //                await _client.Disconnect();
    //            }
    //            DebugToast("Background Task Terminated; Reason: " + reason.ToString());
    //            _deferral.Complete();
    //        }
    //        return;
    //    }
    //    private void DebugToast(string message)
    //    {
    //        // Construct the visuals of the toast
    //        ToastVisual visual = new ToastVisual()
    //        {
    //            BindingGeneric = new ToastBindingGeneric()
    //            {
    //                Children =
    //                {
    //                    new AdaptiveText()
    //                    {
    //                        Text = message
    //                    }
    //                 }
    //            }
    //        };

    //        // Now we can construct the final toast content
    //        ToastContent toastContent = new ToastContent()
    //        {
    //            Visual = visual,
    //        };

    //        // And create the toast notification
    //        var toast = new ToastNotification(toastContent.GetXml());
    //        ToastNotificationManager.CreateToastNotifier().Show(toast);
    //    }

    //    private BackgroundTaskDeferral _deferral;
    //    private DiscordClient _client;
    //}
}
