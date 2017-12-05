using Discord.Converters;
using DiscordClient.EventArgs;
using DiscordClient.ViewModels;
using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord.Controls
{
    public delegate void ChannelChanged(Channel channel);
    public delegate void BasicEvent();
    public sealed partial class ContextMenu : UserControl
    {
        public ContextMenu()
        {
            try
            {
                this.InitializeComponent();

                RefreshControls();

                //Default to selecting the friends first since we know we always have that.
                friendButtonList.SelectedIndex = 0;
                serverList.SelectedIndex = -1;

                GnarlyClient.Instance.UserUpdated += DiscordClient_UserUpdated;
                directMessageList.SelectionChanged += DirectMessageList_SelectionChanged;
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        public ChannelChanged ChannelChanged { get; set; }

        public event BasicEvent ItemClicked;



        public string ServerName
        {
            get { return (string)GetValue(ServerNameProperty); }
            set { SetValue(ServerNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ServerName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ServerNameProperty =
            DependencyProperty.Register("ServerName", typeof(string), typeof(ContextMenu), null);

        public event ChannelSelected NewTextChannelSelected;

        #region CurrentUser Properties
        public string CurrentUserName
        {
            get
            {
                string name = "UnknownName";
                try
                {
                    name = GnarlyClient.Instance.DiscordClient.CurrentUser.Name;
                }
                catch
                {
                    //GNARLY_TODO: Log failure
                }

                return name;
            }
        }

        public string CurrentUserId
        {
            get
            {
                string id = "#unknown";
                try
                {
                    id = "#" + GnarlyClient.Instance.DiscordClient.CurrentUser.Discriminator.ToString();
                }
                catch
                {
                    //GNARLY_TODO: Log failure
                }

                return id;
            }
        }

        public ImageSource CurrentUserIcon
        {
            get
            {
                string url = null;
                try
                {
                    url = GnarlyClient.Instance.DiscordClient.CurrentUser.AvatarUrl;
                    if (url == null)
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                    //GNARLY_TODO: Log failure
                }
                

                return new BitmapImage(new Uri(url, UriKind.Absolute));
            }
        }

        public Status CurrentUserStatus
        {
            get
            {
                Status status = Status.Active;
                try
                {
                    status = UserStatusConverter.Convert(GnarlyClient.Instance.DiscordClient.CurrentUser.Status);
                }
                catch
                {
                    //GNARLY_TODO: Log failure
                }
                return status;
            }
        }
        #endregion

        #region Click Events
        private void serverList_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                
                //Toggle from friends to server
                friendButtonList.SelectedIndex = -1;
                directMessageList.SelectedIndex = -1;
                friendsGrid.Visibility = Visibility.Collapsed;
                channelGrid.Visibility = Visibility.Visible;
                serverError.Visibility = Visibility.Collapsed;

                var discordServer = ((ServerViewModel)e.ClickedItem).DiscordServer;
                ServerName = discordServer.Name;
                textChannelList.DataContext = new PublicChannelListViewModel(discordServer);
            }
            catch (NullReferenceException)
            {
                channelGrid.Visibility = Visibility.Collapsed;
                serverError.Visibility = Visibility.Visible;
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void friendButtonList_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //Toggel from server to friends
                serverList.SelectedIndex = -1;
                textChannelList.SelectedIndex = -1;
                friendsGrid.Visibility = Visibility.Visible;
                channelGrid.Visibility = Visibility.Collapsed;
                serverError.Visibility = Visibility.Collapsed;
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }
        #endregion

        private void DirectMessageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItem = ((PrivateChannelViewModel)directMessageList.SelectedItem)?.PrivateChannel;
                if (directMessageList != null && selectedItem != null)
                {
                    ChannelChanged?.Invoke(selectedItem);
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private async void DiscordClient_UserUpdated(object sender, UserUpdatedEventArgs e)
        {
            try
            {
                //This event triggers when a user goes in and out of game and when their status changes.
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            RefreshControls();
                        });
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void RefreshControls()
        {
            serverList.DataContext = new ServerListViewModel();
            directMessageList.DataContext = new PrivateChannelListViewModel();
        }

        private void directMessageList_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                ItemClicked?.Invoke();
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void textChannelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (textChannelList.SelectedItem != null)
                {
                    NewTextChannelSelected?.Invoke(new NewChannelSelectedEventArgs
                    {
                        NewChannel = (textChannelList.SelectedItem as PublicChannelViewModel)?.Channel
                    });
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }
    }
}
