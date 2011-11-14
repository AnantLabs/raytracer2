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

            Point3D prod = p1 * p2;
            double dotProd = prod.X + prod.Y + prod.Z;
            double p1Len = p1.Length();
            double p2Len = p2.Length();
            double rads = dotProd / (p1Len * p2Len);
            rads = Math.Acos(rads);
            return rads;
        }

        public static double GetDegreesX(Point3D p1, Point3D p2)
        {
            Point3D prod = p1 * p2;
            double dotProd = prod.X + prod.Y + prod.Z;
            double p1Len = p1.Length();
            double p2Len = p2.Length();
            double rads = dotProd / (p1Len * p2Len);
            rads = Math.Acos(rads);
            double degs = EditorMath.Radians2Deg(rads);
            return degs;
        }
    }
}
