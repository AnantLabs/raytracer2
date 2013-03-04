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
                return Points[Points.Length - 1];
            }
            set
            {
                Points[Points.Length - 1] = value;
            }
        }

        private Matrix3D _origShigMatrix;
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
            counter = 0;
            this.Set(custom);
        }

        public void Reset()
        {
            this.SetModelObject(this.ModelObject);
        }
        public override void InitForRaytracer(Matrix3D rotMat)
        {
            Matrix3D transp = rotMat.Transpose();
            transp.TransformPoints(this.Points);
            CustomObject cust = ModelObject as CustomObject;
            List<Triangle> triangles = new List<Triangle>();
            foreach (DrawingTriangle drTr in DrawingFacesList)
            {
                Triangle tr = drTr.ModelObject as Triangle;
                tr.Set(drTr.A, drTr.B, drTr.C);
                triangles.Add(tr);
            }
            cust.Center.Set(this.Center);
            cust.InitializeForRayTr(triangles);
            rotMat.TransformPoints(this.Points);
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
            if (ModelObject != null)
                this.ResetAll();
            this.ModelObject = custom;

            List<Vektor> vecs = new List<Vektor>();
            vecs.Add(new Vektor(custom.Center));
            foreach (Vertex vert in custom.VertexList)
            {
                vecs.Add(new Vektor((Vektor)vert));
            }

            this.Lines = new List<Line3D>();
            DrawingFacesList = new List<DrawingTriangle>();
            foreach (Triangle trian in custom.FaceList)
            {
                DrawingTriangle drTriang = new DrawingFacet(trian, this);
                

                Vektor a = (Vektor)trian.A;
                Vektor b = (Vektor)trian.B;
                Vektor c = (Vektor)trian.C;
                Searching = a;
                a = vecs.Find(VectorPredicate);
                Searching = b;
                b = vecs.Find(VectorPredicate);
                Searching = c;
                c = vecs.Find(VectorPredicate);

                drTriang.SetVertices(a, b, c);

                DrawingFacesList.Add(drTriang);

                Lines.Add(new Line3D(a, b));
                Lines.Add(new Line3D(b, c));
                Lines.Add(new Line3D(c, a));
            }
            vecs.Add(new Vektor(custom.Center));
            Points = vecs.ToArray();

            //foreach (Triangle trian in custom.FaceList)
            //{
            //    DrawingTriangle drTriang = new DrawingFacet(trian, this);
            //    DrawingFacesList.Add(drTriang);
            //}
            _RotatMatrix = Matrix3D.Identity;
            _ShiftMatrix = Matrix3D.Identity;
            _origShigMatrix = Matrix3D.Identity;
            _localMatrix = Matrix3D.Identity;
        }

        private void ResetAll()
        {
            // zresetuje labely
            foreach (DrawingFacet drFac in DrawingFacesList)
            {
                DrawingObject.labels.Remove(drFac.Label);
            }
            counter = 0;
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

        public override double[] GetRotationAngles()
        {
            return _RotatMatrix.GetAnglesFromMatrix();
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

        public override void Rotate(double degAroundX, double degAroundY, double degAroundZ)
        {
            Matrix3D newRot = Matrix3D.NewRotateByDegrees(degAroundX, degAroundY, degAroundZ);

            Matrix3D transpRot = _RotatMatrix.Transpose();
            Matrix3D transpShift = _origShigMatrix.GetOppositeShiftMatrix();

            _origShigMatrix.TransformPoints(Points);    // posunuti na pocatecni pozici
            transpRot.TransformPoints(Points);          // rotace na zakladni pozici

            this._RotatMatrix = newRot;
            _localMatrix = _RotatMatrix * transpShift;  // rotace o novy uhel a posunuti na posledni pozici
            _localMatrix.TransformPoints(Points);
        }

        public override void Move(double moveX, double moveY, double moveZ)
        {
            Matrix3D transpShift = _ShiftMatrix.GetOppositeShiftMatrix();
            transpShift.TransformPoints(Points);

            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(moveX, moveY, moveZ);
            // budeme si pamatovat matici posunuti od puvodni pozice
            _origShigMatrix.PosunutiAddMatrix(_ShiftMatrix);

            _localMatrix = _RotatMatrix * _ShiftMatrix;
        }

        public void AddDrawingTriangle(Triangle trian)
        {
            DrawingTriangle drTriang = new DrawingFacet(trian, this);


            Vektor a = (Vektor)trian.A;
            Vektor b = (Vektor)trian.B;
            Vektor c = (Vektor)trian.C;

            List<Vektor> vektors = new List<Vektor>(Points);

            Searching = a;
            a = Array.Find(Points, VectorPredicate);
            if (a == null)      // a je novy vrchol
            {
                a = (Vektor)trian.A;
                vektors.Add(a);
            }
            Searching = b;
            b = Array.Find(Points, VectorPredicate);
            if (b == null)
            {
                b = (Vektor)trian.B;
                vektors.Add(b);
            }
            Searching = c;
            c = Array.Find(Points, VectorPredicate);
            if (c == null)
            {
                c = (Vektor)trian.C;
                vektors.Add(c);
            }

            Points = vektors.ToArray();

            drTriang.SetVertices(a, b, c);

            DrawingFacesList.Add(drTriang);

            Lines.Add(new Line3D(a, b));
            Lines.Add(new Line3D(b, c));
            Lines.Add(new Line3D(c, a));
            
        }

        
    }
}
