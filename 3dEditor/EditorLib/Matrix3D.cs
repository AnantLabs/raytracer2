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
            return new Matrix3D(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));
            }
        }

        public double[,] Matrix = new double[3, 3];

        public Matrix3D()
        {
            this.Matrix = this.MakeIdentity().Matrix;
        }
        public Matrix3D(Point3D row1, Point3D row2, Point3D row3)
        {
            Matrix[0, 0] = row1.X; Matrix[0, 1] = row1.Y; Matrix[0, 2] = row1.Z;
            Matrix[1, 0] = row2.X; Matrix[1, 1] = row2.Y; Matrix[1, 2] = row2.Z;
            Matrix[2, 0] = row3.X; Matrix[2, 1] = row3.Y; Matrix[2, 2] = row3.Z;
        }

        public static Point3D operator *(Matrix3D matrix, Point3D point3D)
        {
            
            double x = point3D.X * matrix.Matrix[0, 0] +
                    point3D.Y * matrix.Matrix[0, 1] +
                    point3D.Z * matrix.Matrix[0, 2];

            double y = point3D.X * matrix.Matrix[1, 0] +
                    point3D.Y * matrix.Matrix[1, 1] +
                    point3D.Z * matrix.Matrix[1, 2];

            double z = point3D.X * matrix.Matrix[2, 0] +
                    point3D.Y * matrix.Matrix[2, 1] +
                    point3D.Z * matrix.Matrix[2, 2];

            Point3D newP3d = new Point3D(x, y, z);
            return newP3d;
        }

        public static Matrix3D operator *(Matrix3D m1, Matrix3D m2)
        {
            Matrix3D matrix = new Matrix3D();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    matrix.Matrix[i, j] =
                        (m2.Matrix[i, 0] * m1.Matrix[0, j]) +
                        (m2.Matrix[i, 1] * m1.Matrix[1, j]) +
                        (m2.Matrix[i, 2] * m1.Matrix[2, j]);
                }
            }
            return matrix;
        }

        public Matrix3D MakeIdentity()
        {
            return Matrix3D.Identity;
        }
        public static Matrix3D NewRotateAroundX(double radians)
        {
            Matrix3D matrix = new Matrix3D();
            matrix.Matrix[1, 1] = Math.Cos(radians);
            matrix.Matrix[1, 2] = Math.Sin(radians);
            matrix.Matrix[2, 1] = -(Math.Sin(radians));
            matrix.Matrix[2, 2] = Math.Cos(radians);
            return matrix;
        }

        public static Matrix3D NewRotateAroundY(double radians)
        {
            Matrix3D matrix = new Matrix3D();
            matrix.Matrix[0, 0] = Math.Cos(radians);
            matrix.Matrix[0, 2] = -(Math.Sin(radians));
            matrix.Matrix[2, 0] = Math.Sin(radians);
            matrix.Matrix[2, 2] = Math.Cos(radians);
            return matrix;
        }

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
        
    }
}
