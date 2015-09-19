using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XChatter.Chat
{
    /// <summary>
    /// Přepravka představující chatovací zprávu.
    /// </summary>
    public class Message
    {
        public string Username { private set; get; }

        /// <summary>
        /// Samotná zpráva, zatím bez smajlíků.
        /// </summary>
        public string Msg { private set; get; }

        public DateTime Time { private set; get; }

        public Message(string user, string msg, DateTime time)
        {
            Username = user;
            Msg = msg;
            Time = time;
        }
    }
}
