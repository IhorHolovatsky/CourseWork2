using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using OOP_project_radar.SharedConstans;
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;
using Pen = System.Drawing.Pen;
using Point = System.Drawing.Point;

namespace OOP_project_radar.Radar
{
    internal class Radar
    {
        public Image ImageBox;
        public bool IsReverted;
        public List<TargetProvider> Targets = new List<TargetProvider>();

        private readonly Timer _timer = new Timer();
        private int _currentDegree; //in degree
        private readonly int _centerX;
        private readonly int _centerY; //center of the circle
        private int _refresherX;
        private int _refresherY; //HAND coordinate
        private readonly int _offset;
        private readonly int _bitmapWidth;
        private readonly int _bitmapHeight;
        private int[,] _landscape;
        private Pen _clearPen = new Pen(Color.Black, 3f);


        private readonly List<Point> _shadowArray = new List<Point>(9);

        public Bitmap Bmp;
        public Graphics G;

        public Radar(Image imageBox)
        {
            ImageBox = imageBox;

            _bitmapWidth = (int)imageBox.Width;
            _bitmapHeight = (int)imageBox.Height;

            _offset = (_bitmapWidth - Constans.RADAR_WIDTH) / 2;

            //create Bitmap
            Bmp = new Bitmap(_bitmapHeight, _bitmapWidth);

            //center
            _centerX = (int)imageBox.Height / 2;
            _centerY = (int)imageBox.Width / 2;

            //initial degree of HAND
            _currentDegree = 0;

            //timer
            _timer.Interval = 1; //in millisecond
            _timer.Tick += ReloadRadar;

            _landscape = LandscapeProvider.CreateLandscape();

            //Draw start position of radar
            ReloadRadar(null, null);
        }

        /// <summary>
        /// Start's radar scanning
        /// </summary>
        public void Start()
        {
            if (_timer.Enabled)
                _timer.Stop();
            else
                _timer.Start();
        }

        /// <summary>
        /// Sets radar to start position
        /// </summary>
        public void Restart()
        {
            _timer.Stop();

            G = Graphics.FromImage(Bmp);
            G.Clear(Color.Transparent);
            DrawRadarBody();
            ImageBox.Source = BitmapToImageSource(Bmp);

            _currentDegree = 0;
            _shadowArray.Clear();

            _timer.Start();
        }

        /// <summary>
        /// Reverts radar direction of scanning
        /// </summary>
        public void Revert()
        {
            IsReverted = !IsReverted;
            //_shadowArray.Clear();
        }

        /// <summary>
        /// Changing speed value handler
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">Event args for slider</param>
        public void Speed_OnValueChangedHandler(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _timer.Stop();
            _timer.Interval = 100 / Convert.ToInt32(e.NewValue);
            _timer.Start();
        }

        /// <summary>
        /// Reload radar image
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">Event args</param>
        public void ReloadRadar(object sender, EventArgs e)
        {
            G = Graphics.FromImage(Bmp);

            DrawRadarBody();

            //we have shadow with 8 lines
            _shadowArray.Add(new Point(_refresherX, _refresherY));

            int i = _shadowArray.Count - 1;
            //draw Shadow
            foreach (var point in _shadowArray)
            {
                //first line is for landscape
                if (point == _shadowArray.First())
                {
                    i--;
                    continue;
                }

                var color = ColorTranslator.FromHtml(Constans.SHADOW_COLORS[i]);


                //    G.DrawLine(pen, new Point(_centerX, _centerY), point);
                G.FillPie(new SolidBrush(color), new Rectangle(_offset, _offset, Constans.RADAR_WIDTH, Constans.RADAR_HEIGHT), _currentDegree - 90 - i, 1f);
                i--;
            }

            if (_shadowArray.Count == 9)
            {
                var needToEraseLinePoint = _shadowArray.First();

                //Eraser line
                G.FillPie(new SolidBrush(_clearPen.Color), new Rectangle(_offset-6, _offset-6, Constans.RADAR_WIDTH+12, Constans.RADAR_HEIGHT+12), _currentDegree - 90 - 8 + 0.5f, 1f);
                //landscape filter
                G.DrawLine(_clearPen, _centerX, _centerY, needToEraseLinePoint.X, needToEraseLinePoint.Y);

                _shadowArray.Remove(needToEraseLinePoint);

                //draw landscape
                if (_currentDegree - 9 >= 0)
                    DrawLandscape(_currentDegree - 9, needToEraseLinePoint);
                else
                    DrawLandscape(_currentDegree + 9, needToEraseLinePoint);


                //draw targets
                foreach (var target in Targets)
                {
                    if (LineIntersectsTarget(new Point(_centerX, _centerY),
                        new Point(needToEraseLinePoint.X, needToEraseLinePoint.Y), target.TargetZone))
                    {
                        target.IsDetected = true;

                        if (Math.Abs(target.Distance) > 0.01 & (target.Distance - GetDistanse(new Point(_centerX, _centerY), new Point(target.X, target.Y))) > 0.01)
                        {
                            var speed = (target.Distance -
                                         GetDistanse(new Point(_centerX, _centerY), new Point(target.X, target.Y))) * Constans.RADAR_FREQUENCY;
                            target.Speed = speed;
                        }
                            
                        target.Distance = GetDistanse(new Point(_centerX, _centerY), new Point(target.X, target.Y));
                        target.Angle = Convert.ToInt32(GetAngle(new Point(target.X, target.Y)));

                        var color = Color.FromArgb(target.TargetColor.A, target.TargetColor.R, target.TargetColor.G,
                            target.TargetColor.B);

                        G.FillRectangle(new SolidBrush(color), target.TargetZone);

                        //if (target.Angle >= 0 & target.Angle < 90)
                        //{
                        //    G.DrawString(target.Angle + "*", new Font("Arial", 8), new SolidBrush(color),
                        //        target.X - 10, target.Y - 10);
                        //}
                        //else if (target.Angle >= 90 & target.Angle < 180)
                        //{
                        //    G.DrawString(target.Angle + "*", new Font("Arial", 8), new SolidBrush(color),
                        //       target.X + 10, target.Y - 10);
                        //}
                        //else if (target.Angle >= 180 & target.Angle < 270)
                        //{
                        //    G.DrawString(target.Angle + "*", new Font("Arial", 8), new SolidBrush(color),
                        //      target.X + 10, target.Y + 10);
                        //}
                        //else
                        //{
                        //    G.DrawString(target.Angle + "*", new Font("Arial", 8), new SolidBrush(color),
                        //      target.X - 10, target.Y + 10);
                        //}

                    }

                }

            }

            //draw HAND
            //G.DrawLine(Constans.REFRESHER_PEN, new Point(_centerX, _centerY), new Point(_refresherX, _refresherY));
            G.FillPie(new SolidBrush(Constans.REFRESHER_PEN.Color), new Rectangle(_offset, _offset, Constans.RADAR_WIDTH, Constans.RADAR_HEIGHT), _currentDegree - 90, 1);


            //load bitmap in picturebox1
            ImageBox.Source = BitmapToImageSource(Bmp);

            //dispose
            G.Dispose();

            //update
            if (IsReverted)
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

        public void ChangeLandscape()
        {
            _landscape = LandscapeProvider.CreateLandscape();
        }

        public void AddTarget(int x, int y, System.Windows.Media.Color? color, string colorName)
        {
            if (color == null)
            {
                color = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
                colorName = "Red";
            }
            var newTarget = new TargetProvider(x, y) {TargetColor = color.Value, ColorName = colorName};
            Targets.Add(newTarget);
            

            _timer.Tick += newTarget.Move;
        }

        public void RemoveTarget(TargetProvider target)
        {
            _timer.Tick -= target.Move;

            Targets.Remove(target);
        }

        private void DrawLandscape(int currentDegree, Point refresherLinePoint)
        {
            var refresherX = refresherLinePoint.X;
            var refresherY = refresherLinePoint.Y;

            for (var j = 0; j < _landscape.GetLength(1); j++)
            {
                var distance = GetDistanse(new Point(_centerX, _centerY),
                    new Point(_centerX + j * (refresherX - _centerX) / _landscape.GetLength(1), _centerY + j * (refresherY - _centerY) / _landscape.GetLength(1)));
                var pen = new SolidBrush(LandscapeProvider.ChooseLandscapeColor(distance, _landscape[currentDegree, j]));

                G.FillRectangle(pen, _centerX + j * (refresherX - _centerX) / _landscape.GetLength(1),
               _centerY + j * (refresherY - _centerY) / _landscape.GetLength(1), 2, 2);

            }
        }

        //ToDo: We add as extension for Math class
        public double GetDistanse(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        //ToDo: We add as extension for Math class
        private double ToDegree(double radian)
        {
            return radian * 180 / Math.PI;
        }

        //ToDo: We add as extension for Math class
        private double GetAngle(Point a)
        {
            var y = GetDistanse(new Point(_centerX, _centerY), new Point(_centerX, a.Y));
            var x = GetDistanse(new Point(_centerX, _centerY), new Point(a.X, _centerY));

            if (a.X >= _bitmapWidth / 2 & a.Y < _bitmapHeight / 2)
            {
                //I Half
                return 90 - ToDegree(Math.Atan(y / x));
            }
            if (a.X >= _bitmapWidth / 2 & a.Y >= _bitmapHeight / 2)
            {
                //IV Half
                return 90 + ToDegree(Math.Atan(y / x));
            }
            if (a.X < _bitmapWidth / 2 & a.Y >= _bitmapHeight / 2)
            {
                //III Half   
                return 270 - ToDegree(Math.Atan(y / x));
            }
            if (a.X < _bitmapWidth / 2 & a.Y < _bitmapHeight / 2)
            {
                //II Half
                return 270 + ToDegree(Math.Atan(y / x));
            }

            return -1;
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
            G.DrawEllipse(Constans.CIRCLE_PEN, _offset, _offset, Constans.FIRST_CIRCLE_WIDTH,
                Constans.FIRST_CIRCLE_HEIGHT);
            //bigger circle
            G.DrawEllipse(Constans.CIRCLE_PEN, _offset + circleOffset, _offset + circleOffset,
                Constans.SECOND_CIRCLE_WIDTH,
                Constans.SECOND_CIRCLE_HEIGHT); //smaller circle
            G.DrawEllipse(Constans.CIRCLE_PEN, _offset + 2 * circleOffset, _offset + 2 * circleOffset,
                Constans.THIRD_CIRCLE_WIDTH,
                Constans.THIRD_CIRCLE_HEIGHT); //smaller circle
            G.DrawEllipse(Constans.CIRCLE_PEN, _offset + 3 * circleOffset, _offset + 3 * circleOffset,
                Constans.FOURTH_CIRCLE_WIDTH,
                Constans.FOURTH_CIRCLE_HEIGHT); //smaller circle

            //draw perpendicular line
            G.DrawLine(Constans.AXIS_PEN, new Point(_centerX, 0), new Point(_centerX, _bitmapHeight)); // UP-DOWN
            G.DrawLine(Constans.AXIS_PEN, new Point(0, _centerY), new Point(_bitmapWidth, _centerY)); //LEFT-RIGHT

            //draw diagonal lines
            G.DrawLine(Constans.AXIS_PEN, new Point(0, 0), new Point(_bitmapWidth, _bitmapHeight)); // UP-DOWN
            G.DrawLine(Constans.AXIS_PEN, new Point(0, _bitmapWidth), new Point(_bitmapHeight, 0));
            //LEFT-RIGHT

            //Axes labels
            G.DrawString("N", new Font("Arial", 16), new SolidBrush(Color.FloralWhite), _centerX - 20, 1);
            G.DrawString("W", new Font("Arial", 16), new SolidBrush(Color.FloralWhite), 1, _centerY - 20);
            G.DrawString("E", new Font("Arial", 16), new SolidBrush(Color.FloralWhite), 2 * _centerX - 20, _centerY - 20);
            G.DrawString("S", new Font("Arial", 16), new SolidBrush(Color.FloralWhite), _centerX, 2 * _centerY - 20);

            //Graduation
            G.DrawString("0", new Font("Arial", 8), new SolidBrush(Color.FloralWhite), _centerX + 2, 8);
            G.DrawString("45", new Font("Arial", 8), new SolidBrush(Color.FloralWhite), _bitmapWidth - _offset - 75 + 3,
                _offset + 75);
            G.DrawString("90", new Font("Arial", 8), new SolidBrush(Color.FloralWhite), 2 * _centerX - 20, _centerY + 2);
            G.DrawString("135", new Font("Arial", 8), new SolidBrush(Color.FloralWhite),
                _bitmapWidth - _offset - 75 + 4, _bitmapHeight - _offset - 75 - 14);
            G.DrawString("180", new Font("Arial", 8), new SolidBrush(Color.FloralWhite), _centerX - 25, 2 * _centerY - 20);
            G.DrawString("225", new Font("Arial", 8), new SolidBrush(Color.FloralWhite), _offset + 75 - 5,
                _bitmapHeight - _offset - 75 + 5);
            G.DrawString("270", new Font("Arial", 8), new SolidBrush(Color.FloralWhite), 1, _centerY + 2);
            G.DrawString("315", new Font("Arial", 8), new SolidBrush(Color.FloralWhite), _offset + 75 - 8,
                _offset + 75 - 19);
        }

        /// <summary>
        /// Transform Bitmap to ImageSource
        /// </summary>
        /// <param name="bitmap">Bitmap which need to transform</param>
        /// <returns>returns BitmapImage (Image Source)</returns>
        private BitmapImage BitmapToImageSource(Bitmap bitmap)
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

        //ToDo: We add as extension for Math class
        public static bool LineIntersectsTarget(Point p1, Point p2, Rectangle r)
        {
            return LineIntersectsLine(p1, p2, new Point(r.X, r.Y), new Point(r.X + r.Width, r.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y), new Point(r.X + r.Width, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y + r.Height), new Point(r.X, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X, r.Y + r.Height), new Point(r.X, r.Y)) ||
                   (r.Contains(p1) && r.Contains(p2));
        }

        //ToDo: We add as extension for Math class
        private static bool LineIntersectsLine(Point l1p1, Point l1p2, Point l2p1, Point l2p2)
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }

    }
}
