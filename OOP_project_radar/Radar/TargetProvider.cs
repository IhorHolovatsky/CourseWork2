using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_project_radar.Radar
{
    //ToDo: Implement Factory pattern
    class TargetProvider : INotifyPropertyChanged
    {


        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
                NotifyPropertyChanged("X");
            }
        }
        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                NotifyPropertyChanged("Y");
            }
        }
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

        public Color TargetColor
        {
            get { return _targetColor; }
            set
            {
                _targetColor = value;
                NotifyPropertyChanged("TargetColor");
            }
        }


        public Rectangle TargetZone;


        private int _x;
        private int _y;
        private bool _isDetected;
        private Color _targetColor = Color.Red;
        private double _speed;
        private int _angle;

        public TargetProvider(int x, int y)
        {
            _x = x;
            _y = y;
            TargetZone = new Rectangle(X, Y, 5, 5);
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
