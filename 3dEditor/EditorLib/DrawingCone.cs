using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RayTracerLib;

namespace EditorLib
{
    public class DrawingCone : DrawingDefaultShape
    {

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
        public Point3D Dir { get; private set; }
        public double Height { get; private set; }
        public double Radius { get; private set; }


        public DrawingCone(Cone cone)
        {
            Lines = new List<Line3D>();
            List<Point3D> points = new List<Point3D>();
            points.Add(new Point3D(0, 0, 0));

            double rad = cone.Rad;
            double pi4 = Math.PI / 4;
            double x = Math.Sin(pi4) * rad;
            double z = Math.Cos(pi4) * rad;
            Point3D[] baseline = new Point3D[8];
            baseline[0] = new Point3D(rad, 0, 0);
            baseline[1] = new Point3D(x, 0, z);
            baseline[2] = new Point3D(0, 0, rad);
            baseline[3] = new Point3D(-x, 0, z);
            baseline[4] = new Point3D(-rad, 0, 0);
            baseline[5] = new Point3D(-x, 0, -z);
            baseline[6] = new Point3D(0, 0, -rad);
            baseline[7] = new Point3D(x, 0, -z);
            points.AddRange(baseline);

            Point3D top = new Point3D(0, cone.Height, 0);
            points.Add(top);
            for (int i = 0; i < baseline.Length; i++)
            {
                Line3D line = new Line3D(baseline[i], top);
                Lines.Add(line);
            }

            Vektor c = cone.C;
            
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(c.X, c.Y, c.Z);

            Point3D c3 = new Point3D(c.X, c.Y, c.Z);
            Vektor dir = new Vektor(cone.Dir);
            dir.Normalize();
            Point3D cDir3 = new Point3D(dir.X, dir.Y, dir.Z);
            Point3D yAxe = new Point3D(0, 1, 0);


            Point3D crossProd = cDir3.CrossProduct(yAxe);
            double theta = Math.Acos(cDir3 * yAxe);
            Quaternion quatern = new Quaternion(crossProd, MyMath.Rads2Deg(theta));
            double[] degss = quatern.ToEulerDegs();
            Matrix3D matQrt = quatern.Matrix();
            Point3D pointTest = matQrt.Transform2NewPoint(cDir3);
            matQrt = matQrt.Transpose();
            pointTest = matQrt.Transform2NewPoint(yAxe);

            Matrix3D matCr = Matrix3D.NewRotateByDegrees(-degss[0], -degss[1], -degss[2]);
            pointTest = matCr.Transform2NewPoint(cDir3);
            matCr = matCr.Transpose();
            pointTest = matCr.Transform2NewPoint(yAxe);

            _RotatMatrix = matCr;
            double[] degss2 = matCr.GetAnglesFromMatrix();


            _localMatrix = _RotatMatrix * _ShiftMatrix;

            _localMatrix.TransformPoints(points);



            




            /*
            Matrix3D.TestMatrixAnglesBack(10, 40, 90);

            // rotace okolo Z a 90 stupnu:
            Point3D p = new Point3D(1.3, 0.6, 2.5);
            Point3D pRot1;
            Matrix3D matrixOwn = Matrix3D.NewRotateByDegrees(0, 0, 90);
            matrixOwn = matrixOwn * Matrix3D.NewRotateByDegrees(0, 40, 0);
            matrixOwn = matrixOwn * Matrix3D.NewRotateByDegrees(10, 0, 0);
            
            matrixOwn = Matrix3D.NewRotateByDegrees(10, 40, 90);

            pRot1 = matrixOwn.Transform2NewPoint(p);
            degss = matrixOwn.GetAnglesFromMatrix();

            Quaternion q = new Quaternion(new Point3D(1, 0, 0), 10);
            matrixOwn = q.Matrix();
            Quaternion qq = new Quaternion(new Point3D(0, 1, 0), 40);
            matrixOwn = qq.Matrix() * matrixOwn;
            Quaternion qqq = new Quaternion(new Point3D(0, 0, 1), 90);
            matrixOwn = qqq.Matrix() * matrixOwn;

            //matrixOwn = qqq.Matrix();
            matrixOwn = matrixOwn.Transpose();
            Point3D pRot2 = matrixOwn.Transform2NewPoint(p);
            degss = q.ToEulerDegs();
            degss = qq.ToEulerDegs();
            degss = qqq.ToEulerDegs();
            degss = matrixOwn.GetAnglesFromMatrix();
            matrixOwn = matrixOwn.Transpose();
            degss = matrixOwn.GetAnglesFromMatrix();

            Matrix3D.TestMatrixAnglesBack(10, 40, 90);
            */

            




            Points = points.ToArray();
        }
        public override Point3D GetCenter()
        {
            return Center;
        }

        public Point3D[] GetBasePoints()
        {
            Point3D[] ps = new Point3D[8];
            Array.Copy(Points, 1, ps, 0, 8);
            return ps;
        }
    }
}
