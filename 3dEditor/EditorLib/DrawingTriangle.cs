using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

using RayTracerLib;
using System.Drawing;

namespace EditorLib
{
    public class DrawingTriangle : DrawingDefaultShape
    {
        public Point3D A { get { return Points[0]; } private set { Points[0] = value; } }
        public Point3D B { get { return Points[1]; } private set { Points[1] = value; } }
        public Point3D C { get { return Points[2]; } private set { Points[2] = value; } }

        public Brush FillBrush;

        /// <summary>
        /// neni to stread ale jeden z rohu trojuhelnika
        /// </summary>
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

        public DrawingTriangle(Triangle triangle)
        {
            _RotatMatrix = Matrix3D.Identity;
            this.SetModelObject(triangle);
        }

        public override void SetModelObject(object modelObject)
        {
            if (modelObject != null && modelObject.GetType() == typeof(RayTracerLib.Triangle))
                this.SetModelObject((RayTracerLib.Triangle)modelObject);
        }
        /// <summary>
        /// nastavi kouli z Raytraceru do Editoru
        /// RayTracerLib -> EditorLib
        /// </summary>
        /// <param name="sphere"></param>
        public void SetModelObject(Triangle triangle)
        {
            this.ModelObject = triangle;
            FillBrush = new SolidBrush(triangle.Material.Color.SystemColor());
            Points = new Point3D[3];
            Points[0] = new Point3D(triangle.A.X, triangle.A.Y, triangle.A.Z);
            Points[1] = new Point3D(triangle.B.X, triangle.B.Y, triangle.B.Z);
            Points[2] = new Point3D(triangle.C.X, triangle.C.Y, triangle.C.Z);

            _localMatrix = _RotatMatrix;
            _localMatrix.TransformPoints(Points);

            this.Lines = new List<Line3D>(3);
            Lines.Add(new Line3D(A, B));
            Lines.Add(new Line3D(B, C));
            Lines.Add(new Line3D(C, A));
        }

        public Point3D[] GetDrawingPoints()
        {
            return Points;
        }
        public override Point3D GetCenter()
        {
            return Points[0];
        }
    }
}
