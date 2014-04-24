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
using Microsoft.Phone.BackgroundAudio;
using System.IO.IsolatedStorage;
using System.Windows.Resources;
using Microsoft.Phone.Shell;
namespace MinPairs
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        //MP_Comm MPC;
        ML_Main M;
        OneFilter myF;
        ML_Filter F;
        public MainPage()
        {
            InitializeComponent();
            M = (ML_Main)PhoneApplicationService.Current.State["ML"];
            F = (ML_Filter)PhoneApplicationService.Current.State["MF"];
            myF = F.GetFilter("MP");
            this.button1.Width = ML_Main.DAS.dX - 20;
            this.button2.Width = ML_Main.DAS.dX - 20;
            this.button3.Width = ML_Main.DAS.dX - 20;
            this.button4.Width = ML_Main.DAS.dX - 20;
            this.button5.Width = ML_Main.DAS.dX - 20;
            
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //MPC = (MP_Comm)PhoneApplicationService.Current.State["MC"];
            //if (!MPC.bIsReady)
            //{
            //    NavigationService.Navigate(new Uri("/Start.xaml", UriKind.Relative));
            //}
        }
        

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            //ImageBrush bkg = new ImageBrush();
            //bkg.ImageSource = new BitmapImage(new Uri("MP_Button3_P.png", UriKind.Relative));
            //button1.Background = bkg;
            
            NavigationService.Navigate(new Uri("/AllWords.xaml", UriKind.Relative));
            
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {

            //ImageBrush bkg = new ImageBrush();
            //bkg.ImageSource = new BitmapImage(new Uri("MP_Button3_P.png", UriKind.Relative));
            //button2.Background = bkg;
            if (myF.IsFilterSet)
            {
                NavigationService.Navigate(new Uri("/Learn.xaml", UriKind.Relative));
            }
            else
            {
                M.strFilterCallingPage = "Learn";
                NavigationService.Navigate(new Uri("/MP_Filter.xaml", UriKind.Relative));
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {

            //ImageBrush bkg = new ImageBrush();
            //bkg.ImageSource = new BitmapImage(new Uri("MP_Button3_P.png", UriKind.Relative));
            //button3.Background = bkg;
            if (myF.IsFilterSet)
            {
                NavigationService.Navigate(new Uri("/Practice.xaml", UriKind.Relative));
            }
            else
            {
                M.strFilterCallingPage = "Practice";
                NavigationService.Navigate(new Uri("/MP_Filter.xaml", UriKind.Relative));
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {

            //ImageBrush bkg = new ImageBrush();
            //bkg.ImageSource = new BitmapImage(new Uri("MP_Button3_P.png", UriKind.Relative));
            //button4.Background = bkg;
            if (myF.IsFilterSet)
            {
                NavigationService.Navigate(new Uri("/Quizzes.xaml", UriKind.Relative));
            }
            else
            {
                M.strFilterCallingPage = "Quizzes";
                NavigationService.Navigate(new Uri("/MP_Filter.xaml", UriKind.Relative));
            }
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new Uri("/Stats.xaml", UriKind.Relative));
        }

        private void button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button)sender;
            ImageBrush bkg = new ImageBrush();
            bkg.ImageSource = new BitmapImage(new Uri("MP_Button3_P.png", UriKind.Relative));
            btn.Background = bkg;
        }

        private void button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button)sender;
            ImageBrush bkg = new ImageBrush();
            bkg.ImageSource = new BitmapImage(new Uri("MP_Button3.png", UriKind.Relative));
            btn.Background = bkg;
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Stats.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }
        private void ApplicationBarIconButton_Click_3(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        


        

        
    }
}