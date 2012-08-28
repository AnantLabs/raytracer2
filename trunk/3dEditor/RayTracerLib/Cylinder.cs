﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracerLib
{
    public class Cylinder : DefaultShape
    {
        /// <summary>
        /// Stred
        /// </summary>
        public Vektor Center { get; private set; }

        /// <summary>
        /// spodni stred valce
        /// </summary>
        private Vektor CenterLow { get; set; }

        /// <summary>
        /// smer osy valce - puvodni
        /// </summary>
        public Vektor Dir { get; private set; }

        /// <summary>
        /// smer osy - normalizovany
        /// </summary>
        private Vektor DirNorm { get; set; }


        /// <summary>
        /// polomer
        /// </summary>
        public double R { get; private set; }
        /// <summary>
        /// vyska
        /// </summary>
        public double H { get; private set; }

        #region PrivateTempAtributes
        
        /// <summary>
        /// osova primka: L= C + D*h
        /// </summary>
        Vektor L;
        /// <summary>
        /// rovina prvni podstavy
        /// </summary>
        Plane _plane1;
        /// <summary>
        /// stred prvni podstavy
        /// </summary>
        Vektor _c1;
        /// <summary>
        /// rovina druhe podstavy
        /// </summary>
        Plane _plane2;
        /// <summary>
        /// stred druhe podstavy
        /// </summary>
        Vektor _c2;

        #endregion


        /// <summary>
        /// vytvori novy valec
        /// </summary>
        /// <param name="center">uplny stred valce</param>
        /// <param name="direction">smer osy valce</param>
        /// <param name="radius">polomer</param>
        /// <param name="heigh">vyska</param>
        public Cylinder(Vektor center, Vektor direction, double radius, double heigh)
        {
            IsActive = true;
            this.Material = new Material();
            SetValues(center, direction, radius, heigh);
        }

        public Cylinder(Cylinder old)
        {
            IsActive = old.IsActive;
            Center = new Vektor(old.Center);
            CenterLow = new Vektor(old.CenterLow);
            DirNorm = new Vektor(old.DirNorm);
            Dir = new Vektor(old.Dir);
            R = old.R;
            H = old.H;
            L = new Vektor(old.L);
            Material = new Material(old.Material);
            _plane1 = new Plane(old._plane1);
            _c1 = new Vektor(old._c1);
            _plane2 = new Plane(old._plane2);
            _c2 = new Vektor(old._c2);
        }

        /// <summary>
        /// hromadne prenastavi hodnoty
        /// </summary>
        /// <param name="center">uplny stred valce</param>
        /// <param name="direction">smer osy valce</param>
        /// <param name="radius">polomer</param>
        /// <param name="heigh">vyska</param>
        public void SetValues(Vektor center, Vektor direction, double radius, double heigh)
        {
            Center = new Vektor(center);
            Dir = new Vektor(direction);
            DirNorm = new Vektor(direction);
            DirNorm.Normalize();
            R = radius;
            H = heigh;
            CenterLow = new Vektor(center - DirNorm * (H / 2));
            //CenterLow = new Vektor(center);
            L = new Vektor(CenterLow);
            L = L + DirNorm * H;

            Vektor C1 = this.Center + DirNorm * (H / 2);
            Vektor Norm1 = new Vektor(DirNorm);
            double D1 = C1 * Norm1;
            _plane1 = new Plane(Norm1, -D1, this.Material);
            _c1 = C1;

            Vektor C2 = this.Center - DirNorm * (H / 2);
            Vektor Norm2 = new Vektor(Vektor.ZeroVektor - DirNorm);
            double D2 = C2 * Norm2;
            _plane2 = new Plane(Norm2, -D2, this.Material);
            _c2 = C2;

        }

        public override bool Intersects(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint)
        {
            if (!IsActive)
                return false;

            // 1) spocitani pruniku s podstavami
            // rovina jedne podstavy: C*Norm = D
            // musi pak platit: C * Norm + D = 0
            // vime: C .. stred roviny
            //       Norm .. normala roviny = Dir nebo -Dir
            // chceme: D

            if (_plane1 == null)
            {
                _c1 = this.Center + DirNorm * (H / 2);
                Vektor Norm1 = new Vektor(DirNorm);
                double D1 = _c1 * Norm1;

                _plane1 = new Plane(Norm1, -D1, this.Material);
            }
            List<SolidPoint> BasePoints = new List<SolidPoint>();
            _plane1.Intersects(P0, Pd, ref BasePoints);
            foreach (SolidPoint sps in BasePoints)
            {
                if (MyMath.Distance2Points(sps.Coord, _c1) <= this.R)
                {
                    sps.Shape = this;
                    InterPoint.Add(sps);
                }
            }

            if (_plane2 == null)
            {
                _c2 = this.Center - DirNorm * (H / 2);
                Vektor Norm2 = new Vektor(Vektor.ZeroVektor - DirNorm);
                double D2 = _c2 * Norm2;

                _plane2 = new Plane(Norm2, -D2, this.Material);
            }
            BasePoints.Clear();
            _plane2.Intersects(P0, Pd, ref BasePoints);

            foreach (SolidPoint sps in BasePoints)
            {
                if (MyMath.Distance2Points(sps.Coord, _c2) <= this.R)
                {
                    sps.Shape = this;
                    InterPoint.Add(sps);
                }
            }


            ////////////////////////////////////////
            // 2) spocitani pruniku s plastem
            Vektor P1 = new Vektor(Pd);
            P1.Normalize();
            
            // pocatek noveho paprsku
            Vektor RayP0 = new Vektor(P0);
            double skalar = DirNorm * P0;
            RayP0 = P0 - DirNorm * skalar;

            // smer noveho paprsku
            Vektor RayP1 = new Vektor(DirNorm);
            skalar = DirNorm * P1;
            RayP1 = P1 - DirNorm * skalar;

            // stred nove kruznice
            Vektor Center2 = new Vektor(CenterLow);
            skalar = DirNorm * CenterLow;
            Center2 = Center2 - DirNorm * skalar;

            double A = RayP1 * RayP1;
            double B = 2 * (RayP0 * RayP1 - RayP1 * Center2);
            double C = (RayP0 - Center2) * (RayP0 - Center2) - R * R;

            double discr = B * B - 4 * A * C;
            if (discr < MyMath.EPSILON)
                return false;

            discr = Math.Sqrt(discr);
            double jmenovatel = 2 * A;
            double t0 = (-B - discr) / jmenovatel;
            double t1 = (-B + discr) / jmenovatel;

            double tMin = Math.Min(t0, t1);
            double tMax = Math.Max(t0, t1);

            double t = tMin;
            if (tMin < MyMath.EPSILON)
                t = tMax;
            if (t < MyMath.EPSILON)
                return false;

            SolidPoint sp = new SolidPoint();
            sp.T = t;
            sp.Coord = P0 + Pd * t;

            Vektor PC = sp.Coord - CenterLow;
            Vektor PC2 = Vektor.Projection(DirNorm, PC);

            if (PC2 * DirNorm < MyMath.EPSILON || PC2.Size() > H)
                return false;

            //sp.Shape = new Sphere(PC2, this.R);
            Vektor v1 = sp.Coord - CenterLow;
            Vektor v2 = DirNorm * (v1 * DirNorm);
            sp.Normal = v1 - v2;
            sp.Normal.Normalize();

            if (sp.Normal * P1 > 0)
                sp.Normal = Vektor.ZeroVektor - sp.Normal;

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

        public void MoveToPoint(double dx, double dy, double dz)
        {
            //Vektor dVec = new Vektor(dx, dy, dz);
            //this.Origin += dVec;
            this.Center.X = dx;
            this.Center.Y = dy;
            this.Center.Z = dz;
            this.SetValues(Center, Dir, R, H);
        }

        public override string ToString()
        {
            return "Cylinder: Center=" + Center + "; Dir=" + Dir;
        }
    }
}
