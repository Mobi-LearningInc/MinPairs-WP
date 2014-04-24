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

using System.Windows.Data;

using Telerik.Charting;
using Telerik.Windows.Controls;



namespace MinPairs
{
    public partial class Stats : PhoneApplicationPage
    {


        ML_Main M;
        MP_Stats myStats;
        List<MP_PieSoundStats> currPracticePie;
        List<MP_PieSoundStats> currQuizzPie;
        List<MP_LineSoundStats> currPracticeLine;
        List<MP_LineSoundStats> currQuizzLine;
        int iQType = 0;
        int iSoundId = 0;


        // test data;
        public class MyClass
        {
            public int Value
            {
                get;
                set;
            }

            public string Text
            {
                get;
                set;
            }
        }
        List<MyClass> tstData = new List<MyClass>();
        private void GetTestData()
        {
            MyClass c = new MyClass();
            c.Text = Environment.NewLine + Environment.NewLine + "Label1";
            c.Value = 40;
            tstData.Add(c);
            c = new MyClass();
            c.Value = 60;
            c.Text = Environment.NewLine + Environment.NewLine + "Label 2";
            tstData.Add(c);
            BuildLayout();
        }
        public Stats()
        {
            InitializeComponent();
            M = (ML_Main)PhoneApplicationService.Current.State["ML"];
            myStats = new MP_Stats();
            BuildSounds();
            GetData();
            //GetTestData();

        }
        private void BuildLayout()
        {
            ChartSeriesLabelDefinition lbDef;
            if (currPracticePie != null)
            {
                PracticePie.Series[0].ValueBinding = new GenericDataPointBinding<MP_PieSoundStats, uint>() { ValueSelector = MP_PieSoundStats => MP_PieSoundStats.SoundWeight };
                lbDef = new ChartSeriesLabelDefinition();
                //lbDef.Binding = new GenericDataPointBinding<MP_PieSoundStats, string>() { ValueSelector = MP_PieSoundStats => MP_PieSoundStats.SoundDescription };
                PracticePie.Series[0].LabelDefinitions.Add(lbDef);
                PracticePie.Visibility = Visibility.Visible;
                nodataprctice.Visibility = Visibility.Collapsed;
            }
            else
            {
                PracticePie.Visibility = Visibility.Collapsed;
                nodataprctice.Visibility = Visibility.Visible;
            }
            PracticePie.Series[0].ItemsSource = currPracticePie;


            SplineSeries ss = (SplineSeries)PracticeLine.Series[0];
            ss.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "SessionDate" };
            ss.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "CorrectPct" };
            PracticeLine.Series[0].ItemsSource = currPracticeLine;

            if (currQuizzPie != null)
            {
                QuizzPie.Series[0].ValueBinding = new GenericDataPointBinding<MP_PieSoundStats, uint>() { ValueSelector = MP_PieSoundStats => MP_PieSoundStats.SoundWeight };
                lbDef = new ChartSeriesLabelDefinition();
                //lbDef.Binding = new GenericDataPointBinding<MP_PieSoundStats, string>() { ValueSelector = MP_PieSoundStats => MP_PieSoundStats.SoundDescription };
                QuizzPie.Series[0].LabelDefinitions.Add(lbDef);
                QuizzPie.Visibility = Visibility.Visible;
                nodataquiz.Visibility = Visibility.Collapsed;

            }
            else
            {
                QuizzPie.Visibility = Visibility.Collapsed;
                nodataquiz.Visibility = Visibility.Visible;
            }
            QuizzPie.Series[0].ItemsSource = currQuizzPie;

            SplineSeries sd = (SplineSeries)QuizzLine.Series[0];
            sd.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "SessionDate" };
            sd.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "CorrectPct" };
            QuizzLine.Series[0].ItemsSource = currQuizzLine;
            
        }
        

        void currRB_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Button btn = (Button)sender;
            iSoundId = Convert.ToInt32(btn.Tag);
            GetData();
            statPivot.SelectedIndex = 1;
            
        }
        private void BuildSounds()
        {
            Button btnSound;

            ColumnDefinition cd = new ColumnDefinition();
            cd.MinWidth = ML_Main.DAS.dX / 2 - 10;
            cd.MaxWidth = ML_Main.DAS.dX / 2 - 10;
            Sounds.ColumnDefinitions.Add(cd);

            cd = new ColumnDefinition();
            cd.MinWidth = ML_Main.DAS.dX / 2 - 10;
            cd.MaxWidth = ML_Main.DAS.dX / 2 - 10;
            Sounds.ColumnDefinitions.Add(cd);

            //cd = new ColumnDefinition();
            //cd.MinWidth = ML_Main.DAS.dX / 2 - 10;
            //cd.MaxWidth = ML_Main.DAS.dX / 2 - 10;
            //QTypes.ColumnDefinitions.Add(cd);

            //cd = new ColumnDefinition();
            //cd.MinWidth = ML_Main.DAS.dX / 2 - 10;
            //cd.MaxWidth = ML_Main.DAS.dX / 2 - 10;
            //QTypes.ColumnDefinitions.Add(cd);

            //RadioButton rb = new RadioButton();
            //rb.Tag = "0";
            //rb.Content = "All Question Types";
            //Grid.SetColumn(rb, 0);
            //Grid.SetRow(rb, 0);
            //QTypes.Children.Add(rb);

            //rb = new RadioButton();
            //rb.Tag = "1";
            //rb.Content = "Listen & Select";
            //Grid.SetColumn(rb, 1);
            //Grid.SetRow(rb, 0);
            //QTypes.Children.Add(rb);

            //rb = new RadioButton();
            //rb.Tag = "2";
            //rb.Content = "Listen & Type";
            //Grid.SetColumn(rb, 0);
            //Grid.SetRow(rb, 1);
            //QTypes.Children.Add(rb);

            //rb = new RadioButton();
            //rb.Tag = "3";
            //rb.Content = "Read Listen & Select";
            //Grid.SetColumn(rb, 1);
            //Grid.SetRow(rb, 1);
            //QTypes.Children.Add(rb);



            RowDefinition rd = new RowDefinition();
            Sounds.RowDefinitions.Add(rd);


            bool bCol = false;
            int jCnt = 0;
            btnSound = new Button();
            btnSound.Content = "All" + "\t\t\t" + ">";
            btnSound.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            btnSound.Tag = 0;
            btnSound.Width = ML_Main.DAS.dX / 2 - 20;
            btnSound.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(currRB_Tap);
            Grid.SetRow(btnSound, 0);
            Grid.SetColumn(btnSound, 0);
            Sounds.Children.Add(btnSound);
            for (int iCnt = 1; iCnt <= M.CatList.Count-1; iCnt++)
            {
                if (bCol)
                {
                    rd = new RowDefinition();
                    Sounds.RowDefinitions.Add(rd);
                    jCnt++;
                }
                btnSound = new Button();
                btnSound.Content = M.CatList[iCnt].Caption + "\t\t\t" + ">";
                btnSound.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                btnSound.Tag = M.CatList[iCnt].CategoryId;
                btnSound.Width = ML_Main.DAS.dX / 2 - 20;
                btnSound.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(currRB_Tap);
                Grid.SetRow(btnSound, jCnt);
                if (bCol)
                {
                    Grid.SetColumn(btnSound, 0);
                }
                else
                {
                    Grid.SetColumn(btnSound, 1);
                }
                Sounds.Children.Add(btnSound);
                bCol = !bCol;
            }
        }
        private void GetData()
        {
            myStats.MyMain = M;
            myStats.SelectData(iSoundId, iQType);
            //myStats.ComputeData(iSoundId);
            currPracticePie = myStats.PracticePieStats;
            currQuizzPie = myStats.QuizzPieStats;
            currPracticeLine = myStats.PracticeLineStats;
            currQuizzLine = myStats.QuizzLineStats;
            string sFilter = string.Empty;
            string sFilterDates = M.StatStartDate.ToShortDateString() + " - " + M.StatEndDate.ToShortDateString();
            if (currPracticePie != null)
            {
                sFilter = "Statistics for " + currPracticePie[0].SoundDescription + " - " + txtQTypes.Text;
                this.txtPrctice.Text = sFilter;
                this.txtPrcticeDates.Text = sFilterDates;
            }
            else
            {
                this.txtPrctice.Text = "Statistics not available";
                this.txtPrcticeDates.Text = string.Empty;
            }
            if (currQuizzPie != null)
            {
                sFilter = "Statistics for " + currQuizzPie[0].SoundDescription + " - " + txtQTypes.Text;
                this.txtQuizz.Text = sFilter;
                txtQuizzDates.Text = sFilterDates;
            }
            else
            {
                this.txtQuizz.Text = "Statistics not available";
                txtQuizzDates.Text = string.Empty;
            }
            
            BuildLayout();
        }
        private void menuItem1_Click(object sender, EventArgs e)
        {
            
            NavigationService.Navigate(new Uri("/StatDates.xaml", UriKind.Relative));
        }
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }


        private void RadContextMenuItem_Tapped(object sender, ContextMenuItemSelectedEventArgs e)
        {
            RadContextMenuItem cmi=(RadContextMenuItem) sender;
            this.txtQTypes.Text = cmi.Content.ToString();
            iQType = Convert.ToInt32(cmi.Tag);
            GetData();
            statPivot.SelectedIndex = 1;
        }

        
        
    }
}