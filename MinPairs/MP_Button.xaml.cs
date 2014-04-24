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
using System.Text;
using System.Windows.Media.Imaging;

namespace MinPairs
{
    public partial class MP_Button : UserControl
    {

        private string _caption;
        public string Caption { set { _caption = value; } }
        private string _imageName;
        public string ImageName { set { _imageName = value; } }

        public MP_Button()
        {
            InitializeComponent();
            this.WordImage.Source = new BitmapImage(new Uri("/Image/na1.png", UriKind.Relative));
        }

       

        
        public void Flip()
        {
            myClose.Begin();
        }
        private void SwitchImage()
        {
            MyWord.Text = _caption;
           
            if (_imageName.Length > 0)
            {
                if (Application.GetResourceStream(new Uri("img/" + _imageName,UriKind.Relative)) != null)
                {
                    this.WordImage.Source = new BitmapImage(new Uri("/img/" + _imageName, UriKind.Relative));
                }
            }
            else
            {
                this.WordImage.Source = new BitmapImage(new Uri("/Image/na1.png", UriKind.Relative));
            }
        }

        private void myClose_Completed(object sender, EventArgs e)
        {
            SwitchImage();
            myOpen.Begin();
        }

        private void myOpen_Completed(object sender, EventArgs e)
        {
            MyWord.Text = _caption;
        }


    }
}
