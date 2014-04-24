using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
//using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
//using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Text;
using System.Windows.Threading;
using System.Globalization;
using Microsoft.Phone.Shell;
using System.Runtime.Serialization;



namespace MinPairs
{
    [DataContract]
    public class ML_Main
    {
        
        public ML_Main()
        {
            DAS = new sDevActSize();
            DAS.dX = Application.Current.Host.Content.ActualWidth;
            DAS.dY = Application.Current.Host.Content.ActualHeight - 32;    // 32 it takes the Windows info ribbon
        }


        public string strFilterCallingPage = string.Empty;  // called the filter - to return the control to that page

        public struct sDevActSize       // holds the device's screen size
        {
            public double dX;
            public double dY;
        }
        public static sDevActSize DAS;
        [DataMember]
        public Dictionary<int, MP_Item> ItemList = new Dictionary<int, MP_Item>();     // list of the items
        [DataMember]
        public Dictionary<int, iPairs> ItemPairs = new Dictionary<int, iPairs>();       // list of pairs
        [DataMember]
        public Dictionary<int, MP_Category> CatList = new Dictionary<int, MP_Category>();  // list of categories
        [DataMember]
        public Dictionary<int, iCatPairs> CatPairs = new Dictionary<int, iCatPairs>(); // pairs of sounds
        public DateTime StatStartDate = DateTime.Parse("Jan 01, 2000");
        public DateTime StatEndDate = DateTime.Now.AddDays(1);
        public bool IsDateNotChanged = true;
        

        public void LoadCategories()            // stores the sounds in sounds dictionary
        {
            string sRec = string.Empty;
            string[] sa;
            MP_Category ci;
            Assembly a = Assembly.GetExecutingAssembly();
            Stream s = a.GetManifestResourceStream("MinPairs.Data.MP_Categories.dat");
            StreamReader SR = new StreamReader(s);
            while (!SR.EndOfStream)
            {
                sRec = SR.ReadLine();
                sa = sRec.Split('|');
                ci = new MP_Category(sa[0], sa[1], sa[2], sa[3]);
                CatList.Add(ci.CategoryId, ci);

            }
        }
        public void LoadItems()                 // stores the words into the words dictionary
        {
            string sRec = string.Empty;
            string[] sa;
            MP_Item mi;
            Assembly a = Assembly.GetExecutingAssembly();
            Stream s = a.GetManifestResourceStream("MinPairs.Data.MP_Items.dat");
            StreamReader SR = new StreamReader(s);
            while (!SR.EndOfStream)
            {
                sRec = SR.ReadLine();
                sa = sRec.Split('|');
                mi = new MP_Item(sa[0], sa[1], sa[2], sa[3]);
                ItemList.Add(Convert.ToInt32(sa[0]), mi);
            }
        }
        public void LoadCatPairs()              // stores the paired sounds as symetric double pairs into the sound pairs dictionary
        {
            /*  will create the pairing items between the categories (sounds)
             * 
             * 
             * 
             * */
            string sRec = string.Empty;
            string[] sa;
            iCatPairs icp;
            Assembly a = Assembly.GetExecutingAssembly();
            Stream s = a.GetManifestResourceStream("MinPairs.Data.MP_CatPairs.dat");
            StreamReader SR = new StreamReader(s);
            int iCnt = 1;
            while (!SR.EndOfStream)
            {
                sRec = SR.ReadLine();
                sa = sRec.Split('|');
                icp = new iCatPairs();
                icp.iCP1 = Convert.ToInt32(sa[0]);
                icp.iCP2  = Convert.ToInt32(sa[1]);
                CatPairs.Add(iCnt, icp);
                if (icp.iCP1 != 0)
                {
                    iCnt++;
                    icp = new iCatPairs();
                    icp.iCP2 = Convert.ToInt32(sa[0]);
                    icp.iCP1 = Convert.ToInt32(sa[1]);
                    CatPairs.Add(iCnt, icp);
                }
                iCnt++;
            }
        }
        public void LoadPairs()                 // stores the paired words as symetric double pairs into the word pairs dictionary
        {
            string sRec = string.Empty;
            string[] sa;
            iPairs ip;
            Assembly a = Assembly.GetExecutingAssembly();
            Stream s = a.GetManifestResourceStream("MinPairs.Data.MP_Pairs.dat");
            StreamReader SR = new StreamReader(s);
            int iCnt = 1;
            int kCnt = 1;
            while (!SR.EndOfStream)
            {
                sRec = SR.ReadLine();
                sa = sRec.Split('|');
                ip = new iPairs();
                ip.iP1 = Convert.ToInt32(sa[0]);
                ip.iCatId1 = Convert.ToInt32(sa[1]);
                ip.iP2 = Convert.ToInt32(sa[2]);
                ip.iCatId2 = Convert.ToInt32(sa[3]);
                ip.iPairId = kCnt;
                ItemPairs.Add(iCnt, ip);

                iCnt++;
                ip = new iPairs();
                ip.iP1 = Convert.ToInt32(sa[2]);
                ip.iCatId1 = Convert.ToInt32(sa[3]);
                ip.iP2 = Convert.ToInt32(sa[0]);
                ip.iCatId2 = Convert.ToInt32(sa[1]);
                ip.iPairId = -kCnt;
                ItemPairs.Add(iCnt, ip);
                iCnt++;
                kCnt++;
            }

        }               
        public void LoadCatItemsMap()           // complets the sounds' collection of words with their assigned words
        {
            string sRec = string.Empty;
            string[] sa;
            
            Assembly a = Assembly.GetExecutingAssembly();
            Stream s = a.GetManifestResourceStream("MinPairs.Data.MP_Items_Categories.dat");
            StreamReader SR = new StreamReader(s);
            int iCatId;
            while (!SR.EndOfStream)
            {
                sRec = SR.ReadLine();
                sa = sRec.Split('|');
                iCatId = Convert.ToInt32(sa[0]);
                CatList[iCatId].AddItem(Convert.ToInt32(sa[1]));

            }
        }
        public void ResetCategories(int iCatId) // will set the next sound to be played as the category sound
        {
            for (int iCnt = 1; iCnt <= CatList.Count-1; iCnt++)
            {
                if (iCnt != iCatId)
                {
                    CatList[iCnt].ResetCategory();
                }
            }
        }

        
    }

    #region DATA
    [DataContract]
    public class MP_Item    // describes a word  
    {

        public MP_Item(string Id, string Caption, string Audio, string Image)
        {
            _Id = Convert.ToInt32(Id);
            _Caption = Caption;
            _Audio = Audio;
            _Image = Image;
        }
        private int _Id;        // word id
        //public int PairId { set { _Id = value; } get { return _Id; } }     
        private string _Caption;
        [DataMember]
        public string Caption { set { _Caption = value; } get { return _Caption; } } // word's caption
        private string _Audio;
        [DataMember]
        public string Audio { set { _Audio = value; } get { return _Audio; } } //word's sound file name
        private string _Image;
        [DataMember]
        public string Image { set { _Image = value; } get { return _Image; } } // word's image file name
    }
    [DataContract]
    public class MP_Category  // describes a category or a sound
    {
        public MP_Category(string Id, string sCaption, string sAudio, string sImage)
        {
            CategoryId = Convert.ToInt32(Id);
            Caption = sCaption;
            Audio = sAudio;
            Image = sImage;
            _IsWord = false;
            _ItemId = 0;
            myItems = new Dictionary<int, int>();
            SetNextWordId();
        }
        
        [DataMember]
        public int CategoryId { set; get; }     // category id
        
        [DataMember]
        public string Caption { set; get; } //category caption
        [DataMember]
        public string Audio { set; get; } // category's audio file name

        [DataMember]
        public string Image { set; get; } // category's image file name
        [DataMember]
        public int _ItemId { set; get; }  // current word to be played
        [DataMember]
        public bool _IsWord { set; get; }    // flag to tell the consumer to play the audio for the sound or the next word from the assigned word.   
        [DataMember]
        public Dictionary<int, int> myItems { set; get; }        // the list of words assigned to this sound
        
        public void AddItem(int ItemId)
        {
            myItems.Add(myItems.Count + 1, ItemId);
        }           // builds the list of the assigned words to the sound
        
        public int[] GetCategoryItems()
        {
            int[] ItemsId = new int[myItems.Count];
            for (int iCnt = 1; iCnt <= myItems.Count; iCnt++)
            {
                ItemsId[iCnt - 1] = myItems[iCnt];
            }
            return ItemsId;
        }           // returns an array of words' id
        
        public void SetNextWordId()                // calculates which will be the next word to be played
        {
            if (_ItemId == myItems.Count)
            {
                _ItemId = 1;
            }
            else
            {
                _ItemId++;
            }
        }
        public int GetNextWordId()
        {
            return myItems[_ItemId];
        }
        public int GetAudioId()                     // will return the audio id to be played if the return value is -1 the caller will play the audio file for the sound
        {
            // if return -1 will use the category audio else will use the id to pic the word's audion to play
            int AudioId = -1;
            if (_IsWord)
            {
                AudioId = myItems[_ItemId];
                
                SetNextWordId();
            }
            _IsWord = !_IsWord;
            return AudioId;
        }
        public void ResetCategory()
        {
            _IsWord = false;
        }
    }

    public struct iCatPairs
    {
        public int iCP1;        // sound id connected with the other sound
        public int iCP2;        // sound id connected with the previous id
    }

    public struct iPairs        // structure to hold the pairs of words that can be played 
    {
        public int iPairId;     // pair identifier
        public int iP1;         // id of the first word of the pair
        public int iP2;         // id of the second word of the pair
        public int iCatId1;     // id of the sound which the first word is assigned to
        public int iCatId2;     // id of the sound which the second word is assigned to
    }

    #endregion

    #region FILTER
    public class ML_Filter          // the class holds and interfaces the filters for each page that is using OneFilter
    {
        public ML_Filter()
        {
            _myFilters = new ML_Filters();
        }
        public ML_Filters _myFilters;      // instance of the ML_Filters class
        private ML_Main _myMain;            // instance of the class that provides the data
        public ML_Main myMain
        {
            set
            {
                _myMain = value;
            }
        }
        public string GetFilterTitle(string strFilterName)
        {
            return _myFilters.GetFilterTitle(strFilterName);
        }
        public OneFilter GetFilter(string strFilterName)
        {
            return _myFilters.GetFilter(_myMain, strFilterName);
        }
        
    public class ML_Filters
    {
        public Dictionary<string, OneFilter> Filters = new Dictionary<string, OneFilter>();    // collection of filters one per calling page

        public ML_Filters()
        {

        }
        public void CreateFilter(ML_Main m, string sOwner)
        {
            OneFilter objF;
            objF = new OneFilter();
            objF.MyMain=m;
            objF.AddNewMainSound(0);        // initialize the filter with the All sounds checked
            Filters.Add(sOwner, objF);
        }
        public string GetFilterTitle(string strFilterName)
        {
            return Filters[strFilterName].FilterTitle;
        }
        public OneFilter GetFilter(ML_Main m, string sOwner)        // returns an existing filter to caller. Will create a new filter if one is not available
        {
            if (!Filters.ContainsKey(sOwner))
            {
                CreateFilter(m, sOwner);
            }
            return  Filters[sOwner];
                
        }

        public void StoreFilter(OneFilter myFilter, string sOwner)      // store a new or an existing filter in the repository
        {
            if (Filters.ContainsKey(sOwner))
            {
                Filters.Remove(sOwner);
            }
            Filters.Add(sOwner, myFilter);
        }
    }

    }
    [DataContract]      // required for serialization.
    public class OneFilter
    {

        public string _FilterTitle;            // holds the caption representing the current state of the filter
        public bool _FilterSet = false;
        public int _iCatPairId;
        public int CatPairId { get { return _iCatPairId; } }
        [DataMember]
        public Dictionary<int, int> MyPairs = new Dictionary<int, int>();                       // the filtered pairs collection
        [DataMember]
        public Dictionary<int, int> MySecCategories = new Dictionary<int, int>();               // the filtered secondary sounds by main filter
        [DataMember]
        public Dictionary<int, int> MyMainCategories = new Dictionary<int, int>();              // the filtered main sounds
        [DataMember]
        public Dictionary<int, int> MySelectedSecCategories = new Dictionary<int, int>();       // the filtered secondary sounds by user
        [DataMember]
        public ML_Main _MyMain;                                    // the main class instance
        public ML_Main MyMain { set { _MyMain = value; } }

        public void AddNewMainSound(int iMainSound)     // will change
        {
            MyMainCategories.Clear();
            MyMainCategories.Add(MyMainCategories.Count + 1, iMainSound);
            BuildSecList();

        }   
        public void RemoveMainSound(int iMainSound)
        {
            if (MyMainCategories.ContainsValue(iMainSound))
            {
                int[] tempSounds = new int[MyMainCategories.Count];
                int kCnt = 1;
                MyMainCategories.Values.CopyTo(tempSounds, 0);
                MyMainCategories.Clear();
                for (int iCnt = 0; iCnt < tempSounds.Length; iCnt++)
                {
                    if (tempSounds[iCnt] != iMainSound)
                    {
                        MyMainCategories.Add(kCnt, tempSounds[iCnt]);
                        kCnt++;

                    }
                }
                BuildSecList();
            }
        }
        public void AddSecondarySound(int iSecSound)
        {
                MySelectedSecCategories.Clear();
                MySelectedSecCategories.Add(MySelectedSecCategories.Count + 1, iSecSound);
                BuildPairsList();
                _FilterSet = true;
        }
        public void RemoveSecondarySound(int iSecSound)
        {
            if (MySelectedSecCategories.ContainsValue(iSecSound))
            {
                int[] tempSounds = new int[MySelectedSecCategories.Count];
                int kCnt = 1;
                MySelectedSecCategories.Values.CopyTo(tempSounds, 0);
                MySelectedSecCategories.Clear();
                for (int iCnt = 0; iCnt < tempSounds.Length; iCnt++)
                {
                    if (tempSounds[iCnt] != iSecSound)
                    {
                        MySelectedSecCategories.Add(kCnt, tempSounds[iCnt]);
                    }
                }

            }
        }
        private void BuildSecList()
        {
            MySecCategories.Clear();
            for (int iCnt = 1; iCnt <= MyMainCategories.Count; iCnt++)
            {
                for (int jCnt = 1; jCnt <= _MyMain.CatPairs.Count; jCnt++)
                {
                    if (MyMainCategories[iCnt] == _MyMain.CatPairs[jCnt].iCP1)
                    {
                        MySecCategories.Add(MySecCategories.Count+1,_MyMain.CatPairs[jCnt].iCP2);
                    }
                }
            }
            
        }
        public bool IsFilterSet
        {
            get
            {
                if (MyPairs.Count > 0)
                { _FilterSet = true; }
                else
                { _FilterSet = false; }
                return _FilterSet;
            }
        }
        
        private void BuildPairsList()
        {
            MyPairs.Clear();
            for (int iCnt = 1; iCnt <= _MyMain.ItemPairs.Count; iCnt++)
            {
                if (MyMainCategories[1] == 0)
                {
                    MyPairs.Add(MyPairs.Count + 1, iCnt);
                    iCnt++;     // not to double the pairs 
                }
                else
                {
                    if (_MyMain.ItemPairs[iCnt].iCatId1 == MyMainCategories[1] && _MyMain.ItemPairs[iCnt].iCatId2 == MySelectedSecCategories[1])
                    {
                        MyPairs.Add(MyPairs.Count + 1, iCnt);
                    }
                }
            }
            MakeTitle();
        }
        

        private void MakeTitle() // sets the description of the current filter setting
        {
            _FilterTitle = _MyMain.CatList[MyMainCategories[1]].Caption + " vs " + _MyMain.CatList[MySelectedSecCategories[1]].Caption;
            for (int iCnt = 1; iCnt <= _MyMain.CatPairs.Count; iCnt++)
            {
                if (_MyMain.CatPairs[iCnt].iCP1 == MyMainCategories[1] && _MyMain.CatPairs[iCnt].iCP2 == MySelectedSecCategories[1])
                {
                    _iCatPairId = iCnt;
                    break;
                }
            }

        }
        public string FilterTitle
        {
            get { return _FilterTitle; }
        }

        
    }

    public enum MPQuestionType
    {
        ListenSelect = 1,
        ListenType = 2,
        ReadListenSelect = 3
    };
    public class MPTimeOutEventArgs : EventArgs
    {
        public MPTimeOutEventArgs(int TimeOut)
        {
            _TimeOut = TimeOut;
        }
        private int _TimeOut;
        public int TimeOut { set { _TimeOut = value; } get { return _TimeOut; } }

    }
    public delegate void MPTimeOutEventHandler(object sender, MPTimeOutEventArgs e);
    #endregion

    #region TRAINING
    public class MP_Question
    {
        private bool _IsTimed = false;
        public bool IsTimed { set { _IsTimed = value; } get { return _IsTimed; } }

        private int _TimeOutVal = 0;
        public int TimeOutVal { set { _TimeOutVal = value; } get { return _TimeOutVal; } }

        private int _iQuestionCnt = 0;
        public int QuestionNumber { set { _iQuestionCnt = value; } }
        DispatcherTimer dt;

        private MPQuestionType _QType;
        public MPQuestionType QuestionType { get { return _QType; } set { _QType = value; } }

        private int _correctAnswer = -1;
        public int CorrectAnswer { set { _correctAnswer = value; } get { return _correctAnswer; } }

        private int _iPairId = -1;
        public int PairId { set { _iPairId = value; } get { return _iPairId; } }

        private DateTime _dtStart;
        public DateTime Start { get { return _dtStart; } }
        
        private int _iAnswerDuration;
        public int AnswerDuration { get { return _iAnswerDuration; } }

        private bool _bIsCorrect = false;
        public bool IsCorrect { get { return _bIsCorrect; } }

        private DateTime _dtEnd;
        public DateTime End { get { return _dtEnd; } }
        public void StartTiming()
        {
            if (_IsTimed)
            {
                dt = new DispatcherTimer();
                dt.Interval = new TimeSpan(0, 0, _TimeOutVal);
                dt.Tick += new EventHandler(dt_Tick);
                dt.Start();
            }
            _dtStart = DateTime.Now;
        }
        public void CheckAnswer(bool bAnswer)
        {
            _dtEnd = DateTime.Now;
            _iAnswerDuration = Convert.ToInt32(Math.Round(_dtEnd.Subtract(_dtStart).TotalSeconds));
            _bIsCorrect = bAnswer;

        }
        public event MPTimeOutEventHandler TimeOut;
        void dt_Tick(object sender, EventArgs e)
        {
            _dtEnd = DateTime.Now;
            
            dt.Stop();
            
            _iAnswerDuration = Convert.ToInt32(Math.Round(_dtEnd.Subtract(_dtStart).TotalSeconds));
            MPTimeOutEventArgs TO_Args = new MPTimeOutEventArgs(_TimeOutVal);
            TimeOut(this, TO_Args);
        }
        public void EndTiming()
        {
            
            dt.Stop();
        }

        public string QuestionDescription()
        {
            return "Question No: " + _iQuestionCnt.ToString();
        }

    }

    public class MP_AllQuestions
    {
        public MP_AllQuestions(bool bIsQizz, int CatPairId)
        {
            IsolatedStorageSettings mySettings = IsolatedStorageSettings.ApplicationSettings;
            if (!mySettings.Contains("ListenSelect"))
            {
                mySettings.Add("ListenSelect", 5);
                iListenSelect = 5;
            }
            else
            {
                iListenSelect = Convert.ToInt16(mySettings["ListenSelect"]);
            }
            if (!mySettings.Contains("ListenType"))
            {
                mySettings.Add("ListenType", 10);
                iListenType = 10;
            }
            else
            {
                iListenType = Convert.ToInt16(mySettings["ListenType"]);
            }
            if (!mySettings.Contains("ReadListenSelect"))
            {
                mySettings.Add("ReadListenSelect", 5);
                iReadListenSelect = 5;
            }
            else
            {
                iReadListenSelect = Convert.ToInt16(mySettings["ReadListenSelect"]);
            }
            this.BuildQuestions(bIsQizz);
            _bIsQuizz = bIsQizz;
            mySession = new MP_StatSession(bIsQizz);
            mySession.CatPairId = CatPairId;
            mySession.StartDate = DateTime.Now;
            
        }
         ~MP_AllQuestions()
        {
            if (mySession != null)
            {
                mySession.EndDate = DateTime.Now;
                mySession.SaveSession();
                mySession = null;
            }
        }

        private Int16 iListenSelect;
        private Int16 iListenType;
        private Int16 iReadListenSelect;

        public void EndSession()
         {
             if (mySession != null)
             {
                 mySession.EndDate = DateTime.Now;
                 mySession.SaveSession();
                 mySession = null;
             }
         }
        private bool _bIsQuizz = false;
        private int iQCounter=0;
        private Dictionary<int, MP_Question> MyQuestions = new Dictionary<int, MP_Question>();
        public Dictionary<int, int> RandomQuestions = new Dictionary<int, int>();
        private MP_StatSession mySession;
        private void BuildQuestions(bool bIsQuizz)
        {
            OneFilter myF;
            ML_Filter F;
            F = (ML_Filter)PhoneApplicationService.Current.State["MF"];
            myF = F.GetFilter("MP");
            MyQuestions.Clear();
            MP_Question currQ;
            int myPairId;
            for (int iCnt = 1; iCnt <= myF.MyPairs.Count; iCnt++)
            {
                myPairId = myF.MyPairs[iCnt];

                currQ = new MP_Question();
                currQ.PairId = myPairId;
                currQ.CorrectAnswer = 1;
                currQ.QuestionType = MPQuestionType.ListenSelect;
                currQ.IsTimed = bIsQuizz;
                currQ.TimeOutVal = iListenSelect;
                MyQuestions.Add((iCnt-1)*6 + 1, currQ);

                currQ = new MP_Question();
                currQ.PairId = myPairId;
                currQ.CorrectAnswer = 1;
                currQ.QuestionType = MPQuestionType.ListenType;
                currQ.IsTimed = bIsQuizz;
                currQ.TimeOutVal = iListenType;
                MyQuestions.Add((iCnt-1)*6 + 2, currQ);

                currQ = new MP_Question();
                currQ.PairId = myPairId;
                currQ.CorrectAnswer = 1;
                currQ.QuestionType = MPQuestionType.ReadListenSelect;
                currQ.IsTimed = bIsQuizz;
                currQ.TimeOutVal = iReadListenSelect;
                MyQuestions.Add((iCnt - 1) * 6 + 3, currQ);

                currQ = new MP_Question();
                currQ.PairId = myPairId;
                currQ.CorrectAnswer = 2;
                currQ.QuestionType = MPQuestionType.ListenSelect;
                currQ.IsTimed = bIsQuizz;
                currQ.TimeOutVal = iListenSelect;
                MyQuestions.Add((iCnt - 1) * 6 + 4, currQ);

                currQ = new MP_Question();
                currQ.PairId = myPairId;
                currQ.CorrectAnswer = 2;
                currQ.QuestionType = MPQuestionType.ListenType;
                currQ.IsTimed = bIsQuizz;
                currQ.TimeOutVal = iListenType;
                MyQuestions.Add((iCnt - 1) * 6 + 5, currQ);

                currQ = new MP_Question();
                currQ.PairId = myPairId;
                currQ.CorrectAnswer = 2;
                currQ.QuestionType = MPQuestionType.ReadListenSelect;
                currQ.IsTimed = bIsQuizz;
                currQ.TimeOutVal = iReadListenSelect;
                MyQuestions.Add((iCnt - 1) * 6 + 6, currQ);
            }
            BuildRandomList();
        }
        private void BuildRandomList()
        {
            RandomQuestions = new Dictionary<int, int>();
            for (int iCnt = 1; iCnt <= MyQuestions.Count; iCnt++)
            {
                RandomQuestions.Add(iCnt, iCnt);
                
            }
        }
        public MP_Question GetOneQuestion()
        {
            Random myRnd = new Random(DateTime.Now.Second);
            myRnd = new Random(myRnd.Next(int.MaxValue));
            int iQPos = myRnd.Next(1, RandomQuestions.Count + 1);
            MP_Question qRes = (MP_Question)MyQuestions[RandomQuestions[iQPos]];
            if (!_bIsQuizz || iQCounter < _SetSize)
            {

                // remove the selected question from the list 

                Dictionary<int, int> rq = new Dictionary<int, int>();
                int jCnt = 1;
                for (int iCnt = 1; iCnt <= RandomQuestions.Count; iCnt++)
                {
                    if (iCnt != iQPos)
                    {
                        rq.Add(jCnt, RandomQuestions[iCnt]);
                        jCnt++;
                    }
                }
                RandomQuestions.Clear();
                for (int iCnt = 1; iCnt <= rq.Count; iCnt++)
                {
                    RandomQuestions.Add(iCnt, rq[iCnt]);
                }
                if (RandomQuestions.Count == 0)
                {
                    BuildQuestions(_bIsQuizz);
                }
                iQCounter++;
                qRes.QuestionNumber = iQCounter;
            }
            else
            {
                _sFullMessage = "You answered correct at " + mySession.GetPercentage().ToString() + " out of " + _SetSize.ToString() + " questions";
                EndSession();
                iQCounter = 0;
                qRes = null;
                BuildQuestions(_bIsQuizz);
                
            }
            return qRes;

        }
        public void SaveAnswer(MP_Question myQ)
        {
            mySession.SetAnswer(myQ);
        }

        private int _SetSize;
        public int SetSize { set { _SetSize = value; } }
        private string _sFullMessage;
        public string FullMessage { get { return _sFullMessage; } }

    }

    public class MP_StatSession
    {
        public MP_StatSession(bool bIsQuizz)
        {
            _IsQuizz = bIsQuizz;
            GetSessionId();
            myAnswers = new Dictionary<int, MP_StatAnswer>();
        }
        private char sSep = '|';
        private string sDateFormat = "MM-dd-yyyy";
        private string sSessionFileName = "Sessions.dat";
        private string sAnswersFileName = "Answers.dat";
        private int _SessionId;
        private int _CatPairId;
        public int CatPairId { set { _CatPairId = value; } }
        private bool _IsQuizz;
        private Dictionary<int, MP_StatAnswer> myAnswers;
        public int SessionId { set { _SessionId = value; } get { return _SessionId; } }

        private DateTime _StartDate;
        public DateTime StartDate { set { _StartDate = value; } get { return _StartDate; } }

        private DateTime _EndDate;
        public DateTime EndDate { set { _EndDate = value; } get { return _EndDate; } }

        public void SaveSession()
        {
            IsolatedStorageFile iFile = IsolatedStorageFile.GetUserStoreForApplication();

            IsolatedStorageFileStream ifs;
            IsolatedStorageFileStream afs;
            if (iFile.FileExists(sSessionFileName))
            {
                ifs = iFile.OpenFile(sSessionFileName, FileMode.Append, FileAccess.Write);
            }
            else
            {
                ifs = iFile.OpenFile(sSessionFileName, FileMode.OpenOrCreate, FileAccess.Write);
            }
            if (iFile.FileExists(sAnswersFileName))
            {
                afs = iFile.OpenFile(sAnswersFileName, FileMode.Append, FileAccess.Write);
            }
            else
            {
                afs = iFile.OpenFile(sAnswersFileName, FileMode.OpenOrCreate, FileAccess.Write);
            }
            
            
            
            //StoreSessionInfo(new IsolatedStorageFileStream(sSessionFileName, FileMode.Open,FileAccess.Write , iFile), 
            //    new IsolatedStorageFileStream(sAnswersFileName, FileMode.Open, FileAccess.Write, iFile));
            StoreSessionInfo(ifs, afs);
            ifs.Close();
            afs.Close();
        }
        private void StoreSessionInfo(IsolatedStorageFileStream ifs, IsolatedStorageFileStream afs)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            StringBuilder sSessionData = new StringBuilder();
            sSessionData.Append(SessionId + sSep.ToString());
            sSessionData.Append(_CatPairId + sSep.ToString());
            sSessionData.Append(_IsQuizz + sSep.ToString());
            sSessionData.Append(_StartDate.ToString(sDateFormat, ci) + sSep.ToString());
            sSessionData.Append(_EndDate.ToString(sDateFormat, ci));

            StreamWriter sw = new StreamWriter(ifs);
            sw.WriteLine(sSessionData.ToString());
            sw.Flush();
            sw.Close();
            sw.Dispose();
            sw = new StreamWriter(afs);
            MP_StatAnswer myAnswer;
            for (int iCnt=1; iCnt <= myAnswers.Count; iCnt++)
            {
                myAnswer = (MP_StatAnswer)myAnswers[iCnt];
                sw.WriteLine(myAnswer.AnswerDescr(_SessionId));
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
            ifs.Close();
            afs.Close();
        }
        private void GetSessionId()
        {
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            
            IsolatedStorageFileStream ifs = isf.OpenFile(sSessionFileName, FileMode.OpenOrCreate ,FileAccess.Read);
            StreamReader sr = new StreamReader(ifs);
            string sLine = string.Empty;
            _SessionId = 1;
            while (!sr.EndOfStream)
            {
                sLine = sr.ReadLine();
                _SessionId++;
            }
            //if (string.IsNullOrEmpty(sLine))
            //{
            //    _SessionId = 1;
            //}
            //else
            //{
            //    string[] a;
            //    a = sLine.Split('|');
            //    _SessionId = Convert.ToInt32(a[0]) + 1;
            //    sr.Close();
            //    sr.Dispose();
            //}
            sr.Close();
            sr.Dispose();
            ifs.Close();
        }
        public void SetAnswer(MP_Question oQ)
        {
            MP_StatAnswer oAnswer = new MP_StatAnswer();
            oAnswer.IsCorrect = oQ.IsCorrect;
            oAnswer.AnswerDuration = oQ.AnswerDuration;
            oAnswer.PairId = oQ.PairId;
            oAnswer.MyType = oQ.QuestionType;
            myAnswers.Add(myAnswers.Count + 1, oAnswer);

        }

        public int GetPercentage()
        {
            int iRes = 0;
            MP_StatAnswer currAnsewer;
            for (int iCnt = 1; iCnt <= myAnswers.Count; iCnt++)
            {
                currAnsewer = (MP_StatAnswer)myAnswers[iCnt];
                if (currAnsewer.IsCorrect)
                {
                    iRes++;
                }
            }
            return iRes;
        }
    }

    public class MP_StatAnswer
    {
        private MPQuestionType _myType;
        public MPQuestionType MyType { set { _myType = value; } }

        private int _AnswerDuration;
        public int AnswerDuration { set { _AnswerDuration = value; } }

        private bool _isCorrect;
        public bool IsCorrect { set { _isCorrect = value; } get { return _isCorrect; } }

        private int _iPairId;
        public int PairId { set { _iPairId = value; } }

        public string AnswerDescr(int iSessionId)
        {
            string sRes = string.Empty;
            int iMyType = 0;
            switch (_myType)
            {
                case MPQuestionType.ListenSelect:
                    iMyType = 1;
                    break;
                case MPQuestionType.ListenType:
                    iMyType = 2;
                    break;
                case MPQuestionType.ReadListenSelect:
                    iMyType = 3;
                    break;
            }
            //sRes = iSessionId.ToString() + "|" + _iPairId.ToString() + "|" + _myType.ToString() + "|" + _isCorrect.ToString() + "|" + _AnswerDuration.ToString();
            sRes = iSessionId.ToString() + "|" + _iPairId.ToString() + "|" + iMyType.ToString() +"|" + _isCorrect.ToString() + "|" + _AnswerDuration.ToString();

            return sRes;
        }
    }
    #endregion

    #region STATISTICS

    public class MP_PieSoundStats
    {
        private string _SoundDescription;
        public string SoundDescription { get { return _SoundDescription; } set { _SoundDescription = value; } }
        private uint _SoundWeight = 0;
        public uint SoundWeight { get { return _SoundWeight; } set { _SoundWeight = value; } }
    }
    public class MP_LineSoundStats
    {
        private string _SoundDescription;
        public string SoundDescription { get { return _SoundDescription; } set { _SoundDescription = value; } }
        private DateTime _SessionDate;
        public DateTime SessionDate { get { return _SessionDate; } set { _SessionDate = value; } }
        private uint _SoundTotal = 0;
        public uint SoundTotal { get { return _SoundTotal; } set { _SoundTotal = value; } }
        private uint _SoundCorrect = 0;
        public uint SoundCorrect { get { return _SoundCorrect; } set { _SoundCorrect = value; } }
        private double _CorrectPct=0;
        public double CorrectPct { get { return _CorrectPct; } }
        public void CalculatePct()
        {
            if (_SoundTotal > 0)
            {
                _CorrectPct = Convert.ToDouble(_SoundCorrect) / Convert.ToDouble(_SoundTotal);
            }
        }
    }
    public class MP_Stats
    {
        private char sSep = '|';

        private string sSessionFileName = "Sessions.dat";
        private string sAnswersFileName = "Answers.dat";
        private List <MP_PieSoundStats> _PracticePieStats;
        public List<MP_PieSoundStats> PracticePieStats { get { return _PracticePieStats; } }
        private List <MP_LineSoundStats> _PracticeLineStats;
        public List<MP_LineSoundStats> PracticeLineStats { get { return _PracticeLineStats; } }
        private List <MP_PieSoundStats> _QuizzPieStats;
        public List<MP_PieSoundStats> QuizzPieStats { get { return _QuizzPieStats; } }
        private List <MP_LineSoundStats> _QuizzLineStats;
        public List<MP_LineSoundStats> QuizzLineStats { get { return _QuizzLineStats; } }
        private ML_Main _myMain;
        public ML_Main MyMain { set { _myMain = value; } }

        private DateTime _MinDate=DateTime.MaxValue;
        public DateTime MinDate { get { return _MinDate; } }
        private DateTime _MaxDate=DateTime.MinValue;
        public DateTime MaxDate { get { return _MaxDate; } }

        public void ResetStats()
        {
            //_PracticeLineStats.Clear();
            //_PracticePieStats.Clear();
            //_QuizzLineStats.Clear();
            //_QuizzPieStats.Clear();
            IsolatedStorageFile iFile = IsolatedStorageFile.GetUserStoreForApplication();
            iFile.DeleteFile("Sessions.dat");
            iFile.DeleteFile("Answers.dat");
        }

        private class _MySession
        {
            public int SessionId;
            public DateTime SessionDate;
            public bool IsQuizz;
        }
        private class _MyAnswer
        {
            public int iSessionId;
            public bool IsCorrect;
            public int iPairId;
            public MPQuestionType iQuestionType;

        }
        private List<_MySession> Sessions;
        private List<_MyAnswer> Answers;

        private void ComputeData(int SoundId)
        {
            uint dQuizzRight = 0;
            uint dQuizzWrong = 0;
            uint dPracticeRight = 0;
            uint dPracticeWrong = 0;

            string sSoundName = "All sounds";
            if (SoundId != 0) { sSoundName = _myMain.CatList[SoundId].Caption; }

            _PracticePieStats = new List<MP_PieSoundStats>();
            _PracticeLineStats = new List<MP_LineSoundStats>();
            _QuizzPieStats = new List<MP_PieSoundStats>();
            _QuizzLineStats = new List<MP_LineSoundStats>();

            MP_PieSoundStats PracticePieRight = new MP_PieSoundStats();
            MP_PieSoundStats PracticePieWrong = new MP_PieSoundStats();
            MP_PieSoundStats QuizzPieRight = new MP_PieSoundStats();
            MP_PieSoundStats QuizzPieWrong = new MP_PieSoundStats();

            MP_LineSoundStats PracticeLine = null;
            MP_LineSoundStats QuizzLine = null;
            uint iRight = 0;
            uint iWrong = 0;

            PracticePieRight.SoundDescription = sSoundName;
            PracticePieWrong.SoundDescription = sSoundName;
            QuizzPieRight.SoundDescription = sSoundName;
            QuizzPieWrong.SoundDescription = sSoundName;
            
            _MySession currSession;
            DateTime currDate = DateTime.MinValue;
            for (int iCnt = 0; iCnt < Sessions.Count; iCnt++)
            {
                currSession = Sessions[iCnt];
                currDate = currSession.SessionDate;
                if (!currSession.IsQuizz)
                {
                    if (PracticeLine == null)
                    {
                        PracticeLine = new MP_LineSoundStats();
                        PracticeLine.SessionDate = currDate;
                        PracticeLine.SoundDescription = sSoundName;
                        
                    }
                    else
                    {
                        if (PracticeLine.SessionDate != currDate)
                        {
                            // save the currenct values
                            PracticeLine.CalculatePct();
                            _PracticeLineStats.Add(PracticeLine);
                            PracticeLine = new MP_LineSoundStats();
                            PracticeLine.SessionDate = currDate;
                            PracticeLine.SoundDescription = sSoundName;
                        }
                        
                    }
                    GetAnswers(currSession.SessionId, ref iRight, ref iWrong);
                    PracticeLine.SoundCorrect = PracticeLine.SoundCorrect + iRight;
                    PracticeLine.SoundTotal = PracticeLine.SoundTotal + iRight + iWrong;
                    PracticePieRight.SoundWeight = PracticePieRight.SoundWeight + iRight;
                    PracticePieWrong.SoundWeight = PracticePieWrong.SoundWeight + iWrong;
                }
                else
                {
                    if (QuizzLine == null)
                    {
                        QuizzLine = new MP_LineSoundStats();
                        QuizzLine.SessionDate = currDate;
                        QuizzLine.SoundDescription = sSoundName;
                    }
                    else
                    {
                        if (QuizzLine.SessionDate != currDate)
                        {
                            QuizzLine.CalculatePct();
                            _QuizzLineStats.Add(QuizzLine);
                            QuizzLine = new MP_LineSoundStats();
                            QuizzLine.SessionDate = currDate;
                            QuizzLine.SoundDescription = sSoundName;
                        }
                    }
                    GetAnswers(currSession.SessionId, ref iRight, ref iWrong);
                    QuizzLine.SoundCorrect = QuizzLine.SoundCorrect + iRight;
                    QuizzLine.SoundTotal = QuizzLine.SoundTotal + iRight + iWrong;
                    QuizzPieRight.SoundWeight = QuizzPieRight.SoundWeight + iRight;
                    QuizzPieWrong.SoundWeight = QuizzPieWrong.SoundWeight + iWrong;     
                }
                

            }

            // save the current objects
            if (PracticeLine != null) 
            {
                PracticeLine.CalculatePct();
                _PracticeLineStats.Add(PracticeLine); 
            }
            if (QuizzLine != null) 
            {
                QuizzLine.CalculatePct();
                _QuizzLineStats.Add(QuizzLine); 
            }
            if (PracticePieRight.SoundWeight == 0 && PracticePieWrong.SoundWeight == 0)
            {
                _PracticePieStats = null;
            }
            else
            {
                _PracticePieStats.Add(PracticePieWrong);
                _PracticePieStats.Add(PracticePieRight);
            }
            if (QuizzPieRight.SoundWeight == 0 && QuizzPieWrong.SoundWeight == 0)
            {
                _QuizzPieStats = null;
            }
            else
            {
                _QuizzPieStats.Add(QuizzPieWrong);
                _QuizzPieStats.Add(QuizzPieRight);
            }
            

        }

        public void SelectData(int SoundId, int iQuestionType)
        {
            // read a session info from the Sessions.dat file; check if is between the dates and if the sound id is in the list.
            string sLine = string.Empty;
            string[] sText;
            int iSID;
            DateTime currDate;
            int currPair;
            bool bIsQuizz;
            List<int> Pairs = new List<int>();
            Sessions = new List<_MySession>();
            Answers = new List<_MyAnswer>();
            // build the list of Pairs to filter

            #region SELECT Sessions
            _MySession currSession;
            if (SoundId != 0)
            {
                for (int iCnt = 1; iCnt <=_myMain.CatPairs.Count; iCnt++)
                {
                    if (_myMain.CatPairs[iCnt].iCP1 == SoundId || _myMain.CatPairs[iCnt].iCP2 == SoundId)
                    {
                        Pairs.Add(iCnt);
                    }
                }
            }
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream ifs = isf.OpenFile(sSessionFileName, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(ifs);
            bool bSelect=true;

            while (!sr.EndOfStream)
            {
                bSelect = true;
                sLine = sr.ReadLine();
                sText = sLine.Split(sSep);
                iSID = Convert.ToInt32(sText[0]);
                currPair = Convert.ToInt32(sText[1]);
                currDate=Convert.ToDateTime(sText[3]);
                bIsQuizz = Convert.ToBoolean(sText[2]);
                if (currDate > _MaxDate) { _MaxDate = currDate; }
                if (currDate < _MinDate) { _MinDate = currDate; }
                if (_myMain.IsDateNotChanged) { _myMain.StatEndDate = _MaxDate; }
                if (_myMain.IsDateNotChanged) { _myMain.StatStartDate = _MinDate; }
                if (SoundId != 0)
                {
                    bSelect = false;
                    for (int iCnt = 0; iCnt < Pairs.Count; iCnt++)
                    {
                        if (currPair == Pairs[iCnt])
                        {
                            bSelect = true;
                            break;
                        }
                    }
                }
                if (bSelect)
                {
                    if (currDate < _myMain.StatStartDate  || currDate > _myMain.StatEndDate)
                    {
                        bSelect = false;
                    }
                }
                if (bSelect)
                {
                    bool bPosFind = false;
                    int iPos = 1;
                    currSession = new _MySession();
                    currSession.SessionId = iSID;
                    currSession.SessionDate = currDate;
                    currSession.IsQuizz = bIsQuizz;
                    for (int iCnt = 0; iCnt < Sessions.Count; iCnt++)
                    {
                        if (Sessions[iCnt].SessionDate > currDate)
                        {
                            bPosFind = true;
                            iPos = iCnt;
                            break;
                        }
                    }
                    if (bPosFind)
                    {
                        Sessions.Insert(iPos, currSession);
                    }
                    else
                    {
                        Sessions.Add(currSession);
                    }
                    
                }
                
                
                

            }
            sr.Close();
                
            ifs.Close();
            #endregion

            #region SELECT Answers
            _MyAnswer currAnswer;
            Answers = new List<_MyAnswer>();
            ifs = isf.OpenFile(sAnswersFileName, FileMode.OpenOrCreate, FileAccess.Read);
            sr = new StreamReader(ifs);
            while (!sr.EndOfStream)
            {
                sLine = sr.ReadLine();
                sText = sLine.Split(sSep);
                iSID = Convert.ToInt32(sText[0]);
                for (int iCnt = 0; iCnt < Sessions.Count; iCnt++)
                {
                    if (iSID == Sessions[iCnt].SessionId)
                    {
                        currAnswer = new _MyAnswer();
                        currAnswer.iSessionId = iSID;
                        currAnswer.iPairId = Convert.ToInt32(sText[1]);
                        currAnswer.iQuestionType = (MPQuestionType) Convert.ToInt32(sText[2]);
                        currAnswer.IsCorrect = Convert.ToBoolean(sText[3]);
                        if (iQuestionType == 0 || (MPQuestionType)iQuestionType == currAnswer.iQuestionType)
                        {
                            Answers.Add(currAnswer);
                        }
                    }
                }
            }
            sr.Close();
            ifs.Close();
            #endregion

            ComputeData(SoundId);
        }
        private void GetAnswers(int SessionId, ref uint iRight, ref uint iWrong)
        {
            iRight = 0;
            iWrong = 0;
            for (int iCnt = 0; iCnt < Answers.Count; iCnt++)
            {
                if (Answers[iCnt].iSessionId == SessionId)
                {
                    if (Answers[iCnt].IsCorrect)
                    {
                        iRight++;
                    }
                    else
                    {
                        iWrong++;
                    }
                }
            }
        }
    }
  
    #endregion


#region ML_Controls
    class ML_Button : Button
    {
        private bool _IsSelected;
        private SolidColorBrush scbBorder = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        private SolidColorBrush scbBack= new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
        private SolidColorBrush scbFore = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        private SolidColorBrush scbForeSel = new SolidColorBrush(Color.FromArgb(255,126,206,253));
        
        public bool IsSelected { set { _IsSelected = value; SetBackgound(); } get { return _IsSelected; } }
        private void SetBackgound()
        {
            if(_IsSelected)
            {
                BorderBrush = scbBorder;
                Foreground = scbForeSel;
                Background = scbBorder;
            }
            else
            {
                BorderBrush = scbBorder;
                Foreground = scbFore;
                Background = scbBack;
            }
        }
    }

#endregion
    
}
