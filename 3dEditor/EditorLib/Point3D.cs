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
        public double ZZ { get; set; }

        public Point3D() : this(0, 0, 0) { }

        public Point3D(double x, double y, double z): this(x, y, z, 1) { }

        public Point3D(double x, double y, double z, double zz)
        {
            X = x; Y = y; Z = z; ZZ = zz;
        }

        public Point3D(Point3D old)
        {
            this.X = old.X;
            this.Y = old.Y;
            this.Z = old.Z;
            this.ZZ = old.ZZ;
        }

        public double Size()
        {
            double len = X * X + Y * Y + Z * Z;
            len = Math.Sqrt(len);
            return len;
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

        public static Point3D To3D_From2D(PointF p2d, double z, int scale, int zoom, Point centerPoint)
        {
            Point3D point = new Point3D(p2d.X, p2d.Y, 0);
            float divide = ((float)z + scale) / zoom;
            point.X = (point.X - centerPoint.X) * divide / scale;
            point.Y = (point.Y - centerPoint.Y) * divide / scale;
            point.Z = z;
            return point;
        }
        public void MultiplyByMatrix(Matrix3D matrix)
        {
            Point3D newPoint = matrix * this;
            this.X = newPoint.X;
            this.Y = newPoint.Y;
            this.Z = newPoint.Z;
            this.ZZ = newPoint.ZZ;
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
            return "[ " + Math.Round(X, 1) + "; " + Math.Round(Y, 1) + "; " + Math.Round(Z, 1) + " ]";
        }


        public void Otoceni(double xRad, double yRad, double zRad)
        {
            Matrix3D matrix = Matrix3D.NewRotateByRads(xRad, yRad, zRad);
            Point3D p = matrix * this;
            X = p.X;
            Y = p.Y;
            Z = p.Z;
        }
        public void Posunuti(double Px, double Py, double Pz)
        {
            Point3D p = Matrix3D.Posunuti(this, Px, Py, Pz);
            X = p.X;
            Y = p.Y;
            Z = p.Z;
        }


        public void Scale(double Sx, double Sy, double Sz)
        {
            Matrix3D m = Matrix3D.ScalingNewMatrix(Sx, Sy, Sz);
            this.MultiplyByMatrix(m);
        }
       
    }
}
