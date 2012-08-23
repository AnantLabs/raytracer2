using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RayTracerLib;

namespace EditorLib
{
    public class DrawingPlane : DrawingDefaultShape
    {

        /// <summary>
        /// velikost mrizky
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// vzdalenost mezi mrizkami
        /// </summary>
        public float Distance { get; private set; }


        public DrawingPlane(Plane plane)
        {
            this.SetModelObject(plane);
        }
        /// <summary>
        /// Rovina v editoru
        /// TODO: 
        /// --dodelat na nekonecnou rovinu
        /// --dodelat na rovinu zadanou dvema body
        /// </summary>
        /// <param name="size">pocet okynek na jedne strane mrizky</param>
        /// <param name="distance">vzdalenos mezi mrizkami</param>
        /// <param name="xRotDeg">rotace podle x - stupne</param>
        /// <param name="yRotDeg">rotace podle y - stupne</param>
        /// <param name="zRotDeg">rotace podle z - stupne</param>
        /// <param name="leftCorner">levy "dolni" roh roviny</param>
        public DrawingPlane(
            int size, float distance, 
            double xRotDeg, double yRotDeg, double zRotDeg, 
            Point3D leftCorner)
        {
            this.Set(size, distance, xRotDeg, yRotDeg, zRotDeg, leftCorner);
        }

                
        /// <summary>
        /// Rovina v editoru
        /// TODO: 
        /// --dodelat na nekonecnou rovinu
        /// --dodelat na rovinu zadanou dvema body
        /// </summary>
        /// <param name="size">pocet okynek na jedne strane mrizky</param>
        /// <param name="distance">vzdalenos mezi mrizkami</param>
        /// <param name="xRotDeg">rotace podle x - stupne</param>
        /// <param name="yRotDeg">rotace podle y - stupne</param>
        /// <param name="zRotDeg">rotace podle z - stupne</param>
        /// <param name="leftCorner">levy "dolni" roh roviny</param>
        private void Set(
            int size, float distance,
            double xRotDeg, double yRotDeg, double zRotDeg,
            Point3D leftCorner)
        {
            Size = size;
            Distance = distance;
            List<Point3D> points = new List<Point3D>();
            List<Line3D> lines = new List<Line3D>(2 * (Size + 1));
            Line3D line;
            if (leftCorner == null)
                leftCorner = new Point3D(-5, 0, -5);

            // MRIZKA
            for (float i = 0; i <= Size; i += Distance)
            {
                Point3D p1 = new Point3D(leftCorner.X + i, leftCorner.Y, leftCorner.Z);
                Point3D p2 = new Point3D(leftCorner.X + i, leftCorner.Y, leftCorner.Z + Size);
                points.Add(p1);
                points.Add(p2);
                line = new Line3D(p1, p2);
                lines.Add(line);
            }
            for (float i = 0; i <= Size; i += Distance)
            {
                Point3D p1 = new Point3D(leftCorner.X, leftCorner.Y, leftCorner.Z + i);
                Point3D p2 = new Point3D(leftCorner.X + Size, leftCorner.Y, leftCorner.Z + i);
                points.Add(p1);
                points.Add(p2);
                line = new Line3D(p1, p2);
                lines.Add(line);
            }

            this.Points = points.ToArray();
            this.Lines = lines;

            // ROTACE
            Matrix3D matrix = new Matrix3D();
            matrix.SetOnDegrees(xRotDeg, yRotDeg, zRotDeg);
            this.Rotate(matrix);
        }

        public void SetModelObject(RayTracerLib.Plane plane)
        {
            this.ModelObject = plane;
            Point3D leftCorn = new Point3D(plane.Pocatek.X, plane.Pocatek.Y, plane.Pocatek.Z);
            this.Set(10, 1, 10, 20, 30, leftCorn);
        }
    }
}
