using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

using RayTracerLib;

namespace EditorLib
{
    public class DrawingSphere : DrawingObject
    {

 
        public double Radius { get; private set; }

        public Point3D Center
        {
            get
            {
                return Points[0];
            }
            set
            {
                Points[0] = value;
            }
        }

        /// <summary>
        /// pocet oblouku v jedne ctvrtine koule, na ktere je rozdelena
        /// </summary>
        public int NSplit { get; set; }

        /// <summary>
        /// zakladni konstruktor, ktery vytvori i vlastni kouli z Raytraceru
        /// </summary>
        public DrawingSphere()
        {
            Sphere sph = new Sphere(new Vektor(), 1);
            this.SetModelObject(sph);
        }

        public DrawingSphere(Sphere sphere)
        {
            this.SetModelObject(sphere);
        }
        //public List<Rect
        public DrawingSphere(Point3D origin, double rad)
        {
            this.Set(origin, rad);
        }

        private void Set(Point3D origin, double rad)
        {
            NSplit = 8;
            this.Points = new Point3D[1 + 4 * 4 * NSplit];
            Point3D center = new Point3D(0, 0, 0);
            this.Points[0] = center;
            List<Point3D> listPoint = new List<Point3D>();
            listPoint.Add(center);
            double rads = Math.PI / NSplit;

            // prvni faze: poledniky koule:
            for (int i = 0; i < NSplit; i++)
            {
                Point3D p1 = new Point3D(center); p1.Posunuti(0, rad, 0);
                listPoint.Add(p1);
                Point3D p2 = new Point3D(center); p2.Posunuti(-rad, 0, 0);
                p2.Otoceni(0, rads * i, 0);
                listPoint.Add(p2);
                Point3D p3 = new Point3D(center); p3.Posunuti(0, -rad, 0);
                listPoint.Add(p3);
                Point3D p4 = new Point3D(center); p4.Posunuti(rad, 0, 0);
                p4.Otoceni(0, rads * i, 0);
                listPoint.Add(p4);
            }

            // druha faze: rovnobezky koule
            // A) horni polokoule
            double iter = rad / NSplit;
            for (double i = 0; i < rad; i += iter)
            {
                double scale = Math.Cos(i / rad);
                Point3D p1 = new Point3D(center);
                p1.Posunuti(-rad, i, 0);
                p1.Scale(scale, 1, scale);
                listPoint.Add(p1);

                Point3D p2 = new Point3D(center);
                p2.Posunuti(0, i, rad);
                p2.Scale(scale, 1, scale);
                listPoint.Add(p2);

                Point3D p3 = new Point3D(center);
                p3.Posunuti(rad, i, 0);
                p3.Scale(scale, 1, scale);
                listPoint.Add(p3);

                Point3D p4 = new Point3D(center);
                p4.Posunuti(0, i, -rad);
                p4.Scale(scale, 1, scale);
                listPoint.Add(p4);
            }

            // B) DOLNI polokoule
            for (double i = 0; i < rad; i += iter)
            {
                double scale = Math.Cos(i / rad);
                Point3D p1 = new Point3D(center);
                p1.Posunuti(-rad, -i, 0);
                p1.Scale(scale, 1, scale);
                listPoint.Add(p1);

                Point3D p2 = new Point3D(center);
                p2.Posunuti(0, -i, rad);
                p2.Scale(scale, 1, scale);
                listPoint.Add(p2);

                Point3D p3 = new Point3D(center);
                p3.Posunuti(rad, -i, 0);
                p3.Scale(scale, 1, scale);
                listPoint.Add(p3);

                Point3D p4 = new Point3D(center);
                p4.Posunuti(0, -i, -rad);
                p4.Scale(scale, 1, scale);
                listPoint.Add(p4);
            }
            this.Points = listPoint.ToArray();
            foreach (Point3D p in Points)
            {
                p.Posunuti(origin.X, origin.Y, origin.Z);
            }
            this.Radius = rad;
            this.Lines = new List<Line3D>(0);
        }

        /// <summary>
        /// nastavi kouli z Raytraceru do Editoru
        /// RayTracerLib -> EditorLib
        /// </summary>
        /// <param name="sphere"></param>
        public void SetModelObject(RayTracerLib.Sphere sphere)
        {
            this.ModelObject = sphere;
            double rad = sphere.R;
            Point3D origin = new Point3D(sphere.Origin.X, sphere.Origin.Y, sphere.Origin.Z);
            this.Set(origin, rad);
        }

        /// <summary>
        /// Vrati seznam ctveric bodu, ktere jsou potrebne k vykresleni poledniku nebo rovnobezky
        /// pomoci splineu. Vrati tedy ctverec v prostoru, do ktereho se vykresli jedna kruznice.
        /// Tyto body jsou vraceny v poli delky 4
        /// </summary>
        /// <returns></returns>
        public List<Point3D[]> GetQuartets()
        {
            List<Point3D[]> list = new List<Point3D[]>();
            
            for (int i = 1; i < Points.Length; i+=4)
            {
                Point3D[] arr = new Point3D[4];
                arr[0] = Points[i];
                arr[1] = Points[i+1];
                arr[2] = Points[i+2];
                arr[3] = Points[i+3];
                list.Add(arr);
            }
            return list;
        }
    }
}