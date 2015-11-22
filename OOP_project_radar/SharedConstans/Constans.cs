using System.Drawing;

namespace OOP_project_radar.SharedConstans
{
    static class Constans
    {
        public const int RADAR_WIDTH = 550;
        public const int RADAR_HEIGHT = 550;

        public const int HAND = RADAR_WIDTH / 2;

        public const int FIRST_CIRCLE_WIDTH = RADAR_WIDTH;
        public const int FIRST_CIRCLE_HEIGHT = RADAR_HEIGHT;

        public const int SECOND_CIRCLE_WIDTH = 3 * RADAR_WIDTH / 4;
        public const int SECOND_CIRCLE_HEIGHT = 3 * RADAR_HEIGHT / 4;

        public const int THIRD_CIRCLE_WIDTH = RADAR_WIDTH / 2;
        public const int THIRD_CIRCLE_HEIGHT = RADAR_HEIGHT / 2;

        public const int FOURTH_CIRCLE_WIDTH = RADAR_WIDTH / 4;
        public const int FOURTH_CIRCLE_HEIGHT = RADAR_HEIGHT / 4;

        public static readonly Pen AXIS_PEN = new Pen(Color.Aqua, 0.01f);
        public static readonly Pen CIRCLE_PEN = new Pen(Color.LightGreen, 0.03f);
        public static readonly Pen REFRESHER_PEN = new Pen(Color.LimeGreen, 1f);

        public static readonly string[] SHADOW_COLORS = 
        {
            "#32cd32", "#2db82d", "#28a428", "#238f23", "#1e7b1e", "#196619",
            "#145214", "#0f3d0f"
        };
    }
}
