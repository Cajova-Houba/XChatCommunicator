using System;
using System.Security.Permissions;
using XChatter.Main;

namespace XChatter
{
    /// <summary>
    /// Vstupní bod programu.
    /// </summary>
    public static class Program
    {
        [STAThread]
        [PermissionSet(SecurityAction.Demand, Name="FullTrust")]
        public static void Main(String[] args)
        {
            MainApp main = new MainApp();
        }
    }
}
