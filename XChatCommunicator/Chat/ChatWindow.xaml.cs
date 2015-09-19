using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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

        public MainWindow Parrent { private set; get; }

        public RoomLink Link { private set; get; }

        public ChatWindow(MainWindow parrent, RoomLink link)
        {
            InitializeComponent();

            DataContext = this;  //aby šel bindovat title okna
            Parrent = parrent;
            Link = link;
        }
    }
}
