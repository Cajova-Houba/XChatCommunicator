﻿using System;
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

        public Chat Parrent { private set; get; }

        public RoomLink Link { private set; get; }

        private List<Message> Messages { set; get; }

        public ChatWindow(Chat parrent, RoomLink link)
        {
            InitializeComponent();

            DataContext = this;  //aby šel bindovat title okna
            Parrent = parrent;
            Link = link;

            Messages = parrent.getMessages();
            lbChatView.ItemsSource = Messages;
        }
    }
}
