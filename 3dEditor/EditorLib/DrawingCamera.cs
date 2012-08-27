using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracerLib;

namespace EditorLib
{
    public class DrawingCamera : DrawingObject
    {
        /// <summary>
        /// implicitni vzdalenost podstavy od kamery
        /// </summary>
        private const int DIST = 6;
        /// <summary>
        /// implicitni vyska podstavy
        /// </summary>
        private const int HEIGHT = 3;
        /// <summary>
        /// implicitni sirka podstavy
        /// </summary>
        private const int WIDTH = 4;

        /// <summary>
        /// vzdalenost podstavy kuzele od stredu kamery
        /// </summary>
        public int Dist { get; private set; }
        /// <summary>
        /// vyska podstavy kuzele
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// sirka podstavy kuzele
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// definuje, zda se zobrazi kriz na podstave kuzele kamery
        /// </summary>
        public bool ShowCross { get; private set; }
        /// <summary>
        /// definuje, zda se zobrazi jedna stena plaste kuzele kamery
        /// </summary>
        public bool ShowSide1 { get; private set; }
        /// <summary>
        /// definuje, zda se zobrazi druha stena plaste kuzele kamery
        /// </summary>
        public bool ShowSide2 { get; private set; }

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
            : this(cam, DIST, HEIGHT, WIDTH, true, false, false) { }

        public DrawingCamera(Camera cam, int dist, int height, int width, bool showcr, bool showsd1, bool showsd2)
        {
            Set(cam, dist, height, width, showcr, showsd1, showsd2);
        }

        public override void SetModelObject(object modelObject)
        {
            if (modelObject.GetType() == typeof(Camera))
                this.Set((Camera)modelObject, DIST, HEIGHT, WIDTH, true, false, false);
        }
        public void Set(Camera cam, int dist, int height, int width, bool showcr, bool showsd1, bool showsd2)
        {
            this.Dist = dist > 0 ? dist : DIST;
            this.Height = height > 0 ? height : HEIGHT;
            this.Width = width > 0 ? width : WIDTH;
            this.ShowCross = showcr;
            this.ShowSide1 = showsd1;
            this.ShowSide2 = showsd2;
            this.ModelObject = cam;

            Lines = new List<Line3D>();

            List<Point3D> points = new List<Point3D>();
            // prida stred - musi ho pridat jako prvni - je pak v Points[0]
            Point3D center = new Point3D(cam.Source.X, cam.Source.Y, cam.Source.Z);
            points.Add(center);

            Vektor cp = cam.Norm * Dist + cam.Source;
            Point3D cp_ = new Point3D(cp.X, cp.Y, cp.Z);     // stred kuzele

            Vektor p12 = cp - cam.Dy * (Height / 2);
            Point3D p12_ = new Point3D(p12.X, p12.Y, p12.Z);

            Vektor p34 = cp + cam.Dy * (Height / 2);
            Point3D p34_ = new Point3D(p34.X, p34.Y, p34.Z);

            Vektor p23 = cp + cam.Dx * (Width / 2);
            Point3D p23_ = new Point3D(p23.X, p23.Y, p23.Z);

            Vektor p14 = cp - cam.Dx * (Width / 2);
            Point3D p14_ = new Point3D(p14.X, p14.Y, p14.Z);

            points.Add(cp_);
            points.Add(p12_);
            
            points.Add(p14_);
            points.Add(p34_);
            points.Add(p23_);

            Lines.Add(new Line3D(center, cp_)); // osa kuzele
            Lines.Add(new Line3D(p12_, p34_));  // cara podstavy
            Lines.Add(new Line3D(p23_, p14_));  // cara podstavy
            Lines.Add(new Line3D(center, p12_));  // stena kuzele
            Lines.Add(new Line3D(center, p23_));  // stena kuzele
            Lines.Add(new Line3D(center, p34_));  // stena kuzele
            Lines.Add(new Line3D(center, p14_));  // stena kuzele


            Points = points.ToArray();
        }
    }
}
