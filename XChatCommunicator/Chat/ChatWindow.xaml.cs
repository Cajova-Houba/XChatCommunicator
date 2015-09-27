using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using XChatter.Main;
using XChatter.XchatCommunicator;

namespace XChatter.Chat
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        public string RoomName { private set; get; }

        public Chat Parrent { private set; get; }

        public RoomLink Link { private set; get; }

        private MessageCollection Messages { set; get; }

        /// <summary>
        /// Doba obnovení zpráv v sekundách, zatím 20.
        /// </summary>
        private int RefreshTime { set; get; }

        /// <summary>
        /// Cyklické obnovování zpráv. Dobrý jméno co?
        /// </summary>
        private System.Timers.Timer refresher;

        public ChatWindow(Chat parrent, RoomLink link)
        {
            InitializeComponent();

            DataContext = this;  //aby šel bindovat title okna
            Parrent = parrent;
            Link = link;
            RefreshTime = 20;
            refresher = new System.Timers.Timer(RefreshTime * 1000);
            refresher.Elapsed += (object sender, ElapsedEventArgs ea) => {
                Logger.dbgOut("Refresh");

                //tohle tu je, protoze potrebuju kolekci updatovat z jinýho vlákna
                Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate() { 
                        List<Message> ms = parrent.getMessages();
                        Messages.Clear();
                        foreach(Message m in ms)
                        {
                            Messages.Add(m);
                        }
                    }
                );
            };

            Messages = new MessageCollection(parrent.getMessages());
            lbChatView.ItemsSource = Messages;

            //zapnutí obnovování zpráv
            refresher.Start();
        }
    }
}
