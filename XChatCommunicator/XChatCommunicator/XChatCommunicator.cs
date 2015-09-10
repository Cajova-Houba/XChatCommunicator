using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Web;
using XChatter.Main;
using XChatter.XchatCommunicator;

namespace XChatter.XchatCommunicator
{
    /// <summary>
    /// Třída slouží ke komunikaci s xchatem. Implementována podle návrhového vzoru jedináček.
    /// Třída si po úspěšném volání metody logIn uchovává session key.
    /// </summary>
    public class XChatCommunicator
    {
        //privátní konstanty
        private static XChatCommunicator instance = new XChatCommunicator();
        private const String XCHAT_URI = "http://xchat.centrum.cz";
        private const String CLIENT_NAME = "XCom by Mushroomer";
        private const String DEFAULT_CONTENT_TYPE = "application/x-www-form-urlencoded";
        
        //veřejné konstanty
        //přesná délka session key
        public const int SSKEY_LENGTH = 43;

        //kódy xchat chyb
        public const int BAD_PSW_ERR = 1;

        #region properties
        public bool LogedIn { get; private set; }

        //session key přihlášeného uživatele
        public String SessionKey { private get; set; }

        //username přihlášeného uživatele
        public String Username { get; private set; }

        #endregion

        private XChatCommunicator()
        {
            LogedIn = false; //pro testování
        }

        /// <summary>
        /// Metoda vrátí instanci komunikátoru.
        /// </summary>
        /// <returns>Instance XChatCommunicator</returns>
        public static XChatCommunicator getCommunicator()
        {
            return XChatCommunicator.instance;
        }

        

        /// <summary>
        /// Meto slouží k přihlášení do xchatu. Výsledkem je session key, přes který se dá přistupovat
        /// do sekcí pro přihlášené uživatele.
        /// </summary>
        /// <param name="name">Uživateské jméno</param>
        /// <param name="pass">Uživatelské heslo</param>
        /// <returns>Objekt třídy State. Dojde-li při komunikaci s XChatem k chybě, bude objekt obsahovat následující
        ///             Ok = false
        ///             Err = znění chyby
        ///             Res = null
        ///          Nedojde-li k chybě, bude objekt obsahovat následující
        ///             Ok = true
        ///             Err = ""
        ///             Res = session key, typováno na string.
        /// </returns>
        public State logIn(String name, String pass)
        {
            State res = new State();

            String sskey = "";
            //byte array s parametry url, js, name, pass, x, y
            //x, y můžou být 0, url nějaká url z xchatu, stačí /index.php a js=1
            string httpContent = "url=" + XCHAT_URI + "/index.php&js=1&x=0&y=0&name=" + name + "&pass=" + pass;
            httpContent.Replace(":", "%3A");
            httpContent.Replace("/", "%2F");
            HttpWebRequest request = makeRequest(XCHAT_URI + "/login/",httpContent);

            //jako odpoveď přijde http packet, kterej bude obsahovat url s chybou/ssklíčem
            //v response je tato url rozdělena na segmenty -> z těchto segmentů získat potřebné věci
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            //když nebude login úspěšný, v response.responseuri.segments[1] bude "login/", a v query bude "...err=kod_chyby..."
            //jinak je v segments[1] sskey+"/"
            String[] responseUri = response.ResponseUri.Segments;
            if (responseUri.Length >= 2)
            {
                //chyba při přihlášení, třeba zjišťovat v query
                if(responseUri[1] == "login/")
                {
                    Console.WriteLine("Chyba");
                }

                //máme session key
                else if (responseUri[1].Length == SSKEY_LENGTH+1 )
                {
                    sskey = responseUri[1].Replace("/","");
                    SessionKey = sskey;
                    Username = name;
                    LogedIn = true;
                }
            }
            response.Close();

            res.Res = sskey;
            return res;
        }

        /// <summary>
        /// Metoda získá seznam kategorií chatovacích místností.
        /// </summary>
        public Links getChatRoomCategories()
        {
            Links kategorie = new Links();
            kategorie.Add(new CategoryLink("Města a kraje","modchat?op=roomlist&cid=1","info"));
            kategorie.Add(new CategoryLink("Pokec a klábosení","modchat?op=roomlist&cid=2","info"));
            kategorie.Add(new CategoryLink("Seznámení a flirt","modchat?op=roomlist&cid=4","info"));
            kategorie.Add(new CategoryLink("Lechtivá erotika","modchat?op=roomlist&cid=8","info"));
            kategorie.Add(new CategoryLink("Volný čas a sport","modchat?op=roomlist&cid=16","info"));
            kategorie.Add(new CategoryLink("Sex a neřesti do 18 let","modchat?op=roomlist&cid=32","info"));
            kategorie.Add(new CategoryLink("Hudební styly a kapely","modchat?op=roomlist&cid=64","info"));
            kategorie.Add(new CategoryLink("Filmy, knihy a počítače","modchat?op=roomlist&cid=128","info"));

            return kategorie;

            //možno řešit přes parsování html stránky - zatím zbytečně složité
            //metoda zatím vrátí jen seznam prvků [jméno,odkaz]
            //string httpContent = "";

            ////odpovědí na tuhle request je packet z html stránkou, pokud je ssk správně
            //HttpWebRequest request = makeRequest(XCHAT_URI + "/" + ssk + "/index.php",httpContent);

            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //if(response.StatusCode == HttpStatusCode.OK)
            //{
            //    //načtení html stránky ze streamu
            //    Stream stream = response.GetResponseStream();
            //    StreamReader sr = null;

            //    //kvůli kódování řetězců
            //    if(response.CharacterSet == null)
            //    {
            //        sr = new StreamReader(stream);
            //    }
            //    else
            //    {
            //        sr = new StreamReader(stream, Encoding.GetEncoding(response.CharacterSet));
            //    }

            //    //přečtení streamu
            //    string data = sr.ReadToEnd();

            //    WebBrowser wb = new WebBrowser();
            //    wb.NavigateToString(data);

            //    sr.Close();
            //    response.Close();

            //}
        }

        /// <summary>
        /// Metoda slouží pro získání seznamu místností v kategorii, která je zadná linkem.
        /// </summary>
        /// <param name="link">Link z kategorie</param>
        /// <returns>Seznam položek ve tvaru [název,link]</returns>
        public Links getChatRoomsList(String link)
        {
            Links chatRooms = new Links();

            //uri na response se skládá z XCHAT_URI/ssk/link
            String uri = XCHAT_URI + "/" + SessionKey + "/" + link;

            //pro parsování
            HtmlDocument page = new HtmlDocument();

            //načtení stránky
            WebClient wc = new WebClient();
            String htmlString = wc.DownloadString(uri);

            //parsování
            page.LoadHtml(htmlString);
            //vybrat všechny td které mají id=c2
            //z nich pak vybrat <a> -> href, který začíná na XCHAT_URI
            //HtmlNodeCollection tds = page.DocumentNode.SelectNodes("//a[contains(@href,'" + XCHAT_URI + "/" + SessionKey + "/room" + "')]");
            
            //místnosti jsou v tabulce, v každém tr je kompletní záznam o místnosti
            HtmlNodeCollection tds = page.DocumentNode.SelectNodes("//tr");
            bool first = true;  //kvůli detekci prvního prvku, trochu prasárna

            foreach (HtmlNode node in tds)
            {
                //první tr je hlavička tabulky => ignorovat
                if (!first)
                {
                    //potom u td.id:
                    //c2 => link+jméno
                    //c3 => info

                    //výběr správného td a z tohoto td výběr správného odkazu ve tvaru <a href=....>JMÉNO MÍSTNOSTI</a>
                    HtmlNode linkNameNode = node.SelectSingleNode(".//td[contains(@id,'c2')]").SelectSingleNode(".//a[contains(@href,'" + XCHAT_URI + "/" + SessionKey + "/room" + "')]");
                    HtmlNode infoNode = node.SelectSingleNode("//td[contains(@id,'c3')]");

                    //link - je nutné vyparsovat rid a cid
                    Uri chatRoomLink = new Uri(linkNameNode.Attributes["href"].Value);
                    String roomLink = "rid=" + HttpUtility.ParseQueryString(chatRoomLink.Query).Get("rid") +
                                      "&cid=" + HttpUtility.ParseQueryString(chatRoomLink.Query).Get("cid");
                    
                    //name
                    String name = linkNameNode.InnerText;

                    //info
                    String info = infoNode.InnerText;
                    //odebrání " &nbsp;"
                    info = info.Substring(0, info.Length - 6);

                    //info o místnosti zatím nepoužito
                    chatRooms.Add(new RoomLink(name,link,info));
                }
                else
                {
                    first = false;
                }
            }

            return chatRooms;
            
        }

        #region private metody

        /// <summary>
        /// Metoda vytvoří http request na zadané uri se zadaným obsahem a nastaví potřebné parametry.
        /// </summary>
        /// <param name="uri">URL na které bude request směřován</param>
        /// <param name="obsah">Obsah requestu</param>
        /// <returns>Vytvořená HttpWebRequest</returns>
        private HttpWebRequest makeRequest(String uri, string obsah)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.UserAgent = CLIENT_NAME;
            request.Method = "POST";
            request.ContentType = DEFAULT_CONTENT_TYPE;
            request.ContentLength = obsah.Length;

            //zapsani obsahu do requestu
            Stream stream = request.GetRequestStream();
            stream.Write(Encoding.ASCII.GetBytes(obsah), 0, obsah.Length);
            stream.Close();

            return request;
        }

        #endregion
    }
}
