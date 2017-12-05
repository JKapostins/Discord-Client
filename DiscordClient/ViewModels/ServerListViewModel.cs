using Discord;
using Discord.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClient.ViewModels
{
    public class ServerListViewModel : NotificationBase
    {
        public ServerListViewModel()
        {
            try
            {
                Servers = new ObservableCollection<ServerViewModel>();
                foreach (var s in GnarlyClient.Instance.DiscordClient.Servers)
                {
                    Servers.Add(new ServerViewModel(s));
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        public ObservableCollection<ServerViewModel> Servers { get; set; }
    }
}
