using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using RayTracerLib;

namespace RayTracerLib
{
    /// <summary>
    /// objekt z RayTraceru, ktery se bude zobrazovat v obrazku editoru
    /// </summary>
    public class Placeable
    {
        /// <summary>
        /// X-ova souradnice objektu v editoru
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y-ova souradnice objektu v editoru
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Objekt z Raytraceru, ktery se muze zobrazit v editoru
        /// </summary>
        public DefaultShape Object { get; set; }

        private Point Offset;

        private 

        /// <summary>
        /// Barva objektu v editoru
        /// </summary>
        System.Drawing.Color Color { get; set; }

        public int Meritko { get; set; }

        public Placeable(DefaultShape obj, Color col, int x, int y, int meritko, Point offset)
        {
            if (obj == null)
                throw new Exception("do Placeable objektu nelze pridal null");
            Object = obj;
            Color = col;
            X = x;
            Y = y;
            Meritko = meritko;
            Offset = offset;
        }

        /// <summary>
        /// Zjisti, zda objekt obsahuje bod o zadanych souradnicich
        /// </summary>
        /// <param name="x">X-ova souradnice</param>
        /// <param name="y">Y-ova souradnice</param>
        /// <returns></returns>
        public bool Contains(int x, int y)
        {
            if (Object.GetType() == typeof(Sphere))
            {
                Sphere sph = (Sphere)Object;
                if (MyMath.Distance2Points2d(X, Y, x, y) <= sph.R * Meritko)
                    return true;
                return false;
            }
            else if (Object.GetType() == typeof(Box))
            {
                Box box = (Box)Object;
                if (MyMath.Distance2Points2d(X, Y, x, y) <= box.Size*Meritko / 2)
                    return true;
                return false;
            }
            else if (Object.GetType() == typeof(Cylinder))
            {
                Cylinder cyl = (Cylinder)Object;


                double rad = cyl.R * Meritko;

                //if (x>minX && x<maxX && y>minY && y<maxY)
                //    return true;

                Vektor DirNom = new Vektor(cyl.Dir);
                DirNom.Normalize();

                Vektor Clower = cyl.Center - DirNom * (cyl.H / 2);
                Point Plower = new Point((int)(Clower.X * Meritko), (int)(Clower.Y * Meritko));
                Plower.Offset(Offset);
                Vektor Cupper = cyl.Center + DirNom * (cyl.H / 2);
                Point Pupper = new Point((int)(Cupper.X * Meritko), (int)(Cupper.Y * Meritko));
                Pupper.Offset(Offset);
                Point Pcenter = new Point((int)(cyl.Center.X * Meritko), (int)(cyl.Center.Y * Meritko));
                Pcenter.Offset(Offset);

                Point dirP = new Point(Plower.X - Pupper.X, Plower.Y - Pupper.Y);
                Vektor dir = Clower - Cupper;
                Vektor norm = new Vektor(-dir.Y, dir.X, dir.Z);
                Point Pnorm = new Point((int)(norm.X * Meritko), (int)(norm.Y * Meritko));
                Pnorm.Offset(Offset);

                // test dolni kruznice
                if (MyMath.Distance2Points2d(Plower.X, Plower.Y, x, y) <= rad)
                    return true;

                // test horni kruznice
                if (MyMath.Distance2Points2d(Pupper.X, Pupper.Y, x, y) <= rad)
                    return true;

                Pnorm.X = -dirP.Y;
                Pnorm.Y = dirP.X;

                double a = (double)-DirNom.Y;
                double b = (double)DirNom.X;
                double c = -(double)(a * Pcenter.X) - (double)(b * Pcenter.Y);

                double vzdal = Math.Abs((double)(a * x) + (double)(b * y + c)) / Math.Sqrt(a * a + b * b);
                if (vzdal > rad)
                    return false;


                Point middleP1 = new Point((int)(Pcenter.X + rad * Pnorm.X), (int)(Pcenter.Y + rad * Pnorm.Y));
                Point middleP2 = new Point((int)(Pcenter.X - rad * Pnorm.X), (int)(Pcenter.Y - rad * Pnorm.Y));


                dirP = new Point(middleP1.X - middleP2.X, middleP1.Y - middleP2.Y);
                Pnorm = new Point(-dirP.Y, dirP.X);
                a = (double)Pnorm.X;
                b = (double)Pnorm.Y;
                c = -(double)(a * Pcenter.X) - (double)(b * Pcenter.Y);

                vzdal = Math.Abs((double)(a * x) + (double)(b * y + c)) / Math.Sqrt(a * a + b * b);
                if (vzdal < (cyl.H/2) * Meritko)
                    return true;

                
                //if (MyMath.Distance2Points2d(X, Y, x, y) <= (cyl.H/2) * Meritko)
                //    return true;
                return false;
                
            }
            return false;
        }

        public void Set(int dx, int dy)
        {
            if (Object.GetType() == typeof(Sphere))
            {
                Sphere sph = (Sphere)Object;
                sph.Origin.X = dx / Meritko;
                sph.Origin.Y = dy / Meritko;
            }
            else if (Object.GetType() == typeof(Box))
            {
                Box box = (Box)Object;
                box.Center.X = dx / Meritko;
                box.Center.Y = dy / Meritko;
                box.SetCenterSize(box.Center, box.Size);
                //box.SetCenterSize
            }
            else if (Object.GetType() == typeof(Cylinder))
            {
                Cylinder cyl = (Cylinder)Object;
                cyl.Center.X = dx / Meritko;
                cyl.Center.Y = dy / Meritko;
                cyl.SetValues(cyl.Center, cyl.Dir, cyl.R, cyl.H);
            }
            
        }


    }
}
