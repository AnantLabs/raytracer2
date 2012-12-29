using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mathematics
{
public class Matrix3D
    {
        public static Matrix3D Identity
        { get {
            return new Matrix3D(
                new Vektor(1, 0, 0, 0),
                new Vektor(0, 1, 0, 0),
                new Vektor(0, 0, 1, 0),
                new Vektor(0, 0, 0, 1));
            }
        }

        public double[,] Matrix = new double[4, 4];

        public Matrix3D()
        {
            this.Matrix = this.MakeIdentity().Matrix;
        }
        public Matrix3D(Vektor row1, Vektor row2, Vektor row3) : this(row1, row2, row3, new Vektor(0, 0, 0, 1)) { }
        public Matrix3D(Vektor row1, Vektor row2, Vektor row3, Vektor row4)
        {
            Matrix[0, 0] = row1.X; Matrix[0, 1] = row1.Y; Matrix[0, 2] = row1.Z; Matrix[0, 3] = row1.ZZ;
            Matrix[1, 0] = row2.X; Matrix[1, 1] = row2.Y; Matrix[1, 2] = row2.Z; Matrix[1, 3] = row2.ZZ;
            Matrix[2, 0] = row3.X; Matrix[2, 1] = row3.Y; Matrix[2, 2] = row3.Z; Matrix[2, 3] = row3.ZZ;
            Matrix[3, 0] = row4.X; Matrix[3, 1] = row4.Y; Matrix[3, 2] = row4.Z; Matrix[3, 3] = row4.ZZ;
        }

        public void Set(Vektor row1, Vektor row2, Vektor row3)
        {
            Matrix[0, 0] = row1.X; Matrix[0, 1] = row1.Y; Matrix[0, 2] = row1.Z;
            Matrix[1, 0] = row2.X; Matrix[1, 1] = row2.Y; Matrix[1, 2] = row2.Z;
            Matrix[2, 0] = row3.X; Matrix[2, 1] = row3.Y; Matrix[2, 2] = row3.Z;
        }

        public static Vektor operator *(Matrix3D matrix, Vektor Vektor)
        {

            double x = Vektor.X * matrix.Matrix[0, 0] +
                    Vektor.Y * matrix.Matrix[0, 1] +
                    Vektor.Z * matrix.Matrix[0, 2] +
                    Vektor.ZZ * matrix.Matrix[0, 3];


            double y = Vektor.X * matrix.Matrix[1, 0] +
                    Vektor.Y * matrix.Matrix[1, 1] +
                    Vektor.Z * matrix.Matrix[1, 2] +
                    Vektor.ZZ * matrix.Matrix[1, 3];

            double z = Vektor.X * matrix.Matrix[2, 0] +
                    Vektor.Y * matrix.Matrix[2, 1] +
                    Vektor.Z * matrix.Matrix[2, 2] +
                    Vektor.ZZ * matrix.Matrix[2, 3];

            double zz = Vektor.X * matrix.Matrix[3, 0] +
                    Vektor.Y * matrix.Matrix[3, 1] +
                    Vektor.Z * matrix.Matrix[3, 2] +
                    Vektor.ZZ * matrix.Matrix[3, 3];

            Vektor newP3d = new Vektor(x, y, z, zz);
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
                        MyMath.Degrees2Rad(degreesX),
                        MyMath.Degrees2Rad(degreesY),
                        MyMath.Degrees2Rad(degreesZ)
                   );
        }

        public static Matrix3D NewRotateByAxis_Degs(Vektor axis, double theta)
        {
            Vektor ax = new Vektor(axis);
            ax.Normalize();
            double a = ax.X;
            double b = ax.Y;
            double c = ax.Z;
            double sin = Math.Sin(-theta);
            double cos = Math.Cos(theta);

            double k = 1 - cos;
            Vektor row1 = new Vektor();
            row1[0] = a * a * k + cos;
            row1[1] = a * b * k - c * sin;
            row1[2] = a * c * k + b * sin;
            row1[3] = 0;

            Vektor row2 = new Vektor();
            row2[0] = a * b * k - c * sin;
            row2[1] = b * b * k + cos;
            row2[2] = b * c * k - a * sin;
            row2[3] = 0;

            Vektor row3 = new Vektor();
            row3[0] = a * c * k - b * sin;
            row3[1] = b * c * k + a * sin;
            row3[2] = c * c * k + cos;
            row3[3] = 0;

            Vektor row4 = new Vektor(0, 0, 0, 1);
            Matrix3D matrix = new Matrix3D(row1, row2, row3, row4);

            return matrix;
        }



        public void SetOnDegrees(double degX, double degY, double degZ)
        {
            Matrix3D transformMatrix = NewRotateByDegrees(degX, degY, degZ);
            this.Matrix = transformMatrix.Matrix;
        }

        public Vektor Transform2NewPoint(Vektor point)
        {
            Vektor newPoint = this * point;
            return newPoint;
        }

        public void TransformPoint(Vektor point)
        {
            point.MultiplyByMatrix(this);
        }

        /// <summary>
        /// Prevede seznam bodu pres transformacni matici <code>Matrix</code>
        /// </summary>
        /// <param name="points">seznam bodu</param>
        /// <returns>novy seznam transformovanych bodu</returns>
        public Vektor[] Transform2NewPoints(Vektor[] points)
        {
            if (points == null)
                throw new ArgumentNullException();

            Vektor[] newPoints = new Vektor[points.Length];
            int i =0;
            foreach (Vektor p in points)
            {
                newPoints[i] = this.Transform2NewPoint(p);
                i++;
            }
            return newPoints;
        }
        public void TransformPoints(Vektor[] points)
        {
            if (points == null)
                throw new ArgumentNullException();

            foreach (Vektor p in points)
            {
                this.TransformPoint(p);
            }
        }

        /// <summary>
        /// Prevede seznam bodu pres transformacni matici <code>Matrix</code>
        /// </summary>
        /// <param name="points">seznam bodu</param>
        /// <returns>novy seznam transformovanych bodu</returns>
        public List<Vektor> Transform2NewPoints(List<Vektor> points)
        {
            Vektor[] newPoints = points.ToArray();
            newPoints = Transform2NewPoints(newPoints);
            List<Vektor> newPointList = new List<Vektor>(newPoints);
            return newPointList;
        }
        public void TransformPoints(List<Vektor> points)
        {
            foreach (Vektor p in points)
            {
                this.TransformPoint(p);
            }
        }

        public Line3D Transform2NewLine(Line3D line)
        {
            Vektor a = Transform2NewPoint(line.A);
            Vektor b = Transform2NewPoint(line.B);
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

        public void NormalizeRows()
        {
            Vektor row1 = new Vektor(Matrix[0, 0], Matrix[0, 1], Matrix[0, 2]);
            row1.Normalize();
            Vektor row2 = new Vektor(Matrix[1, 0], Matrix[1, 1], Matrix[1, 2]);
            row2.Normalize();
            Vektor row3 = new Vektor(Matrix[2, 0], Matrix[2, 1], Matrix[2, 2]);
            row3.Normalize();
            this.Set(row1, row2, row3);
        }
        public void NormalizeCols()
        {
            Matrix3D mtransp = this.Transpose();
            mtransp.NormalizeRows();

            Vektor row1 = new Vektor(mtransp.Matrix[0, 0], mtransp.Matrix[0, 1], mtransp.Matrix[0, 2]);
            Vektor row2 = new Vektor(mtransp.Matrix[1, 0], mtransp.Matrix[1, 1], mtransp.Matrix[1, 2]);
            Vektor row3 = new Vektor(mtransp.Matrix[2, 0], mtransp.Matrix[2, 1], mtransp.Matrix[2, 2]);
            this.Set(row1, row2, row3);
        }

        /// OPERACE POSUNUTI - TRANSLACE:
        /// vektor posunuti: [Px, Py, Pz]
        /// matice:
        /// 1 0 0 Px
        /// 0 1 0 Py
        /// 0 0 1 Pz
        public static Vektor Posunuti(Vektor Vektor, double Px, double Py, double Pz)
        {
            Matrix3D matrix = new Matrix3D(
                new Vektor(1, 0, 0, Px),
                new Vektor(0, 1, 0, Py),
                new Vektor(0, 0, 1, Pz));

            double x = Vektor.X * matrix.Matrix[0, 0] +
                    //Vektor.Y * matrix.Matrix[0, 1] +
                    //Vektor.Z * matrix.Matrix[0, 2] +
                    Vektor.ZZ * matrix.Matrix[0, 3];

            double y = //Vektor.X * matrix.Matrix[1, 0] +
                    Vektor.Y * matrix.Matrix[1, 1] +
                    //Vektor.Z * matrix.Matrix[1, 2] +
                    Vektor.ZZ * matrix.Matrix[1, 3];

            double z = //Vektor.X * matrix.Matrix[2, 0] +
                    //Vektor.Y * matrix.Matrix[2, 1] +
                    Vektor.Z * matrix.Matrix[2, 2] +
                    Vektor.ZZ * matrix.Matrix[2, 3];

            Vektor newP3d = new Vektor(x, y, z);
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
            //    new Vektor(1, 0, 0),
            //    new Vektor(0, 1, 0),
            //    new Vektor(Px, Py, Pz));

            matrix = new Matrix3D(
                new Vektor(1, 0, 0, Px),
                new Vektor(0, 1, 0, Py),
                new Vektor(0, 0, 1, Pz),
                new Vektor(0, 0, 0, 1));

            return matrix;
        }
        public static Matrix3D PosunutiNewMatrix(Vektor shift)
        {
            Matrix3D matrix = new Matrix3D();
            //matrix = new Matrix3D(
            //    new Vektor(1, 0, 0),
            //    new Vektor(0, 1, 0),
            //    new Vektor(Px, Py, Pz));

            matrix = new Matrix3D(
                new Vektor(1, 0, 0, shift[0]),
                new Vektor(0, 1, 0, shift[1]),
                new Vektor(0, 0, 1, shift[2]),
                new Vektor(0, 0, 0, 1));

            return matrix;
        }

        public Matrix3D GetOppositeShiftMatrix()
        {
            Matrix3D matrix = Matrix3D.Identity;
            matrix.Matrix[0, 3] = -this.Matrix[0, 3];
            matrix.Matrix[1, 3] = -this.Matrix[1, 3];
            matrix.Matrix[2, 3] = -this.Matrix[2, 3];
            matrix.Matrix[3, 3] = 1;
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
                new Vektor(Sx, 0, 0, 0),
                new Vektor(0, Sy, 0, 0),
                new Vektor(0, 0, Sz, 0),
                new Vektor(0, 0, 0, 1));

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
            Vektor[] points1 = new Vektor[count];
            Vektor[] points2 = new Vektor[count];
            double n1, n2, n3;
            // naplnime 2 pole stejnymi cisly
            for (int i = 0; i < 30; i++)
            {
                n1 = (double)(rnd.Next(100) / 100.0);
                n2 = (double)(rnd.Next(100) / 100.0);
                n3 = (double)(rnd.Next(100) / 100.0);
                points1[i] = new Vektor(n1, n2, n3);
                points1[i].Normalize();
                points2[i] = new Vektor(points1[i]);
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
            Vektor pRight = new Vektor(10, 20, 30);
            Vektor pShitf = new Vektor(10, 20, 30);
            Vektor pRot = new Vektor(10, 20, 30);
            Vektor pRot1 = new Vektor(10, 20, 30);
            Vektor pRot2 = new Vektor(10, 20, 30);
            Vektor pRot3 = new Vektor(10, 20, 30);

            mRot.TransformPoint(pRight);
            mShift.TransformPoint(pRight);

            mShift.TransformPoint(pShitf);
            mRot.TransformPoint(pShitf);

            mRotCorrect.TransformPoint(pRot1);
            mRot2.TransformPoint(pRot2);
            mRot3.TransformPoint(pRot3);
        }


        /// <summary>
        /// Z rotacni matice vrati seznam uhlu, pres ktere lze matici vypocitat
        /// Uhlu je v principu vzdy vice trojic
        /// </summary>
        /// <returns>vrati pole uhlu: [0] = uhel okolo osy X. [1] = Y, [2] = Z</returns>
        public double[] GetAnglesFromMatrix()
        {
            double[] angles1 = new double[3];
            double[] angles2 = new double[3];

            double theta1, theta2;  // rotace kolem X
            double psi1, psi2;      // rotace kolem Y
            double fi1,fi2;         //rotace kolem Z

            if (Matrix[2, 0] != -1 && Matrix[2, 0] != 1)
            {
                theta1 = -Math.Asin(Matrix[2, 0]);
                theta2 = Math.PI - theta1;
                psi1 = Math.Atan2(Matrix[2, 1] / Math.Cos(theta1), Matrix[2, 2] / Math.Cos(theta1));
                psi2 = Math.Atan2(Matrix[2, 1] / Math.Cos(theta2), Matrix[2, 2] / Math.Cos(theta2));
                fi1 = Math.Atan2(Matrix[1, 0] / Math.Cos(theta1), Matrix[0, 0] / Math.Cos(theta1));
                fi2 = Math.Atan2(Matrix[1, 0] / Math.Cos(theta2), Matrix[0, 0] / Math.Cos(theta2));

                angles1[0] = MyMath.Radians2Deg(-psi1);
                angles1[1] = MyMath.Radians2Deg(-theta1);
                angles1[2] = MyMath.Radians2Deg(-fi1);

                angles2[0] = MyMath.Radians2Deg(-psi2);
                angles2[1] = MyMath.Radians2Deg(-theta2);
                angles2[2] = MyMath.Radians2Deg(-fi2);
            }
            else
            {
                fi1 = 0;
                if (Matrix[2, 0] == -1)
                {
                    theta1 = Math.PI / 2;
                    psi1 = fi1 + Math.Atan2(Matrix[0, 1], Matrix[0, 2]);
                }
                else
                {
                    theta1 = -Math.PI / 2;
                    psi1 = -fi1 + Math.Atan2(-Matrix[0, 1], -Matrix[0, 2]);
                }
                angles1[0] = MyMath.Radians2Deg(-psi1);
                angles1[1] = MyMath.Radians2Deg(-theta1);
                angles1[2] = MyMath.Radians2Deg(-fi1);
            }
            for (int i = 0; i < angles1.Length; i++)
            {
                angles1[i] = Math.Round(angles1[i], 0);
            }
            return angles1;
        }

        public static bool TestMatrixAnglesBack(double angX, double angY, double angZ)
        {
            angX = angX % 180;
            angY = angY % 180;
            angZ = angZ % 180;
            double[] angles = new double[] { angX, angY, angZ };
            Vektor point = new Vektor(4, 2, 3);
            Matrix3D m1 = Matrix3D.NewRotateByDegrees(angX, angY, angZ);
            Vektor point1 = m1.Transform2NewPoint(point);
            double[] angles1 = m1.GetAnglesFromMatrix();
            

            Matrix3D m2 = Matrix3D.NewRotateByDegrees(angles1[0], angles1[1], angles1[2]);
            Vektor point2 = m2.Transform2NewPoint(point);
            double[] angles2 = m2.GetAnglesFromMatrix();

            Matrix3D m3 = Matrix3D.NewRotateByDegrees(angles2[0], angles2[1], angles2[2]);
            Vektor point3 = m3.Transform2NewPoint(point);
            double[] angles3 = m3.GetAnglesFromMatrix();

            return true;
        }
    }
}
