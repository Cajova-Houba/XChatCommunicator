using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XChatter.XchatCommunicator;

namespace XChatter.Main
{
    /// <summary>
    /// Hlavní třída programu.
    /// </summary>
    public class MainApp
    {
        /// <summary>
        /// Tru pro debugovací výpisy.
        /// </summary>
        private const bool DBG = true;

        /// <summary>
        /// Instance XChatCommunicatoru.
        /// </summary>
        private XChatCommunicator xComm;

        public MainApp()
        {
            //start aplikace
            Logger.dbgOut("Start aplikace");
            
            //nová instance XChatCommunicatoru
            xComm = XChatCommunicator.getCommunicator();
            Logger.dbgOut("Instance XCommunicatoru načtena.");

            //hlavní okno aplikace
            Logger.dbgOut("Spouštím halvní okno aplikace");
            MainWindow mw = new MainWindow(this);
            mw.Show();

            Application app = new Application();
            app.Run();
        }

        /// <summary>
        /// Metoda přes XChatCommunicator zjistí, jestli je někdo přihlášen.
        /// </summary>
        /// <returns></returns>
        public bool isLogged()
        {
            Logger.dbgOut("Zjišťuji jestli je někdo přihlášen.");

            return xComm.LogedIn;
        }

        public Links getChatRoomList(String link)
        {

            return xComm.getChatRoomsList(link);
        }

        /// <summary>
        /// Metoda z xCommu získá seznam kategorií chatovacích místností a ten vrátí.
        /// Zatím jen špagety kód, ale později tu bude kontrola chyb
        /// </summary>
        /// <returns></returns>
        public Links getChatRoomCategories()
        {
            return xComm.getChatRoomCategories();
        }

        /// <summary>
        /// Metoda se pokusí přihlásit se zadanými údaji. vrátí true pokud přihlášení proběhne úspěšně.
        /// </summary>
        public Boolean logIn(String login, String psw)
        {
            State res = xComm.logIn(login, psw);

            //tady to bude chtít nějak ukládat info o případný chybě
            if(res.Ok && ((string)res.Res).Length == XChatCommunicator.SSKEY_LENGTH)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Metoda vrátí username právě přihlášeného uživatele. Pokud není nikdo přihlášen, vrátí
        /// prázdný řetězec.
        /// </summary>
        public String getUsername()
        {
            if(xComm.LogedIn)
            {
                return xComm.Username;
            }
            else
            {
                return "";
            }
        }
    }
}
