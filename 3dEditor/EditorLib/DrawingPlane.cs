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

        const int SIZE = 10;
        const float DISTANCE = 1;

        public DrawingPlane(Plane plane) : this(SIZE, DISTANCE, 0, 0, 0, plane) { }
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
            Plane plane)
        {
            _RotatMatrix = Matrix3D.NewRotateByDegrees(xRotDeg, yRotDeg, zRotDeg);
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(plane.Pocatek.X, plane.Pocatek.Y, plane.Pocatek.Z);
            this.ModelObject = plane;
            Size = size > 0 ? size : SIZE;
            Distance = distance > 0 ? distance : DISTANCE;
            this.Set();
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
        private void Set()
        {
            Plane plane = (Plane)ModelObject;
            Point3D leftCorner = new Point3D(plane.Pocatek.X, plane.Pocatek.Y, plane.Pocatek.Z);

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

            _localMatrix = _RotatMatrix * _ShiftMatrix;
            _localMatrix.TransformPoints(Points);
        }

        public override void SetModelObject(object modelObject)
        {
            if (modelObject.GetType() == typeof(Plane))
                this.SetModelObject((Plane)modelObject);
        }

        public void SetModelObject(RayTracerLib.Plane plane, int size, float distance)
        {
            this.ModelObject = plane;
            Size = size > 0 ? size : SIZE;
            Distance = distance > 0 ? distance : DISTANCE;
            this.Set();
        }

        public void SetModelObject(RayTracerLib.Plane plane)
        {
            this.SetModelObject(plane, Size, Distance);
        }

        //public void RotatePlane(double x, double y, double z)
        //{
        //    Matrix3D newRot = Matrix3D.NewRotateByDegrees(x, y, z);
        //    Matrix3D transpLoc = _localMatrix.Transpose();

        //    transpLoc.TransformPoints(Points);

        //    this._RotatMatrix = newRot;
        //    this.SetModelObject(this.ModelObject);
        //}

        public override Point3D GetCenter()
        {
            return new Point3D(-10, -10, 10);
        }
    }
}
