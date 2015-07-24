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

namespace XChatter.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Odkaz na aplikační třídu
        //skrz ní budou volány všechny metody pro komunikaci s xchatem => gui musí být co nejvíc odděleno od app logiky
        MainApp mainApp;

        /**
         * Přepínání mezi ListBoxy
         * -----------------------
         * 
         * Existují dva LB, jeden pro kategorie místností, jeden pro místnosti. Vždy je viditelný pouze jeden.
         * 
         * 
         * lbCategory je viditelný
         * ------------------------
         * Klikne se na nějakou z kategorií, lbRoomCollection se naplní patřičnejma místnostma
         * lbRoom se zviditelní a lbCategory se zneviditelní. Pokud se klikne na tlačítko zpět, nic se nestane.
         * 
         * 
         * lbRoom je viditelný
         * --------------------
         * Klikne se na místnost, vytvoří se nové chatovací okno. Pokud chceme znovu zobrazit kategorie klikneme na tlačítko zpět. 
         * Po kliknutí se lbCatCollection naplní patřičnými daty, lbCategory se zviditelní a lbRoom se zneviditelní.
         */

        //!!!jako ItemsSource pro ListBoxy s kategoriema/místnostma používat jenom tyhle dvě kolekce!!!
        //ItemsSource pro lbCategory, nastaveni v konstruktoru
        Links lbCatCollection;

        //Itemsource pro lbRoom, nastaveni v konstruktoru
        Links lbRoomCollection;

        public MainWindow(MainApp mainApp)
        {
            InitializeComponent();
            this.mainApp = mainApp;

            //kontrola přihlášení
            if (!mainApp.isLogged())
            {
                Logger.dbgOut("Nikdo nepřihlášen, spouštím přihlašovací okno.");

            }

            //vytvoření a přiřazení itemssource
            lbCatCollection = new Links();
            lbCategory.ItemsSource = lbCatCollection;

            lbRoomCollection = new Links();
            lbRoom.ItemsSource = lbRoomCollection;

            naplnLB();
        }

        /// <summary>
        /// Testovací naplnění ListBoxu.
        /// </summary>
        private void naplnLB()
        {
            Links kolekce = mainApp.getChatRoomCategories();
            lbCatCollection.Clear();
            foreach(CommonLink cl in kolekce)
            {
                lbCatCollection.Add(cl);
            }
        }

        /// <summary>
        /// Reakce na událost výběru prvku z listboxu, který zobrazuje kategorie místností.
        /// Typicky se jedná o kliknutí na kategorii místnosti => zviditelnit druhý listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LBConSelect(Object sender, EventArgs e)
        {
            if (lbCategory.Visibility == System.Windows.Visibility.Hidden) { return; }
            if (lbCategory.SelectedIndex == -1) { return; }
            String name = ((CategoryLink)lbCategory.SelectedItem).Name;
            String link = ((CategoryLink)lbCategory.SelectedItem).Link;
            Logger.dbgOut("Klik na " + name);

            Links kolekce = mainApp.getChatRoomList(link);
            lbRoomCollection.Clear();
            foreach(CommonLink cl in kolekce)
            {
                lbRoomCollection.Add(cl);
            }

            lbRoom.Visibility = System.Windows.Visibility.Visible;
            lbCategory.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Reakce na událost výběru prvku z listboxu, který zobrazuje místnosti.
        /// Typicky se jedná o kliknutí na místnost => vstup do chatovací místnosti.
        /// </summary>
        private void LBRonSelect(Object sender, EventArgs e)
        {
            if (lbRoom.Visibility == System.Windows.Visibility.Hidden) { return; }
            String name = ((RoomLink)lbRoom.SelectedItem).Name;
            Logger.dbgOut("Klik na místnost: " + name);
        }

        /// <summary>
        /// Reakce na událost kliknutí na tlačítko zpět.
        /// Typicky načte kategorie a přepne zobrazení listboxů na listbox s kategoriema.
        /// Zatím je zbytečně pokaždé znovunačítat kategorie => pouze se přepne viditelnost.
        /// </summary>
        private void btnBackonClick(object sender, EventArgs e)
        {
            Logger.dbgOut("Klik na tlačítko zpět.");

            lbCategory.Visibility = System.Windows.Visibility.Visible;
            lbRoom.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}

