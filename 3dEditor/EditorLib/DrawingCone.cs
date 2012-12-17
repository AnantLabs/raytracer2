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
            Vektor c = cone.C;
            Point3D c3 = new Point3D(c.X, c.Y, c.Z);
            Vektor dir = new Vektor(cone.Dir);
            dir.Normalize();
            Vektor cDir = c + dir;
            cDir.Normalize();
            Point3D cDir3 = new Point3D(dir.X, dir.Y, dir.Z);
            Point3D yAxe = new Point3D(0, 1, 0);

            Matrix3D m = new Matrix3D();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    m.Matrix[i, j] = cDir3[i] * yAxe[j];

            Point3D aa1 = m.Transform2NewPoint(c3);
            Point3D bb1 = m.Transform2NewPoint(cDir3);
            Point3D cc1 = m.Transform2NewPoint(yAxe);

            Matrix3D mTransp = m.Transpose();
            Point3D aa2 = mTransp.Transform2NewPoint(c3);
            Point3D bb2 = mTransp.Transform2NewPoint(cDir3);
            Point3D cc2 = mTransp.Transform2NewPoint(yAxe);

            
            Matrix3D.TestMatrixAnglesBack(30, 40, 80);

            double[] angles;
            Matrix3D mX = new Matrix3D();
            mX.SetOnDegrees(30, 0, 0);

            Matrix3D mY = new Matrix3D();
            mY.SetOnDegrees(0, 30, 0);

            Matrix3D mZ = new Matrix3D();
            mZ.SetOnDegrees(0, 0, 30);

            Point3D axis = new Point3D(1, 1, 1);
            axis = axis.Normalize();

            Matrix3D mXYZ = new Matrix3D();
            mXYZ.SetOnDegrees(30, 30, 30);

            Matrix3D mAxis1 = Matrix3D.NewRotateByDegrees(30, 30, 30);
            angles = mAxis1.GetAnglesFromMatrix();
            Matrix3D mAxis2 = Matrix3D.NewRotateByAxis_Degs(new Point3D(1, 1, 1), 30);
            angles = mAxis2.GetAnglesFromMatrix();
            Matrix3D mAxis3 = Matrix3D.NewRotateByAxis_Degs(new Point3D(0, 1, 0), 30);
            angles = mAxis3.GetAnglesFromMatrix();
            Matrix3D mAxis4 = Matrix3D.NewRotateByAxis_Degs(new Point3D(0, 0, 1), 30);
            angles = mAxis4.GetAnglesFromMatrix();
        }
        public override Point3D GetCenter()
        {
            throw new NotImplementedException();
        }
    }
}
