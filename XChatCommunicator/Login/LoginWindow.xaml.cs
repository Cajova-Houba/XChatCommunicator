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

namespace XChatter.Login
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        //odkaz na hlavní aplikaci, ze které bude volána většina věcí.
        private MainApp mainApp;

        public LoginWindow(MainApp ma)
        {
            InitializeComponent();
            this.mainApp = ma;
        }

        /// <summary>
        /// Reakce na událost kliknutí na login tlačítko.
        /// Načtou se přihlašovací jméno a heslo (+kontrola), s těmito udáji se zkusí login.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bLoginonClick(Object sender, EventArgs e)
        {
            Logger.dbgOut("Klik na login tlačítko.");
            
            String login = TBjmeno.Text;
            String psw = TBheslo.Password;

            bool res = mainApp.logIn(login, psw);

            if(!res)
            {
                LChyba.Content = "Chyba při přihlášení.";
            }
            else
            {
                //tohle tu musí být kvůli dispatcheru, který je nastaven na událost close
                this.Close();
            }
        }
    }
}
