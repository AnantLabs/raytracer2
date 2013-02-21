using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

using RayTracerLib;
using System.Drawing;
using Mathematics;

namespace EditorLib
{
    public class DrawingTriangle : DrawingDefaultShape
    {
        public Vektor A { get { return Points[1]; } private set { Points[1] = value; } }
        public Vektor B { get { return Points[2]; } private set { Points[2] = value; } }
        public Vektor C { get { return Points[3]; } private set { Points[3] = value; } }

        public Brush FillBrush;

        /// <summary>
        /// neni to stread ale jeden z rohu trojuhelnika
        /// </summary>
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

        public DrawingTriangle(Triangle triangle)
        {
            _RotatMatrix = Matrix3D.Identity;
            SetLabelPrefix("triang");
            this.SetModelObject(triangle);
        }

        public override void SetModelObject(object modelObject)
        {
            if (modelObject != null && modelObject.GetType() == typeof(RayTracerLib.Triangle))
                this.SetModelObject((RayTracerLib.Triangle)modelObject);
        }
        /// <summary>
        /// nastavi triangl z Raytraceru do Editoru
        /// RayTracerLib -> EditorLib
        /// </summary>
        /// <param name="sphere"></param>
        public void SetModelObject(Triangle triangle)
        {
            this.ModelObject = triangle;
            FillBrush = new SolidBrush(triangle.Material.Color.SystemColor());
            Points = new Vektor[4];
            Points[0] = new Vektor(0, 0, 0);
            Points[1] = new Vektor(triangle.A.X, triangle.A.Y, triangle.A.Z);
            Points[2] = new Vektor(triangle.B.X, triangle.B.Y, triangle.B.Z);
            Points[3] = new Vektor(triangle.C.X, triangle.C.Y, triangle.C.Z);

            _ShiftMatrix = triangle._ShiftMatrix;
            _localMatrix = _RotatMatrix * _ShiftMatrix;
            _localMatrix.TransformPoints(Points);

            this.Lines = new List<Line3D>(3);
            Lines.Add(new Line3D(A, B));
            Lines.Add(new Line3D(B, C));
            Lines.Add(new Line3D(C, A));
        }

        public Vektor[] GetDrawingPoints()
        {
            Vektor[] ps = new Vektor[3];
            Array.Copy(Points, 1, ps, 0, 3);
            return ps;
        }
        public override Vektor GetCenter()
        {
            return Points[0];
        }
    }
}