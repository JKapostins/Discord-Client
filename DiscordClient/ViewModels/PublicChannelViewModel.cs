using Discord;
using Discord.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClient.ViewModels
{
    public class PublicChannelViewModel : NotificationBase
    {
        public PublicChannelViewModel(Channel channel)
        {
            Channel = channel;
            ChannelName = "#" + Channel.Name;
        }

        public Channel Channel { get; private set; }

        public string ChannelName { get; private set; }
    }
}
