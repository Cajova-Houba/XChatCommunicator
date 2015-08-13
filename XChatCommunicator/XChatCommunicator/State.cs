using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XChatter.XChatCommunicator
{
    /// <summary>
    /// Třída představuje výsledek komunikace s Xchatem. Přenáší údaje o úspěchu komunikace,
    /// případné chybě a případném výsledku.
    /// </summary>
    public class State
    {
        /// <summary>
        /// True pokud komunikace proběhla úspěšně.
        /// </summary>
        public bool Ok { get; set; }

        /// <summary>
        /// Znění chyby, pokud nějaká při komunikaci nastala.
        /// </summary>
        public string Err { get; set; }

        /// <summary>
        /// Obecný výsledek, pokud má nějaký být.
        /// </summary>
        public object Res { get; set; }
    
 
        /// <summary>
        /// Bezparametrický konstruktor. Nastaví Ok = true, Err = "" a Res = null.
        /// </summary>
        public State()
        {
            Ok = true;
            Err = "";
            Res = null;
        }

        public State(bool ok, string err, object res)
        {
            Ok = ok;
            Err = err;
            Res = res;
        }
    }
}
