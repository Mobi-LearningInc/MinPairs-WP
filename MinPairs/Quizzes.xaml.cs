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

namespace MinPairs
{
    public partial class Quizzes : PhoneApplicationPage
    {
        ML_Main M;
        OneFilter myF;
        ML_Filter F;
        MP_AllQuestions AllQ;
        MP_Question myQ;
        MP_Button currBtn;
        RowDefinition rd;
        ColumnDefinition cd;
        Button btnPlay;
        Button btnAnswer;
        TextBox txtBox;
        TextBlock txtBlock;
        RadioButton radBtn1;
        RadioButton radBtn2;
        string sWord1 = string.Empty;
        string sWord2 = string.Empty;

        string sCtrlIdBase = "C";
        int iCtrlId = 0;
        int iCtrlMinorId = 0;
        string sCtrlId = string.Empty;

        SolidColorBrush btnBrdNormal = new SolidColorBrush(Color.FromArgb(198, 0, 0, 0));
        SolidColorBrush btnBrdHigh = new SolidColorBrush(Color.FromArgb(198, 255, 255, 255));
        ImageBrush brPlay = new ImageBrush();

        List<MP_Button> MPBtnGroup = new List<MP_Button>();
        int iItemId = 0;

        string sHelp = "This will explain how the quizzes screen works.......";

        public Quizzes()
        {
            InitializeComponent();
            //ShowHelp(null, null);
            M = (ML_Main)PhoneApplicationService.Current.State["ML"];
            F = (ML_Filter)PhoneApplicationService.Current.State["MF"];
            myF = F.GetFilter("MP");
            AllQ = new MP_AllQuestions(true, myF.CatPairId);
            
            AllQ.SetSize = 10;
            //AllQ.SetFull += new MPSessionEndEventHandler(AllQ_SetFull);
            //btnMainFilter.Content = myF.FilterTitle;
            GetOneQuestion();
            
        }
        void AllQ_SetFull()
        {
            CleanLayout();
            
            MessageBox.Show(AllQ.FullMessage);

            myQ = null;

            AllQ = null;
            MessageBoxResult res = MessageBox.Show("New test", "", MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                res = MessageBox.Show("New contrast?", "", MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    //AllQ = null;
                    M.strFilterCallingPage = "Quizzes";
                    NavigationService.Navigate(new Uri("/MP_Filter.xaml", UriKind.Relative));
                }
                else
                {

                    AllQ = new MP_AllQuestions(true, myF.CatPairId);
                    AllQ.SetSize = 10;
                    GetOneQuestion();
                }
            }
            else
            {

                M.strFilterCallingPage = "";
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }

        }

        void myQ_TimeOut(object sender, MPTimeOutEventArgs e)
        {
            if (myQ != null)
            {
                myQ.CheckAnswer(false);
                if (AllQ != null)
                {
                    AllQ.SaveAnswer(myQ);
                }
                
                GetOneQuestion();
            }
        }
        private void BuildLayout()
        {
            string sInfoText = string.Empty;
            iCtrlId++;
            iCtrlMinorId = 0;
            iCtrlMinorId++;

            sCtrlId = sCtrlIdBase + iCtrlId.ToString() + iCtrlMinorId.ToString();
            Content.Children.Clear();
            Content.RowDefinitions.Clear();
            Content.ColumnDefinitions.Clear();

            switch (myQ.QuestionType)
            {
                case MPQuestionType.ListenSelect:
                
                    #region ListenSelect
                    // render two buttons for selection one play button

                    sInfoText = "Listen the word and select the right option";

                    cd = new ColumnDefinition();
                    cd.MinWidth = ML_Main.DAS.dX / 2;
                    cd.MaxWidth = ML_Main.DAS.dX / 2;
                    Content.ColumnDefinitions.Add(cd);

                    cd = new ColumnDefinition();
                    cd.MinWidth = ML_Main.DAS.dX / 2;
                    cd.MaxWidth = ML_Main.DAS.dX / 2;
                    Content.ColumnDefinitions.Add(cd);

                    rd = new RowDefinition();   // the row for question description
                    rd.MinHeight = 80;
                    rd.MaxHeight = 80;
                    Content.RowDefinitions.Add(rd);
                    // set the info text
                    txtBlock = new TextBlock();
                    txtBlock.Text = sInfoText;
                    txtBlock.VerticalAlignment = VerticalAlignment.Bottom;
                    txtBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetColumnSpan(txtBlock, 2);
                    Grid.SetColumn(txtBlock, 0);
                    Grid.SetRow(txtBlock, 0);
                    Content.Children.Add(txtBlock);


                    rd = new RowDefinition();
                    rd.MinHeight = 120;
                    rd.MaxHeight = 120;
                    Content.RowDefinitions.Add(rd);

                    btnPlay = new Button();
                    btnPlay.Name = sCtrlId;
                    //currBtn.WordImage.Source = new BitmapImage(new Uri("/Image/Play2.Png", UriKind.Relative));
                    brPlay.ImageSource=new BitmapImage(new Uri("/Image/Play.Png", UriKind.Relative));
                    brPlay.Stretch = Stretch.None;
                    btnPlay.Background = brPlay;
                    btnPlay.Width = 400;
                    btnPlay.Height = 120;
                    btnPlay.BorderThickness = new Thickness(3);
                    btnPlay.BorderBrush = btnBrdHigh;
                    btnPlay.VerticalAlignment = VerticalAlignment.Bottom;
                    btnPlay.HorizontalAlignment = HorizontalAlignment.Center;
                    if (myQ.CorrectAnswer == 1)
                    {
                        btnPlay.Tag = M.ItemPairs[myQ.PairId].iP1;
                    }
                    else
                    {
                        btnPlay.Tag = M.ItemPairs[myQ.PairId].iP2;
                    }
                    btnPlay.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(Play);
                    Grid.SetColumn(btnPlay, 0);
                    Grid.SetColumnSpan(btnPlay, 2);
                    Grid.SetRow(btnPlay, 1);
                    Content.Children.Add(btnPlay);


                    rd = new RowDefinition();   // the row for audio play control(s)
                    rd.MinHeight = 210;
                    rd.MaxHeight = 210;
                    Content.RowDefinitions.Add(rd);

                    iCtrlMinorId++;
                    sCtrlId = sCtrlIdBase + iCtrlId.ToString() + iCtrlMinorId.ToString();
                    currBtn = new MP_Button();
                    currBtn.Name = sCtrlId;
                    currBtn.MyWord.Text = M.ItemList[M.ItemPairs[myQ.PairId].iP1].Caption;
                    currBtn.WordImage.Source = GetImage(M.ItemPairs[myQ.PairId].iP1, 1);
                    currBtn.HorizontalAlignment = HorizontalAlignment.Center;
                    currBtn.VerticalAlignment = VerticalAlignment.Bottom;
                    currBtn.Tag = 1;
                    currBtn.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(SelectAnswer);
                    Grid.SetColumn(currBtn, 0);
                    Grid.SetRow(currBtn, 2);
                    Content.Children.Add(currBtn);
                    MPBtnGroup.Add(currBtn);

                    iCtrlMinorId++;
                    sCtrlId = sCtrlIdBase + iCtrlId.ToString() + iCtrlMinorId.ToString();
                    currBtn = new MP_Button();
                    currBtn.Name = sCtrlId;
                    currBtn.MyWord.Text = M.ItemList[M.ItemPairs[myQ.PairId].iP2].Caption;
                    currBtn.WordImage.Source = GetImage(M.ItemPairs[myQ.PairId].iP2, 1);
                    currBtn.HorizontalAlignment = HorizontalAlignment.Center;
                    currBtn.VerticalAlignment = VerticalAlignment.Bottom;
                    currBtn.Tag = 2;
                    currBtn.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(SelectAnswer);
                    Grid.SetColumn(currBtn, 1);
                    Grid.SetRow(currBtn, 2);
                    Content.Children.Add(currBtn);
                    MPBtnGroup.Add(currBtn);


                    rd = new RowDefinition();
                    rd.MinHeight = 70;
                    rd.MaxHeight = 70;
                    Content.RowDefinitions.Add(rd);

                    btnPlay = new Button();
                    btnPlay.Content = "Answer";
                    btnPlay.Width = 400;
                    btnPlay.Height = 70;
                    if (myQ.CorrectAnswer == 1)
                    {
                        btnPlay.Tag = M.ItemPairs[myQ.PairId].iP1;
                    }
                    else
                    {
                        btnPlay.Tag = M.ItemPairs[myQ.PairId].iP2;
                    }
                    btnPlay.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(Answer);
                    btnPlay.HorizontalAlignment = HorizontalAlignment.Center;
                    btnPlay.VerticalAlignment = VerticalAlignment.Bottom;
                    Grid.SetRow(btnPlay, 3);
                    Grid.SetColumn(btnPlay, 0);
                    Grid.SetColumnSpan(btnPlay, 2);
                    Content.Children.Add(btnPlay);

                    #endregion
                    break;
                case MPQuestionType.ListenType:

                    #region Listen Type
                    //PracticeInfo.Text = "Listen to a word and type it";
                    sInfoText = "Listen to a word and type it";

                    cd = new ColumnDefinition();
                    cd.MinWidth = ML_Main.DAS.dX / 2;
                    cd.MaxWidth = ML_Main.DAS.dX / 2;
                    Content.ColumnDefinitions.Add(cd);

                    cd = new ColumnDefinition();
                    cd.MinWidth = ML_Main.DAS.dX / 2;
                    cd.MaxWidth = ML_Main.DAS.dX / 2;
                    Content.ColumnDefinitions.Add(cd);


                    rd = new RowDefinition();   // the row for question description
                    rd.MinHeight = 80;
                    rd.MaxHeight = 80;
                    Content.RowDefinitions.Add(rd);
                    // set the info text
                    txtBlock = new TextBlock();
                    txtBlock.Text = sInfoText;
                    txtBlock.VerticalAlignment = VerticalAlignment.Bottom;
                    txtBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetColumnSpan(txtBlock, 2);
                    Grid.SetColumn(txtBlock, 0);
                    Grid.SetRow(txtBlock, 0);
                    Content.Children.Add(txtBlock);


                    rd = new RowDefinition();
                    rd.MinHeight = 120;
                    rd.MaxHeight = 120;
                    Content.RowDefinitions.Add(rd);

                    sCtrlId = sCtrlIdBase + iCtrlId.ToString() + iCtrlMinorId.ToString();
                    btnPlay = new Button();
                    btnPlay.Name = sCtrlId;
                    //currBtn.WordImage.Source = new BitmapImage(new Uri("/Image/Play2.Png", UriKind.Relative));
                    brPlay.ImageSource=new BitmapImage(new Uri("/Image/Play.Png", UriKind.Relative));
                    brPlay.Stretch = Stretch.None;
                    btnPlay.Background = brPlay;
                    btnPlay.Width = 400;
                    btnPlay.Height = 120;
                    btnPlay.BorderBrush = btnBrdHigh;
                    btnPlay.BorderThickness = new Thickness(3);
                    btnPlay.VerticalAlignment = VerticalAlignment.Bottom;
                    btnPlay.HorizontalAlignment = HorizontalAlignment.Center;
                    if (myQ.CorrectAnswer == 1)
                    {
                        btnPlay.Tag = M.ItemPairs[myQ.PairId].iP1;
                    }
                    else
                    {
                        btnPlay.Tag = M.ItemPairs[myQ.PairId].iP2;
                    }
                    btnPlay.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(Play);
                    Grid.SetColumn(btnPlay, 0);
                    Grid.SetColumnSpan(btnPlay, 2);
                    Grid.SetRow(btnPlay, 1);
                    Content.Children.Add(btnPlay);


                    rd = new RowDefinition();   // the row for question description
                    rd.MinHeight = 110;
                    rd.MaxHeight = 110;
                    Content.RowDefinitions.Add(rd);
                    txtBlock = new TextBlock();
                    txtBlock.Padding = new Thickness(20, 0, 0, 0);
                    txtBlock.Text = "Answer:";
                    txtBlock.VerticalAlignment = VerticalAlignment.Bottom;
                    txtBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    Grid.SetColumn(txtBlock, 0);
                    Grid.SetRow(txtBlock, 2);
                    Content.Children.Add(txtBlock);

                    rd = new RowDefinition();   // the row for question description
                    rd.MinHeight = 100;
                    rd.MaxHeight = 100;
                    Content.RowDefinitions.Add(rd);
                    txtBox = new TextBox();
                    txtBox.Width = 400;
                    txtBox.Height = 90;
                    txtBox.VerticalAlignment = VerticalAlignment.Bottom;
                    txtBox.HorizontalAlignment = HorizontalAlignment.Center;
                    txtBox.KeyUp += new KeyEventHandler(txtBox_KeyUp);
                    Grid.SetColumn(txtBox, 0);
                    Grid.SetRow(txtBox, 3);
                    Grid.SetColumnSpan(txtBox, 3);
                    Content.Children.Add(txtBox);


                    rd = new RowDefinition();
                    rd.MinHeight = 70;
                    rd.MaxHeight = 70;
                    Content.RowDefinitions.Add(rd);

                    btnAnswer = new Button();
                    btnAnswer.Content = "Answer";
                    btnAnswer.Width = 400;
                    btnAnswer.Height = 70;
                    btnAnswer.VerticalAlignment = VerticalAlignment.Bottom;
                    btnAnswer.HorizontalAlignment = HorizontalAlignment.Center;
                    btnAnswer.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(Answer);
                    Grid.SetRow(btnAnswer, 4);
                    Grid.SetColumn(btnAnswer, 0);
                    Grid.SetColumnSpan(btnAnswer, 3);
                    Content.Children.Add(btnAnswer);

                    #endregion
                    break;


                case MPQuestionType.ReadListenSelect:
                    #region Read Listen Select
                    if (myQ.CorrectAnswer == 1)
                    {
                        sInfoText = "Listen to both words and select the one saying " + M.ItemList[M.ItemPairs[myQ.PairId].iP1].Caption;
                    }
                    else
                    {
                        sInfoText = "Listen to both words and select the one saying " + M.ItemList[M.ItemPairs[myQ.PairId].iP2].Caption;
                    }
                    
                    cd = new ColumnDefinition();
                    cd.MinWidth = ML_Main.DAS.dX / 2;
                    cd.MaxWidth = ML_Main.DAS.dX / 2;
                    Content.ColumnDefinitions.Add(cd);
                    
                    cd = new ColumnDefinition();
                    cd.MinWidth = ML_Main.DAS.dX / 2;
                    cd.MaxWidth = ML_Main.DAS.dX / 2;
                    Content.ColumnDefinitions.Add(cd);


                   rd = new RowDefinition();   // the row for question description
                    rd.MinHeight = 80;
                    rd.MaxHeight = 80;
                    Content.RowDefinitions.Add(rd);
                    // set the info text
                    txtBlock = new TextBlock();
                    txtBlock.Text = sInfoText;
                    txtBlock.VerticalAlignment = VerticalAlignment.Bottom;
                    txtBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetColumnSpan(txtBlock, 2);
                    Grid.SetColumn(txtBlock, 0);
                    Grid.SetRow(txtBlock, 0);
                    Content.Children.Add(txtBlock);


                    rd = new RowDefinition();   
                    rd.MinHeight = 120;
                    rd.MaxHeight = 120;
                    Content.RowDefinitions.Add(rd);

                    sCtrlId = sCtrlIdBase + iCtrlId.ToString() + iCtrlMinorId.ToString();

                    btnPlay = new Button();
                    btnPlay.Name = sCtrlId;
                    //currBtn.WordImage.Source = new BitmapImage(new Uri("/Image/Play2.Png", UriKind.Relative));
                    brPlay.ImageSource=new BitmapImage(new Uri("/Image/Play.Png", UriKind.Relative));
                    brPlay.Stretch = Stretch.None;
                    btnPlay.Background = brPlay;
                    btnPlay.Width = 200;
                    btnPlay.Height = 120;
                    btnPlay.BorderThickness = new Thickness(3);
                    btnPlay.BorderBrush = btnBrdHigh;
                    btnPlay.VerticalAlignment = VerticalAlignment.Bottom;
                    btnPlay.HorizontalAlignment = HorizontalAlignment.Center;
                    btnPlay.Tag = M.ItemPairs[myQ.PairId].iP1;
                    btnPlay.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(Play);
                    Grid.SetColumn(btnPlay, 0);
                    Grid.SetRow(btnPlay, 1);
                    Content.Children.Add(btnPlay);

                    iCtrlMinorId++;
                    sCtrlId = sCtrlIdBase + iCtrlId.ToString() + iCtrlMinorId.ToString();
                    btnPlay = new Button();
                    btnPlay.Name = sCtrlId;
                    //currBtn.WordImage.Source = new BitmapImage(new Uri("/Image/Play2.Png", UriKind.Relative));
                    brPlay.ImageSource=new BitmapImage(new Uri("/Image/Play.Png", UriKind.Relative));
                    brPlay.Stretch = Stretch.None;
                    btnPlay.Background = brPlay;  
                    btnPlay.Width = 200;
                    btnPlay.Height = 120;
                    btnPlay.BorderThickness = new Thickness(3);
                    btnPlay.BorderBrush = btnBrdHigh;
                    btnPlay.VerticalAlignment = VerticalAlignment.Bottom;
                    btnPlay.HorizontalAlignment = HorizontalAlignment.Center;
                    btnPlay.Tag = M.ItemPairs[myQ.PairId].iP2;
                    btnPlay.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(Play);
                    Grid.SetColumn(btnPlay, 1);
                    Grid.SetRow(btnPlay, 1);
                    Content.Children.Add(btnPlay);


                    rd = new RowDefinition();
                    rd.MinHeight = 210;
                    rd.MaxHeight = 210;
                    Content.RowDefinitions.Add(rd);
                    
                    radBtn1 = new RadioButton();
                    radBtn1.HorizontalAlignment = HorizontalAlignment.Center;
                    radBtn1.VerticalAlignment = VerticalAlignment.Center;
                    radBtn1.Tag = 1;
                    radBtn1.Content = "";
                    Grid.SetColumn(radBtn1, 0);
                    Grid.SetRow(radBtn1, 2);
                    Content.Children.Add(radBtn1);

                    radBtn2 = new RadioButton();
                    radBtn2.HorizontalAlignment = HorizontalAlignment.Center;
                    radBtn2.VerticalAlignment = VerticalAlignment.Center;
                    radBtn2.Tag = 2;
                    radBtn2.Content = "";
                    Grid.SetColumn(radBtn2, 1);
                    Grid.SetRow(radBtn2, 2);
                    Content.Children.Add(radBtn2);

                    rd = new RowDefinition();
                    rd.MinHeight = 70;
                    rd.MaxHeight = 70;
                    Content.RowDefinitions.Add(rd);

                    btnAnswer = new Button();
                    btnAnswer.Content = "Answer";
                    btnAnswer.Width = 400;
                    btnAnswer.Height = 70;
                    btnAnswer.VerticalAlignment = VerticalAlignment.Bottom;
                    btnAnswer.HorizontalAlignment = HorizontalAlignment.Center;
                    btnAnswer.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(Answer);
                    Grid.SetRow(btnAnswer, 3);
                    Grid.SetColumn(btnAnswer, 0);
                    Grid.SetColumnSpan(btnAnswer, 2);
                    Content.Children.Add(btnAnswer);

#endregion
                    break;
            }

        }

        void txtBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Answer(null, null);
            }
        }

        private void GetOneQuestion()
        {
            myQ = null;
            if (AllQ != null)
            {
                myQ = AllQ.GetOneQuestion();
                if (myQ != null)
                {
                    myQ.TimeOut += new MPTimeOutEventHandler(myQ_TimeOut);
                    BuildLayout();
                    //QuizzInfo.Text = myQ.QuestionDescription();
                    myQ.StartTiming();
                }
                else
                {
                    AllQ_SetFull();
                }
            }
        }

        private void SelectAnswer(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // deal with highlighting
            for (int iCnt = 0; iCnt < MPBtnGroup.Count; iCnt++)
            {
                MPBtnGroup[iCnt].BorderBrush = btnBrdNormal;
                MPBtnGroup[iCnt].BorderThickness = new Thickness(0);

            }
            MP_Button btn = (MP_Button)sender;
            btn.BorderBrush = btnBrdHigh;
            btn.BorderThickness = new Thickness(3);
            //btn.Background = btnBrdHigh;

            // set the selected answer.
            iItemId = Convert.ToInt32(btn.Tag);
        }

        private void Answer(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //MP_Button btnAnswer1;
            Button btnAnswer2;
            int iAnswer = 0;
            string sAnswer = string.Empty;
            myQ.EndTiming();
            
            switch (myQ.QuestionType)
            {
                case MPQuestionType.ListenSelect:
                    iAnswer = iItemId;
                    break;
                case MPQuestionType.ListenType:
                    sAnswer = txtBox.Text;
                    if (myQ.CorrectAnswer == 1)
                    {
                        if (sAnswer.ToUpperInvariant() == M.ItemList[M.ItemPairs[myQ.PairId].iP1].Caption.ToUpperInvariant())
                        {
                            iAnswer = 1;
                        }
                    }
                    else
                    {
                        if (sAnswer.ToUpperInvariant() == M.ItemList[M.ItemPairs[myQ.PairId].iP2].Caption.ToUpperInvariant())
                        {
                            iAnswer = 2;
                        }
                    }
                    break;
                case MPQuestionType.ReadListenSelect:
                    btnAnswer2 = (Button)sender;
                    if ((bool)radBtn1.IsChecked)
                    {
                        iAnswer = Convert.ToInt32(radBtn1.Tag);
                    }
                    else if ((bool)radBtn2.IsChecked)
                    {
                        iAnswer = Convert.ToInt32(radBtn2.Tag);
                    }
                    break;
            }
            if (iAnswer == myQ.CorrectAnswer)
            {
                myQ.CheckAnswer(true);
            }
            else
            {
                myQ.CheckAnswer(false);
            }
            AllQ.SaveAnswer(myQ);
            myQ = null;
            GetOneQuestion();
        }

        private void Play(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Button btnPlay1;
            string sAudioFile;
            iItemId = 0;
            btnPlay1 = (Button)sender;
            iItemId = Convert.ToInt32(btnPlay1.Tag);

            sAudioFile = M.ItemList[iItemId].Audio;
            if (mediaElement1.CurrentState != MediaElementState.Playing)
            {
                mediaElement1.Source = new Uri("/Audio/" + sAudioFile, UriKind.Relative);
                mediaElement1.Play();
            }
        }

        private void CleanLayout()
        {
            Content.Children.Clear();
            Content.RowDefinitions.Clear();
            Content.ColumnDefinitions.Clear();
        }
        private void mediaElement1_MediaEnded(object sender, RoutedEventArgs e)
        {

        }
        private void mediaElement1_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {

            if (AllQ != null)
            {
                AllQ.EndSession();
            }
            AllQ = null;
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (AllQ != null)
            {
                AllQ.EndSession();
            }
            AllQ = null;
        }

        private BitmapImage GetImage(int ItemId, int ItemType)
        {
            string sPath = string.Empty;
            BitmapImage bmRes = null;
            if (ItemType == 1)     // Image for Words
            {
                sPath = M.ItemList[ItemId].Image;
            }
            else        // Image for sounds
            {
                sPath = M.CatList[ItemId].Image;
            }
            if (Application.GetResourceStream(new Uri("img/" + sPath, UriKind.Relative)) != null)
            {
                bmRes = new BitmapImage(new Uri("/img/" + sPath, UriKind.Relative));
            }
            else
            {
                bmRes = new BitmapImage(new Uri("/Image/na1.png", UriKind.Relative));
            }

            return bmRes;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (AllQ != null)
            {
                AllQ.EndSession();
            }
            AllQ = null;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            if (AllQ != null)
            {
                AllQ.EndSession();
            }
            AllQ = null;
            NavigationService.Navigate(new Uri("/Stats.xaml", UriKind.Relative));
        }
        private void ShowHelp(object sender, EventArgs e)
        {
            //MessageBox.Show(sHelp, "About quizzes", MessageBoxButton.OK);
            if (AllQ != null)
            {
                AllQ.EndSession();
            }
            AllQ = null;
            NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            //ShowFilter();
            if (AllQ != null)
            {
                AllQ.EndSession();
            }
            AllQ = null;
            M.strFilterCallingPage = "Quizzes";
            NavigationService.Navigate(new Uri("/MP_Filter.xaml", UriKind.Relative));
        }
    }
}