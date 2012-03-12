using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using RayTracerLib;

namespace RayTracerForm
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
        public object Object { get; set; }


        private EditorHelper EditHelp { get; set; }

        /// <summary>
            /// Barva objektu v editoru
            /// </summary>
        System.Drawing.Color Color { get; set; }


        public Placeable(object obj, Color col, int x, int y, EditorHelper editHelp)
        {
            if (obj == null)
                throw new Exception("do Placeable objektu nelze pridal null");
            Object = obj;
            Color = col;
            X = x;
            Y = y;
            EditHelp = editHelp;
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
                if (MyMath.Distance2Points2d(X, Y, x, y) <= sph.R * EditHelp.Meritko)
                    return true;
                return false;
            }
            else if (Object.GetType() == typeof(Box))
            {
                Box box = (Box)Object;
                if (MyMath.Distance2Points2d(X, Y, x, y) <= box.Size * EditHelp.Meritko / 2)
                    return true;
                return false;
            }
            else if (Object.GetType() == typeof(Cube))
            {
                Cube c = (Cube)Object;
                if (MyMath.Distance2Points2d(X, Y, x, y) <= c.Size * EditHelp.Meritko / 2)
                    return true;
                return false;
            }
            else if (Object.GetType() == typeof(Cylinder))
            {
                Cylinder cyl = (Cylinder)Object;


                double rad = cyl.R * EditHelp.Meritko;

                //if (x>minX && x<maxX && y>minY && y<maxY)
                //    return true;

                Vektor DirNom = new Vektor(cyl.Dir);
                DirNom.Normalize();

                Vektor Clower = cyl.Center - DirNom * (cyl.H / 2);
                Vektor Cupper = cyl.Center + DirNom * (cyl.H / 2);
                Point Plower;
                Point Pupper;
                Point Pcenter;
                if (EditHelp.Axes == EditorAxesType.XY)
                {
                    
                    Plower = new Point((int)(Clower.X * EditHelp.Meritko), (int)(Clower.Y * EditHelp.Meritko));
                    Plower.Offset(EditHelp.Center);
                    
                    Pupper = new Point((int)(Cupper.X * EditHelp.Meritko), (int)(Cupper.Y * EditHelp.Meritko));
                    Pupper.Offset(EditHelp.Center);

                    Pcenter = new Point((int)(cyl.Center.X * EditHelp.Meritko), (int)(cyl.Center.Y * EditHelp.Meritko));
                    Pcenter.Offset(EditHelp.Center);

                    Point dirP = new Point(Plower.X - Pupper.X, Plower.Y - Pupper.Y);
                    Vektor dir = Clower - Cupper;
                    Vektor norm = new Vektor(-dir.Y, dir.X, dir.Z);
                    Point Pnorm = new Point((int)(norm.X * EditHelp.Meritko), (int)(norm.Y * EditHelp.Meritko));
                    Pnorm.Offset(EditHelp.Center);

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
                    if (vzdal < (cyl.H / 2) * EditHelp.Meritko)
                        return true;

                }
                else if (EditHelp.Axes == EditorAxesType.ZY)
                {
                    Plower = new Point((int)(Clower.Z * EditHelp.Meritko), (int)(Clower.Y * EditHelp.Meritko));
                    Plower.Offset(EditHelp.Center);

                    Pupper = new Point((int)(Cupper.Z * EditHelp.Meritko), (int)(Cupper.Y * EditHelp.Meritko));
                    Pupper.Offset(EditHelp.Center);

                    Pcenter = new Point((int)(cyl.Center.Z * EditHelp.Meritko), (int)(cyl.Center.Y * EditHelp.Meritko));
                    Pcenter.Offset(EditHelp.Center);

                    Point dirP = new Point(Plower.X - Pupper.X, Plower.Y - Pupper.Y);
                    Vektor dir = Clower - Cupper;

                    // primka kolma na smer valce
                    Vektor norm = new Vektor(-dir.Y, dir.Z, dir.Z);
                    Point Pnorm = new Point((int)(norm.X * EditHelp.Meritko), (int)(norm.Y * EditHelp.Meritko));
                    Pnorm.Offset(EditHelp.Center);

                    // test dolni kruznice
                    if (MyMath.Distance2Points2d(Plower.X, Plower.Y, x, y) <= rad)
                        return true;

                    // test horni kruznice
                    if (MyMath.Distance2Points2d(Pupper.X, Pupper.Y, x, y) <= rad)
                        return true;

                    Pnorm.X = -dirP.Y;
                    Pnorm.Y = dirP.X;

                    double a = (double)-DirNom.Y;
                    double b = (double)DirNom.Z;
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
                    if (vzdal < (cyl.H / 2) * EditHelp.Meritko)
                        return true;
                }
                // osy X--Z
                else if (EditHelp.Axes == EditorAxesType.XZ)
                {

                    Plower = new Point((int)(Clower.X * EditHelp.Meritko), (int)(Clower.Z * EditHelp.Meritko));
                    Plower.Offset(EditHelp.Center);

                    Pupper = new Point((int)(Cupper.X * EditHelp.Meritko), (int)(Cupper.Z * EditHelp.Meritko));
                    Pupper.Offset(EditHelp.Center);

                    Pcenter = new Point((int)(cyl.Center.X * EditHelp.Meritko), (int)(cyl.Center.Z * EditHelp.Meritko));
                    Pcenter.Offset(EditHelp.Center);

                    Point dirP = new Point(Plower.X - Pupper.X, Plower.Y - Pupper.Y);
                    Vektor dir = Clower - Cupper;

                    // primka kolma na smer valce
                    Vektor norm = new Vektor(-dir.Z, dir.Y, dir.X);
                    Point Pnorm = new Point((int)(norm.X * EditHelp.Meritko), (int)(norm.Y * EditHelp.Meritko));
                    Pnorm.Offset(EditHelp.Center);

                    // test dolni kruznice
                    if (MyMath.Distance2Points2d(Plower.X, Plower.Y, x, y) <= rad)
                        return true;

                    // test horni kruznice
                    if (MyMath.Distance2Points2d(Pupper.X, Pupper.Y, x, y) <= rad)
                        return true;

                    Pnorm.X = -dirP.Y;
                    Pnorm.Y = dirP.X;

                    double a = (double)-DirNom.Z;
                    double b = (double)DirNom.Y;
                    double c = -(double)(a * Pcenter.X) - (double)(b * Pcenter.Y);

                    double vzdal = Math.Abs((double)(a * x) + (double)(b * y + c)) / Math.Sqrt(a * a + b * b);
                    if (Double.IsNaN(vzdal))
                    {
                        a = (double)-DirNom.Y;
                        b = (double)DirNom.X;
                        c = -(double)(a * Pcenter.X) - (double)(b * Pcenter.Y);

                        vzdal = Math.Abs((double)(a * x) + (double)(b * y + c)) / Math.Sqrt(a * a + b * b);
                    }
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
                    if (vzdal < (cyl.H / 2) * EditHelp.Meritko)
                        return true;
                }
                //if (MyMath.Distance2Points2d(X, Y, x, y) <= (cyl.H/2) * EditHelp.Meritko_Editoru)
                //    return true;
                return false;

            }

            else if (Object.GetType() == typeof(Light))
            {
                Light l = (Light)Object;

                int w = Properties.Resources.bulb_transp.Width;
                int h = Properties.Resources.bulb_transp.Height;

                Point uppLeft = new Point((int)(X - w / 2), (int)(Y - h / 2));
                Point uppRight = new Point((int)(X + w / 2), (int)(Y - h / 2));
                Point lowLeft = new Point((int)(X - w / 2), (int)(Y + h / 2));
                Point lowRight = new Point((int)(X + w / 2), (int)(Y + h / 2));

                if (x > uppLeft.X && x < uppRight.X && x > lowLeft.X && x < lowRight.X
                    && y > uppLeft.Y && y > uppRight.Y && y < lowLeft.Y && y < lowRight.Y)
                    return true;

                return false;
            }

            else if (Object.GetType() == typeof(Camera))
            {
                Camera cam = (Camera)Object;

                int w = Properties.Resources.camera.Width;
                int h = Properties.Resources.camera.Height;

                Point uppLeft = new Point((int)(X - w / 2), (int)(Y - h / 2));
                Point uppRight = new Point((int)(X + w / 2), (int)(Y - h / 2));
                Point lowLeft = new Point((int)(X - w / 2), (int)(Y + h / 2));
                Point lowRight = new Point((int)(X + w / 2), (int)(Y + h / 2));

                if (x > uppLeft.X && x < uppRight.X && x > lowLeft.X && x < lowRight.X
                    && y > uppLeft.Y && y > uppRight.Y && y < lowLeft.Y && y < lowRight.Y)
                    return true;

                return false;
            }
            return false;
        }

        /// <summary>
        /// nastavi polohu objektu na zadane souradnice vzhledem k editoru
        /// -> treba pred zmenou souradnic zmenit i meritko nastavovanych souradnic
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void Set(int dx, int dy)
        {
            if (Object.GetType() == typeof(Sphere))
            {
                Sphere sph = (Sphere)Object;
                if (EditHelp.Axes == EditorAxesType.XY)
                {
                    sph.Origin.X = dx / EditHelp.Meritko;
                    sph.Origin.Y = dy / EditHelp.Meritko;
                }
                else if (EditHelp.Axes == EditorAxesType.ZY)
                {
                    sph.Origin.Z = dx / EditHelp.Meritko;
                    sph.Origin.Y = dy / EditHelp.Meritko;
                }
                else
                {
                    sph.Origin.X = dx / EditHelp.Meritko;
                    sph.Origin.Z = dy / EditHelp.Meritko;

                }
                
            }
            else if (Object.GetType() == typeof(Box))
            {
                Box box = (Box)Object;
                if (EditHelp.Axes == EditorAxesType.XY)
                {
                    box.Center.X = dx / EditHelp.Meritko;
                    box.Center.Y = dy / EditHelp.Meritko;
                }
                else if (EditHelp.Axes == EditorAxesType.ZY)
                {
                    box.Center.Z = dx / EditHelp.Meritko;
                    box.Center.Y = dy / EditHelp.Meritko;
                }
                else
                {
                    box.Center.X = dx / EditHelp.Meritko;
                    box.Center.Z = dy / EditHelp.Meritko;
                }
                
                box.SetCenterSize(box.Center, box.Size);
                //box.SetCenterSize
            }
            else if (Object.GetType() == typeof(Cube))
            {
                Cube c = (Cube)Object;
                if (EditHelp.Axes == EditorAxesType.XY)
                {
                    c.Center.X = dx / EditHelp.Meritko;
                    c.Center.Y = dy / EditHelp.Meritko;
                }
                else if (EditHelp.Axes == EditorAxesType.ZY)
                {
                    c.Center.Z = dx / EditHelp.Meritko;
                    c.Center.Y = dy / EditHelp.Meritko;
                }
                else
                {
                    c.Center.X = dx / EditHelp.Meritko;
                    c.Center.Z = dy / EditHelp.Meritko;
                }
                
                c.SetValues(c.Center, c.Dir, c.Size);
                //box.SetCenterSize
            }
            else if (Object.GetType() == typeof(Cylinder))
            {
                Cylinder cyl = (Cylinder)Object;
                if (EditHelp.Axes == EditorAxesType.XY)
                {
                    cyl.Center.X = dx / EditHelp.Meritko;
                    cyl.Center.Y = dy / EditHelp.Meritko;
                }
                else if (EditHelp.Axes == EditorAxesType.ZY)
                {
                    cyl.Center.Z = dx / EditHelp.Meritko;
                    cyl.Center.Y = dy / EditHelp.Meritko;
                }
                else
                {
                    cyl.Center.X = dx / EditHelp.Meritko;
                    cyl.Center.Z = dy / EditHelp.Meritko;
                }
                
                cyl.SetValues(cyl.Center, cyl.Dir, cyl.R, cyl.H);
            }

            else if (Object.GetType() == typeof(Light))
            {
                Light l = (Light)Object;
                if (EditHelp.Axes == EditorAxesType.XY)
                {
                    l.Coord.X = dx / EditHelp.Meritko;
                    l.Coord.Y = dy / EditHelp.Meritko;
                }
                else if (EditHelp.Axes == EditorAxesType.ZY)
                {
                    l.Coord.Z = dx / EditHelp.Meritko;
                    l.Coord.Y = dy / EditHelp.Meritko;
                }
                else
                {
                    l.Coord.X = dx / EditHelp.Meritko;
                    l.Coord.Z = dy / EditHelp.Meritko;
                }
                
            }

            else if (Object.GetType() == typeof(Camera))
            {
                Camera cam = (Camera)Object;
                if (EditHelp.Axes == EditorAxesType.XY)
                {
                    cam.Source.X = dx / EditHelp.Meritko;
                    cam.Source.Y = dy / EditHelp.Meritko;
                }
                else if (EditHelp.Axes == EditorAxesType.ZY)
                {
                    cam.Source.Z = dx / EditHelp.Meritko;
                    cam.Source.Y = dy / EditHelp.Meritko;
                }
                else
                {
                    cam.Source.X = dx / EditHelp.Meritko;
                    cam.Source.Z = dy / EditHelp.Meritko;
                }
            }

        }


    }
}