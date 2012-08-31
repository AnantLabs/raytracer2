using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

using RayTracerLib;

namespace EditorLib
{
    public class DrawingSphere : DrawingDefaultShape
    {
        public double Radius { get; private set; }

        const int _SIDES = 20;
        const int _DECREM_THETA = 20;
        const int _DECREM_PHI = 20;

        public int Sides { get; private set; }
        public int DecremTheta { get; private set; }
        public int DecremPhi { get; private set; }

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
        public DrawingSphere() : this (new Sphere(new Vektor(), 1))
        {
            Sides = _SIDES;
            DecremTheta = _DECREM_THETA;
            DecremPhi = _DECREM_PHI;
        }

        public DrawingSphere(Sphere sphere)
        {
            Sides = _SIDES;
            DecremTheta = _DECREM_THETA;
            DecremPhi = _DECREM_PHI;
            this.SetModelObject(sphere);
        }
        //public List<Rect
        public DrawingSphere(Point3D origin, double rad)
            : this(new Sphere(new Vektor(origin.X,origin.Y,origin.Z), rad))
        {
            this.Set(origin, rad);
        }

        public override void SetModelObject(object modelObject)
        {
            if (modelObject != null && modelObject.GetType() == typeof(RayTracerLib.Sphere))
                this.SetModelObject((RayTracerLib.Sphere)modelObject);
        }
        /// <summary>
        /// nastavi kouli z Raytraceru do Editoru
        /// RayTracerLib -> EditorLib
        /// </summary>
        /// <param name="sphere"></param>
        public void SetModelObject(RayTracerLib.Sphere sphere)
        {
            this.SetModelObject(sphere, _SIDES, _DECREM_THETA, _DECREM_PHI);
        }
        
        public void SetModelObject(RayTracerLib.Sphere sphere, int numSides, int decrTheta, int decrPhi)
        {
            this.ModelObject = sphere;
            double rad = sphere.R;
            Point3D origin = new Point3D(sphere.Origin.X, sphere.Origin.Y, sphere.Origin.Z);
            this.Set(origin, rad, numSides, decrTheta, decrPhi);
        }
        private void Set(Point3D origin, double rad)
        {
            this.Set(origin, rad, Sides, DecremTheta, DecremPhi);
        }
        private void Set(Point3D origin, double rad, int numSides, int decrTheta, int decrPhi)
        {
            Sides = numSides;
            DecremTheta = decrTheta;
            DecremPhi = decrPhi;
            List<Point3D> poledniky = this.getPoledniky(1, numSides, decrTheta);
            List<Point3D> rovnobezky = this.getRovnobezky(1, numSides, decrPhi);
            rovnobezky.AddRange(poledniky);

            List<Point3D> points = new List<Point3D>();
            points.Add(new Point3D(0, 0, 0));
            points.AddRange(rovnobezky);
            Points = points.ToArray();

            this.Radius = rad;
            
            Matrix3D scale = Matrix3D.ScalingNewMatrix(rad, rad, rad);
            Matrix3D posunuti = Matrix3D.PosunutiNewMatrix(origin.X, origin.Y, origin.Z);
            Matrix3D mm = scale * posunuti; // nejdrive scaling, pak posunuti
            mm.TransformPoints(Points);
            
            this.Lines = new List<Line3D>(0);
        }

        private void Set2(Point3D origin, double rad)
        {
            NSplit = 2;
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
            
        }


        private List<Point3D> getPoledniky(double radius, int sidesNum, double decremTheta)
        {
            List<Point3D> allPoints = new List<Point3D>();
            int sides = sidesNum;  // The amount of segment to create the circle
            double theta = 360;
            while (theta > 0)
            {
                double thetaRads = theta * Math.PI / 180;
                List<Point3D> points = new List<Point3D>();
                for (int a = 0; a < 360; a += 360 / sides)
                {
                    double phi = a * Math.PI / 180;
                    float x = (float)(Math.Cos(thetaRads) * Math.Sin(phi));
                    float y = (float)(Math.Sin(phi) * Math.Sin(thetaRads));
                    float z = (float)(Math.Cos(phi));
                    Point3D p = new Point3D(x, z, y);
                    points.Add(p);
                }
                points.Add(new Point3D(points[0].X, points[0].Y, points[0].Z));

                allPoints.AddRange(points);
                theta -= decremTheta;
            }
            return allPoints;
        }

        /// <summary>
        /// x = r * Sin(phi)*Cos(theta)
        /// y = r * Sin(theta)*Sin(phi)
        /// z = r * Cos(theta)
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <param name="sidesNum"></param>
        /// <param name="decrementRad"></param>
        /// <returns></returns>
        private List<Point3D> getRovnobezky(double radius, int sidesNum, double decremPhi)
        {
            List<Point3D> allPoints = new List<Point3D>();
            int sides = sidesNum;  // The amount of segment to create the circle
            double theta = 360;
            while (theta > 0)
            {
                double thetaRads = theta * Math.PI / 180;
                List<Point3D> points = new List<Point3D>();
                for (int a = 0; a < 360; a += 360 / sides)
                {
                    double phi = a * Math.PI / 180;
                    float x = (float)(Math.Cos(phi) * Math.Sin(thetaRads));
                    float y = (float)(Math.Sin(phi) * Math.Sin(thetaRads));
                    float z = (float)(Math.Cos(thetaRads));
                    Point3D p = new Point3D(x, z, y);
                    points.Add(p);
                }
                points.Add(new Point3D(points[0].X, points[0].Y, points[0].Z));

                allPoints.AddRange(points);
                theta -= decremPhi;
            }
            return allPoints;
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

        public Point3D[] GetDrawingPoints()
        {
            List<Point3D> ls = new List<Point3D>(Points);
            return ls.GetRange(1, ls.Count-1).ToArray();
        }
    }
}