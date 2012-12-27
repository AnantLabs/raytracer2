using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

using RayTracerLib;
using Mathematics;

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

        public Vektor Center
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

            _RotatMatrix = Matrix3D.Identity;
            

            this.SetModelObject(sphere);
        }
        //public List<Rect
        public DrawingSphere(Vektor origin, double rad)
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
            Vektor origin = new Vektor(sphere.Origin.X, sphere.Origin.Y, sphere.Origin.Z);
            this.Set(origin, rad, numSides, decrTheta, decrPhi);
        }
        private void Set(Vektor origin, double rad)
        {
            this.Set(origin, rad, Sides, DecremTheta, DecremPhi);
        }
        private void Set(Vektor origin, double rad, int numSides, int decrTheta, int decrPhi)
        {
            Sides = numSides;
            DecremTheta = decrTheta;
            DecremPhi = decrPhi;
            List<Vektor> poledniky = this.getPoledniky(1, numSides, decrTheta);
            List<Vektor> rovnobezky = this.getRovnobezky(1, numSides, decrPhi);
            rovnobezky.AddRange(poledniky);

            List<Vektor> points = new List<Vektor>();
            points.Add(new Vektor(0, 0, 0));
            points.AddRange(rovnobezky);
            Points = points.ToArray();

            this.Radius = rad;
            
            Matrix3D scale = Matrix3D.ScalingNewMatrix(rad, rad, rad);
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(origin.X, origin.Y, origin.Z);

            //Matrix3D mm = scale * _ShiftMatrix; // nejdrive scaling, pak posunuti
            //mm.TransformPoints(Points);

            _localMatrix = scale * _RotatMatrix;
            _localMatrix = _localMatrix * _ShiftMatrix;
            _localMatrix.TransformPoints(Points);

            this.Lines = new List<Line3D>(0);
        }

        private void Set2(Vektor origin, double rad)
        {
            NSplit = 2;
            this.Points = new Vektor[1 + 4 * 4 * NSplit];
            Vektor center = new Vektor(0, 0, 0);
            this.Points[0] = center;
            List<Vektor> listPoint = new List<Vektor>();
            listPoint.Add(center);
            double rads = Math.PI / NSplit;

            // prvni faze: poledniky koule:
            for (int i = 0; i < NSplit; i++)
            {
                Vektor p1 = new Vektor(center); p1.Posunuti(0, rad, 0);
                listPoint.Add(p1);
                Vektor p2 = new Vektor(center); p2.Posunuti(-rad, 0, 0);
                p2.Otoceni(0, rads * i, 0);
                listPoint.Add(p2);
                Vektor p3 = new Vektor(center); p3.Posunuti(0, -rad, 0);
                listPoint.Add(p3);
                Vektor p4 = new Vektor(center); p4.Posunuti(rad, 0, 0);
                p4.Otoceni(0, rads * i, 0);
                listPoint.Add(p4);
            }

            // druha faze: rovnobezky koule
            // A) horni polokoule
            double iter = rad / NSplit;
            for (double i = 0; i < rad; i += iter)
            {
                double scale = Math.Cos(i / rad);
                Vektor p1 = new Vektor(center);
                p1.Posunuti(-rad, i, 0);
                p1.Scale(scale, 1, scale);
                listPoint.Add(p1);

                Vektor p2 = new Vektor(center);
                p2.Posunuti(0, i, rad);
                p2.Scale(scale, 1, scale);
                listPoint.Add(p2);

                Vektor p3 = new Vektor(center);
                p3.Posunuti(rad, i, 0);
                p3.Scale(scale, 1, scale);
                listPoint.Add(p3);

                Vektor p4 = new Vektor(center);
                p4.Posunuti(0, i, -rad);
                p4.Scale(scale, 1, scale);
                listPoint.Add(p4);
            }

            // B) DOLNI polokoule
            for (double i = 0; i < rad; i += iter)
            {
                double scale = Math.Cos(i / rad);
                Vektor p1 = new Vektor(center);
                p1.Posunuti(-rad, -i, 0);
                p1.Scale(scale, 1, scale);
                listPoint.Add(p1);

                Vektor p2 = new Vektor(center);
                p2.Posunuti(0, -i, rad);
                p2.Scale(scale, 1, scale);
                listPoint.Add(p2);

                Vektor p3 = new Vektor(center);
                p3.Posunuti(rad, -i, 0);
                p3.Scale(scale, 1, scale);
                listPoint.Add(p3);

                Vektor p4 = new Vektor(center);
                p4.Posunuti(0, -i, -rad);
                p4.Scale(scale, 1, scale);
                listPoint.Add(p4);
            }
            this.Points = listPoint.ToArray();
            foreach (Vektor p in Points)
            {
                p.Posunuti(origin.X, origin.Y, origin.Z);
            }
            this.Radius = rad;
            
        }


        private List<Vektor> getPoledniky(double radius, int sidesNum, double decremTheta)
        {
            List<Vektor> allPoints = new List<Vektor>();
            int sides = sidesNum;  // The amount of segment to create the circle
            double theta = 360;
            while (theta > 0)
            {
                double thetaRads = theta * Math.PI / 180;
                List<Vektor> points = new List<Vektor>();
                for (int a = 0; a < 360; a += 360 / sides)
                {
                    double phi = a * Math.PI / 180;
                    float x = (float)(Math.Cos(thetaRads) * Math.Sin(phi));
                    float y = (float)(Math.Sin(phi) * Math.Sin(thetaRads));
                    float z = (float)(Math.Cos(phi));
                    Vektor p = new Vektor(x, z, y);
                    points.Add(p);
                }
                points.Add(new Vektor(points[0].X, points[0].Y, points[0].Z));

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
        private List<Vektor> getRovnobezky(double radius, int sidesNum, double decremPhi)
        {
            List<Vektor> allPoints = new List<Vektor>();
            int sides = sidesNum;  // The amount of segment to create the circle
            double theta = 360;
            while (theta > 0)
            {
                double thetaRads = theta * Math.PI / 180;
                List<Vektor> points = new List<Vektor>();
                for (int a = 0; a < 360; a += 360 / sides)
                {
                    double phi = a * Math.PI / 180;
                    float x = (float)(Math.Cos(phi) * Math.Sin(thetaRads));
                    float y = (float)(Math.Sin(phi) * Math.Sin(thetaRads));
                    float z = (float)(Math.Cos(thetaRads));
                    Vektor p = new Vektor(x, z, y);
                    points.Add(p);
                }
                points.Add(new Vektor(points[0].X, points[0].Y, points[0].Z));

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
        public List<Vektor[]> GetQuartets()
        {
            List<Vektor[]> list = new List<Vektor[]>();
            
            for (int i = 1; i < Points.Length; i+=4)
            {
                Vektor[] arr = new Vektor[4];
                arr[0] = Points[i];
                arr[1] = Points[i+1];
                arr[2] = Points[i+2];
                arr[3] = Points[i+3];
                list.Add(arr);
            }
            return list;
        }

        public Vektor[] GetDrawingPoints()
        {
            List<Vektor> ls = new List<Vektor>(Points);
            return ls.GetRange(1, ls.Count-1).ToArray();
        }

        public override Vektor GetCenter()
        {
            return new Vektor(this.Center);
        }
    }
}