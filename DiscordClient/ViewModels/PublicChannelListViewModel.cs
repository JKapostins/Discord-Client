using Discord;
using Discord.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace DiscordClient.ViewModels
{
    public class PublicChannelListViewModel : NotificationBase
    {
        public PublicChannelListViewModel(Server server)
        {
            try
            {
                TextChannels = new ObservableCollection<PublicChannelViewModel>();

                foreach (var c in server.TextChannels)
                {
                    TextChannels.Add(new PublicChannelViewModel(c));
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        public ObservableCollection<PublicChannelViewModel> TextChannels { get; set; }
    }
}
