using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace EditorLib
{
    public class Point3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point3D() : this(0, 0, 0) { }

        public Point3D(double x, double y, double z)
        {
            X = x; Y = y; Z = z;
        }

        public Point3D(Point3D old)
        {
            this.X = old.X;
            this.Y = old.Y;
            this.Z = old.Z;
        }
        public Point3D Normalize()
        {
            double len = X * X + Y * Y + Z * Z;
            len = Math.Sqrt(len);
            Point3D p = new Point3D(X / len, Y / len, Z / len);
            return p;
        }

        public PointF To2D()
        {
            return new PointF((float)X, (float)Y);
        }

        public PointF To2D(int scale, int zoom, Point centerPoint)
        {
            PointF point = new PointF((float)X, (float)Y);
            float zFloat = (float)Z;
            float divide = (zFloat + scale) / zoom;
            point.X = point.X * scale / divide + centerPoint.X;
            point.Y = point.Y * scale / divide + centerPoint.Y;
            return point;
        }

        /// <summary>
        /// vzdalenost od pocatku
        /// </summary>
        /// <returns>sqrt(x^2 + y^2 + z^2)</returns>
        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        public static Point3D operator +(Point3D a, Point3D b)
        {
            return new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Point3D operator -(Point3D a, Point3D b)
        {
            return new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Point3D operator *(Point3D a, Point3D b)
        {
            return new Point3D(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Point3D operator *(Point3D point3d, double coefficient)
        {
            return new Point3D(point3d.X * coefficient, point3d.Y * coefficient, point3d.Z * coefficient);
        }

        public override string ToString()
        {
            return "[" + X + ";" + Y + ";" + Z + "]";
        }
        
    }
}
