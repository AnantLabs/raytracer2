using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RayTracerLib;
using Mathematics;

namespace EditorLib
{
    public class DrawingCone : DrawingDefaultShape
    {

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
        public Vektor Dir { get; private set; }
        public double Height { get; private set; }
        public double Radius { get; private set; }


        public DrawingCone(Cone cone)
        {
            
            SetModelObject(cone);
        }


        public override void SetModelObject(object modelObject)
        {
            if (modelObject != null && modelObject.GetType() == typeof(RayTracerLib.Cone))
                this.SetModelObject((RayTracerLib.Cone)modelObject);
        }

        public void SetModelObject(Cone cone)
        {
            this.ModelObject = cone;

            Lines = new List<Line3D>();
            List<Vektor> points = new List<Vektor>();
            points.Add(new Vektor(0, 0, 0));

            double h = cone.Height;
            double rad = cone.Rad;
            double pi4 = Math.PI / 4;
            double x = Math.Sin(pi4) * rad;
            double z = Math.Cos(pi4) * rad;
            Vektor[] baseline = new Vektor[8];
            baseline[0] = new Vektor(rad, h, 0);
            baseline[1] = new Vektor(x, h, z);
            baseline[2] = new Vektor(0, h, rad);
            baseline[3] = new Vektor(-x, h, z);
            baseline[4] = new Vektor(-rad, h, 0);
            baseline[5] = new Vektor(-x, h, -z);
            baseline[6] = new Vektor(0, h, -rad);
            baseline[7] = new Vektor(x, h, -z);
            points.AddRange(baseline);

            Vektor top = new Vektor(0, 0, 0);
            points.Add(top);
            for (int i = 0; i < baseline.Length; i++)
            {
                Line3D line = new Line3D(baseline[i], top);
                Lines.Add(line);
            }
            Points = points.ToArray();
            /*
            Vektor dir = new Vektor(cone.Dir);
            dir.MultiplyBy(-1);
            dir.Normalize();
            Vektor cDir3 = new Vektor(dir.X, dir.Y, dir.Z);
            Vektor yAxe = new Vektor(0, 1, 0);

            Vektor c = cone.Center;
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(c.X, c.Y, c.Z);


            Vektor crossProd = cDir3.CrossProduct(yAxe);
            double theta = Math.Acos(cDir3 * yAxe);
            Quaternion quatern = new Quaternion(crossProd, MyMath.Radians2Deg(theta));
            double[] degss = quatern.ToEulerDegs();
            Matrix3D matQrt = quatern.Matrix();
            Vektor pointTest = matQrt.Transform2NewPoint(cDir3);
            matQrt = matQrt.Transpose();
            pointTest = matQrt.Transform2NewPoint(yAxe);

            Matrix3D matCr = Matrix3D.NewRotateByDegrees(-degss[0], -degss[1], -degss[2]);
            pointTest = matCr.Transform2NewPoint(cDir3);
            matCr = matCr.Transpose();
            pointTest = matCr.Transform2NewPoint(yAxe);

            _RotatMatrix = matCr;
            double[] degss2 = matCr.GetAnglesFromMatrix();


            _localMatrix = _RotatMatrix * _ShiftMatrix;
            */
            _RotatMatrix = cone._RotatMatrix;
            Vektor c = cone.Peak;
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(c.X, c.Y, c.Z);
            _localMatrix = _RotatMatrix * _ShiftMatrix;
            _localMatrix.TransformPoints(Points);

            /// poradi transformaci
            /// transpozice posouvaci matice neni jeji inverze!!!!!!!!!!!!!!
            //Matrix3D transp = _RotatMatrix.Transpose();
            //Matrix3D transpShift = _ShiftMatrix.GetOppositeShiftMatrix();
            //transpShift.TransformPoints(points);
            //transp.TransformPoints(points);
            //Matrix3D.TestTranspose();



            /*
            Matrix3D.TestMatrixAnglesBack(10, 40, 90);

            // rotace okolo Z a 90 stupnu:
            Vektor p = new Vektor(1.3, 0.6, 2.5);
            Vektor pRot1;
            Matrix3D matrixOwn = Matrix3D.NewRotateByDegrees(0, 0, 90);
            matrixOwn = matrixOwn * Matrix3D.NewRotateByDegrees(0, 40, 0);
            matrixOwn = matrixOwn * Matrix3D.NewRotateByDegrees(10, 0, 0);
            
            matrixOwn = Matrix3D.NewRotateByDegrees(10, 40, 90);

            pRot1 = matrixOwn.Transform2NewPoint(p);
            degss = matrixOwn.GetAnglesFromMatrix();

            Quaternion q = new Quaternion(new Vektor(1, 0, 0), 10);
            matrixOwn = q.Matrix();
            Quaternion qq = new Quaternion(new Vektor(0, 1, 0), 40);
            matrixOwn = qq.Matrix() * matrixOwn;
            Quaternion qqq = new Quaternion(new Vektor(0, 0, 1), 90);
            matrixOwn = qqq.Matrix() * matrixOwn;

            //matrixOwn = qqq.Matrix();
            matrixOwn = matrixOwn.Transpose();
            Vektor pRot2 = matrixOwn.Transform2NewPoint(p);
            degss = q.ToEulerDegs();
            degss = qq.ToEulerDegs();
            degss = qqq.ToEulerDegs();
            degss = matrixOwn.GetAnglesFromMatrix();
            matrixOwn = matrixOwn.Transpose();
            degss = matrixOwn.GetAnglesFromMatrix();

            Matrix3D.TestMatrixAnglesBack(10, 40, 90);
            */
        }
        public override Vektor GetCenter()
        {
            return Center;
        }

        public Vektor[] GetBasePoints()
        {
            Vektor[] ps = new Vektor[8];
            Array.Copy(Points, 1, ps, 0, 8);
            return ps;
        }
    }
}
