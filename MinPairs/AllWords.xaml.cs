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
    public partial class AllWords : PhoneApplicationPage
    {
        ML_Main M;
        SolidColorBrush bb = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        SolidColorBrush bbp = new SolidColorBrush(Color.FromArgb(255, 22, 133, 197));
        MP_Button currBtn;
        public AllWords()
        {
            
            InitializeComponent();
            M = (ML_Main)PhoneApplicationService.Current.State["ML"];
            AddButtons();
            SetFormLayout();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void AddButtons()
        {
                  
            
            int jCnt;
            int kCnt;
            jCnt = 0;
            kCnt=0;
            RowDefinition rd;
            MP_Button b = new MP_Button();
            string sImageName = string.Empty;
            Thickness tks = new Thickness(2.0);
            for(int iCnt = 1; iCnt < M.CatList.Count; iCnt++)
            {
                if (jCnt == 0)
                {
                    rd = new RowDefinition();
                    rd.MinHeight = 220;
                    rd.MaxHeight = 220;
                    
                    ContentPanel.RowDefinitions.Add(rd);
                    kCnt++;
                }
                b=new MP_Button();
                

                b.BorderBrush = bbp;
                b.BorderThickness=tks;
                b.MyWord.Text = M.CatList[iCnt].Caption;
                b.Tag = M.CatList[iCnt].CategoryId;
                if(M.CatList[iCnt].Image.Length > 0)
                {
                    sImageName = M.CatList[iCnt].Image;
                    if (Application.GetResourceStream(new Uri("img/" + sImageName, UriKind.Relative)) != null)
                    {
                        b.WordImage.Source = new BitmapImage(new Uri("/img/" + sImageName, UriKind.Relative));
                    }
                }
                b.ImageName = sImageName;
                b.Tap+=new EventHandler<System.Windows.Input.GestureEventArgs>(PlayWord);
                //b.WordImage.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(PlayWord);
                Grid.SetColumn(b, jCnt);
                Grid.SetRow(b, kCnt);
                ContentPanel.Children.Add(b);
                jCnt++;
                if (jCnt == 3) { jCnt = 0; }

            }
            rd = new RowDefinition();
            rd.MinHeight = 220;
            rd.MaxHeight = 220;
            ContentPanel.RowDefinitions.Add(rd);
            rd = new RowDefinition();
            rd.MinHeight = 75;
            rd.MaxHeight = 75;
            ContentPanel.RowDefinitions.Add(rd);
            rd = new RowDefinition();
            rd.MinHeight = 280;
            rd.MaxHeight = 280;
            ContentPanel.RowDefinitions.Add(rd);

        }


        private void PlayWord(object sender, RoutedEventArgs e)
        {

            string sFileName = string.Empty;
            string sImageName = string.Empty;
            string sCaption;
            int iCatId;
            MP_Button newBtn = (MP_Button)sender;
            if (currBtn!=null && newBtn.Tag != currBtn.Tag)
            {
                iCatId = Convert.ToInt32(currBtn.Tag);
                currBtn.ImageName = M.CatList[iCatId].Image;
                currBtn.Caption = M.CatList[iCatId].Caption;
                currBtn.Flip();
            }
            
            currBtn  = newBtn;
            iCatId = Convert.ToInt32(currBtn.Tag);
            M.ResetCategories(iCatId);
            int iSoundId = M.CatList[iCatId].GetAudioId();
            
            if (iSoundId == -1)
            {
                sFileName = M.CatList[iCatId].Audio;
                sImageName = M.ItemList[M.CatList[iCatId].GetNextWordId()].Image;
                sCaption = M.ItemList[M.CatList[iCatId].GetNextWordId()].Caption;

            }
            else
            {
                sFileName = M.ItemList[iSoundId].Audio;
                //if (M.CatList[iCatId].Image.Length > 0)
                //{
                //    sImageName = M.CatList[iCatId].Image;
                //    //if (Application.GetResourceStream(new Uri("img/" + sImageName, UriKind.Relative)) != null)
                //    //{
                //    //    currBtn.WordImage.Source = new BitmapImage(new Uri("/img/" + sImageName, UriKind.Relative));
                //    //}
                //}
                sImageName = M.CatList[iCatId].Image;
                sCaption = M.CatList[iCatId].Caption;
            }

            currBtn.Caption = sCaption;
            currBtn.ImageName = sImageName;
            if (mediaElement1.CurrentState == MediaElementState.Playing)
            {
                mediaElement1.Stop();
            }

            mediaElement1.Source = new Uri("/Audio/" + sFileName, UriKind.Relative);
            mediaElement1.Play();
            
        }

        private void mediaElement1_MediaEnded(object sender, RoutedEventArgs e)
        {
            currBtn.Flip();
            
            mediaElement1.Source = null;
        }

        private void mediaElement1_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            currBtn.Flip();
            mediaElement1.Source = null;
        }

        
        

        private void SetFormLayout()
        {
            this.ContentPanel.ColumnDefinitions[0].MinWidth = ML_Main.DAS.dX / 3 - 3;
            this.ContentPanel.ColumnDefinitions[0].MaxWidth = ML_Main.DAS.dX / 3 - 3;
            this.ContentPanel.ColumnDefinitions[1].MinWidth = ML_Main.DAS.dX / 3 - 3;
            this.ContentPanel.ColumnDefinitions[1].MaxWidth = ML_Main.DAS.dX / 3 - 3;
            this.ContentPanel.ColumnDefinitions[2].MinWidth = ML_Main.DAS.dX / 3 - 3;
            this.ContentPanel.ColumnDefinitions[2].MaxWidth = ML_Main.DAS.dX / 3 - 3;
            //this.ContentPanel.ColumnDefinitions[3].MinWidth = ML_Main.DAS.dX / 4;
           // this.ContentPanel.ColumnDefinitions[3].MaxWidth = ML_Main.DAS.dX / 4;
            
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Stats.xaml", UriKind.Relative));
        }
        
    }
}