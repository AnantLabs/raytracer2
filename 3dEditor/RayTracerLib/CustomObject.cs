using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;
using System.Runtime.Serialization;

namespace RayTracerLib
{
    
    [DataContract]
    public class CustomObject : DefaultShape
    {
        /// <summary>
        /// seznam vrcholu
        /// </summary>
        public List<Vertex> VertexList;
        /// <summary>
        /// seznam plosek
        /// </summary>
        [DataMember]
        public List<Triangle> FaceList;

        /// stred objektu
        [DataMember]
        public Vektor Center { get; private set; }

        public CustomObject() : this(CustomObject.CreateCube()) { }
        public CustomObject(CustomObject old):base(old)
        {
            IsActive = old.IsActive;
            if (old.VertexList != null)
                this.VertexList = new List<Vertex>(old.VertexList);
            if (old.FaceList != null)
                this.FaceList = new List<Triangle>(old.FaceList);
            if (old._ShiftMatrix != null)
                _ShiftMatrix = new Matrix3D(old._ShiftMatrix);
            if (old._RotatMatrix != null)
                _RotatMatrix = new Matrix3D(old._RotatMatrix);
            if (old._localMatrix != null)
                _localMatrix = new Matrix3D(old._localMatrix);
            if (old.Material != null)
                this.Material = new Material(old.Material);
        }
        /// <summary>
        /// zadani objektu pres body a seznamem indexu
        /// </summary>
        /// <param name="points">vsechny body objektu</param>
        /// <param name="indeces">seznam brany po trojicich. Prvni 3 indexy ukazuji na vrcholy k prvni plosce. 
        /// Dalsi 3 na druhou plosku atd. Seznam musi byt delitelny tremi.</param>
        public CustomObject(Vektor[] points, int[] indeces)
        {
            if (points.Length < 3) throw new Exception("Wrong number of points");
            
            SetLabelPrefix("custom");
            IsActive = true;
            this.Material = new Material();

            VertexList = new List<Vertex>(points.Length);
            foreach (Vektor vec in points)
            {
                VertexList.Add(new Vertex(vec));
            }
            int rem;
            int len = Math.DivRem(indeces.Length, 3, out rem);
            len = indeces.Length - rem;
            FaceList = new List<Triangle>(len / 3);
            for (int i = 0; i < len; i+=3)
            {
                FaceList.Add(new Triangle(VertexList[indeces[i]], VertexList[indeces[i + 1]], VertexList[indeces[i + 2]]));
            }
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(0, 0, 0);
        }

        private static Vertex Searching;
        private static bool VertexPredicate(Vertex vec)
        {
            if (vec.X == Searching.X && vec.Y == Searching.Y && vec.Z == Searching.Z)
                return true;
            else return false;
        }
        public CustomObject(List<Triangle> triangList, Vektor center)
        {
            SetLabelPrefix("custom");
            Center = center;
            IsActive = true;
            this.Material = new Material();
            InitializeForRayTr(triangList);
        }
        public CustomObject(List<Triangle> triangList) : this(triangList, new Vektor(1, 1, 1)) { }

        public void InitializeForRayTr()
        {
            this.InitializeForRayTr(this.FaceList);
        }
        public void InitializeForRayTr(List<Triangle> triangList)
        {
            this.SetFromTriangles(triangList);
            SetCenter(this.Center);
        }
        private void SetFromTriangles(List<Triangle> triangList)
        {
            VertexList = new List<Vertex>();
            FaceList = new List<Triangle>();

            foreach (Triangle tr in triangList)
            {
                Vertex a = new Vertex((Vektor)tr.A);
                Searching = a;
                Vertex found = VertexList.Find(VertexPredicate);
                if (found != null)
                    a = found;
                else
                    VertexList.Add(a);

                Vertex b = new Vertex((Vektor)tr.B);
                Searching = b;
                found = VertexList.Find(VertexPredicate);
                if (found != null)
                    b = found;
                else
                    VertexList.Add(b);

                Vertex c = new Vertex((Vektor)tr.C);
                Searching = c;
                found = VertexList.Find(VertexPredicate);
                if (found != null)
                    c = found;
                else
                    VertexList.Add(c);
                Triangle face = new Triangle(a, b, c, tr.Material);
                FaceList.Add(face);
            }

        }

        /// <summary>
        /// HLAVNI METODA NASTAVENI normal objektu
        /// </summary>
        /// <param name="center"></param>
        private void SetCenter(Vektor center)
        {
            Center = center;
            // zmen orientaci normal vsech trojuhelniku podle stredu
            SetNormsFaces();
            // zmen normaly vrcholu k okolnim ploskam
            SetNormsVertices();
        }
        /// <summary>
        /// zmen orientaci normal vsech plosek podle stredu.
        /// Normala plosky musi ukazovat stejnym smerem jako vektor ze stredu objektu do plosky
        /// </summary>
        private void SetNormsFaces()
        {
            foreach (Triangle face in FaceList)
            {
                Vektor v = face.GetCenter() - Center;
                v.Normalize();
                if (v * face.Norm < 0)
                    face.Norm.MultiplyBy(-1);
            }
        }
        /// <summary>
        /// zmen normaly vrcholu k okolnim ploskam
        /// </summary>
        private void SetNormsVertices()
        {
            foreach (Vertex vertex in VertexList)
            {
                vertex.SetNorm();
            }
        }


        public void SetMaterial2All(Material material)
        {
            this.Material = new Material(material);
            foreach (Triangle tr in FaceList)
            {
                tr.Material = new Material(material);
            }
        }

        public override bool Intersects(Mathematics.Vektor P0, Mathematics.Vektor Pd, ref List<SolidPoint> InterPoint, bool isForLight, double lightDist)
        {
            if (!IsActive)
                return false;

            if (isForLight && InterPoint.Count > 0)
            {
                foreach (SolidPoint solp in InterPoint)
                    if (lightDist > solp.T) return true;
            }

            bool result = false;
            foreach (Triangle tr in FaceList)
            {
                bool res = tr.Intersects(P0, Pd, ref InterPoint, isForLight, lightDist);
                if (!result && res) result = true;
            }
            return result;
        }

        public override void Move(double dx, double dy, double dz)
        {
            throw new NotImplementedException();
        }

        public override void MoveToPoint(double dx, double dy, double dz)
        {
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(dx, dy, dz);
            _ShiftMatrix.TransformPoint(Center);
            foreach (Vertex vert in VertexList)
            {
                Vektor vec = (Vektor)vert;
                _ShiftMatrix.TransformPoint(vec);
                vert.X = vec.X;
                vert.Y = vec.Y;
                vert.Z = vec.Z;
            }
            
        }

        public override void Rotate(double degAroundX, double degAroundY, double degAroundZ)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Label + " {" + VertexList.Count + " vertices, " + FaceList.Count + " faces}"; ;
        }
        public static CustomObject CreateCube()
        {
            Vektor v0 = new Vektor(0, 0, 0);
            Vektor v1 = new Vektor(0, 0, 2);
            Vektor v2 = new Vektor(2, 0, 2);
            Vektor v3 = new Vektor(2, 0, 0);
            Vektor v4 = new Vektor(0, 2, 0);
            Vektor v5 = new Vektor(0, 2, 2);
            Vektor v6 = new Vektor(2, 2, 2);
            Vektor v7 = new Vektor(2, 2, 0);
            Vektor v8 = new Vektor(1, 0, 1);
            Vektor v9 = new Vektor(1, 2, 1);
            Vektor v10 = new Vektor(1, 1, 2);
            Vektor v11 = new Vektor(2, 1, 1);
            Vektor v12 = new Vektor(1, 1, 0);
            Vektor v13 = new Vektor(0, 1, 1);
            List<Vektor> vecs = new List<Vektor>();
            vecs.Add(v0); vecs.Add(v1); vecs.Add(v2);
            vecs.Add(v3); vecs.Add(v4); vecs.Add(v5);
            vecs.Add(v6); vecs.Add(v7); vecs.Add(v8);
            vecs.Add(v9); vecs.Add(v10); vecs.Add(v11);
            vecs.Add(v12); vecs.Add(v13);
            int[] faces = new int[]{ // indexy vrcholu
                0,3,8,
                0,1,8,
                1,2,8,
                2,3,8,
                5,6,10,
                1,5,10,
                1,2,10,
                2,6,10,
                6,7,11,
                2,6,11,
                2,3,11,
                3,7,11,
                4,7,12,
                0,4,12,
                0,3,12,
                3,7,12,
                4,5,13,
                1,5,13,
                0,1,13,
                0,4,13,
                4,7,9,
                4,5,9,
                5,6,9,
                6,7,9};
            CustomObject cube = new CustomObject(vecs.ToArray(), faces);
            cube.SetCenter(new Vektor(1, 1, 1));
            return cube;
        }

        public static CustomObject CreatePlane()
        {
            Vektor v0 = new Vektor(-2, 0, 0);
            Vektor v1 = new Vektor(2, 0, 0);
            Vektor v2 = new Vektor(2, 0, 2);
            Vektor v3 = new Vektor(-2, 0, 2);

            List<Vektor> vecs = new List<Vektor>();
            vecs.Add(v0); vecs.Add(v1);
            vecs.Add(v2); vecs.Add(v3);
            int[] faces = new int[]{ // indexy vrcholu
                0,1,2,
                0,2,3,
            };
            CustomObject plane = new CustomObject(vecs.ToArray(), faces);
            plane.SetCenter(new Vektor(0, -0.1, 1));
            return plane;
        }

        public override DefaultShape FromDeserial()
        {
            CustomObject cust = new CustomObject(this.FaceList, this.Center);
            cust.Label = this.Label;
            cust.Material = this.Material;
            cust.IsActive = this.IsActive;
            return cust;
        }
    }
}
