using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClient.EventArgs
{
    public delegate void ChatMessage(RawMessageEventArgs e);
    public class RawMessageEventArgs : System.EventArgs
    {
        public string Message { get; set; }
    }
}
