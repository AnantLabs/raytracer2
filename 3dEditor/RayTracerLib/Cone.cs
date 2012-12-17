using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracerLib
{
    public class Cone : DefaultShape
    {
        /// <summary>
        /// Vrchol kuzele
        /// </summary>
        public Vektor C { get; private set; }

        /// <summary>
        /// Osa kuzele - smerucijici z vrcholu C
        /// </summary>
        public Vektor Dir { get; private set; }

        /// <summary>
        /// polomer podstravy
        /// </summary>
        public double Rad { get; private set; }

        /// <summary>
        /// vyska kuzele
        /// </summary>
        public double Height { get; private set; }

        private Vektor DirNom;
        private double S;
        Plane Bottom;

        public Cone() : this(new Vektor(1, 2, 3), new Vektor(1, 1, 1), 2, 6) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c">vrchol kuzele</param>
        /// <param name="dir">osa kuzele (nemusi byt normalizovana)</param>
        /// <param name="rad">polomer podstavy</param>
        /// <param name="height">vyska kuzele</param>
        public Cone(Vektor c, Vektor dir, double rad, double height)
        {
            IsActive = true;
            Material = new Material();
            this.Set(c, dir, rad, height);
        }

        public Cone(Cone old)
        {
            this.IsActive = old.IsActive;
            this.C = new Vektor(old.C);
            this.Dir = new Vektor(old.Dir);
            this.DirNom = new Vektor(old.DirNom);
            this.Height = old.Height;
            this.Material = new Material(old.Material);
            this.Rad = old.Rad;
            this.S = old.S;
        }

        public void Set(Vektor c, Vektor dir, double rad, double height)
        {
            C = c;
            
            Rad = rad;
            Height = height;

            this.DirNom = new Vektor(dir);
            DirNom.Normalize();

            Dir = C + DirNom * height;
            double T = Rad / Dir.Size();
            S = 1 + T * T;

            // rovina podstavy: C*Norm = D
            // musi pak platit: C * Norm + D = 0
            // vime: C .. stred roviny
            //       Norm .. normala roviny = Dir nebo -Dir
            // chceme: D

            Bottom = new Plane(DirNom, -(Dir * DirNom), this.Material);
        }

        public override bool Intersects(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint)
        {
            if (!IsActive) return false;

            bool toReturn = false;
            /////////////////////////////////////////
            // 1) prunik paprsku s podstavou
            
            List<SolidPoint> BasePoints = new List<SolidPoint>();
            Bottom.Intersects(P0, Pd, ref BasePoints);
            foreach (SolidPoint sps in BasePoints)
            {
                if (MyMath.Distance2Points(sps.Coord, Dir) <= this.Rad)
                {
                    sps.Shape = this;
                    InterPoint.Add(sps);
                    toReturn = true;
                }
            }


            /////////////////////////////////////////
            // 2) prunik paprsku s plastem
            
            Vektor p = P0 - C;

            double pp = p * p;
            double pu = p * Pd;
            double uu = Pd * Pd;
            double uw = Pd * DirNom;
            double pw = p * DirNom;

            double a = uu - S * uw * uw;
            double b = 2 * (pu - S * pw * uw);
            double c = pp - S * pw * pw;

            double discr = b * b - 4 * a * c;

            if (discr < MyMath.EPSILON) return toReturn;

            double discrSqrt = Math.Sqrt(discr);
            double t1 = (-b - discrSqrt) / (2 * a);
            double t2 = (-b + discrSqrt) / (2 * a);

            // vybereme mensi t, ktere je kladne
            double tMin = Math.Min(t1, t2);
            double tMax = Math.Max(t1, t2);

            double t = tMin;
            if (tMin < MyMath.EPSILON)
                t = tMax;
            if (t < MyMath.EPSILON)
                return toReturn;

            Vektor Point = P0 + Pd * t;
            Vektor q = Point - C;
            Vektor q0 = q - DirNom * (q * DirNom);
            //q0.Normalize();
            Vektor qt = Vektor.CrossProduct(DirNom, q0);
            Vektor len = DirNom * (q * DirNom);
            double leng = len.Size();
            if (leng > Height) return toReturn;
            //qt.Normalize();
            Vektor n = Vektor.CrossProduct(qt, q);
            n.Normalize();

            //// TED MAME DVA KUZELE, Z NICHZ MUSIME VYBRAT JEN TEN SPRAVNEJ
            // pro nej plati, ze bod pruniku lezi v kladne polorovine:
            double qw = q * DirNom;
            if (qw <= 0) return toReturn;

            SolidPoint sp = new SolidPoint();
            sp.Color = this.Material.Color;
            sp.Coord = Point;
            sp.T = t;
            sp.Normal = n;
            sp.Shape = this;
            sp.Material = this.Material;

            InterPoint.Add(sp);
            return true;
        }

        public override void Move(double dx, double dy, double dz)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Center: " + this.C + "; Dir: " + this.Dir;
        }
    }
}
