﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracerLib
{

    /// <summary>
    /// zakladni objekt sceny - koule
    /// 
    /// </summary>
    public class Sphere : DefaultShape
    {

        /// <summary>
        /// polomer koule
        /// </summary>
        public double R { get; set; }

        /// <summary>
        /// pocatek koule
        /// </summary>
        public Vektor Origin { get; set; }

//        public Sphere(Vektor origin, double r) : this(origin, r, new Color(0.7, 0.5, 0.3, 1.0)) { }

        public Sphere(Vektor origin, double r)
        {
            IsActive = true;
            Origin = new Vektor(origin);
            R = r;
            this.Material = new Material();

        }

        public Sphere(Vektor origin, double r, Colour c) : this(origin, r)
        {
            this.Material.Color = new Colour(c);
        }

        public Sphere(Sphere sp)
        {
            IsActive = sp.IsActive;
            this.R = sp.R;
            this.Origin = new Vektor(sp.Origin);
            this.Material = new Material(sp.Material);
        }

        /// <summary>
        /// Zjisti, zda paprsek protina kouli a vrati vsechny body pruniku
        /// </summary>
        /// <param name="P0">pocatek paprsku</param>
        /// <param name="P1">smerovy vektor paprsku</param>
        /// <param name="InterPoint">pripadny vysledny bod pruniku</param>
        /// <returns>true, kdyz existuje bod pruniku s paprskem</returns>
        public override bool Intersects(Vektor P0, Vektor P1, ref List<SolidPoint> InterPoint)
        {
            if (!IsActive)
                return false;

            Vektor P0minusOrigin = P0 - Origin;

            Vektor Pd = new Vektor(P1);
            Pd.Normalize();

            double B = P0minusOrigin * Pd;
            B *= 2;

            double C = P0minusOrigin * P0minusOrigin - R * R;

            double discr = B * B - 4 * C;

            if (discr < MyMath.EPSILON)
            {
                return false;
            }

            double odmoc = Math.Sqrt(discr);
            double t0 = (-B - odmoc) / 2;
            double t1 = (-B + odmoc) / 2;

            double jmenovatel = Math.Sqrt(discr) / 2;
            //double t0 = (-B - jmenovatel);
            //double t1 = (-B + jmenovatel);

            SolidPoint p1 = new SolidPoint();
            p1.T = t0;
            p1.Coord = P0 + Pd * t0;           // souradnice bodu pruniku
            p1.Normal = Vektor.ToDirectionVektor(Origin, p1.Coord);     // normala v bode pruniku (smerem od pocatku)
            p1.Normal.Normalize();             // nebo p1.Normal = p1 * (1/_r);
            p1.Color = new Colour(this.Material.Color);
            p1.Material = this.Material;
            p1.Shape = this;


            SolidPoint p2 = new SolidPoint();
            p2.T = t1;
            p2.Coord = P0 + Pd * t1;           // souradnice bodu pruniku
            p2.Normal = Vektor.ToDirectionVektor(Origin, p2.Coord);     // normala v bode pruniku (smerem od pocatku)
            p2.Normal.Normalize();
            p2.Color = new Colour(this.Material.Color);
            p2.Material = this.Material;
            p2.Shape = this;

            //if (t0 > 0)
                InterPoint.Add(p1);
            //else
                InterPoint.Add(p2);

            return true;
        }


        /// <summary>
        /// Moves the shpere according to given differences
        /// </summary>
        /// <param name="dx">x-direction move</param>
        /// <param name="dy">y-direction move</param>
        /// <param name="dz">z-direction move</param>
        public override void Move(double dx, double dy, double dz)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Sphere: Center=" + Origin + "; R=" + R;
        }

    }
}