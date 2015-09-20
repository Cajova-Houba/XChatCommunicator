using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Drawing;
using XChatter.Main;
using XChatter.XchatCommunicator;
using System.Text.RegularExpressions;
using XChatter.Chat;

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
            HttpWebRequest request = makeRequest(XCHAT_URI + "/login/",httpContent,"POST");

            //jako odpoveď přijde http packet, kterej bude obsahovat url s chybou/ssklíčem
            //v response je tato url rozdělena na segmenty -> z těchto segmentů získat potřebné věci
            WebResponse response = request.GetResponse();
            Logger.dbgOut(((HttpWebResponse)response).StatusDescription);

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
        /// Odkaz na místnost je ve formátu /modchat?op=mainframeset&rid=*******&js=0&nonjs=1
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

            /*
             * Přímej link na místnost má tvar 
             * http://xchat.centrum.cz/ssk/modchat/room/nazev-mistnosti
             * 
             * nazev-mistnosti je skutečnej název místnosti, bez diakritiky, s pomlčkama místo mezer.
             * Neni tedy nutný parsovat odkazy, stačí pouze získat jména místností a ty přeformátovat.
             * 
             * Pokud chce uživatel domístnosti vstoupit poprvé, je odkázán na intro stránku, kde musí souhlasit s podmínkama.
             * To se dá přeskočit, když odešlu POST packet s disclaim=on. Budu muset použít předchozí verzi a vrátit se rid. 
             */

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

                    //rid pres regex "rid=(\d{7})"
                    String url = linkNameNode.GetAttributeValue("href", "nic").ToString();
                    Regex reg = new Regex("(?<rid>rid=[\\d]*)&");
                    Match rid = reg.Match(url);

                    //name
                    String name = linkNameNode.InnerText;

                    //link name - zde se skutecne jmeno mistnosti preformatuje na jmeno mistnosti do odkazu
                    String linkName = name.ToLower().Replace(' ', '-');
                    //odstranění diakritiky
                    char[] diakritika = new char[] { 'á', 'č', 'ď', 'ě', 'é', 'í', 'ó', 'ř', 'š', 'ú', 'ů', 'ý', 'ž' };
                    char[] normalne = new char[] { 'a', 'c', 'd', 'e', 'e', 'i', 'o', 'r', 's', 'u', 'u', 'y', 'z' };
                    for(int i=0;i<diakritika.Length;i++)
                    {
                        linkName = linkName.Replace(diakritika[i], normalne[i]);
                    }

                    //info
                    String info = infoNode.InnerText;
                    //odebrání " &nbsp;"
                    info = info.Substring(0, info.Length - 6);

                    //info o místnosti zatím nepoužito
                    chatRooms.Add(new RoomLink(name, "/modchat?op=mainframeset&"+rid.Value.ToString()+"&js=0&nonjs=1", info, rid.Groups["rid"].ToString()));
                }
                else
                {
                    first = false;
                }
            }

            return chatRooms;
            
        }

        /// <summary>
        /// Metoda získá náhled profilové fotky uživatele. Pokud není nikdo přihlášený, vrátí null.
        /// </summary>
        /// <returns>Objekt třídy State. Dojde-li při komunikaci s XChatem k chybě, bude objekt obsahovat následující
        ///             Ok = false
        ///             Err = znění chyby
        ///             Res = null
        ///          Nedojde-li k chybě, bude objekt obsahovat následující
        ///             Ok = true
        ///             Err = ""
        ///             Res = Stream dat s imagem. Pokud není nikdo přihlášený, null.
        /// </returns>
        public State getProfilePhotoPreviewStream()
        {
            State res = new State();
            if (LogedIn)
            {
                //vytvoření requestu na xchat.centrum.cz/SSK/index.php
                String uri = "http://xchat.centrum.cz/"+SessionKey+"/index.php";
                HtmlDocument page= new HtmlDocument();

                //načtení stránky
                WebClient wc = new WebClient();
                String htmlString = wc.DownloadString(uri);
                page.LoadHtml(htmlString);

                //parsování
                //vybrat <div id="hlavicka"> -> <span class="float2"> -> <img src="tohleto">

                HtmlNode img = page.DocumentNode.SelectSingleNode("//div[contains(@id,'hlavicka')]")
                               .SelectSingleNode(".//span[contains(@class,'float2')]")
                               .SelectSingleNode(".//img");

                //url obrázku
                String imgSrc = img.Attributes["src"].Value;

                //načtení streamu dat
                var req = WebRequest.Create(imgSrc);
                var resp = req.GetResponse();
                res.Res = resp.GetResponseStream();

                return res;
            }


            //pokud neni nikdo prhlaseny vrati null
            else
            {
                return res;
            }
        }

        /// <summary>
        /// Metoda získá zprávy pro zadanou místnost.
        /// </summary>
        /// <param name="rid">Room ID</param>
        /// <returns>Objekt třídy State. Dojde-li při komunikaci s XChatem k chybě, bude objekt obsahovat následující
        ///             Ok = false
        ///             Err = znění chyby
        ///             Res = null
        ///          Nedojde-li k chybě, bude objekt obsahovat následující
        ///             Ok = true
        ///             Err = ""
        ///             Res = List typovaný na Message.
        /// </returns>
        public State getMessages(string rid)
        {
            State res = new State();

            //url ktera vede primo na frame se zpravama
            //http://xchat.centrum.cz/ssk/modchat?op=roomtopng&rid=4030012&js=0
            String roomUri = "http://xchat.centrum.cz/" + SessionKey + "/modchat?op=roomtopng&"+rid+"&js=0";

            HtmlDocument page = new HtmlDocument();
            page.LoadHtml(getHtmlString(roomUri));

            /*zprávy ve framu jsou v elementu body a mají následující formát
             *  20:09:34 <font color="#150e97"><span class="umsg_room"><b>Merchant:</b> ahoj, mi se ještě neznáme. Já jsem Merchant </span></font><br />\n
                20:09:11 <font color="#000000"><span class="umsg_room"><b>Steward:</b> Ahoj</span></font><br />\n
                <font size="-2" class="systemtime">20:08:54 </font><font size="-2" class="systemtext">&quot;Uživatelka <b class="system in 10-0">Steward</b> vstoupila do místnosti&quot;</font><br />\n
                <font size="-2" class="systemtime">20:07:03 </font><font size="-2" class="systemtext">&quot;Uživatel <b class="system out">risa99</b> opustil  místnost&quot;</font><br />\n
             * 
             * nejlehčí asi bude použít split('\n') a z jednotlivejch řádků pak vyparsovat potřebný věci
             * 
             * když je první znak na řádce číslo, je to obyčejná zpráva od uživatele, s výjimkou prvního řádku - tam jsou před správou dva tagy font
             * řekněme, že ten kód se skoro určitě nebude měnit, takže prvních 86 znaků na prvním řádku můžu vynechat.
             * 
             * když řádek začíná tagem font s class=systemtime, jedná se systémovou zprávu (kdo přišel/odešel z místnosti atp)
            */

            String[] rows = page.DocumentNode.SelectSingleNode("//body").InnerHtml.Split('\n');

            if (rows.Length > 0)
            {
                rows[0] = rows[0].Substring(81);

                List<Message> msgs = new List<Message>();

                foreach (string row in rows)
                {
                    //kdyby se tam připletly nějaký blbosti
                    if (row.Length < 8) { continue; }
                    Message msg = parseMessage(row);
                    if (msg != null) { msgs.Add(msg); }
                }

                res.Res = msgs;
            }

            return res;
        }

        /// <summary>
        /// Metoda vstoupí do zadané místnosti. Vstup v tomto případe znamená, že metoda odešle na introstránku místnosti
        /// souhlas s podmínkami.
        /// </summary>
        /// <param name="rid">Room ID</param>
        /// <returns>Objekt třídy State. Dojde-li při komunikaci s XChatem k chybě, bude objekt obsahovat následující
        ///             Ok = false
        ///             Err = znění chyby
        ///             Res = null
        ///          Nedojde-li k chybě, bude objekt obsahovat následující
        ///             Ok = true
        ///             Err = ""
        ///             Res = True v případě, že se vstup podaří (=> přesměrování na stránku s místností), False v případě, že se nepodaří.
        /// </returns>
        public State enterRoom(string rid)
        {
            State res = new State();

            //link na introstránku je
            //http://xchat.centrum.cz/ssk/room/intro.php?rid=*******
            String introUrl = "http://xchat.centrum.cz/" + SessionKey + "/room/intro.php?" + rid;

            //je nutné odeslat POST packet s hodnotami
            //_btn_enter=wanna_enter_man?
            //btn_enter=Vstoupit do místnosti
            //disclaim=on
            //sexwarn=1  - kvůli erotickým místnostem
            string httpContent = "_btn_enter=wanna_enter_man?&btn_enter=Vstoupit do mistnosti&disclaim=on&sexwarn=1";

            HttpWebRequest req = makeRequest(introUrl, httpContent,"POST");

            //pokud vše proběhne v pořádku, bude v response.Responseuri odkaz http://xchat.centrum.cz/ssk/modchat/room/nazev-mistnosti
            WebResponse response = req.GetResponse();

            Logger.dbgOut("Response URI: " + response.ResponseUri);

            Stream dataStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(dataStream);
            string responseData = sr.ReadToEnd();
            
            return res;
        }

        #region private metody

        /// <summary>
        /// Metoda vytvoří http request na zadané uri se zadaným obsahem a nastaví potřebné parametry.
        /// </summary>
        /// <param name="uri">URL na které bude request směřován</param>
        /// <param name="obsah">Obsah requestu</param>
        /// <param name="method">POST nebo GET</param>
        /// <returns>Vytvořená HttpWebRequest</returns>
        private HttpWebRequest makeRequest(String uri, string obsah, string method)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.UserAgent = CLIENT_NAME;
            request.Method = method;
            request.ContentType = DEFAULT_CONTENT_TYPE;
            request.ContentLength = obsah.Length;

            //zapsani obsahu do requestu - jen pro POST
            if (method == "POST")
            {
                Stream stream = request.GetRequestStream();
                stream.Write(Encoding.ASCII.GetBytes(obsah), 0, obsah.Length);
                stream.Close();
            }

            return request;
        }

        /// <summary>
        /// Metoda stáhne html zadané stránky a vrátí jej.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private String getHtmlString(String uri)
        {
            //tohle dela timeout pri vic chatovacich oknech
            //zkusim pres http request/response
            //WebClient wc = new WebClient();
            //return wc.DownloadString(uri);

            HttpWebRequest req = makeRequest(uri, "","GET");
            WebResponse resp = req.GetResponse();
            Stream dataStream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(dataStream);
            string responseData = sr.ReadToEnd();
            return responseData;

        }

        /// <summary>
        /// Metoda rozparsuje zadaný řádek a sestaví z něj Message, kterou vrátí. Pokud dojde k chybě, vrátí null.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Message parseMessage(String row)
        {
            /*
             * příklady řádků se zprávami
             * 20:09:34 <font color="#150e97"><span class="umsg_room"><b>Merchant:</b> ahoj, mi se ještě neznáme. Já jsem Merchant </span></font><br />\n
                20:09:11 <font color="#000000"><span class="umsg_room"><b>Steward:</b> Ahoj</span></font><br />\n
                <font size="-2" class="systemtime">20:08:54 </font><font size="-2" class="systemtext">&quot;Uživatelka <b class="system in 10-0">Steward</b> vstoupila do místnosti&quot;</font><br />\n
                <font size="-2" class="systemtime">20:07:03 </font><font size="-2" class="systemtext">&quot;Uživatel <b class="system out">risa99</b> opustil  místnost&quot;</font><br />\n
             */

            int type;
            string time;
            string msg;
            string user;

            //parsování podle typu zprávy
            if (row[0] == '<')
            {
                if (row.Length < 43) { return null; }

                type = Message.SYSTEM_MESSAGE;
                time = row.Substring(35, 8);
                user = "";

                //ve wpf lze použít něco na způsob html tagů => nahradím <b class..></b> <Bold></Bold>
                //dále je třeba nahradit "&quot;" uvozovkama
                //smazat </font><br />\n na konci řádku
                row = row.Replace("&quot;", "\"").Replace("</font><br />\n","").Replace("</b>","").Replace("</a>","");

                //pomocí regexu najdu tag <b class=...>
                //tag b nemusí mít vždycky třídu, někdy je jen <b>
                Regex bold = new Regex("<b( class=\"[a-zA-Z\\d\\s]*\")?>");
                Match m = bold.Match(row);
                if (m.Success)
                {
                    row = row.Replace(m.Groups[0].ToString(), "");
                }

                //to stejný udělám i s odkazem
                Regex a = new Regex("<a href=\"[a-zA-Z\\d\\s:\\-/.]*\">");
                m = a.Match(row);
                if (m.Success)
                {
                    row.Replace(m.Groups[0].ToString(), "");
                }

                //nevim proč se tam znak ľ objevuje místo ž
                msg = row.Substring(87).Replace("ľ","ž");

            }
            else
            {
                type = Message.USER_MESSAGE;
                time = row.Substring(0, 8);

                //regexp na jmeno
                //jméno je mezi <b> a </b>
                Regex jmeno = new Regex("<b>(?<uname>[0-9a-zA-Z.-_]*)</b>");
                user = jmeno.Match(row).Groups["uname"].ToString();

                //regexp na zprávu
                //zpráva je mezi </b> a </span>
                Regex zprava = new Regex("</b>(?<msg>.*)</span>");
                msg = zprava.Match(row).Groups["msg"].ToString();
            }

            return new Message(user, msg, time, type);
        }

        #endregion
    }
}
