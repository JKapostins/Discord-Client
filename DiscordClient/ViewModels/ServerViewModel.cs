using Discord;
using Discord.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace DiscordClient.ViewModels
{
    public class ServerViewModel : NotificationBase
    {
        public ServerViewModel(Server server)
        {
            DiscordServer = server;
            Initialize();
        }

        public ImageSource ServerIcon
        {
            get { return _serverIcon; }
        }

        public Server DiscordServer { get; private set; }

        private void Initialize()
        {
            try
            {
                _serverIcon = null;
                if (DiscordServer.IconUrl != null)
                {
                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            _serverIcon = new BitmapImage(new Uri(DiscordServer.IconUrl, UriKind.Absolute));
                        });
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private ImageSource _serverIcon;
    }
}
