using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
namespace MinPairs
{
    public partial class MP_Filter : PhoneApplicationPage
    {
        ML_Main M;
        ML_Filter F;
        OneFilter myF;
        Thickness lbiTick = new Thickness(2.0);
        Brush lbiBrd = new SolidColorBrush(Color.FromArgb(255, 22, 133, 197));
        Brush lbiFore = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        public MP_Filter()
        {
            InitializeComponent();
            M = (ML_Main)PhoneApplicationService.Current.State["ML"];
            F = (ML_Filter)PhoneApplicationService.Current.State["MF"];
            myF = F.GetFilter("MP");
            if (myF != null && myF.IsFilterSet)
            {
                btn2Filters.IsEnabled = true;
            }
            this.LayoutRoot.RowDefinitions[2].MinHeight = ML_Main.DAS.dY -112;
            this.LayoutRoot.RowDefinitions[2].MaxHeight = ML_Main.DAS.dY -112;
            grdFilter.Height = ML_Main.DAS.dY - 112;
            BuildCatList();
        }
        
        private void BuildCatList()
        {

            ML_Button lbi;
            for (int iCnt = 0; iCnt < M.CatList.Count; iCnt++)
            {

                lbi = new ML_Button();
                lbi.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                lbi.Content = M.CatList[iCnt].Caption;
                //lbi.BorderBrush = lbiBrd;
                lbi.BorderThickness = lbiTick;
                lbi.MinWidth = 180;
                lbi.MinWidth = 180;
                lbi.Padding = lbiTick;
                //lbi.Background = lbiBrd;
                //lbi.Foreground = lbiFore;
                lbi.FontWeight = FontWeights.Normal;
                lbi.IsSelected = false;
                
                lbi.Tag = M.CatList[iCnt].CategoryId;
                lbi.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(lbi_Tap);
                if (myF.MyMainCategories.Values.Contains(M.CatList[iCnt].CategoryId))
                {
                    lbi.IsSelected = true;
                }
                
                lstMainFilter.Items.Add(lbi);

            }
            
            this.lstMainFilter.UpdateLayout();
            BuildCatList2();
            
        }

        void lbi_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ML_Button lbi;
            foreach(ML_Button b in lstMainFilter.Items)
            {
                if (b.IsSelected)
                {
                    b.IsSelected = false;
                }
            }
            try
            {
                lbi = (ML_Button)sender;
                if (lbi != null)
                {
                    myF.AddNewMainSound(Convert.ToInt32(lbi.Tag));
                    lbi.IsSelected = true;
                }
                BuildCatList2();
            }
            catch
            {
            }
        }

        
        private void CheckMainList()
        {
            ListBoxItem lbi;
            for (int iCnt = 0; iCnt < lstMainFilter.Items.Count; iCnt++)
            {
                try
                {
                    lbi = (ListBoxItem)lstMainFilter.Items[iCnt];
                    if (lbi != null)
                    {
                        lbi.IsSelected = myF.MyMainCategories.ContainsValue(Convert.ToInt32(lbi.Tag));
                    }
                    
                }
                catch {}
            }
        }


        private void BuildCatList2()
        {
            lstSecFilter.Items.Clear();
            ML_Button lbi;
            for (int iCnt = 1; iCnt <= myF.MySecCategories.Count; iCnt++)
            {
                lbi = new ML_Button();
                lbi.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                //lbi.BorderBrush = lbiBrd;
                lbi.BorderThickness = lbiTick;
                lbi.MinWidth = 180;
                lbi.MinWidth = 180;
                lbi.Padding = lbiTick;
                //lbi.Background = lbiBrd;
                //lbi.Foreground = lbiFore;
                lbi.FontWeight = FontWeights.Normal;
                lbi.Content = M.CatList[myF.MySecCategories[iCnt]].Caption;
                lbi.Tag = myF.MySecCategories[iCnt];
                lbi.IsSelected = false;
                if (myF.MySelectedSecCategories.Values.Contains(myF.MySecCategories[iCnt]))
                {
                    lbi.IsSelected = true;
                }
                lbi.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(lbi_Tap_sec);
                lstSecFilter.Items.Add(lbi);
            }
        }

        void lbi_Tap_sec(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ML_Button lbi;
            foreach (ML_Button b in lstSecFilter.Items)
            {
                if (b.IsSelected)
                {
                    b.IsSelected = false;
                }
            }
            try
            {
                lbi = (ML_Button)sender;
                if (lbi != null)
                {
                    lbi.IsSelected = true;
                    myF.AddSecondarySound(Convert.ToInt32(lbi.Tag));
                    btn2Filters.IsEnabled = true;
                    //btn2Filters.Content = myF.FilterTitle;
                }
                
            }
            catch
            {
            }
        }
        private void btn2Filters_Click(object sender, RoutedEventArgs e)
        {
            string strPage = "/" + M.strFilterCallingPage + ".xaml";
            NavigationService.Navigate(new Uri(strPage, UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            lstMainFilter.UpdateLayout();
            lstMainFilter.ScrollIntoView(lstMainFilter.SelectedItem);
        }
    }
}