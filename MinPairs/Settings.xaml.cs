using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;

namespace MinPairs
{
    public partial class Settings : PhoneApplicationPage
    {

        bool isInitDone = false;
        IsolatedStorageSettings mySettings = IsolatedStorageSettings.ApplicationSettings;
        Int16 iListenSelect;
        Int16 iListenType;
        Int16 iReadListenSelect;
        ML_Main M;
        public class iSetting
        {
            public string SetingValue { get; set; }
            public string Name { get; set; }
            
        }
        List<iSetting> myValues1 = new List<iSetting>();
        List<iSetting> myValues2 = new List<iSetting>();
        List<iSetting> myValues3 = new List<iSetting>();

        #region test
        public class Cities
        {
            public string Name
            {
                get;
                set;
            }

            public string Country
            {
                get;
                set;
            }

            public string Language
            {
                get;
                set;
            }
        } 

        #endregion


        public Settings()
        {
            InitializeComponent();
            M = (ML_Main)PhoneApplicationService.Current.State["ML"];
            this.button1.Width = ML_Main.DAS.dX - 20;
            this.List1.Width = ML_Main.DAS.dX - 20;
            this.List2.Width = ML_Main.DAS.dX - 20;
            this.List3.Width = ML_Main.DAS.dX - 20;

            //List1.Items.Clear();
            //List2.Items.Clear();
            //List3.Items.Clear();

            
            
            
            

            if (!mySettings.Contains("ListenSelect"))
            {
                iListenSelect = 5;
            }
            else
            {
                iListenSelect = Convert.ToInt16(mySettings["ListenSelect"]);
            }
            if (!mySettings.Contains("ListenType"))
            {
                iListenType = 10;
            }
            else
            {
                iListenType = Convert.ToInt16(mySettings["ListenType"]);
            }
            if (!mySettings.Contains("ReadListenSelect"))
            {
                iReadListenSelect = 5;
            }
            else
            {
                iReadListenSelect = Convert.ToInt16(mySettings["ReadListenSelect"]);
            }
            
            for (Int16 iCnt = 1; iCnt < 21; iCnt++)
            {
                myValues1.Add(new iSetting() { SetingValue = iCnt.ToString(), Name = " - seconds" });
                myValues2.Add(new iSetting() { SetingValue = iCnt.ToString(), Name = " - seconds" });
                myValues3.Add(new iSetting() { SetingValue = iCnt.ToString(), Name = " - seconds" });
            }

            this.List1.ItemsSource = myValues1;
            
            this.List2.ItemsSource = myValues2;
            this.List3.ItemsSource = myValues3;

            List1.SelectedIndex  = iListenSelect - 1;
            List3.SelectedIndex  = iListenType - 1;
            List2.SelectedIndex = iReadListenSelect - 1;
            isInitDone = true;
        }
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void List1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitDone)
            {
                iSetting myIS = (iSetting)List1.SelectedItem;
                iListenSelect = Convert.ToInt16(myIS.SetingValue);
                if (!mySettings.Contains("ListenSelect"))
                {
                    mySettings.Add("ListenSelect", iListenSelect);
                }
                else
                {
                    mySettings["ListenSelect"] = iListenSelect;
                }
            }
        }

        private void List2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitDone)
            {
                iSetting myIS = (iSetting)List2.SelectedItem;
                iReadListenSelect = Convert.ToInt16(myIS.SetingValue);
                if (!mySettings.Contains("ReadListenSelect"))
                {
                    mySettings.Add("ReadListenSelect", iReadListenSelect);
                }
                else
                {
                    mySettings["ReadListenSelect"] = iReadListenSelect;
                }
            }
        }

        private void List3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitDone)
            {
                iSetting myIS = (iSetting)List2.SelectedItem;
                iListenType = Convert.ToInt16(myIS.SetingValue);
                if (!mySettings.Contains("ListenType"))
                {
                    mySettings.Add("ListenType", iListenType);
                }
                else
                {
                    mySettings["ListenType"] = iListenType;
                }
            }

        }

        private void button1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MP_Stats myStats;
            myStats = new MP_Stats();
            myStats.ResetStats();
        }

        
    }
}