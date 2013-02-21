using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mathematics;
using RayTracerLib;

namespace EditorLib
{
    public class DrawingCustom : DrawingDefaultShape
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

        public List<DrawingTriangle> DrawingFacesList { get; private set; }
        public Brush FillBrush;

        public DrawingCustom(CustomObject custom)
        {
            SetLabelPrefix("custom");
            Set(custom);
        }
                
        public override void SetModelObject(object modelObject)
        {
            if (modelObject != null && modelObject.GetType() == typeof(CustomObject))
                this.SetModelObject((CustomObject)modelObject);
        }
        /// <summary>
        /// nastavi CustomObject z Raytraceru do Editoru
        /// RayTracerLib -> EditorLib
        /// </summary>
        public void SetModelObject(CustomObject custom)
        {
            this.Set(custom);
        }

        private static Vektor Searching;
        private static bool VectorPredicate(Vektor vec)
        {
            if (vec.X == Searching.X && vec.Y == Searching.Y && vec.Z == Searching.Z)
                return true;
            else return false;
        }
        private void Set(CustomObject custom)
        {
            this.ModelObject = custom;

            FillBrush = new SolidBrush(custom.Material.Color.SystemColor());

            List<Vektor> vecs = new List<Vektor>();
            vecs.Add(new Vektor(custom.Center));
            foreach (Vertex vert in custom.VertexList)
            {
                vecs.Add(new Vektor((Vektor)vert));
            }

            Points = vecs.ToArray();
            this.Lines = new List<Line3D>();
            foreach (Triangle trian in custom.FaceList)
            {
                Vektor a = (Vektor)trian.A;
                Vektor b = (Vektor)trian.B;
                Vektor c = (Vektor)trian.C;
                Searching = a;
                a = Array.Find(Points, VectorPredicate);
                Searching = b;
                b = Array.Find(Points, VectorPredicate);
                Searching = c;
                c = Array.Find(Points, VectorPredicate);
                
                Lines.Add(new Line3D(a, b));
                Lines.Add(new Line3D(b, c));
                Lines.Add(new Line3D(c, a));
            }

            DrawingFacesList = new List<DrawingTriangle>();
            foreach (Triangle trian in custom.FaceList)
            {
                DrawingTriangle drTriang = new DrawingTriangle(trian);
                drTriang.SetLabelPrefix("facet");
                DrawingFacesList.Add(drTriang);
            }
            _RotatMatrix = Matrix3D.Identity;
            _ShiftMatrix = Matrix3D.Identity;
            _localMatrix = Matrix3D.Identity;
        }

        public override void ApplyRotationMatrix(Matrix3D rotationMatrix)
        {
            foreach (DrawingTriangle drTriang in DrawingFacesList)
            {
                drTriang.ApplyRotationMatrix(rotationMatrix);
            }
            base.ApplyRotationMatrix(rotationMatrix);
        }


        public override Vektor GetCenter()
        {
            return new Vektor(this.Center);
        }
    }
}
