using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RayTracerLib;
using Mathematics;

namespace EditorLib
{
    public class DrawingPlane : DrawingDefaultShape
    {

        public Vektor Center
        {
            get
            {
                return Points[0];
            }
            set
            {
                Points[0] = value;
            }
        }

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

        public DrawingPlane(Plane plane) : this(SIZE, DISTANCE, plane) { }
        /// <summary>
        /// Rovina v editoru
        /// TODO: 
        /// --dodelat na nekonecnou rovinu
        /// --dodelat na rovinu zadanou dvema body
        /// </summary>
        /// <param name="size">pocet okynek na jedne strane mrizky</param>
        /// <param name="distance">vzdalenos mezi mrizkami</param>
        public DrawingPlane(
            int size, float distance, Plane plane)
        {
            Size = size > 0 ? size : SIZE;
            Distance = distance > 0 ? distance : DISTANCE;
//            SetLabelPrefix("plane");
            this.Set(plane);
        }

        /// <summary>
        /// Rovina v editoru
        /// TODO: 
        /// --dodelat na nekonecnou rovinu
        /// --dodelat na rovinu zadanou dvema body
        /// </summary>
        private void Set(Plane plane)
        {
            this.ModelObject = plane;

            _RotatMatrix = plane._RotatMatrix;
            _ShiftMatrix = plane._ShiftMatrix;
            _localMatrix = _RotatMatrix * _ShiftMatrix;

            Vektor leftCorner = new Vektor(plane.Pocatek.X, plane.Pocatek.Y, plane.Pocatek.Z);

            List<Vektor> points = new List<Vektor>();
            points.Add(leftCorner);
            List<Line3D> lines = new List<Line3D>(2 * (Size + 1));
            Line3D line;
            if (leftCorner == null)
                leftCorner = new Vektor(-5, 0, -5);

            // MRIZKA
            for (float i = 0; i <= Size; i += Distance)
            {
                Vektor p1 = new Vektor(leftCorner.X + i, leftCorner.Y, leftCorner.Z);
                Vektor p2 = new Vektor(leftCorner.X + i, leftCorner.Y, leftCorner.Z + Size);
                points.Add(p1);
                points.Add(p2);
                line = new Line3D(p1, p2);
                lines.Add(line);
            }
            for (float i = 0; i <= Size; i += Distance)
            {
                Vektor p1 = new Vektor(leftCorner.X, leftCorner.Y, leftCorner.Z + i);
                Vektor p2 = new Vektor(leftCorner.X + Size, leftCorner.Y, leftCorner.Z + i);
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
            Size = size > 0 ? size : SIZE;
            Distance = distance > 0 ? distance : DISTANCE;
            this.Set(plane);
        }

        public void SetModelObject(RayTracerLib.Plane plane)
        {
            this.SetModelObject(plane, Size, Distance);
        }

        public override Vektor GetCenter()
        {
            return this.Center;
        }
    }
}
