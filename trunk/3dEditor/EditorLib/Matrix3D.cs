using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorLib
{
    public class Matrix3D
    {
        public static Matrix3D Identity
        { get {
            return new Matrix3D(
                new Point3D(1, 0, 0, 0),
                new Point3D(0, 1, 0, 0),
                new Point3D(0, 0, 1, 0),
                new Point3D(0, 0, 0, 1));
            }
        }

        public double[,] Matrix = new double[4, 4];

        public Matrix3D()
        {
            this.Matrix = this.MakeIdentity().Matrix;
        }
        public Matrix3D(Point3D row1, Point3D row2, Point3D row3) : this(row1, row2, row3, new Point3D(0, 0, 0, 1)) { }
        public Matrix3D(Point3D row1, Point3D row2, Point3D row3, Point3D row4)
        {
            Matrix[0, 0] = row1.X; Matrix[0, 1] = row1.Y; Matrix[0, 2] = row1.Z; Matrix[0, 3] = row1.ZZ;
            Matrix[1, 0] = row2.X; Matrix[1, 1] = row2.Y; Matrix[1, 2] = row2.Z; Matrix[1, 3] = row2.ZZ;
            Matrix[2, 0] = row3.X; Matrix[2, 1] = row3.Y; Matrix[2, 2] = row3.Z; Matrix[2, 3] = row3.ZZ;
            Matrix[3, 0] = row4.X; Matrix[3, 1] = row4.Y; Matrix[3, 2] = row4.Z; Matrix[3, 3] = row4.ZZ;
        }

        public static Point3D operator *(Matrix3D matrix, Point3D point3D)
        {

            double x = point3D.X * matrix.Matrix[0, 0] +
                    point3D.Y * matrix.Matrix[0, 1] +
                    point3D.Z * matrix.Matrix[0, 2] +
                    point3D.ZZ * matrix.Matrix[0, 3];


            double y = point3D.X * matrix.Matrix[1, 0] +
                    point3D.Y * matrix.Matrix[1, 1] +
                    point3D.Z * matrix.Matrix[1, 2] +
                    point3D.ZZ * matrix.Matrix[1, 3];

            double z = point3D.X * matrix.Matrix[2, 0] +
                    point3D.Y * matrix.Matrix[2, 1] +
                    point3D.Z * matrix.Matrix[2, 2] +
                    point3D.ZZ * matrix.Matrix[2, 3];

            double zz = point3D.X * matrix.Matrix[3, 0] +
                    point3D.Y * matrix.Matrix[3, 1] +
                    point3D.Z * matrix.Matrix[3, 2] +
                    point3D.ZZ * matrix.Matrix[3, 3];

            Point3D newP3d = new Point3D(x, y, z, zz);
            return newP3d;
        }

        public static Matrix3D operator *(Matrix3D m1, Matrix3D m2)
        {
            Matrix3D matrix = new Matrix3D();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrix.Matrix[i, j] =
                        (m2.Matrix[i, 0] * m1.Matrix[0, j]) +
                        (m2.Matrix[i, 1] * m1.Matrix[1, j]) +
                        (m2.Matrix[i, 2] * m1.Matrix[2, j]) +
                        (m2.Matrix[i, 3] * m1.Matrix[3, j]);
                }
            }
            return matrix;
        }

        public Matrix3D MakeIdentity()
        {
            return Matrix3D.Identity;
        }

        public Matrix3D Transpose()
        {
            Matrix3D transp = new Matrix3D();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    transp.Matrix[i, j] = this.Matrix[j, i];
                }
            }
            return transp;
        }

        /// <summary>
        /// matice:
        /// 1  0         0
        /// 0  cos(rad)  sin(rad)
        /// 0  -sin(rad) cos(rad)
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Matrix3D NewRotateAroundX(double radians)
        {
            Matrix3D matrix = new Matrix3D();
            matrix.Matrix[1, 1] = Math.Cos(radians);
            matrix.Matrix[1, 2] = Math.Sin(radians);
            matrix.Matrix[2, 1] = -(Math.Sin(radians));
            matrix.Matrix[2, 2] = Math.Cos(radians);
            return matrix;
        }

        /// <summary>
        /// matice:
        /// cos(rad)  0  -sin(rad)
        /// 0         1  0
        /// sin(rad)  0  cos(rad)
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Matrix3D NewRotateAroundY(double radians)
        {
            Matrix3D matrix = new Matrix3D();
            matrix.Matrix[0, 0] = Math.Cos(radians);
            matrix.Matrix[0, 2] = -(Math.Sin(radians));
            matrix.Matrix[2, 0] = Math.Sin(radians);
            matrix.Matrix[2, 2] = Math.Cos(radians);
            return matrix;
        }

        /// <summary>
        /// matice:
        /// cos(rad)  sin(rad) 0
        /// -sin(rad) cos(rad) 0
        /// 0         0        1 
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Matrix3D NewRotateAroundZ(double radians)
        {
            Matrix3D matrix = new Matrix3D();
            matrix.Matrix[0, 0] = Math.Cos(radians);
            matrix.Matrix[0, 1] = Math.Sin(radians);
            matrix.Matrix[1, 0] = -(Math.Sin(radians));
            matrix.Matrix[1, 1] = Math.Cos(radians);
            return matrix;
        }

        public static Matrix3D NewRotateByRads(double radiansX, double radiansY, double radiansZ)
        {
            Matrix3D matrix = NewRotateAroundX(radiansX);
            matrix = matrix * NewRotateAroundY(radiansY);
            matrix = matrix * NewRotateAroundZ(radiansZ);
            return matrix;
        }

        public static Matrix3D NewRotateByDegrees(double degreesX, double degreesY, double degreesZ)
        {
            return NewRotateByRads(
                        EditorMath.Degrees2Rad(degreesX),
                        EditorMath.Degrees2Rad(degreesY),
                        EditorMath.Degrees2Rad(degreesZ)
                   );
        }


        public void SetOnDegrees(double degX, double degY, double degZ)
        {
            Matrix3D transformMatrix = NewRotateByDegrees(degX, degY, degZ);
            this.Matrix = transformMatrix.Matrix;
        }

        public Point3D Transform2NewPoint(Point3D point)
        {
            Point3D newPoint = this * point;
            return newPoint;
        }

        public void TransformPoint(Point3D point)
        {
            point.MultiplyByMatrix(this);
        }

        /// <summary>
        /// Prevede seznam bodu pres transformacni matici <code>Matrix</code>
        /// </summary>
        /// <param name="points">seznam bodu</param>
        /// <returns>novy seznam transformovanych bodu</returns>
        public Point3D[] Transform2NewPoints(Point3D[] points)
        {
            if (points == null)
                throw new ArgumentNullException();

            Point3D[] newPoints = new Point3D[points.Length];
            int i =0;
            foreach (Point3D p in points)
            {
                newPoints[i] = this.Transform2NewPoint(p);
                i++;
            }
            return newPoints;
        }
        public void TransformPoints(Point3D[] points)
        {
            if (points == null)
                throw new ArgumentNullException();

            foreach (Point3D p in points)
            {
                this.TransformPoint(p);
            }
        }

        /// <summary>
        /// Prevede seznam bodu pres transformacni matici <code>Matrix</code>
        /// </summary>
        /// <param name="points">seznam bodu</param>
        /// <returns>novy seznam transformovanych bodu</returns>
        public List<Point3D> Transform2NewPoints(List<Point3D> points)
        {
            Point3D[] newPoints = points.ToArray();
            newPoints = Transform2NewPoints(newPoints);
            List<Point3D> newPointList = new List<Point3D>(newPoints);
            return newPointList;
        }
        public void TransformPoints(List<Point3D> points)
        {
            foreach (Point3D p in points)
            {
                this.TransformPoint(p);
            }
        }

        public Line3D Transform2NewLine(Line3D line)
        {
            Point3D a = Transform2NewPoint(line.A);
            Point3D b = Transform2NewPoint(line.B);
            return new Line3D(a, b);
        }
        public void TransformLine(Line3D line)
        {
            this.TransformPoint(line.A);
            this.TransformPoint(line.B);
        }

        public List<Line3D> Transform2NewLines(List<Line3D> lines)
        {
            List<Line3D> newLines = new List<Line3D>(lines.Count);
            foreach (Line3D l in lines)
            {
                Line3D newLine = Transform2NewLine(l);
                newLines.Add(newLine);
            }
            return newLines;
        }
        public void TransformLines(List<Line3D> lines)
        {
            foreach (Line3D l in lines)
            {
                this.TransformLine(l);
            }
        }

        /// OPERACE POSUNUTI - TRANSLACE:
        /// vektor posunuti: [Px, Py, Pz]
        /// matice:
        /// 1 0 0 Px
        /// 0 1 0 Py
        /// 0 0 1 Pz
        public static Point3D Posunuti(Point3D point3D, double Px, double Py, double Pz)
        {
            Matrix3D matrix = new Matrix3D(
                new Point3D(1, 0, 0, Px),
                new Point3D(0, 1, 0, Py),
                new Point3D(0, 0, 1, Pz));

            double x = point3D.X * matrix.Matrix[0, 0] +
                    //point3D.Y * matrix.Matrix[0, 1] +
                    //point3D.Z * matrix.Matrix[0, 2] +
                    point3D.ZZ * matrix.Matrix[0, 3];

            double y = //point3D.X * matrix.Matrix[1, 0] +
                    point3D.Y * matrix.Matrix[1, 1] +
                    //point3D.Z * matrix.Matrix[1, 2] +
                    point3D.ZZ * matrix.Matrix[1, 3];

            double z = //point3D.X * matrix.Matrix[2, 0] +
                    //point3D.Y * matrix.Matrix[2, 1] +
                    point3D.Z * matrix.Matrix[2, 2] +
                    point3D.ZZ * matrix.Matrix[2, 3];

            Point3D newP3d = new Point3D(x, y, z);
            return newP3d;
        }


        /// <summary>OPERACE POSUNUTI - TRANSLACE:
        /// vektor posunuti: [Px, Py, Pz]
        /// matice je na leve strance operace nasobeni;
        /// matice * BOD = posunutyBOD
        /// 1 0 0 Px
        /// 0 1 0 Py
        /// 0 0 1 Pz
        /// 0 0 0 1
        /// </summary>
        public static Matrix3D PosunutiNewMatrix(double Px, double Py, double Pz)
        {
            Matrix3D matrix =new Matrix3D();
            //matrix = new Matrix3D(
            //    new Point3D(1, 0, 0),
            //    new Point3D(0, 1, 0),
            //    new Point3D(Px, Py, Pz));

            matrix = new Matrix3D(
                new Point3D(1, 0, 0, Px),
                new Point3D(0, 1, 0, Py),
                new Point3D(0, 0, 1, Pz),
                new Point3D(0, 0, 0, 1));

            return matrix;
        }

        /// SCALING / zmena meritka
        /// matice:
        /// Sx 0 0
        /// 0 Sy 0
        /// 0 0 Sz
        public static Matrix3D ScalingNewMatrix(double Sx, double Sy, double Sz)
        {
            Matrix3D matrix = new Matrix3D(
                new Point3D(Sx, 0, 0, 0),
                new Point3D(0, Sy, 0, 0),
                new Point3D(0, 0, Sz, 0),
                new Point3D(0, 0, 0, 1));

            return matrix;
        }

        /// <summary>
        /// testovani, ze transponovanim rotacni matice obdrzime inverzni matici
        /// nefunguje to obecne, jen pri aplikaci rotacnich operaci
        /// </summary>
        public static bool TestTranspose()
        {
            int count = 30;
            Random rnd = new Random();
            Point3D[] points1 = new Point3D[count];
            Point3D[] points2 = new Point3D[count];
            double n1, n2, n3;
            // naplnime 2 pole stejnymi cisly
            for (int i = 0; i < 30; i++)
            {
                n1 = (double)(rnd.Next(100) / 100.0);
                n2 = (double)(rnd.Next(100) / 100.0);
                n3 = (double)(rnd.Next(100) / 100.0);
                points1[i] = new Point3D(n1, n2, n3);
                points1[i].Normalize();
                points2[i] = new Point3D(points1[i]);
            }

            Matrix3D m1 = Matrix3D.NewRotateByDegrees(10, 20, 30);
            
            Matrix3D m2 = m1.Transpose();

            // aplikujeme rotacni matici na prvni pole points1
            m1.TransformPoints(points1);
            // aplikujeme zpet transponovanou matici
            m2.TransformPoints(points1);

            // melo by platit: points1 == points2
            for (int i = 0; i < count; i++)
            {
                if (Math.Round(points1[i].X, 2) != Math.Round(points2[i].X, 2) ||
                    Math.Round(points1[i].Y,2) != Math.Round(points2[i].Y,2) ||
                    Math.Round(points1[i].Z,2) != Math.Round(points2[i].Z,2))
                    return false;
            }

            return true;
        }

        public static void TestSkladaniTransformaci()
        {
            Matrix3D mShift = Matrix3D.PosunutiNewMatrix(1, 2, 3);
            Matrix3D mRot = Matrix3D.NewRotateByDegrees(45, 0, 0);
            Matrix3D mRotCorrect = mRot * mShift;       // spravne skladani
            Matrix3D mRot2 = mShift * mRot;
            Matrix3D mRot3 = mShift * mShift;
            Point3D pRight = new Point3D(10, 20, 30);
            Point3D pShitf = new Point3D(10, 20, 30);
            Point3D pRot = new Point3D(10, 20, 30);
            Point3D pRot1 = new Point3D(10, 20, 30);
            Point3D pRot2 = new Point3D(10, 20, 30);
            Point3D pRot3 = new Point3D(10, 20, 30);

            mRot.TransformPoint(pRight);
            mShift.TransformPoint(pRight);

            mShift.TransformPoint(pShitf);
            mRot.TransformPoint(pShitf);

            mRotCorrect.TransformPoint(pRot1);
            mRot2.TransformPoint(pRot2);
            mRot3.TransformPoint(pRot3);
        }
    }
}
