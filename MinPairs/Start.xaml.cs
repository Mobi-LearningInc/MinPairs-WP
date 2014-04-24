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
    public partial class Start : PhoneApplicationPage
    {
        private ProgressIndicator _progressIndicator;
        MP_Comm MPC;
        public Start()
        {
            InitializeComponent();
            
        }

        private void MakeComm()
        {
            MPC = (MP_Comm)PhoneApplicationService.Current.State["MC"];
            MPC.DownloadStarted += new MPDownloadStart(MPC_DownloadStarted);
            MPC.DownloadEnded += new MPDownloadEnd(MPC_DownloadEnded);
            MPC.AskDownload += new MPAskForDowload(MPC_AskDownload);
            MPC.GetAppGUID();
            
        }

        void MPC_AskDownload(object o, MPDataEventArgs e)
        {
            //throw new NotImplementedException();
            if (_progressIndicator != null)
            {
                _progressIndicator.IsIndeterminate = false;
            }
            MessageBoxResult mbr= MessageBox.Show(e.MPMessage, "Dowload ?", MessageBoxButton.OKCancel);
            if (mbr == MessageBoxResult.OK)
            {
            }
        }

        void MPC_DownloadEnded(object o, MPDataEventArgs e)
        {
            //throw new NotImplementedException();
            if (_progressIndicator != null)
            {
                _progressIndicator.IsIndeterminate = false;
            }
        }

        void MPC_DownloadStarted(object o, MPDataEventArgs e)
        {
            //throw new NotImplementedException();
            //MessageBox.Show(e.MPMessage, "Download started", MessageBoxButton.OK);
            if (null == _progressIndicator)
            {
                _progressIndicator = new ProgressIndicator();
                _progressIndicator.IsVisible = true;
                SystemTray.ProgressIndicator = _progressIndicator;
            }
            _progressIndicator.IsIndeterminate = true;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            MakeComm();
        }
    }
}