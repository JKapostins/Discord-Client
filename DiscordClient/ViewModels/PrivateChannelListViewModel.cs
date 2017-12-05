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
    public class PrivateChannelListViewModel : NotificationBase
    {
        public PrivateChannelListViewModel()
        {
            try
            {
                PrivateChannels = new ObservableCollection<PrivateChannelViewModel>();
                foreach (var c in GnarlyClient.Instance.DiscordClient.PrivateChannels)
                {
                    //GNARLY_HACK: Workaround for bug where all private channels make the recipient appear offline.
                    //Servers have accurate data so we search all servers until we find the user and use that data to populate the direct message list.
                    var recipient = c.Recipient;
                    foreach (var server in GnarlyClient.Instance.DiscordClient.Servers)
                    {
                        foreach (var user in server.Users)
                        {
                            if(user.Id == recipient.Id)
                            {
                                recipient = user;
                                break;
                            }
                        }
                    }
                    PrivateChannels.Add(new PrivateChannelViewModel(c, recipient));
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        public ObservableCollection<PrivateChannelViewModel> PrivateChannels { get; set; }
    }
}
