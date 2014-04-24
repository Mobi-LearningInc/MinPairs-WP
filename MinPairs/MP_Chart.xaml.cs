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

namespace MinPairs
{
    public partial class MP_Chart : UserControl
    {
        public MP_Chart()
        {
            InitializeComponent();
        }

        private Size _MySize;
        private double _PracticePct;
        public double PracticePct { set { _PracticePct = value; } }
        private double _QuizzPct;
        public double QuizzPct { set { _QuizzPct = value; } }

        



        #region CONSTANTS
        private const byte TitlePosX = 20;
        private const byte TitlePosY = 2;
        private const byte TitleSizeX = 60;
        private const byte TitleSizeY = 10;

        private const byte AxisXPosX = 5;
        private const byte AxisXPosY = 5;
        private const byte AxisXSizeY = 90;

        private const byte AxisYPosX = 5;
        private const byte AxisYPosY = 5;
        private const byte AxisYSizeX = 90;

        private const byte LegendPosX = 70;
        private const byte LegendPosY = 60;
        private const byte LegendSizeX = 28;
        private const byte LegendSizeY = 38;

        private const byte ChartPosX = 5;
        private const byte ChartPosY = 5;
        private const byte ChartSizeX = 90;
        private const byte ChartSizeY = 90;

        private const byte TitleBorderThickness = 2;
        private const byte AxisThickness = 1;
        private const byte LegendBorderThickness = 1;
        private const byte CanvasBorderThickness = 3;
        private const byte ChartBorderThickness = 1;


        private Color CanvasBackground = Color.FromArgb(255, 62, 69, 76);
        private Color CanvasBorderColor = Color.FromArgb(255, 126, 206, 253);
        private Color ChartBackground = Color.FromArgb(255, 72, 80, 90);
        private Color ChartBorderColor = Color.FromArgb(255, 116, 196, 240);

        private Color CorrectAnswers = Color.FromArgb(255, 50, 255, 50);
        private Color WrongAnswers = Color.FromArgb(255, 255, 50, 50);

        
        #endregion

        #region Points
        private Point PracticePieCenter;
        private Point QuizzPieCenter;
        private double PieRadius = 0;
        #endregion


        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _MySize = e.NewSize;
            ResizeMe();
        }


        private void ResizeMe()
        {
            this.Width = _MySize.Width;
            this.Height = _MySize.Height;
            AllCanvas.Margin = new Thickness(0, 0, 0, 0);
            AllCanvas.Background = new SolidColorBrush(CanvasBackground);
            AllCanvas.Width = _MySize.Width;
            AllCanvas.Height = _MySize.Height;
            Rectangle AllBorder = new Rectangle();
            AllBorder.Width = _MySize.Width;
            AllBorder.Height = _MySize.Height;
            AllBorder.Stroke = new SolidColorBrush(CanvasBorderColor);
            AllBorder.StrokeThickness = CanvasBorderThickness;
            AllCanvas.Children.Add(AllBorder);

            PracticePieCenter = new Point(_MySize.Width / 4, _MySize.Height / 2);
            QuizzPieCenter = new Point(3*_MySize.Width / 4, _MySize.Height / 2);
            
            CreateChartArea();

        }

        private void CreateChartArea()
        {
            Canvas ChartCanvas = new Canvas();
            ChartCanvas.Height=(_MySize.Height * ChartSizeY)/100;
            ChartCanvas.Width = (_MySize.Width * ChartSizeX) / 100;
            PieRadius = ChartCanvas.Height / 2 - ChartCanvas.Height / 10;

            ChartCanvas.Background = new SolidColorBrush(ChartBackground);
            ChartCanvas.Margin = new Thickness((_MySize.Width * ChartPosX) / 100, (_MySize.Height * (100.0 - ChartPosY - ChartSizeY)) / 100, (_MySize.Width * (100 - ChartPosX - ChartSizeX)) / 100, (_MySize.Height * ChartPosY) / 100);
            Rectangle ChartBorder = new Rectangle();
            ChartBorder.Width = ChartCanvas.Width;
            ChartBorder.Height = ChartCanvas.Height;
            ChartBorder.Stroke = new SolidColorBrush(ChartBorderColor);
            ChartBorder.StrokeThickness = ChartBorderThickness;
            ChartCanvas.Children.Add(ChartBorder);
            ChartCanvas.Children.Add(PieChart());
            AllCanvas.Children.Add(ChartCanvas);

        }

        private Path PieChart()
        {
            // draw the Practice pie
            double dAngle1;
            double dAngle2;
            dAngle1 = 360 * _PracticePct;
            dAngle2=360-dAngle1;
            Point P1 = new Point(Math.Cos(dAngle1) * PieRadius + PracticePieCenter.X, Math.Sin(dAngle1) * PieRadius + PracticePieCenter.Y);

            Path PT = new Path();
            GeometryGroup GG = new GeometryGroup();
            PathGeometry PG = new PathGeometry();
            PathFigure PF = new PathFigure();
            LineSegment L = new LineSegment();
            ArcSegment A = new ArcSegment();
            PF.IsClosed = true;
            PF.IsFilled = true;
            PF.StartPoint = PracticePieCenter;
            
            L.Point = new Point(PracticePieCenter.X , PracticePieCenter.Y - PieRadius);
            PF.Segments.Add(L);

            if (dAngle1 >= 180)
            {
                A.IsLargeArc = true;
            }
            else
            {
                A.IsLargeArc = false;
            }
            A.SweepDirection = SweepDirection.Clockwise;
            A.Point = P1;
            A.Size = new Size(PieRadius, PieRadius);
            PF.Segments.Add(A);
            L = new LineSegment();
            L.Point = PracticePieCenter;

            PF.Segments.Add(L);
            PG.Figures.Add(PF);

            PT.Data = PG;
            PT.Fill = new SolidColorBrush(CorrectAnswers);

            return PT;
        }



        public void SetSize(Size ChartSize)
        {
            _MySize = ChartSize;
            ResizeMe();
        }
    }
}
