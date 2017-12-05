using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClient.EventArgs
{
    public delegate void ChannelSelected(NewChannelSelectedEventArgs args);
    public class NewChannelSelectedEventArgs : System.EventArgs
    {
        public Channel NewChannel { get; set; }
    }
}
