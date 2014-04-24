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
    public partial class Learn1 : PhoneApplicationPage
    {
        ML_Main M;
        Button currBtn;
        Boolean bFilterOpen = false;
        string strFilterCrt = string.Empty;
        public Learn1()
        {
            InitializeComponent();
            M = (ML_Main)PhoneApplicationService.Current.State["ML"];
            BuildCatList();
            SetupPage();
        }
        private void SetupPage()
        {
            //btnFilterMove.Margin.Left = cFilter.Width - 5 - btnFilterMove.Width;
            if (bFilterOpen)
            {
                cFilter.Height = 500;
                
                lbFilter1.Visibility = Visibility.Visible;
                lbFilter2.Visibility = Visibility.Visible;
                //ContentPanel.Visibility = Visibility.Visible;
                scrlMain.Visibility = Visibility.Collapsed;
            }
            else
            {
                cFilter.Height = 50;
                
                lbFilter1.Visibility = Visibility.Collapsed;
                lbFilter2.Visibility = Visibility.Collapsed;
                //ContentPanel.Visibility = Visibility.Visible;
                scrlMain.Visibility = Visibility.Visible;
            }
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            
        }
        private void BuildCatList()
        {
            ListBoxItem lbi;
            for (int iCnt = 0; iCnt < M.CatList.Count; iCnt++)
            {
                lbi = new ListBoxItem();
                lbi.Content = M.CatList[iCnt].Caption;
                lbi.Tag = M.CatList[iCnt].CategoryId;
                lbFilter1.Items.Add(lbi);
                if (iCnt == 0)
                {
                    this.lbFilter1.SelectedItem = lbi;
                }
            }
        }
        private void BuildCatList2()
        {
            ListBoxItem lbi;
            lbFilter2.Items.Clear();
            for (int iCnt = 0; iCnt < M.CatList.Count; iCnt++)
            {
                lbi = new ListBoxItem();
                lbi.Content = M.CatList[iCnt].Caption;
                lbi.Tag = M.CatList[iCnt].CategoryId;
                lbFilter2.Items.Add(lbi);
                //if (iCnt == 0)
                //{
                //    lbFilter2.SelectedItem = lbi;
                //}
                
            }
        }

        private void mediaElement1_MediaEnded(object sender, RoutedEventArgs e)
        {
            ImageBrush bkg = new ImageBrush();
            bkg.ImageSource = new BitmapImage(new Uri("MP_Button3.png", UriKind.Relative));
            currBtn.Background = bkg;
            currBtn = null;
        }

        private void mediaElement1_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ImageBrush bkg = new ImageBrush();
            bkg.ImageSource = new BitmapImage(new Uri("MP_Button3.png", UriKind.Relative));
            currBtn.Background = bkg;
            currBtn = null;
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem lbi = (ListBoxItem)((ListBox)sender).SelectedItem;
            strFilterCrt = lbi.Content.ToString() + " - All";
            txtFilter.Text = strFilterCrt;
            //M.SetMainCategory(Convert.ToInt32(lbi.Tag));
            BuildCatList2();
            //M.BuildCurrItemList();
            CleanButtons();
            AddButtons();
        }

        private void CleanButtons()
        {
            ContentPanel.Children.Clear();
            ContentPanel.RowDefinitions.Clear();
        }

        private void AddButtons()
        {
            int jCnt;
            int kCnt;
            Button btn;
            jCnt = 0;
            kCnt = 0;
            RowDefinition rd;
            rd = new RowDefinition();
            rd.MinHeight = 75;
            rd.MaxHeight = 75;
            ContentPanel.RowDefinitions.Add(rd);
            ImageBrush bkg = new ImageBrush();
            bkg.ImageSource = new BitmapImage(new Uri("MP_Button3.png", UriKind.Relative));
            Thickness tks = new Thickness(0.0);
            for (int iCnt = 1; iCnt <= M.CatPairs.Count; iCnt++)
            {
                rd = new RowDefinition();
                rd.MinHeight = 75;
                rd.MaxHeight = 75;
                ContentPanel.RowDefinitions.Add(rd);
                //iPairs ip = M.CatPairs[iCnt];

                btn = new Button();
                //btn.Content = M.ItemList[ip.iP1].Caption;
                btn.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                btn.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                btn.FontSize = 18;
                btn.Height = 75;
                btn.Width = 170;
                btn.Background = bkg;
                btn.ClickMode = ClickMode.Press;
                btn.BorderThickness = tks;
                btn.VerticalAlignment = VerticalAlignment.Stretch;
                //btn.Tag = M.ItemList[ip.iP1].File;
                btn.Click += new RoutedEventHandler(PlayWord);
                Grid.SetColumn(btn, 0);
                Grid.SetRow(btn, iCnt);
                ContentPanel.Children.Add(btn);

                btn = new Button();
                //btn.Content = M.ItemList[ip.iP2].Caption;
                btn.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                btn.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                btn.FontSize = 18;
                btn.Height = 75;
                btn.Width = 170;
                btn.Background = bkg;
                btn.ClickMode = ClickMode.Press;
                btn.BorderThickness = tks;
                btn.VerticalAlignment = VerticalAlignment.Stretch;
                //btn.Tag = M.ItemList[ip.iP2].File;
                btn.Click += new RoutedEventHandler(PlayWord);
                Grid.SetColumn(btn, 1);
                Grid.SetRow(btn, iCnt );
                ContentPanel.Children.Add(btn);


            } 
            rd = new RowDefinition();
            rd.MinHeight = 75;
            rd.MaxHeight = 75;
            ContentPanel.RowDefinitions.Add(rd);
            rd = new RowDefinition();
            rd.MinHeight = 75;
            rd.MaxHeight = 75;
            ContentPanel.RowDefinitions.Add(rd);
            rd = new RowDefinition();
            rd.MinHeight = 75;
            rd.MaxHeight = 75;
            ContentPanel.RowDefinitions.Add(rd);
        }

        private void PlayWord(object sender, RoutedEventArgs e)
        {
            ImageBrush bkg;
            if (currBtn != null)
            {
                bkg = new ImageBrush();
                bkg.ImageSource = new BitmapImage(new Uri("MP_Button3.png", UriKind.Relative));
                currBtn.Background = bkg;
            }
            bkg = new ImageBrush();
            bkg.ImageSource = new BitmapImage(new Uri("MP_Button3_P.png", UriKind.Relative));
            currBtn = (Button)sender;
            currBtn.Background = bkg;
            if (mediaElement1.CurrentState == MediaElementState.Playing)
            {
                mediaElement1.Stop();
            }
            mediaElement1.Source = new Uri("/Audio/" + currBtn.Tag.ToString(), UriKind.Relative);
            mediaElement1.Play();
        }

        private void btnFilterMove_Click(object sender, RoutedEventArgs e)
        {
            bFilterOpen = !bFilterOpen;
            SetupPage();
        }
        private void listBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem lbi = (ListBoxItem)((ListBox)sender).SelectedItem;
            if (lbi != null)
            {
                strFilterCrt = strFilterCrt.Substring(0,strFilterCrt.IndexOf("- ") + 2);
                strFilterCrt = strFilterCrt + lbi.Content.ToString();
                txtFilter.Text = strFilterCrt;
                //M.SetSecondaryCategory(Convert.ToInt32(lbi.Tag));
                //M.BuildCurrItemList();
                CleanButtons();
                AddButtons();
                bFilterOpen = false;
                SetupPage();
            }

        }
    }
}