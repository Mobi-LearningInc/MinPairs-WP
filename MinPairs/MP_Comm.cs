using System;
using System.Net;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Security.Cryptography;


namespace MinPairs
{
    public delegate void MPAskForDowload(object o, MPDataEventArgs e);
    public delegate void MPCommError(object o, MPDataEventArgs e);
    public delegate void MPDownloadStart(object o, MPDataEventArgs e);
    public delegate void MPDownloadEnd(object o, MPDataEventArgs e);
    public delegate void MPDownloadProgress(object o, MPDataEventArgs e);
    public class MP_Comm
    {

        public bool bIsReady = false;
        
        WebClient WC = new WebClient();
        string sCommURL = @"http://mobilearning.no-ip.org/MPServer/MPService.svc/";
        //string sCommURL = @"http://localhost/MP_Server/MPService.svc/";
        string sAppGUID = "8378F636-DDDC-4039-96D2-E2A622F6C157";
        string sAppStateFileName = "MPState.txt";
        string sAppStateDirectory = "App";
        string sDataFilesDirectory = "DataFiles";
        string sMediaFilesDirectory = "Media";
        string sDownloadTempDirectory = "MPTemp";
        string sCategFIleName = "MP_Categories.dat";
        string sItemsFileName = "MP_Items.dat";
        Int16 iAppVersion = -1;
        Int16 iAppNewVersion = -1;
        string sDataType = string.Empty;
        private string sStatus = "False";
        public void GetAppGUID()
        {
            sDataType = "MPRegAppOut";
            if (!CheckStorageFile(sAppStateDirectory + @"\" + sAppStateFileName))
            {   //register the application
                MPRegAppIn oIn = new MPRegAppIn();
                oIn.AppID = sAppGUID;
                string sParam = string.Empty;
                sParam = JsonConvert.SerializeObject(oIn);
                sParam = Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(sParam));
                string sURL = sCommURL + @"RegisterApplication/" + sParam;

                WC.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WC_DownloadProgressChanged);
                WC.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WC_DownloadAppRegCompleted);
                string sMsg = "Reg app downlod started";
                MPDataEventArgs e = new MPDataEventArgs(sMsg);
                OnMPDownloadStarted((object)this, e);
                // raise DownloadedStarted event
                WC.DownloadStringAsync(new Uri(sURL));

                //now that the app is registered you need to download the data files


            }
            else
            {
                // check if the app has the latest version
                sDataType = "MPGetVersion";
                string sParam = string.Empty;
                MPDataVersionIn oIn = new MPDataVersionIn();
                oIn.AppID = sAppGUID;
                sParam = JsonConvert.SerializeObject(oIn);
                sParam = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(sParam));
                string sURL = sCommURL + @"GetDataVersion/" + sParam;

                WC.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WC_DownloadProgressChanged);
                WC.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WC_DownloadAppVerCompleted);
                WC.DownloadStringAsync(new Uri(sURL));

            }
        }

        public void GetAppData()
        {
            sDataType = "MPAppData";
            MPDataFilesIn oIn = new MPDataFilesIn();
            oIn.AppID = sAppGUID;
            oIn.AppVersion = iAppNewVersion;
            string sParam = JsonConvert.SerializeObject(oIn);
            sParam = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(sParam));
            string sURL = sCommURL + @"GetAppData/" + sParam;
            WC.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WC_DownloadProgressChanged);
            WC.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WC_DownloadAppDataFilesCompleted);
            WC.DownloadStringAsync(new Uri(sURL));
        }

        public void GetAppMedia()
        {
            bool bSuccess = true;
            string sDataFileName;
            string sRec = string.Empty;
            // fill the array with the file names
            // categories
            sDataFileName = sDownloadTempDirectory + @"\" + sCategFIleName;
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            string[] fn;
            IsolatedStorageFileStream FS = isf.OpenFile(sDataFileName, FileMode.Open);
            StreamReader sr = new StreamReader(FS);
            while (!sr.EndOfStream)
            {
                sRec = sr.ReadLine();
                fn = sRec.Split('|');
            }

            
        }
        private bool DownloadAudioFile(string sFileName)
        {
            MPAudioFileIn oIn = new MPAudioFileIn();
            oIn.AppID = sAppGUID;
            oIn.AppVersion = iAppNewVersion;
            oIn.FileName = sFileName;

            return false;


        }
        private bool DownloadImageFile(string sFileName)
        {
            MPImageFileIn oIn = new MPImageFileIn();
            oIn.AppID = sAppGUID;
            oIn.AppVersion = iAppNewVersion;
            oIn.FileName = sFileName;
            return false;

        }
        
        void WC_DownloadAppRegCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string sMsg = string.Empty;
            WebClient wc1 = (WebClient)sender;
            MPDataEventArgs ef = new MPDataEventArgs(sMsg);
            OnMPDownloadEnded((object)this, ef);
            WC.DownloadStringCompleted -= new DownloadStringCompletedEventHandler(WC_DownloadAppRegCompleted);
            #region RegApp
                if (!e.Cancelled && e.Error == null)
                {

                    string sRes = e.Result.Substring(1, e.Result.Length - 2);
                    sRes = MP_Static.DecodeFrom64(sRes);
                    MPRegAppOut oOut = JsonConvert.DeserializeObject<MPRegAppOut>(sRes);
                    if (oOut.Res.Success)
                    {
                        sAppGUID = oOut.NewAppId;
                        iAppVersion = oOut.AppVersion;
                        iAppNewVersion = oOut.AppVersion;
                        ProcessDwnloadedData(oOut.NewAppId + "|" + oOut.AppVersion.ToString() + "|" + sStatus);
                        GetAppData();
                    }
                    else
                    {
                        sMsg = "Application failed to register. Try again later.";
                        sMsg = sMsg + System.Environment.NewLine + oOut.Res.ErrMsg;
                        MPDataEventArgs e1 = new MPDataEventArgs(sMsg);
                        OnMPError((object)this, e1);
                    }
                }
                else
                {
                    // throw error for register app
                    sMsg = "Application failed to register. Try again later.";
                    sMsg = sMsg + System.Environment.NewLine + e.Error.Message;
                    MPDataEventArgs e1 = new MPDataEventArgs(sMsg);
                    OnMPError((object)this, e1);
                }
                #endregion
        }
        void WC_DownloadAppVerCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string sMsg = string.Empty;
            MPDataEventArgs ef = new MPDataEventArgs(sMsg);
            OnMPDownloadEnded((object)this, ef);
            WC.DownloadStringCompleted -= new DownloadStringCompletedEventHandler(WC_DownloadAppVerCompleted);
            #region Check App Version


            if (!e.Cancelled && e.Error == null)
            {
                string sRes = e.Result.Substring(1, e.Result.Length - 2);
                sRes = MP_Static.DecodeFrom64(sRes);
                MPDataVersionOut oOut = JsonConvert.DeserializeObject<MPDataVersionOut>(sRes);
                if (oOut.Res.Success)
                {
                    if (oOut.DataVersion != iAppVersion)
                    {
                        // raise event for download the data files
                        iAppNewVersion = oOut.DataVersion;
                        sMsg = "New set of sounds and words are available.";
                        sMsg = sMsg + System.Environment.NewLine + "Download now ?";
                        MPDataEventArgs e1 = new MPDataEventArgs(sMsg);
                        OnMPAsk((object)this, e1);
                    }
                    else
                    {

                    }
                }
                else
                {
                    // throw error for getting app version
                    sMsg = "Application failed to get latest version. Try again later.";
                    sMsg = sMsg + System.Environment.NewLine + oOut.Res.ErrMsg;
                    MPDataEventArgs e1 = new MPDataEventArgs(sMsg);
                    OnMPError((object)this, e1);
                }
            }
            else
            {
                // throw error for getting app version
                sMsg = "Application failed to get latest version. Try again later.";
                sMsg = sMsg + System.Environment.NewLine + e.Error.Message;
                MPDataEventArgs e1 = new MPDataEventArgs(sMsg);
                OnMPError((object)this, e1);
            }


            #endregion

        }
        void WC_DownloadAppDataFilesCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string sMsg = string.Empty;
            MPDataEventArgs ef = new MPDataEventArgs(sMsg);
            OnMPDownloadEnded((object)this, ef);
            WC.DownloadStringCompleted -= new DownloadStringCompletedEventHandler(WC_DownloadAppDataFilesCompleted);
            #region Get Data Files
            if (!e.Cancelled && e.Error == null)
            {
                // got the data files
                string sRes = e.Result.Substring(1, e.Result.Length - 2);
                sRes = MP_Static.DecodeFrom64(sRes);
                MPDataFilesOut oOut = JsonConvert.DeserializeObject<MPDataFilesOut>(sRes);
                if (oOut.Res.Success)
                {
                    foreach (MPFile mpf in oOut.DataFiles)
                    {
                        try
                        {
                            //SaveFile(sDataFilesDirectory, mpf.FileName, mpf.Content);
                            SaveFile(sDownloadTempDirectory, mpf.FileName, mpf.Content);
                        }
                        catch (Exception ex)
                        {
                            sMsg = "Application failed to save data. Try again later.";
                            sMsg = sMsg + System.Environment.NewLine + ex.Message;
                            MPDataEventArgs e1 = new MPDataEventArgs(sMsg);
                            OnMPError((object)this, e1);
                        }

                    }


                }
                else
                {
                    sMsg = "Application failed to download data. Try again later.";
                    sMsg = sMsg + System.Environment.NewLine + oOut.Res.ErrMsg;
                    MPDataEventArgs e1 = new MPDataEventArgs(sMsg);
                    OnMPError((object)this, e1);
                }

            }
            else
            {
                sMsg = "Application failed to download data. Try again later.";
                sMsg = sMsg + System.Environment.NewLine + e.Error.Message;
                MPDataEventArgs e1 = new MPDataEventArgs(sMsg);
                OnMPError((object)this, e1);
            }


            #endregion
        }
       
        

        void WC_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
            
            

        }


        
        void OnMPError(object o, MPDataEventArgs e)
        {
            if (ThrowError != null)
            {
                ThrowError(o, e);
            }
        }
        void OnMPDownloadStarted(object o, MPDataEventArgs e)
        {
            if (DownloadStarted != null)
            {
                DownloadStarted((object)this, e);
            }
        }
        void OnMPDownloadEnded(object o, MPDataEventArgs e)
        {
            if (DownloadEnded != null)
            {
                DownloadEnded((object)this, e);
            }
        }
        void OnMPDownloadProgress(object o, MPDataEventArgs e)
        {
            if (DownloadProgress != null)
            {
                DownloadProgress((object)this, e);
            }
        }
        void OnMPAsk(object o, MPDataEventArgs e)
        {
            if (AskDownload != null)
            {
                AskDownload((object)this, e);
            }
        }
        

        public event MPAskForDowload AskDownload;
        public event MPCommError ThrowError;
        public event MPDownloadStart DownloadStarted;
        public event MPDownloadEnd DownloadEnded;
        public event MPDownloadProgress DownloadProgress;


        private bool CheckStorageFile(string sFileName)
        {
            bool bRes = false;
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            bRes = isf.FileExists(sFileName);
            if (bRes)
            {
                try
                {
                    IsolatedStorageFileStream FS= isf.OpenFile(sFileName, FileMode.Open);
                    if (sDataType == "MPRegAppOut")
                    {
                        StreamReader sr = new StreamReader(FS);
                        string strR = string.Empty;
                        string[] sa;
                        strR = sr.ReadToEnd();
                        sa = strR.Split('|');
                        sAppGUID = sa[0];
                        iAppVersion = Convert.ToInt16(sa[1]);
                        bRes = true;
                    }
                }
                catch (Exception ex)
                {
                    bRes = false;
                    string sMsg = "Application failed to read stored data. Please restart the app.";
                    sMsg = sMsg + System.Environment.NewLine + ex.Message;
                    MPDataEventArgs e1 = new MPDataEventArgs(sMsg);
                    OnMPError((object)this, e1);
                }

            }
            return bRes;
        }


        private void ProcessDwnloadedData(string strData)
        {
            if (sDataType == "MPRegAppOut")
            {
                ProcRegAppData(strData);
                return;
            }
        }

        private void ProcRegAppData(string strData)
        {  
            SaveFile(sAppStateDirectory, sAppStateFileName, strData);
        }

        private void SaveFile(string sFolder, string sFileName, string FileData)
        {
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (isf.GetDirectoryNames(sFolder).Length == 0)
                {
                    isf.CreateDirectory(sFolder);
                }
                sFileName = sFolder + @"\" + sFileName;
                if(isf.FileExists(sFileName))
                {
                    isf.DeleteFile(sFileName);
                }
                IsolatedStorageFileStream fs = new IsolatedStorageFileStream(sFileName, FileMode.CreateNew, isf);
                byte[] bData = System.Text.UTF8Encoding.UTF8.GetBytes(FileData);
                int iCnt = System.Text.UTF8Encoding.UTF8.GetByteCount(FileData);
                fs.Write(bData,0,iCnt);
                fs.Flush();
            }
            catch (Exception ex)
            {
                string sMsg = string.Empty;
                if (sDataType == "MPRegAppOut")
                {
                    
                    sMsg = "Application failed to register. Try again later.";
                }
                sMsg = sMsg + System.Environment.NewLine + ex.Message;
                MPDataEventArgs e1 = new MPDataEventArgs(sMsg);
                OnMPError((object)this, e1);
            }
        }
        private void SaveFile(string sFolder, string sFileName, byte[] FileData)
        {
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (isf.GetDirectoryNames(sFolder).Length == 0)
                {
                    isf.CreateDirectory(sFolder);
                }
                sFileName = sFolder + @"\" + sFileName;
                if(isf.FileExists(sFileName))
                {
                    isf.DeleteFile(sFileName);
                }
                IsolatedStorageFileStream fs = new IsolatedStorageFileStream(sFileName, FileMode.CreateNew, isf);
                fs.Write(FileData,0,FileData.Length);
                fs.Flush();
                fs.Close();
            }
            catch (Exception ex)
            {
                string sMsg = "Application failed to register. Try again later.";
                sMsg = sMsg + System.Environment.NewLine + ex.Message;
                MPDataEventArgs e1 = new MPDataEventArgs(sMsg);
                OnMPError((object)this, e1);
            }
        }
    }

    public class MPDataEventArgs : EventArgs
    {
        public readonly string MPMessage;
        public MPDataEventArgs(string sMsg)
        {
            MPMessage = sMsg;
        }
    }
    public class MPResult
    {
        string _ErrMsg = string.Empty;
        bool _Success = true;

        public string ErrMsg
        {
            get { return _ErrMsg; }
            set { _ErrMsg = value; }
        }

        public bool Success
        {
            get { return _Success; }
            set { _Success = value; }
        }
    }
    public class MPCommIn_Base
    {
        private string _ID;
        public string AppID
        {
            get { return _ID; }
            set { _ID = value; }
        }
    }
    public class MPCommOut_Base
    {
        private MPResult _res;
        public MPResult Res { set { _res = value; } get { return _res; } }
    }
    public class MPRegAppIn : MPCommIn_Base
    {

    }
    public class MPRegAppOut : MPCommOut_Base
    {
        private string _NewAppId;
        public string NewAppId { set { _NewAppId = value; } get { return _NewAppId; } }

        private Int16 _AppVersion;
        public Int16 AppVersion { set { _AppVersion = value; } get { return _AppVersion; } }
    }
    // data files classes
    public class MPFile
    {
        private string _FileName;
        public string FileName { set { _FileName = value; } get { return _FileName; } }

        private byte[] _Content;
        public byte[] Content { set { _Content = value; } get { return _Content; } }

        private string _Hash;
        public string Hash { set { _Hash = value; } get { return _Hash; } }
    }
    public class MPDataFilesIn : MPCommIn_Base
    {
        private Int16 _AppVersion;
        public Int16 AppVersion { set { _AppVersion = value; } get { return _AppVersion; } }
    }
    public class MPDataFilesOut : MPCommOut_Base
    {
        public List<MPFile> DataFiles;
    }
    public class MPImageFileIn : MPCommIn_Base
    {
        private Int16 _AppVersion;
        public Int16 AppVersion { set { _AppVersion = value; } get { return _AppVersion; } }

        private string _FileName;
        public string FileName { set { _FileName = value; } get { return _FileName; } }
    }
    public class MPAudioFileIn : MPCommIn_Base
    {
        private Int16 _AppVersion;
        public Int16 AppVersion { set { _AppVersion = value; } get { return _AppVersion; } }

        private string _FileName;
        public string FileName { set { _FileName = value; } get { return _FileName; } }
    }
    public class MPImageFileOut : MPCommOut_Base
    {
        public MPFile ImageFile;
    }
    public class MPAudioFileOut : MPCommOut_Base
    {
        public MPFile AudioFile;
    }
    public class MPDataVersionIn : MPCommIn_Base
    {
    }
    public class MPDataVersionOut : MPCommOut_Base
    {
        private Int16 _DataVersion;
        public Int16 DataVersion { set { _DataVersion = value; } get { return _DataVersion; } }
    }
    


    public static class MP_Static
    {
        public static string EncodeTo64(string strSource)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(strSource));
        }
        public static string DecodeFrom64(string strSource)
        {
            //return Encoding.UTF8.GetString(Convert.FromBase64String(strSource));
            byte[] b = Convert.FromBase64String(strSource);
            return Encoding.UTF8.GetString(b, 0, b.Length);
        }
    }
}
