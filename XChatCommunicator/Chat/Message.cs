using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XChatter.Chat
{
    /// <summary>
    /// Přepravka představující chatovací zprávu.
    /// </summary>
    public class Message
    {
        //typy zpráv
        public const int SYSTEM_MESSAGE = 0, USER_MESSAGE = 1;

        public string Username { private set; get; }

        /// <summary>
        /// Samotná zpráva, zatím bez smajlíků.
        /// </summary>
        public string Msg { private set; get; }

        public string Time { private set; get; }

        /// <summary>
        /// Typ zprávy - systémová, uživatel...
        /// Zatím jsou jen dvě možnosti, ale necham to typovaný na číslo, kdyby bylo potřeba.
        /// Měly by se používat konstanty z týhle třídy.
        /// </summary>
        public int Type { private set; get; }

        /// <summary>
        /// Barva písma.
        /// </summary>
        public SolidColorBrush FontColor { private set; get; }

        public Message(string user, string msg, string time, int type, Color fontColor)
        {
            Username = user;
            Msg = msg;
            Time = time;
            Type = type;
            FontColor = new SolidColorBrush(fontColor);
        }

        public override string ToString()
        {
            return Time + " " + Username + " " + Msg; ;
        }
    }

    /// <summary>
    /// Kolekce, která se bude zobrazovat do ListBoxu v chatovacícm okně.
    /// </summary>
    public class MessageCollection : ObservableCollection<Message>
    {
        public MessageCollection(List<Message> msgs)
        {
            foreach (Message m in msgs)
            {
                this.Add(m);
            }
        }


    }
}
