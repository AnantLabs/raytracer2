using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracerLib;

namespace EditorLib
{
    public class DrawingCamera : DrawingObject
    {

        private const float dist = 5;
        private const float height = 3;
        private const float width = 4;

        /// <summary>
        /// umisteni v editoru = poloha
        /// </summary>
        public Point3D Center
        {
            get
            {
                return Points[0];
            }
            private set
            {
                Points[0] = value;
            }
        }

        public DrawingCamera() : this(new Camera()) { }

        public DrawingCamera(Camera cam)
        {
            this.ModelObject = cam;
            Lines = new List<Line3D>();

            List<Point3D> points = new List<Point3D>();
            // prida stred - musi ho pridat jako prvni - je pak v Points[0]
            Point3D center = new Point3D(cam.Source.X, cam.Source.Y, cam.Source.Z);
            points.Add(center);

            Vektor cp = cam.Norm * dist + cam.Source;
            Point3D cp_ = new Point3D(cp.X, cp.Y, cp.Z);     // stred kuzele

            Vektor p12 = cam.Dy * (height / 2) - cp;
            Point3D p12_ = new Point3D(p12.X, p12.Y, p12.Z);

            Vektor p34 = cam.Dy * (height / 2) + cp;
            Point3D p34_ = new Point3D(p34.X, p34.Y, p34.Z);

            Vektor p23 = cam.Dx * (width / 2) + cp;
            Point3D p23_ = new Point3D(p23.X, p23.Y, p23.Z);

            Vektor p14 = cam.Dx * (width / 2) - cp;
            Point3D p14_ = new Point3D(p14.X, p14.Y, p14.Z);

            points.Add(cp_);
            points.Add(p12_);
            
            points.Add(p34_);
            
            points.Add(p14_);

            points.Add(p23_);

            Lines.Add(new Line3D(center, cp_));
            Lines.Add(new Line3D(p12_, p34_));
            Lines.Add(new Line3D(p14_, p23_));

            Points = points.ToArray();
        }
    }
}
