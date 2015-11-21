using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_project_radar
{
    static class Constans
    {
        public static int BITMAP_WIDTH = 600;
        public static int BITMAP_HEIGHT = 600;


        public static int RADAR_WIDTH = 550;
        public static int RADAR_HEIGHT = 550;

        public static int HAND = RADAR_WIDTH /2;

        public static int FIRST_CIRCLE_WIDTH = RADAR_WIDTH;
        public static int FIRST_CIRCLE_HEIGHT = RADAR_HEIGHT;

        public static int SECOND_CIRCLE_WIDTH = 3*RADAR_WIDTH/4;
        public static int SECOND_CIRCLE_HEIGHT = 3*RADAR_HEIGHT/4;
        
        public static int THIRD_CIRCLE_WIDTH = RADAR_WIDTH/2;
        public static int THIRD_CIRCLE_HEIGHT = RADAR_HEIGHT/2;  
        
        public static int FOURTH_CIRCLE_WIDTH = RADAR_WIDTH/4;
        public static int FOURTH_CIRCLE_HEIGHT = RADAR_HEIGHT/4;

        public static Pen AXIS_PEN = new Pen(Color.Aqua, 0.01f);
        public static Pen CIRCLE_PEN = new Pen(Color.LightGreen, 0.03f);
        public static Pen REFRESHER_PEN = new Pen(Color.LimeGreen, 3f);

    }
}
