using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mathematics
{
    public class Line3D
    {
        public Vektor A { get; set; }
        public Vektor B { get; set; }

        public Line3D() : this(new Vektor(), new Vektor()) { }
        public Line3D(Vektor begin, Vektor end)
        {
            A = begin;
            B = end;
        }

        public static double GetDegreesX(Line3D l1, Line3D l2)
        {
            Vektor p1 = l1.B - l1.A;
            Vektor p2 = l2.B - l2.A;

            double dotProd = p1 * p2;
            double p1Len = p1.Size();
            double p2Len = p2.Size();
            double rads = dotProd / (p1Len * p2Len);
            rads = Math.Acos(rads);
            return rads;



        }

        public static double GetDegreesX(Vektor p1, Vektor p2)
        {
            double dotProd = p1 * p2;
            double p1Len = p1.Size();
            double p2Len = p2.Size();
            double rads = dotProd / (p1Len * p2Len);
            rads = Math.Acos(rads);
            double degs = MyMath.Radians2Deg(rads);
            return degs;
        }

        public static double GetDegrees2D(double x1, double y1, double x2, double y2)
        {
            double dotprod = x1 * x2 + y1 * y2;
            double len1 = Math.Sqrt(x1 * x1 + y1 * y1);
            double len2 = Math.Sqrt(x2 * x2 + y2 * y2);
            double rads = dotprod / (len1 * len2);
            rads = Math.Acos(rads);
            return MyMath.Radians2Deg(rads);
        }
    }
}
