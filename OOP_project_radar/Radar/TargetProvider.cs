using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Windows.Media.Color;

namespace OOP_project_radar.Radar
{
    //ToDo: Implement Factory pattern
    class TargetProvider : INotifyPropertyChanged
    {


        public int X { get; set; }

        public int Y { get; set; }

        public bool IsDetected
        {
            get { return _isDetected; }
            set
            {
                _isDetected = value;
                NotifyPropertyChanged("IsDetected");
            }
        }

        public int Angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                NotifyPropertyChanged("Angle");
            }
        }

        public double Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                NotifyPropertyChanged("Speed");
            }
        }

        public double Distance
        {
            get { return _distance; }
            set
            {
                _distance = value;
                NotifyPropertyChanged("Distance");
            }
        }

        public Color TargetColor
        {
            get { return _targetColor; }
            set
            {
                _targetColor = value;
                NotifyPropertyChanged("TargetColor");
            }
        }

        public string ColorName
        {
            get { return _colorName; }
            set
            {
                _colorName = value;
                NotifyPropertyChanged("ColorName");
            }
        }

        public Rectangle TargetZone;


        private bool _isDetected;
        private Color _targetColor ;
        private double _speed;
        private int _angle;
        private double _distance;
        private string _colorName;

        public TargetProvider(int x, int y)
        {
            X = x;
            Y = y;
            TargetZone = new Rectangle(X, Y, 6, 6);
        }

        public void Move(object sender, EventArgs e)
        {
            var rand = new Random();
            X += rand.Next(0, 2) * rand.Next(0, 2) - rand.Next(0, 2) * rand.Next(0, 2);
            Y += rand.Next(0, 2) * rand.Next(0, 2) - rand.Next(0, 2) * rand.Next(0, 2);

            TargetZone.X = X;
            TargetZone.Y = Y;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
