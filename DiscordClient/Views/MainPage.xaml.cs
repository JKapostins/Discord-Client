using DiscordClient.Types;
using DiscordClient.Views;
using DiscordCommon;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Discord
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
              this.InitializeComponent();
            try
            {

                //Open the context menu by default on boot.
                ShowContextMenu();
                contextMenu.ChannelChanged = ChannelChanged;
                contextMenu.ItemClicked += ContextMenu_ItemClicked;
                contextMenu.NewTextChannelSelected += ContextMenu_NewTextChannelSelected;
                chatWindow.OnMessageSent += ChatWindow_OnMessageSent;
                GnarlyClient.Instance.MessageReceived += DiscordClient_MessageReceived;
                ActiveChannel = null;

                if (PurchaseManager.Instance.HasLeftTip)
                {
                    tipButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    tipButton.Visibility = Visibility.Visible;
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        public void ProcessMessages(Message[] messages)
        {
            try
            {
                if (chatWindow != null)
                {
                    foreach (var message in messages)
                    {
                        chatWindow.ProcessMessage(message);
                    }
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        public Channel ActiveChannel { get; private set; }

        public const int MaxMessageDownloadCount = 100;
        private void ContextMenu_NewTextChannelSelected(global::DiscordClient.EventArgs.NewChannelSelectedEventArgs args)
        {
            try
            {
                ChannelChanged(args.NewChannel);
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void ContextMenu_ItemClicked()
        {
            try
            {
                HideContextMenu();
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private async void DiscordClient_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                if (ActiveChannel != null && e.Channel.Id == ActiveChannel.Id && e.Message.IsAuthor == false)
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            chatWindow.ProcessMessage(e.Message);
                        });

                    //Need to add a small delay hack to get this to fire after the ui is updated.
                    var delay = System.Threading.Tasks.Task.Delay(System.TimeSpan.FromMilliseconds(HackScrollDelay));
                    delay.Wait();
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () =>
                            {
                                chatWindow.ScrollToBottom();
                            });

                }
                else if (e.Message.IsAuthor)
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            chatWindow.ScrollToBottom();
                        });
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }

        }

        private async void ChatWindow_OnMessageSent(global::DiscordClient.EventArgs.RawMessageEventArgs e)
        {
            try
            {
                if (ActiveChannel != null && e.Message.Length > 0)
                {
                    Message message = await ActiveChannel.SendMessage(e.Message);
                    chatWindow.ProcessMessage(message);
                }
            }
            catch (NullReferenceException)
            {
                try
                {
                    if (chatWindow != null)
                    {
                        chatWindow.PrintErrorMessage(string.Format("The following message failed to send: {0}", e.Message));
                    }
                }
                catch (Exception exception)
                {
                    App.LogUnhandledError(exception);
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)contextMenuButton.IsChecked)
                {
                    HidePeople();
                    ShowContextMenu();

                }
                else
                {
                    HideContextMenu();
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private async void People_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PurchaseManager.Instance.HasLeftTip)
                {
                    if ((bool)peopleButton.IsChecked)
                    {
                        HideContextMenu();
                        ShowPeople();
                    }
                    else
                    {
                        HidePeople();
                    }
                }
                else
                {
                    var popup = new MessageDialog(
                    ProjectGlobals.EarlyAccessRequiredMessage,
                    ProjectGlobals.EarlyAccessRequiredHeader);
                    popup.Commands.Add(new UICommand(ProjectGlobals.Enable) { Id = 0 });
                    popup.Commands.Add(new UICommand(ProjectGlobals.NoThanks) { Id = 1 });

                    var result = await popup.ShowAsync();
                    peopleButton.IsChecked = false;
                    if (result != null && (int)result.Id == 0)
                    {
                        Frame rootFrame = Window.Current.Content as Frame;
                        if (rootFrame != null)
                        {
                            rootFrame.Navigate(typeof(TipPage));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void HideContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                contextMenuButton.IsChecked = false;
                HideContextMenu();
                HidePeople();
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void ShowPeople()
        {
            try
            {
                chatWindow.CloseKeyboard();
                peopleButton.IsChecked = true;
                hideContextMenuButton.Visibility = Visibility.Visible;
                channelUsers.Visibility = Visibility.Visible;
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void HidePeople()
        {
            try
            {
                peopleButton.IsChecked = false;
                hideContextMenuButton.Visibility = Visibility.Collapsed;
                channelUsers.Visibility = Visibility.Collapsed;
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void ShowContextMenu()
        {
            try
            {
                chatWindow.CloseKeyboard();
                contextMenuButton.IsChecked = true;
                hideContextMenuButton.Visibility = Visibility.Visible;
                contextMenu.Visibility = Visibility.Visible;
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void HideContextMenu()
        {
            try
            {
                chatWindow.CloseKeyboard();
                chatWindow.ScrollToBottom();
                contextMenuButton.IsChecked = false;
                hideContextMenuButton.Visibility = Visibility.Collapsed;
                contextMenu.Visibility = Visibility.Collapsed;
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private async void  ChannelChanged(Channel channel)
        {
            try
            {
                friendsListGrid.Visibility = Visibility.Collapsed;
                chatWindow.Visibility = Visibility.Visible;
                ActiveChannel = channel;
                DeterminePeopleButtonVisibility(ActiveChannel);
                SetMenuBarHeader(ActiveChannel);
                chatWindow.ClearChatWindow();
                Message[] messages = await channel.DownloadMessages(MaxMessageDownloadCount);
                var reverseMessages = messages.Reverse().ToArray();
                ProcessMessages(reverseMessages);
            }
            catch (TaskCanceledException)
            {
                App.TryGracefulRelaunch();
            }
            catch (OperationCanceledException)
            {
                App.TryGracefulRelaunch();
            }
            catch (HttpRequestException)
            {
                var popup = new MessageDialog(
                    "You have lost internet connection. Please try again when internet is available.",
                    "Connection Lost");
                popup.Commands.Add(new UICommand("Exit App") { Id = 0 });
                var result = await popup.ShowAsync();
                CoreApplication.Exit();
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void SetMenuBarHeader(Channel channel)
        {
            try
            {
                if (channel.IsPrivate)
                {
                    commandBarHeaderText.Text = "@" + channel.Recipient.Name;
                }
                else
                {
                    commandBarHeaderText.Text = "#" + channel.Name;
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private async void DeterminePeopleButtonVisibility(Channel channel)
        {
            try
            {
                if (channel.IsPrivate)
                {
                    peopleButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    channelUsers.Channel = channel;
                    await channelUsers.RefreshControls();
                    peopleButton.Visibility = Visibility.Visible;
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }
        //This may need to be tuned for different hardware. If bugs are reported about autoscroll not working this value should be increased.
        private const int HackScrollDelay = 50;

        private void tipButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Frame currentFrame = Window.Current.Content as Frame;
                if (currentFrame != null)
                {
                    currentFrame.Navigate(typeof(TipPage));
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private async void trelloButton_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://trello.com/b/HgHREOjb"));
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private async void Hyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://trello.com/b/HgHREOjb"));
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private async void followButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://twitter.com/Gnarlysoft"));
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private async void likeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.facebook.com/Gnarlysoft-350689715122713/?ref=aymt_homepage_panel"));
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame != null)
                {
                    GnarlyClient.Instance.Disconnect();
                    Windows.Security.Credentials.PasswordVault valut = new Windows.Security.Credentials.PasswordVault();
                    foreach (var c in valut.RetrieveAll())
                    {
                        valut.Remove(c);
                    }
                    rootFrame.Navigate(typeof(LoginSelection));
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }
    }
}
