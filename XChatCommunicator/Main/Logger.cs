using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XChatter.Main
{
    /// <summary>
    /// Třída slouží k logování a debugovacím výpisům.
    /// </summary>
    public static class Logger
    {
        public const bool DBG = true;

        /// <summary>
        /// Metoda slouží pro debugovací výpis. Vypisuje se pokud je DBG=true.
        /// </summary>
        /// <param name="msg"></param>
        public static void dbgOut(String msg)
        {
            if (DBG)
            {
                Console.WriteLine("[" + DateTime.Now + "]: " + msg);
            }
        }
    }
}
