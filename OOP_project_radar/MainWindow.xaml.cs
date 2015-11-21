using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Point = System.Drawing.Point;

namespace OOP_project_radar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Timer _timer = new Timer();
        private int _currentDegree;  //in degree
        private int _centerX;
        private int _centerY;     //center of the circle
        private int _refresherX;
        private int _refresherY;       //HAND coordinate
        private readonly int _offset = (Constans.BITMAP_WIDTH - Constans.RADAR_WIDTH) / 2;
        private bool _isReverted;
        private readonly List<Point> _shadowArray = new List<Point>(8);

        public Bitmap Bmp;
        public Graphics G;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {

            //create Bitmap
            Bmp = new Bitmap(Constans.BITMAP_HEIGHT, Constans.BITMAP_WIDTH);

            //center
            _centerX = Constans.RADAR_WIDTH / 2 + _offset;
            _centerY = Constans.RADAR_WIDTH / 2 + _offset;

            //initial degree of HAND
            _currentDegree = 0;

            //timer
            _timer.Interval = 1; //in millisecond
            _timer.Tick += ReloadRadar;

            //Draw start position of radar
            ReloadRadar(null, null);
        }

        private void ReloadRadar(object sender, EventArgs e)
        {
            G = Graphics.FromImage(Bmp);
            G.Clear(Color.Black);

            DrawRadarBody();

            //we have shadow with 8 lines
            _shadowArray.Add(new Point(_refresherX, _refresherY));
            if (_shadowArray.Count == 8)
            {
                _shadowArray.Remove(_shadowArray.First());
            }

            int i = _shadowArray.Capacity-1;
            //draw Shadow
            foreach (var point in _shadowArray)
            {
                var color = ColorTranslator.FromHtml(Constans.SHADOW_COLORS[i]);
                var pen = new Pen(new SolidBrush(color), 3f);

                G.DrawLine(pen, new Point(_centerX, _centerY), point);
                i--;
            }

            //draw HAND
            G.DrawLine(Constans.REFRESHER_PEN, new Point(_centerX, _centerY), new Point(_refresherX, _refresherY));

            //load bitmap in picturebox1
            ImageBox.Source = BitmapToImageSource(Bmp);

            //dispose
            G.Dispose();

            //update
            if (_isReverted)
            {
                _currentDegree--;
                if (_currentDegree == 0)
                {
                    _currentDegree = 360;
                }
            }
            else
            {
                _currentDegree++;
                if (_currentDegree == 360)
                {
                    _currentDegree = 0;
                }
            }
        }

        /// <summary>
        /// Draw Axes & Axes labels , Circles and graduation
        /// </summary>
        private void DrawRadarBody()
        {
            if (_currentDegree >= 0 && _currentDegree <= 180)
            {
                //right half
                _refresherX = _centerX + (int)(Constans.HAND * Math.Sin(Math.PI * _currentDegree / 180));
                _refresherY = _centerY - (int)(Constans.HAND * Math.Cos(Math.PI * _currentDegree / 180));
            }
            else
            {
                _refresherX = _centerX - (int)(Constans.HAND * -Math.Sin(Math.PI * _currentDegree / 180));
                _refresherY = _centerY - (int)(Constans.HAND * Math.Cos(Math.PI * _currentDegree / 180));
            }

            // Constans.FIRST_CIRCLE_WIDTH/2 - radius, and then div for 4 - because we have 4 circles
            var circleOffset = Constans.FIRST_CIRCLE_WIDTH / 2 / 4;
            //draw circle
            G.DrawEllipse(Constans.CIRCLE_PEN, _offset, _offset, Constans.FIRST_CIRCLE_WIDTH, Constans.FIRST_CIRCLE_HEIGHT);
            //bigger circle
            G.DrawEllipse(Constans.CIRCLE_PEN, _offset + circleOffset, _offset + circleOffset, Constans.SECOND_CIRCLE_WIDTH,
                Constans.SECOND_CIRCLE_HEIGHT); //smaller circle
            G.DrawEllipse(Constans.CIRCLE_PEN, _offset + 2 * circleOffset, _offset + 2 * circleOffset, Constans.THIRD_CIRCLE_WIDTH,
                Constans.THIRD_CIRCLE_HEIGHT); //smaller circle
            G.DrawEllipse(Constans.CIRCLE_PEN, _offset + 3 * circleOffset, _offset + 3 * circleOffset, Constans.FOURTH_CIRCLE_WIDTH,
                Constans.FOURTH_CIRCLE_HEIGHT); //smaller circle

            //draw perpendicular line
            G.DrawLine(Constans.AXIS_PEN, new Point(_centerX, 0), new Point(_centerX, Constans.BITMAP_HEIGHT)); // UP-DOWN
            G.DrawLine(Constans.AXIS_PEN, new Point(0, _centerY), new Point(Constans.BITMAP_WIDTH, _centerY)); //LEFT-RIGHT

            //draw diagonal lines
            G.DrawLine(Constans.AXIS_PEN, new Point(0, 0), new Point(Constans.BITMAP_WIDTH, Constans.BITMAP_HEIGHT)); // UP-DOWN
            G.DrawLine(Constans.AXIS_PEN, new Point(0, Constans.BITMAP_WIDTH), new Point(Constans.BITMAP_HEIGHT, 0));
            //LEFT-RIGHT

            //Axes labels
            G.DrawString("N", new Font("Arial", 16), new SolidBrush(Color.FloralWhite), _centerX - 20, 1);
            G.DrawString("W", new Font("Arial", 16), new SolidBrush(Color.FloralWhite), 1, _centerY - 20);
            G.DrawString("E", new Font("Arial", 16), new SolidBrush(Color.FloralWhite), 2 * _centerX - 20, _centerY - 20);
            G.DrawString("S", new Font("Arial", 16), new SolidBrush(Color.FloralWhite), _centerX, 2 * _centerY - 20);

            //Graduation
            G.DrawString("0", new Font("Arial", 8), new SolidBrush(Color.FloralWhite), _centerX + 2, 8);
            G.DrawString("45", new Font("Arial", 8), new SolidBrush(Color.FloralWhite), Constans.BITMAP_WIDTH - _offset - 75 + 3,
                _offset + 75);
            G.DrawString("90", new Font("Arial", 8), new SolidBrush(Color.FloralWhite), 2 * _centerX - 20, _centerY + 2);
            G.DrawString("135", new Font("Arial", 8), new SolidBrush(Color.FloralWhite),
                Constans.BITMAP_WIDTH - _offset - 75 + 4, Constans.BITMAP_WIDTH - _offset - 75 - 14);
            G.DrawString("180", new Font("Arial", 8), new SolidBrush(Color.FloralWhite), _centerX - 25, 2 * _centerY - 20);
            G.DrawString("225", new Font("Arial", 8), new SolidBrush(Color.FloralWhite), _offset + 75 - 5,
                Constans.BITMAP_WIDTH - _offset - 75 + 5);
            G.DrawString("270", new Font("Arial", 8), new SolidBrush(Color.FloralWhite), 1, _centerY + 2);
            G.DrawString("315", new Font("Arial", 8), new SolidBrush(Color.FloralWhite), _offset + 75 - 8, _offset + 75 - 19);
        }

        /// <summary>
        /// Transform Bitmap to ImageSource
        /// </summary>
        /// <param name="bitmap">Bitmap which need to transform</param>
        /// <returns>returns BitmapImage (Image Source)</returns>
        public BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }


        #region BUTTONS handlers

        private void btnRevert_OnClick(object sender, RoutedEventArgs e)
        {
            _isReverted = !_isReverted;
            _shadowArray.Clear();
        }

        private void btnStart_OnClick(object sender, RoutedEventArgs e)
        {
            if (_timer.Enabled)
                _timer.Stop();
            else
                _timer.Start();
        }

        private void btnRestart_OnClick(object sender, RoutedEventArgs e)
        {
            _currentDegree = 0;
            _shadowArray.Clear();

            if (!_timer.Enabled)
                _timer.Start();
        }

        #endregion

        private void Speed_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _timer.Stop();
            _timer.Interval = 100 / Convert.ToInt32(e.NewValue);
            _timer.Start();
        }
    }
}
