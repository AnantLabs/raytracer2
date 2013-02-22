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

        /// <summary>
        /// zda se objekt zobrazi vyplneny, nebo jen dratovy
        /// </summary>
        public bool ShowFilled { get; set; }

        public List<DrawingTriangle> DrawingFacesList { get; private set; }

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

            List<Vektor> vecs = new List<Vektor>();
            vecs.Add(new Vektor(custom.Center));
            foreach (Vertex vert in custom.VertexList)
            {
                vecs.Add(new Vektor((Vektor)vert));
            }

            Points = vecs.ToArray();
            this.Lines = new List<Line3D>();
            DrawingFacesList = new List<DrawingTriangle>();
            foreach (Triangle trian in custom.FaceList)
            {
                DrawingTriangle drTriang = new DrawingFacet(trian, this);
                

                Vektor a = (Vektor)trian.A;
                Vektor b = (Vektor)trian.B;
                Vektor c = (Vektor)trian.C;
                Searching = a;
                a = Array.Find(Points, VectorPredicate);
                Searching = b;
                b = Array.Find(Points, VectorPredicate);
                Searching = c;
                c = Array.Find(Points, VectorPredicate);

                drTriang.SetVertices(a, b, c);

                DrawingFacesList.Add(drTriang);

                Lines.Add(new Line3D(a, b));
                Lines.Add(new Line3D(b, c));
                Lines.Add(new Line3D(c, a));
            }

            
            //foreach (Triangle trian in custom.FaceList)
            //{
            //    DrawingTriangle drTriang = new DrawingFacet(trian, this);
            //    DrawingFacesList.Add(drTriang);
            //}
            _RotatMatrix = Matrix3D.Identity;
            _ShiftMatrix = Matrix3D.Identity;
            _localMatrix = Matrix3D.Identity;
        }

        //public override void ApplyRotationMatrix(Matrix3D rotationMatrix)
        //{
        //    foreach (DrawingTriangle drTriang in DrawingFacesList)
        //    {
        //        drTriang.ApplyRotationMatrix(rotationMatrix);
        //    }
        //    base.ApplyRotationMatrix(rotationMatrix);
        //}


        public override Vektor GetCenter()
        {
            return new Vektor(this.Center);
        }

        public void SetMaterial2All(Material material)
        {
            CustomObject cust = ModelObject as CustomObject;
            cust.Material = new Material(material);
            foreach (DrawingTriangle drTr in this.DrawingFacesList)
            {
                drTr.SetMaterial(new Material(material));
            }
        }

        public override void Move(double moveX, double moveY, double moveZ)
        {
            //DefaultShape ds = (DefaultShape)ModelObject;
            //ds.MoveToPoint(moveX, moveY, moveZ);

            Matrix3D transpShift = _ShiftMatrix.GetOppositeShiftMatrix();
            transpShift.TransformPoints(Points);

            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(moveX, moveY, moveZ);

            _localMatrix = _RotatMatrix * _ShiftMatrix;

            //foreach (DrawingTriangle drTr in DrawingFacesList)
            //{
            //    drTr.Move(moveX, moveY, moveZ);
            //}
        }

        
    }
}
