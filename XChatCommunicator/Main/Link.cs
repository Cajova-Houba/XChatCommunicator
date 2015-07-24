using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XChatter.Main
{
    /// <summary>
    /// Třída představuje přepravku, která přenáší obecný odkaz. 
    /// </summary>
    public class CommonLink
    {
        public String Name { get; set; }
        public String Link { get; set; }

        public CommonLink(string name, string link)
        {
            Name = name;
            Link = link;
        }
    }

    /// <summary>
    /// Třída předsrtavuje odkaz na kategorii místností.
    /// </summary>
    public class CategoryLink : CommonLink
    {
        /// <summary>
        /// Info o kategorii - zatím nevyužito.
        /// </summary>
        public String Info { get; set; }

        public CategoryLink(string name, string link, string info) : base(name,link)
        {
            Info = info;
        }
    }

    /// <summary>
    /// Třída představuje odkaz na chatovací místnost.
    /// </summary>
    public class RoomLink : CommonLink
    {
        /// <summary>
        /// Info o místnosti - zatím nevyužito.
        /// </summary>
        public String Info { get; set; }

        public RoomLink(string name, string link, string info) : base(name,link)
        {
            Info = info;
        }
    }

    /// <summary>
    /// Kolekce, která se bude zobrazovat do ListBoxu v hlavním okně. 
    /// </summary>
    public class Links : ObservableCollection<CommonLink>
    {
        public Links()
        {
        }
    }
}
