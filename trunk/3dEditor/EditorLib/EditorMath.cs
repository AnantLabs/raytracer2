using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorLib
{
    public class EditorMath
    {
        public static double Radians2Deg(double radians)
        {
            return radians / Math.PI * 180.0;
        }

        public static double Degrees2Rad(double degrees)
        {
            return degrees / 180.0 * Math.PI;
        }

        public static int As180(int angle)
        {
            angle = angle % 360;

            angle = (angle < -180) ?
                        180 - Math.Abs(angle % 180)
                        : (angle > 180) ?
                            Math.Abs(angle % 180) - 180
                            : angle;
            return angle;
        }
    }
}
