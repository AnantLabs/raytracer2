using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RayTracerLib;
using System.Drawing;

namespace EditorLib
{
    public class DrawingAnimation : DrawingObject
    {
        private const double _INIT_A = 5.0;
        private const double _INIT_B = 5.0;
        private const double _INIT_FPS = 25;
        private const double _INIT_TIME = 3.0;
        private const string _INIT_FILE_FULL_PATH = "D:\\anim";
        private const AnimationType _INIT_ANIM_TYPE = AnimationType.VideoOnly;

        private const int _SIDE_NUM = 50;

        public Point3D CenterWorld { get; set; }

        public static Pen EllipsePen = new Pen(Color.DimGray, 2.5f);
        public static Pen EllipseSelectedPen = new Pen(Color.DarkSlateGray, 3.5f);
        public static Pen AxisAPen = new Pen(Color.Blue, 1.5f);
        public static Pen AxisBPen = new Pen(Color.Red, 1.5f);
        /// <summary>
        /// stred elipsy v editoru
        /// </summary>
        public Point3D Center
        {
            get { return Points[0]; }
            private set { Points[0] = value; }
        }

        public Line3D AxisA
        {
            get { return Lines[0]; }
            private set { Lines[0] = value; }
        }

        public Line3D AxisB
        {
            get { return Lines[1]; }
            private set { Lines[1] = value; }
        }
        /// <summary>
        /// zda se ma trajektorie zobrazit v Editoru
        /// </summary>
        public bool ShowAnimation { get; set; }

        public double A { get; private set; }
        public double B { get; private set; }
        public string FileFullPath { get; set; }
        public double FPS { get; set; }
        public double Time { get; set; }
        public AnimationType TypeAnim { get; set; }


        public DrawingAnimation() : this(new Point3D(1, 0, 1), _INIT_A, _INIT_B) { }

        public DrawingAnimation(Point3D center, double a, double b)
        {
            ShowAnimation = true;
            FileFullPath = _INIT_FILE_FULL_PATH;
            FPS = _INIT_FPS;
            Time = _INIT_TIME;
            TypeAnim = _INIT_ANIM_TYPE;
            _RotatMatrix = Matrix3D.Identity;
            this.Set(center, a, b);
        }

        /// <summary>
        /// vytvori body pro vykresleni trajektorie v editoru
        /// </summary>
        public void Set(Point3D center, double a, double b)
        {
            A = a;
            B = b;
            CenterWorld = new Point3D(center);

            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(center.X, center.Y, center.Z);
            Point3D a1 = new Point3D(a, 0, 0);
            Point3D a2 = new Point3D(-a, 0, 0);
            Line3D line1 = new Line3D(a1, a2);  // axis A
            Point3D c1 = new Point3D(0, b, 0);
            Point3D c2 = new Point3D(0, -b, 0);
            Line3D line2 = new Line3D(c1, c2);  // axis B
            
            List<Point3D> points = new List<Point3D>();
            points.Add(new Point3D(0, 0, 0));
            points.Add(a1); points.Add(a2);
            points.Add(c1); points.Add(c2);

            Lines = new List<Line3D>();
            Lines.Add(line1);
            Lines.Add(line2);

            List<Point3D> poledniky = getPoledniky();
            //List<Point3D> poledniky = getPolednikyElipsy(Theta);
            
            points.AddRange(poledniky);
            Points = points.ToArray();
            _localMatrix = _RotatMatrix * _ShiftMatrix;
            _localMatrix.TransformPoints(Points);

        }


        private List<Point3D> getPoledniky()
        {
            int sides = _SIDE_NUM;  // The amount of segment to create the circle
            //double theta = 360;

            //double thetaRads = theta * Math.PI / 180;
            double thetaRads = 0;
            List<Point3D> points = new List<Point3D>();
            for (int a = 0; a < 360; a += 360 / sides)
            {
                double phi = a * Math.PI / 180;
                float x = (float)(Math.Cos(thetaRads) * Math.Sin(phi) * A);
                float y = (float)(Math.Sin(phi) * Math.Sin(thetaRads));
                float z = (float)(Math.Cos(phi) * B);
                Point3D p = new Point3D(x, z, y);
                points.Add(p);
            }
            points.Add(new Point3D(points[0].X, points[0].Y, points[0].Z));

            return points;
        }

        public override void SetModelObject(object modelObject)
        {
            this.Set(CenterWorld, A, B);
        }
        public Point3D[] GetDrawingPoints()
        {
            List<Point3D> ls = new List<Point3D>(Points);
            return ls.GetRange(5, ls.Count - 5).ToArray();
        }

        public override string ToString()
        {
            return "Center= " + CenterWorld + ";A= " + A + ";B= " + B + ";";
        }
    }
}
