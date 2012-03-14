using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracerLib
{
    public class MatrixRayTr
    {
        public double[][] Matrix;

        public static MatrixRayTr Identity
        {
            get
            {
                return new MatrixRayTr(new Vektor(1, 0, 0), new Vektor(0, 1, 0), new Vektor(0, 0, 1));
            }
        }

        public MatrixRayTr()
        {
            Matrix = new double[3][];
            Matrix[0] = new double[3] { 1, 0, 0 };
            Matrix[1] = new double[3] { 0, 1, 0 };
            Matrix[2] = new double[3] { 0, 0, 1 };
        }

        public MatrixRayTr(Vektor row1, Vektor row2, Vektor row3)
        {
            Matrix = new double[3][];
            Matrix[0] = row1.GetArray();
            Matrix[1] = row2.GetArray();
            Matrix[2] = row3.GetArray();
        }

        public static Vektor operator *(MatrixRayTr matrix, Vektor vec)
        {

            double x = vec.X * matrix.Matrix[0][0] +
                    vec.Y * matrix.Matrix[0][1] +
                    vec.Z * matrix.Matrix[0][2];

            double y = vec.X * matrix.Matrix[1][0] +
                    vec.Y * matrix.Matrix[1][1] +
                    vec.Z * matrix.Matrix[1][2];

            double z = vec.X * matrix.Matrix[2][0] +
                    vec.Y * matrix.Matrix[2][1] +
                    vec.Z * matrix.Matrix[2][2];

            Vektor newVec = new Vektor(x, y, z);
            return newVec;
        }

        public static MatrixRayTr operator *(MatrixRayTr m1, MatrixRayTr m2)
        {
            MatrixRayTr matrix = new MatrixRayTr();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    matrix.Matrix[i][j] =
                        (m2.Matrix[i][0] * m1.Matrix[0][j]) +
                        (m2.Matrix[i][1] * m1.Matrix[1][j]) +
                        (m2.Matrix[i][2] * m1.Matrix[2][j]);
                }
            }
            return matrix;
        }

        public MatrixRayTr Transpose()
        {
            MatrixRayTr transp = new MatrixRayTr();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    transp.Matrix[i][j] = this.Matrix[j][i];
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
        public static MatrixRayTr NewRotateAroundX(double radians)
        {
            MatrixRayTr matrix = new MatrixRayTr();
            matrix.Matrix[1][1] = Math.Cos(radians);
            matrix.Matrix[1][2] = Math.Sin(radians);
            matrix.Matrix[2][1] = -(Math.Sin(radians));
            matrix.Matrix[2][2] = Math.Cos(radians);
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
        public static MatrixRayTr NewRotateAroundY(double radians)
        {
            MatrixRayTr matrix = new MatrixRayTr();
            matrix.Matrix[0][0] = Math.Cos(radians);
            matrix.Matrix[0][2] = -(Math.Sin(radians));
            matrix.Matrix[2][0] = Math.Sin(radians);
            matrix.Matrix[2][2] = Math.Cos(radians);
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
        public static MatrixRayTr NewRotateAroundZ(double radians)
        {
            MatrixRayTr matrix = new MatrixRayTr();
            matrix.Matrix[0][0] = Math.Cos(radians);
            matrix.Matrix[0][1] = Math.Sin(radians);
            matrix.Matrix[1][0] = -(Math.Sin(radians));
            matrix.Matrix[1][1] = Math.Cos(radians);
            return matrix;
        }

        public static MatrixRayTr NewRotateByRads(double radiansX, double radiansY, double radiansZ)
        {
            MatrixRayTr matrix = NewRotateAroundX(radiansX);
            matrix = matrix * NewRotateAroundY(radiansY);
            matrix = matrix * NewRotateAroundZ(radiansZ);
            return matrix;
        }

        public static MatrixRayTr NewRotateByDegrees(double degreesX, double degreesY, double degreesZ)
        {
            return NewRotateByRads(
                        MyMath.Degs2Rad(degreesX),
                        MyMath.Degs2Rad(degreesY),
                        MyMath.Degs2Rad(degreesZ)
                   );
        }

        public void SetOnDegrees(double degX, double degY, double degZ)
        {
            MatrixRayTr transformMatrix = NewRotateByDegrees(degX, degY, degZ);
            this.Matrix = transformMatrix.Matrix;
        }

        public Vektor Transform2NewPoint(Vektor point)
        {
            Vektor newPoint = this * point;
            return newPoint;
        }

        public void TransformPoint(Vektor point)
        {
            Vektor transformed = this * point;
            point.X = transformed.X;
            point.Y = transformed.Y;
            point.Z = transformed.Z;
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
            int i = 0;
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
    }
}
