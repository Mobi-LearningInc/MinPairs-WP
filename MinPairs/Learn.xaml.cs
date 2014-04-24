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
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Specialized;

namespace MinPairs
{
    public partial class Learn : PhoneApplicationPage
    {
        ML_Main M;
        OneFilter myF;
        ML_Filter F;
        MP_Button btn;
        SolidColorBrush bbp = new SolidColorBrush(Color.FromArgb(255, 22, 133, 197));
        public Learn()
        {
            InitializeComponent();
            M = (ML_Main) PhoneApplicationService.Current.State["ML"];
            F = (ML_Filter)PhoneApplicationService.Current.State["MF"];
            myF = F.GetFilter("MP");
            //btnMainFilter.Content = myF.FilterTitle;
            FilterText.Text = myF.FilterTitle;
            LoadPairs();

        }
        
        private void LoadPairs()
        {
            MP_Item W;
            MP_Button currBtn;
            RowDefinition rd;
            for (int iCnt = 1; iCnt <= myF.MyPairs.Count; iCnt++)
            {

                rd = new RowDefinition();
                rd.MinHeight = 210;
                rd.MaxHeight = 210;
                ContentGrid.RowDefinitions.Add(rd);

                W = M.ItemList[M.ItemPairs[myF.MyPairs[iCnt]].iP1];
                currBtn = new MP_Button();
                Thickness tks = new Thickness(2.0);
                currBtn.BorderBrush = bbp;
                currBtn.BorderThickness = tks;
                currBtn.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(ht_Tap);
                currBtn.MyWord.Text = W.Caption;
                currBtn.Caption = W.Caption;
                if (W.Image.Length > 0)
                {
                    if (Application.GetResourceStream(new Uri("img/" + W.Image, UriKind.Relative)) != null)
                    {
                        currBtn.WordImage.Source = new BitmapImage(new Uri("/img/" + W.Image, UriKind.Relative));
                        
                    }
                }
                currBtn.ImageName = W.Image;
                currBtn.Tag = W.Audio;
                Grid.SetColumn(currBtn, 0);
                Grid.SetRow(currBtn, iCnt - 1);
                ContentGrid.Children.Add(currBtn);
                currBtn.MyWord.Text = W.Caption;
                W = M.ItemList[M.ItemPairs[myF.MyPairs[iCnt]].iP2];
                currBtn = new MP_Button();
                
                currBtn.BorderBrush = bbp;
                currBtn.BorderThickness = tks;
                currBtn.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(ht_Tap);
                currBtn.MyWord.Text = W.Caption;
                currBtn.Caption = W.Caption;
                if (W.Image.Length > 0)
                {
                    if (Application.GetResourceStream(new Uri("img/" + W.Image, UriKind.Relative)) != null)
                    {
                        currBtn.WordImage.Source = new BitmapImage(new Uri("/img/" + W.Image, UriKind.Relative));
                    }
                }
                currBtn.ImageName = W.Image;

                currBtn.Tag = W.Audio;
                Grid.SetColumn(currBtn, 2);
                Grid.SetRow(currBtn, iCnt - 1);
                ContentGrid.Children.Add(currBtn);

            }
            rd = new RowDefinition();
            rd.MinHeight = 150;
            rd.MaxHeight = 150;
            ContentGrid.RowDefinitions.Add(rd);
        }

        void ht_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
             btn = (MP_Button)sender;
            string sFileName = string.Empty;
            sFileName = btn.Tag.ToString();
            if (mediaElement1.CurrentState == MediaElementState.Playing)
            {
                mediaElement1.Stop();
            }
            mediaElement1.Source = new Uri("/Audio/" + sFileName, UriKind.Relative);
            mediaElement1.Play();
        }
        
        private void RenderPairs()
        {

        }
        
        private void FlipFilterLists(bool bMain, bool bSec)
        {
            
            
        }


        private void mediaElement1_MediaEnded(object sender, RoutedEventArgs e)
        {
            btn.Flip();
        }

        private void mediaElement1_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            btn.Flip();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Stats.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            //ShowFilter();
            M.strFilterCallingPage = "Learn";
            NavigationService.Navigate(new Uri("/MP_Filter.xaml", UriKind.Relative));
        }
    }
}