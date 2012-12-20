using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorLib
{
    public class Line3D
    {
        public Point3D A { get; set; }
        public Point3D B { get; set; }

        public Line3D() : this(new Point3D(), new Point3D()) { }
        public Line3D(Point3D begin, Point3D end)
        {
            A = begin;
            B = end;
        }

        public static double GetDegreesX(Line3D l1, Line3D l2)
        {
            Point3D p1 = l1.B - l1.A;
            Point3D p2 = l2.B - l2.A;

            double dotProd = p1 * p2;
            double p1Len = p1.Length();
            double p2Len = p2.Length();
            double rads = dotProd / (p1Len * p2Len);
            rads = Math.Acos(rads);
            return rads;



        }

        public static double GetDegreesX(Point3D p1, Point3D p2)
        {
            double dotProd = p1 * p2;
            double p1Len = p1.Length();
            double p2Len = p2.Length();
            double rads = dotProd / (p1Len * p2Len);
            rads = Math.Acos(rads);
            double degs = EditorMath.Radians2Deg(rads);
            return degs;
        }

        public static double GetDegrees2D(double x1, double y1, double x2, double y2)
        {
            double dotprod = x1 * x2 + y1 * y2;
            double len1 = Math.Sqrt(x1 * x1 + y1 * y1);
            double len2 = Math.Sqrt(x2 * x2 + y2 * y2);
            double rads = dotprod / (len1 * len2);
            rads = Math.Acos(rads);
            return EditorMath.Radians2Deg(rads);
        }
    }
}
