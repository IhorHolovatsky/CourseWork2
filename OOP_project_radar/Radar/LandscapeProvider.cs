using System;
using System.Drawing;
using OOP_project_radar.SharedConstans;

namespace OOP_project_radar.Radar
{
    static class LandscapeProvider
    {
        public static int[,] CreateLandscape()
        {
            var landArray = new int[361, Constans.RADAR_WIDTH / 2];
            var rand = new Random();

            for (var i = 0; i < landArray.GetLength(0); i++)
            {
                for (var j = 0; j < landArray.GetLength(1); j++)
                {
                    landArray[i, j] = rand.Next(0, 120);
                }
            }

            return landArray;
        }

        public static Color ChooseLandscapeColor(double distance, int colorValue)
        {
            var rand = new Random();

            var color = Color.Empty;

            if (distance > Constans.RADAR_WIDTH / 4 + rand.Next(0, 50))
            {
                if (colorValue >= 0 & colorValue < 115)
                {
                    color = Color.Black;
                }
                if (colorValue >= 115 & colorValue < 119)
                {
                    color = ColorTranslator.FromHtml(Constans.SHADOW_COLORS[7]);
                }
                if (colorValue >= 119 & colorValue < 120)
                {
                    color = ColorTranslator.FromHtml(Constans.SHADOW_COLORS[2]);
                }
            }
            else if (distance > Constans.RADAR_WIDTH / 4)
            {
                if (colorValue >= 0 & colorValue < 90)
                {
                    color = Color.Black;
                }
                if (colorValue >= 90 & colorValue < 120)
                {
                    color = ColorTranslator.FromHtml(Constans.SHADOW_COLORS[6]);
                }

            }
            else
            {
                if (colorValue >= 0 & colorValue < 80)
                {
                    color = Color.Black;
                }
                if (colorValue >= 80 & colorValue < 90)
                {
                    color = ColorTranslator.FromHtml(Constans.SHADOW_COLORS[7]);
                }
                if (colorValue >= 90 & colorValue < 115)
                {
                    color = ColorTranslator.FromHtml(Constans.SHADOW_COLORS[6]);
                }
                if (colorValue >= 115 & colorValue < 120)
                {
                    color = ColorTranslator.FromHtml(Constans.SHADOW_COLORS[0]);
                }
            }


            return color;
        }


    }
}
