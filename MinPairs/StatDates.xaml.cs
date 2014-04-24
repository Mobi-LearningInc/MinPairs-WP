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

namespace MinPairs
{
    public partial class StatDates : PhoneApplicationPage
    {
        ML_Main M;
        bool bIsDateUserChanged=false;
        public StatDates()
        {
            InitializeComponent();
            M = (ML_Main)PhoneApplicationService.Current.State["ML"];

            this.StartDate.Value = M.StatStartDate;
            this.EndDate.Value = M.StatEndDate;
            bIsDateUserChanged = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            M.StatStartDate = Convert.ToDateTime(StartDate.Value);
            M.StatEndDate = Convert.ToDateTime(EndDate.Value);

            NavigationService.Navigate(new Uri("/Stats.xaml", UriKind.Relative));
        }

        private void Date_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            if (bIsDateUserChanged)
            {
                M.IsDateNotChanged = false;
            }
            
        }
    }
}