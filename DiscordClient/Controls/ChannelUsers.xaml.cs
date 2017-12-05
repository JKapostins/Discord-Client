using Discord;
using Discord.ViewModels;
using DiscordClient.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DiscordClient.Controls
{
    public sealed partial class ChannelUsers : UserControl
    {
        public ChannelUsers()
        {
            this.InitializeComponent();
            GnarlyClient.Instance.DiscordClient.UserUpdated += DiscordClient_UserUpdated;
        }

        public string Topic
        {
            get { return (string)GetValue(TopicProperty); }
            set { SetValue(TopicProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Topic.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopicProperty =
            DependencyProperty.Register("Topic", typeof(string), typeof(ChannelUsers), null);

        public Channel Channel { get; set; } = null;

        
        public async Task RefreshControls()
        {
            await InitializeDataContexts();
        }

        private async Task InitializeDataContexts()
        {
            try
            {
                if (Channel != null)
                {
                    //This event triggers when a user goes in and out of game and when their status changes.
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () =>
                            {
                                Topic = Channel.Topic;
                                onlineListView.DataContext = new UserListViewModel(Channel);
                                offlineListview.DataContext = new UserListViewModel(Channel);
                            });
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
                await RefreshControls();
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }


    }
}
