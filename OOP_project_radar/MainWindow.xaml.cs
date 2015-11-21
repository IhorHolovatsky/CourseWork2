using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;


namespace OOP_project_radar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();

        int WIDTH = 300, HEIGHT = 300, HAND = 150;

        int u;  //in degree
        int cx, cy;     //center of the circle
        int x, y;       //HAND coordinate

        int tx, ty, lim = 20;

        Bitmap bmp;
        Pen p;
        Graphics g;
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            //create Bitmap
            bmp = new Bitmap(WIDTH + 1, HEIGHT + 1);

            //background color

            
            //center
            cx = WIDTH / 2;
            cy = HEIGHT / 2;

            //initial degree of HAND
            u = 0;

            //timer
            t.Interval = 5; //in millisecond
            t.Tick += new EventHandler(this.t_Tick);
            t.Start();
        }

        private void t_Tick(object sender, EventArgs e)
        {
            //pen
            p = new Pen(Color.Green, 1f);

            //graphics
            g = Graphics.FromImage(bmp);

            //calculate x, y coordinate of HAND
            int tu = (u - lim) % 360;

            if (u >= 0 && u <= 180)
            {
                //right half
                //u in degree is converted into radian.

                x = cx + (int)(HAND * Math.Sin(Math.PI * u / 180));
                y = cy - (int)(HAND * Math.Cos(Math.PI * u / 180));
            }
            else
            {
                x = cx - (int)(HAND * -Math.Sin(Math.PI * u / 180));
                y = cy - (int)(HAND * Math.Cos(Math.PI * u / 180));
            }

            if (tu >= 0 && tu <= 180)
            {
                //right half
                //tu in degree is converted into radian.

                tx = cx + (int)(HAND * Math.Sin(Math.PI * tu / 180));
                ty = cy - (int)(HAND * Math.Cos(Math.PI * tu / 180));
            }
            else
            {
                tx = cx - (int)(HAND * -Math.Sin(Math.PI * tu / 180));
                ty = cy - (int)(HAND * Math.Cos(Math.PI * tu / 180));
            }

            //draw circle
            g.DrawEllipse(p, 0, 0, WIDTH, HEIGHT);  //bigger circle
            g.DrawEllipse(p, 80, 80, WIDTH - 160, HEIGHT - 160);    //smaller circle

            //draw perpendicular line
            g.DrawLine(p, new System.Drawing.Point(cx, 0), new System.Drawing.Point(cx, HEIGHT)); // UP-DOWN
            g.DrawLine(p, new System.Drawing.Point(0, cy), new System.Drawing.Point(WIDTH, cy)); //LEFT-RIGHT

            //draw HAND
            g.DrawLine(new Pen(Color.Black, 1f), new System.Drawing.Point(cx, cy), new System.Drawing.Point(tx, ty));
            g.DrawLine(p, new System.Drawing.Point(cx, cy), new System.Drawing.Point(x, y));

            //load bitmap in picturebox1
            ImageBox.Source = BitmapToImageSource(bmp);

            //dispose
            p.Dispose();
            g.Dispose();

            //update
            u++;
            if (u == 360)
            {
                u = 0;
            }
        }
        public BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
