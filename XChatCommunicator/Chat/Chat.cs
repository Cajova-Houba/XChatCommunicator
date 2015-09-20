using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XChatter.Main;
using XChatter.XchatCommunicator;

namespace XChatter.Chat
{
    /// <summary>
    /// Třída zajišťující aplikační logiku pro chatovací místnosti. Zde budou metody pro komunikaci s xchatem, vytvoření okna místnosti atd.
    /// Spouštění nové místnosti bude prováděno přes tuto třídu. Třída si sama spustí vlákno pro nové okno chatovací místnosti.
    /// </summary>
    public class Chat
    {
        private XChatCommunicator xComm {set; get;}

        private RoomLink Link { set; get; }

        public Chat(RoomLink roomLink)
        {
            Link = roomLink;
            xComm = XChatCommunicator.getCommunicator();
            openChatRoom();
        }

        /// <summary>
        /// Vytvoří vlákno s GUI místnosti.
        /// </summary>
        private void openChatRoom()
        {
            xComm.enterRoom(Link.RID);
            ChatWindow chw = new ChatWindow(this, Link);
            chw.Show();
        }

        /// <summary>
        /// Metoda slouží k získání chatovacích zpráv v místnosti.
        /// </summary>
        /// <returns></returns>
        public List<Message> getMessages()
        {
            return (List<Message>)xComm.getMessages(Link.RID).Res;
        }
    }
}
