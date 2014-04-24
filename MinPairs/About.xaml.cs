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
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace MinPairs
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {

            //PhoneApplicationService.Current.State["ActiveItem"] = "";

              

            //Create a new task

            EmailComposeTask task = new EmailComposeTask();

            //Add the current item’s EMail address

            task.To = "feedback@mobilearninginc.com";

            //Just a little text for the message

            task.Subject = "Feedback for Minimal Pairs app";

            //Launch the task

            task.Show();

        }

    }
}