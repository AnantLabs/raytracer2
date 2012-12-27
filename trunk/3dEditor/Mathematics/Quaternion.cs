using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mathematics
{
    /// <summary>
    /// QUATERNION
    /// Call the temporary quaternion: local_rotation. 
    /// Call the permanent quaternion: total.
    /// total = local_rotation * total  //multiplication order matters on this line
    /// </summary>
    public class Quaternion
    {

        public static Quaternion Identity { get { return new Quaternion(1, 0, 0, 0); } }

        private double[] values = new double[4];

        public double W { get { return values[0]; } set { values[0] = value; } }
        public double X { get { return values[1]; } set { values[1] = value; } }
        public double Y { get { return values[2]; } set { values[2] = value; } }
        public double Z { get { return values[3]; } set { values[3] = value; } }

        public Quaternion(double w, double x, double y, double z)
        {
            values = new double[] { w, x, y, z };
        }
        public Quaternion(double w, Vektor p)
        {
            W = w;
            X = p.X;
            Y = p.Y;
            Z = p.Z;
        }

        /// <summary>
        /// vytvori kvaternion jako rotace okolo vektoru o uhel
        /// </summary>
        /// <param name="axis">vektor (nemusi byt normalizovany, ale z jeho normalizovaneho tvaru se vytvori quaternion)</param>
        /// <param name="thetaDegs">uhel (ve stupnich)</param>
        public Quaternion(Vektor axis, double thetaDegs)
        {
            Vektor ax = new Vektor(axis);
            ax.Normalize();
            double thetaPul = MyMath.Degrees2Rad(thetaDegs) / 2;
            W = Math.Cos(thetaPul);
            double sin = Math.Sin(thetaPul);
            X = ax.X * sin;
            Y = ax.Y * sin;
            Z = ax.Z * sin;
            Normalize();
        }

        public double Size()
        {
            double size = W * W + X * X + Y * Y + Z * Z;
            size = Math.Sqrt(size);
            return size;
        }

        public void Normalize()
        {
            double size = Size();
            W = W / size;
            X = X / size;
            Y = Y / size;
            Z = Z / size;
        }

        /// <summary>
        /// vrati rotacni matici vytvorenou s aktualniho kvatelnionu
        /// aktualni kvaternion musi byt normalizovany
        /// </summary>
        /// <returns>rotacni matice</returns>
        public Matrix3D Matrix()
        {
            Vektor row1 = new Vektor(
                1 - 2 * Y * Y - 2 * Z * Z,
                2 * X * Y - 2 * W * Z, 
                2 * X * Z + 2 * W * Y, 
                0);
            Vektor row2 = new Vektor(
                2 * X * Y + 2 * W * Z, 
                1 - 2 * X * X - 2 * Z * Z,
                2 * Y * Z - 2 * W * X, 
                0);
            Vektor row3 = new Vektor(
                2 * X * Z - 2 * W * Y, 
                2 * Y * Z + 2 * W * X, 
                1 - 2 * X * X - 2 * Y * Y, 
                0);
            Vektor row4 = new Vektor(0, 0, 0, 1);
            Matrix3D m = new Matrix3D(row1, row2, row3, row4);
            return m;
        }

        /// <summary>
        /// z quaternionu vrati Eulerovy uhly ve stupnich
        /// </summary>
        /// <returns>pole uhlu: fi, theta, psi</returns>
        public double[] ToEulerDegs()
        {
            double fi = Math.Atan2(2 * (W * X + Y * Z), 1 - 2 * (X * X + Y * Y));
            fi = MyMath.Radians2Deg(fi);
            double theta = Math.Asin(2 * (W * Y - Z * X));
            theta = MyMath.Radians2Deg(theta);
            double psi = Math.Atan2(2 * (W * Z + X * Y), 1 - 2 * (Y * Y + Z * Z));
            psi = MyMath.Radians2Deg(psi);
            return new double[] { fi, theta, psi };
        }

        /// <summary>
        /// vynasobi 2 quaterniony
        /// </summary>
        /// <param name="q1">levy quaternion</param>
        /// <param name="q2">pravy quaternion</param>
        /// <returns>soucin</returns>
        public static Quaternion operator* (Quaternion q1, Quaternion q2){
            return Quaternion.Multiply(q1, q2);
        }

        /// <summary>
        /// vynasobi 2 quaterniony
        /// </summary>
        /// <param name="q1">levy quaternion</param>
        /// <param name="q2">pravy quaternion</param>
        /// <returns>soucin</returns>
        public static Quaternion Multiply (Quaternion q1, Quaternion q2){

            double w = q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z;
            double x = q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y;
            double y = q1.W * q2.Y - q1.X * q2.Z + q1.Y * q2.W + q1.Z * q2.X;
            double z = q1.W * q2.Z + q1.X * q2.Y - q1.Y * q2.X + q1.Z * q2.W;
            Quaternion q = new Quaternion(w, x, y, z);
            return q;
        }

        /// <summary>
        /// demonstrace poradi nasobeni z -> y -> x
        /// </summary>
        public static void TestQuaternion1()
        {

            // rotace okolo Z a 90 stupnu:
            double[] degss;
            Vektor p = new Vektor(1.3, 0.6, 2.5);
            Vektor pRot1;
            Matrix3D matrixOwn = Matrix3D.NewRotateByDegrees(0, 0, 90);
            matrixOwn = matrixOwn * Matrix3D.NewRotateByDegrees(0, 40, 0);
            matrixOwn = matrixOwn * Matrix3D.NewRotateByDegrees(10, 0, 0);

            //matrixOwn = Matrix3D.NewRotateByDegrees(10, 0, 0);

            pRot1 = matrixOwn.Transform2NewPoint(p);
            degss = matrixOwn.GetAnglesFromMatrix();

            Quaternion q = new Quaternion(new Vektor(0, 0, 1), 90);
            matrixOwn = q.Matrix();
            Quaternion qq = new Quaternion(new Vektor(0, 1, 0), 40);
            matrixOwn = qq.Matrix() * matrixOwn;
            Quaternion qqq = new Quaternion(new Vektor(1, 0, 0), 10);
            matrixOwn = qqq.Matrix() * matrixOwn;

            //matrixOwn = qqq.Matrix();

            matrixOwn = matrixOwn.Transpose();
            Vektor pRot2 = matrixOwn.Transform2NewPoint(p);
            degss = qq.ToEulerDegs();


        }

        /// <summary>
        /// demonstrace poradi nasobeni x -> y -> z
        /// </summary>
        public void TestQuaternion2()
        {
            // rotace okolo Z a 90 stupnu:
            double[] degss;
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
        }
    }
}
