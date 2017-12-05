using Discord;
using Discord.ViewModels;

namespace DiscordClient.ViewModels
{
    public class PrivateChannelViewModel : FriendListItemViewModel
    {
        public PrivateChannelViewModel(Channel channel, Discord.User recipient)
            : base(recipient)
        {
            PrivateChannel = channel;
        }

        public Channel PrivateChannel { get; private set; }

    }
}
