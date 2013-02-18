using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracerLib;
using Mathematics;

namespace EditorLib
{
    public class DrawingCamera : DrawingObject
    {
        /// <summary>
        /// implicitni vzdalenost podstavy od kamery
        /// </summary>
        private const double DIST = 6;
        /// <summary>
        /// implicitni vyska podstavy
        /// </summary>
        private const double HEIGHT = 3;
        /// <summary>
        /// implicitni sirka podstavy
        /// </summary>
        private const double WIDTH = 4;

        /// <summary>
        /// vzdalenost podstavy kuzele od stredu kamery
        /// </summary>
        public double Dist { get; private set; }
        /// <summary>
        /// vyska podstavy kuzele
        /// </summary>
        public double Height { get; private set; }
        /// <summary>
        /// sirka podstavy kuzele
        /// </summary>
        public double Width { get; private set; }

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
        public Vektor Center
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

        public DrawingCamera(Camera cam, double dist, double height, double width, bool showcr, bool showsd1, bool showsd2)
        {
            Label = GetUniqueName();
            Set(cam, dist, height, width, showcr, showsd1, showsd2);
        }

        /// <summary>
        /// vytvori jednoznacne jmeno pro kameru
        /// </summary>
        /// <returns>jednoznacny retezec popisku svetla</returns>
        protected override String GetUniqueName()
        {
            return String.Empty;
        }

        public override void SetModelObject(object modelObject)
        {
            if (modelObject.GetType() == typeof(Camera))
                this.Set((Camera)modelObject, DIST, HEIGHT, WIDTH, true, false, false);
        }
        public void Set(Camera cam, double dist, double height, double width, bool showcr, bool showsd1, bool showsd2)
        {
            this.Dist = dist > 0 ? dist : DIST;
            this.Height = height > 0 ? height : HEIGHT;
            this.Width = width > 0 ? width : WIDTH;
            this.ShowCross = showcr;
            this.ShowSide1 = showsd1;
            this.ShowSide2 = showsd2;
            this.ModelObject = cam;

            Lines = new List<Line3D>();

            List<Vektor> points = new List<Vektor>();
            // prida stred - musi ho pridat jako prvni - je pak v Points[0]
            Vektor center = new Vektor(cam.Source.X, cam.Source.Y, cam.Source.Z);
            points.Add(center);

            Vektor cp = cam.Norm * Dist + cam.Source;
            Vektor cp_ = new Vektor(cp.X, cp.Y, cp.Z);     // stred kuzele

            Vektor p12 = cp - cam.Up * (Height / 2);
            Vektor p12_ = new Vektor(p12.X, p12.Y, p12.Z);

            Vektor p34 = cp + cam.Up * (Height / 2);
            Vektor p34_ = new Vektor(p34.X, p34.Y, p34.Z);

            Vektor p23 = cp + cam.Dx * (Width / 2);
            Vektor p23_ = new Vektor(p23.X, p23.Y, p23.Z);

            Vektor p14 = cp - cam.Dx * (Width / 2);
            Vektor p14_ = new Vektor(p14.X, p14.Y, p14.Z);

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
        public override Vektor GetCenter()
        {
            return new Vektor(this.Center);
        }

        public override string ToString()
        {
            return ModelObject.ToString();
        }
    }
}
