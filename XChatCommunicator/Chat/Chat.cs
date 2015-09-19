using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XChatter.Main;
using XChatter.XchatCommunicator;

namespace XChatter.Chat
{
    /// <summary>
    /// Třída zajišťující aplikační logiku pro chatovací místnosti. Zde budou metody pro komunikaci s xchatem, vytvoření okna místnosti atd.
    /// Spouštění nové místnosti bude prováděno přes tuto třídu.
    /// </summary>
    public class Chat
    {
        private XChatCommunicator xComm {set; get;}

        private RoomLink Link { set; get; }

        public Chat(RoomLink roomLink)
        {
            Link = roomLink;
        }
    }
}
