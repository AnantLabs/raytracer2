using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{
    public class Plane2 : DefaultShape
    {

        Vektor P1, P2, P3, P4;
        Vektor[] Points;
        Vektor Normal;
        public double D { get; set; }

        Matrix3D Matrix;
        Matrix3D TranspMat;

        public double MinX, MinY, MinZ, MaxX, MaxY, MaxZ;

        public Plane2(Material material, double degX, double degY, double degZ, double d, 
            double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
        {
            this.Set(material, degX, degY, degZ, d, minX, minY, minZ, maxX, maxY, maxZ);
        }

        public Plane2()
        {
            IsActive = true;
            Matrix = new Matrix3D();

            Material = new Material();
            Material.Color = new Colour(1, 0.2, 0.4, 0);

            D = 0.0;
            P1 = new Vektor(Double.MinValue, -1, -1);
            P2 = new Vektor(8, -1, -1);
            P3 = new Vektor(8, -1, -9);
            P4 = new Vektor(-8, 1, Double.MinValue);

            List<Vektor> points = new List<Vektor>();
            points.Add(P1);
            points.Add(P2);
            points.Add(P3);
            points.Add(P4);
            Points = points.ToArray();

            Normal = new Vektor(0, 1, 0);
            Matrix.SetOnDegrees(0, 0, 40);
            TranspMat = Matrix.Transpose();

            //Matrix.TransformPoints(Points);
            //Matrix.TransformPoint(Normal);
            Normal.Normalize();

            MinX = Math.Min(P1.X, Math.Min(P2.X, Math.Min(P3.X, P4.X)));
            MinY = Math.Min(P1.Y, Math.Min(P2.Y, Math.Min(P3.Y, P4.Y)));
            MinZ = Math.Min(P1.Z, Math.Min(P2.Z, Math.Min(P3.Z, P4.Z)));

            MaxX = Math.Max(P1.X, Math.Max(P2.X, Math.Max(P3.X, P4.X)));
            MaxY = Math.Max(P1.Y, Math.Max(P2.Y, Math.Max(P3.Y, P4.Y)));
            MaxZ = Math.Max(P1.Z, Math.Max(P2.Z, Math.Max(P3.Z, P4.Z)));
        }

        public Plane2(Plane2 oldPlane)
        {
            IsActive = oldPlane.IsActive;
            this.Normal = oldPlane.Normal;
            this.D = oldPlane.D;
            this.Material = new Material(oldPlane.Material);
            this.Matrix = oldPlane.Matrix;
            this.TranspMat = oldPlane.TranspMat;
            MinX = oldPlane.MinX;
            MaxX = oldPlane.MaxX;
            MinY = oldPlane.MinY;
            MaxY = oldPlane.MaxY;
            MinZ = oldPlane.MinZ;
            MaxZ = oldPlane.MaxZ;
        }



        private void Set(Material material, double degX, double degY, double degZ, double d,
            double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
        {
            IsActive = true;
            Material = material;
            Material.Color = new Colour(1, 0.2, 0.4, 0);
            Normal = new Vektor(0, 1, 0);
            D = d;

            Matrix = new Matrix3D();
            Matrix.SetOnDegrees(degX, degY, degZ);
            TranspMat = Matrix.Transpose();

            MinX = Math.Min(minX, maxX);
            MinY = Math.Min(minY, maxY);
            MinZ = Math.Min(minZ, maxZ);

            MaxX = Math.Max(maxX, minX);
            MaxY = Math.Max(maxY, minY);
            MaxZ = Math.Max(maxZ, minZ);
        }

        public override bool Intersects(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint)
        {
            if (!IsActive)
                return false;

            Vektor normal = new Vektor(Normal);
            normal.Normalize();

            Vektor p0 = TranspMat.Transform2NewPoint(P0);       // prevedeni do kanonickych souradnic
            Vektor pd = TranspMat.Transform2NewPoint(Pd);       // prevedeni do kanonickych souradnic
            //Vektor p0 = P0;
            //Vektor pd = Pd;
            pd.Normalize();

            double Vd = normal * pd;

            // paprsek rovinu neprotina: - kdyz Vd==0, pak paprsek neprotina
            if (Math.Abs(Vd) < MyMath.EPSILON)
                return false;

            // spravna verze (spocitano):
            double V0 = -(normal * p0 + D);  // asi je tahle verze spise vic dobre (funguje pro krychli)

            //double V0 = D - normal * P0;        // asi je tahle verzi opacne (funguje pro valec)
            double t = V0 / Vd;

            // paprsek protina rovinu zezadu
            if (t < 0.0)
                return false;


            Vektor interPoint = p0 + pd * t;
            if (interPoint.X < MinX || interPoint.X > MaxX ||
                interPoint.Y < MinY || interPoint.Y > MaxY ||
                interPoint.Z < MinZ || interPoint.Z > MaxZ)
                return false;

            SolidPoint sp = new SolidPoint();
            sp.T = t;
            sp.Coord = interPoint;              // souradnice bodu pruniku
            Matrix.TransformPoint(sp.Coord);    // prevedeme do objektovych souradnic roviny
            if (Vd < 0)
                sp.Normal = new Vektor(normal);
            else
                sp.Normal = Vektor.ZeroVektor - normal;

            Matrix.TransformPoint(sp.Normal);   // prevedeme do objektovych souradnic roviny
            sp.Normal.Normalize();
            sp.Color = new Colour(this.Material.Color);
            sp.Material = this.Material;
            sp.Shape = this;

            InterPoint.Add(sp);

            return true;
        }

        public override void Move(double dx, double dy, double dz)
        {
            throw new NotImplementedException();
        }

        public override void MoveToPoint(double dx, double dy, double dz)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return "Plane: [" + Normal.X + "; " + Normal.Y + "; " + Normal.Z + "; " + this.D + "]";
        }

        public override void Rotate(double degAroundX, double degAroundY, double degAroundZ)
        {
            throw new NotImplementedException();
        }
    }
}
