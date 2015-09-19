﻿using System;
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
            openChatRoom();
        }

        /// <summary>
        /// Vytvoří vlákno s GUI místnosti.
        /// </summary>
        private void openChatRoom()
        {
            Thread t = new Thread(() =>
            {
                ChatWindow chw = new ChatWindow(this, Link);
                chw.Show();
                chw.Closed += (sender, e) => chw.Dispatcher.InvokeShutdown();

                System.Windows.Threading.Dispatcher.Run();
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        /// <summary>
        /// Metoda slouží k získání chatovacích zpráv v místnosti.
        /// </summary>
        /// <returns></returns>
        public List<Message> getMessages()
        {

            return null;
        }
    }
}
