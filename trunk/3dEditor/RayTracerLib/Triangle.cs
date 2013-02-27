using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{
    /// <summary>
    ///           C
    ///          / \
    ///         /   \
    ///        A-----B
    /// </summary>
    public class Triangle : DefaultShape
    {
        /// <summary>
        /// Levy dolni vrchol
        /// </summary>
        public Vertex A { get { return Vertices[0]; } private set { Vertices[0] = value; } }
        /// <summary>
        /// Pravy dolni vrchol
        /// </summary>
        public Vertex B { get { return Vertices[1]; } private set { Vertices[1] = value; } }
        /// <summary>
        /// Horni Vrchol
        /// </summary>
        public Vertex C { get { return Vertices[2]; } private set { Vertices[2] = value; } }
        Vertex[] Vertices;

        // POMOCNE PROMENNE
        // ///////////////////////////
        /// <summary>
        /// Normala roviny tvorena trojuhelnikem
        /// </summary>
        public Vektor Norm;
        /// <summary>
        /// vektor s pocatkem v A
        /// </summary>
        Vektor AB, AC;

        public Triangle() : this(new Vektor(1, 0, 0), new Vektor(0, 1, 0), new Vektor(0, 0, 1)) { }
        public Triangle(Vertex a, Vertex b, Vertex c) : this(a, b, c, new Material()) { }
        public Triangle(Vertex a, Vertex b, Vertex c, Material mat)
        {
            Set(a, b, c);
            this.Material = mat;
        }
        public Triangle(Vektor a, Vektor b, Vektor c)
        {
            Vertices = new Vertex[3] { new Vertex(), new Vertex(), new Vertex() };
            A.AddFace(this);
            B.AddFace(this);
            C.AddFace(this);
            Set(a, b, c);
            this.Material = new Material();
        }

        public void Set(Vektor a, Vektor b, Vektor c)
        {
            //CustomObject cube = CustomObject.CreateCube();
            IsActive = true;
            A.Set(a);
            B.Set(b);
            C.Set(c);
            //Vertices = new Vertex[3] { new Vertex(a), new Vertex(b), new Vertex(c) };
            

            AB = B - A;
            //AB.Normalize();
            AC = C - A;
            //AC.Normalize();
            Norm = Vektor.CrossProduct(AB, AC);
            Norm.Normalize();
            _ShiftMatrix = Matrix3D.Identity;
            _RotatMatrix = Matrix3D.Identity;
            SetTempValues();
        }

        public void Set(Vertex a, Vertex b, Vertex c)
        {
            IsActive = true;
            Vertices = new Vertex[3] { a, b, c };
            A.AddFace(this);
            B.AddFace(this);
            C.AddFace(this);

            AB = B - A;
            //AB.Normalize();
            AC = C - A;
            //AC.Normalize();
            Norm = Vektor.CrossProduct(AB, AC);
            Norm.Normalize();
            _ShiftMatrix = Matrix3D.Identity;
            _RotatMatrix = Matrix3D.Identity;
            SetTempValues();
        }

        Vektor _v0, _v1;
        double _dot00, _dot01, _dot11, _invDelitel;

        /// <summary>
        /// predspocitane hodnoty, aby byl vypocet pruniku s paprskem rychlejsi
        /// </summary>
        private void SetTempValues()
        {
            _v0 = AC;
            _v1 = AB;

            _dot00 = _v0 * _v0;
            _dot01 = _v0 * _v1;
            _dot11 = _v1 * _v1;
            _invDelitel = 1 / (_dot00 * _dot11 - _dot01 * _dot01);
        }

        public override bool Intersects(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint)
        {
            if (!IsActive) return false;

            Vektor u = A - P0; // vektor z P0 do C

            double n_v = Norm * Pd;
            if (Math.Abs(n_v) < MyMath.EPSILON) return false; // rovnobezne, nebo splyvaji
            double n_u = Norm * u;
            double alfa = n_u / n_v;
            if (alfa < MyMath.EPSILON) return false;  // paprsek neprotina rovinu trojuhelniku

            // ted vime, ze paprsek protina rovinu
            // musime zjistit, zda bod lezi uvnitr trojuhelniku
            Vektor Pi = P0 + Pd * alfa;
            Vektor v2 = Pi - A;

            double dot02 = _v0 * v2;
            double dot12 = _v1 * v2;

            double uB = (_dot11 * dot02 - _dot01 * dot12) * _invDelitel;
            double vB = (_dot00 * dot12 - _dot01 * dot02) * _invDelitel;
            double wB = 1 - uB - vB;

            if (uB < 0 || vB < 0 || uB + vB > 1) return false;      // bod uvnitr trojuhelniku

            //A.Normal = Norm * -1;
            //B.Normal = Norm * -1;
            //C.Normal = Norm * -1;
            // P = C*u + B*v + A*w      ... procentualni nastaveni noveho vektoru
            Vektor interpVec = C.Normal * uB + B.Normal * vB + A.Normal * wB;
            interpVec.Normalize();

            SolidPoint sp = new SolidPoint();
            sp.Coord = Pi;
            sp.T = alfa;
            sp.Normal = interpVec;
            sp.Color = new Colour(this.Material.Color);
            sp.Material = this.Material;
            sp.Shape = this;

            InterPoint.Add(sp);
            return true;
        }
        /*
         *             if (!IsActive) return false;

            Vektor u = A - P0; // vektor z P0 do C

            double n_v = Norm * Pd;
            if (Math.Abs(n_v) < MyMath.EPSILON) return false; // rovnobezne, nebo splyvaji
            double n_u = Norm * u;
            double alfa = n_u / n_v;
            if (alfa < MyMath.EPSILON) return false;  // paprsek neprotina rovinu trojuhelniku

            // ted vime, ze paprsek protina rovinu
            // musime zjistit, zda bod lezi uvnitr trojuhelniku
            Vektor Pi = P0 + Pd * alfa;

            Vektor v0 = C - A;
            v0 = AC;
            Vektor v1 = B - A;
            v1 = AB;
            Vektor v2 = Pi - A;

            double dot00 = v0 * v0;
            double dot01 = v0 * v1;
            double dot02 = v0 * v2;
            double dot11 = v1 * v1;
            double dot12 = v1 * v2;

            double invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            double uB = (dot11 * dot02 - dot01 * dot12) * invDenom;
            double vB = (dot00 * dot12 - dot01 * dot02) * invDenom;
            double wB = 1 - uB - vB;

            if (uB < 0 || vB < 0 || uB + vB > 1) return false;      // bod uvnitr trojuhelniku
         * */
        public  bool Intersects2(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint)
        {
            Vektor w1 = AB;
            Vektor w2 = AC;
            Vektor u = A - P0; // vektor z P0 do C

            double n_v = Norm * Pd;
            if (Math.Abs(n_v) < MyMath.EPSILON) return false; // rovnobezne, nebo splyvaji
            double n_u = Norm * u;
            double alfa = n_u / n_v;
            if (alfa < MyMath.EPSILON) return false;  // paprsek neprotina rovinu trojuhelniku

            // ted vime, ze paprsek protina rovinu
            // musime zjistit, zda bod lezi uvnitr trojuhelniku
            Vektor Pi = P0 + Pd * alfa;
            Vektor w = Pi - A;

            double uv = w1 * w2;
            double wv = w * w2;
            double vv = w2 * w2;
            double wu = w * w1;
            double uu = w1 * w1;
            double s = (uv * wv - vv * wu) / (uv * uv - uu * vv);
            double t = (uv * wu - uu * wv) / (uv * uv - uu * vv);
            if (!(s >= 0 && t >= 0 && (s + t) <= 1)) return false;

            SolidPoint sp = new SolidPoint();
            sp.Coord = Pi;
            sp.T = alfa;
            sp.Normal = Norm;
            sp.Color = new Colour(this.Material.Color);
            sp.Material = this.Material;
            sp.Shape = this;

            InterPoint.Add(sp);
            return true;
        }

        private Vektor Interpolate(Vektor P)
        {
            Vektor v0 = C - A;
            Vektor v1 = B - A;
            Vektor v2 = P - A;

            double dot00 = v0 * v0;
            double dot01 = v0 * v1;
            double dot02 = v0 * v2;
            double dot11 = v1 * v1;
            double dot12 = v1 * v2;

            double invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            double u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            double v = (dot00 * dot12 - dot01 * dot02) * invDenom;
            double w = 1 - u - v;

            if (u < 0 || v < 0 || u + v > 1) return null;

            // P = C*u + B*v + A*w      ... procentualni nastaveni noveho vektoru
            Vektor interpVec = C.Normal * u + B.Normal * v + A.Normal * w;

            return interpVec;
        }


        /// <summary>
        /// prunik paprsku s rovinou
        /// vrati bod lezici na rovine trojuhelniku
        /// bod muze lezet kdekoli na cele rovine - nemusi byt uvnitr trojuhelniku
        /// </summary>
        /// <param name="P0">pocatek paprsku</param>
        /// <param name="Pd">smer paprsku</param>
        /// <returns>bod pruniku, nebo NULL</returns>
        private Vektor GetPointOnThisPlane(Vektor P0, Vektor Pd)
        {
            Vektor point = null;

            Vektor u = C - P0; // vektor z P0 do C
            double n_v = Norm * Pd;
            if (Math.Abs(n_v) < MyMath.EPSILON) return point; // rovnobezne, nebo splyvaji
            double n_u = Norm * u;
            double alfa = n_u / n_v;
            if (alfa < MyMath.EPSILON) return point;  // paprsek neprotina rovinu trojuhelniku
            point = P0 + Pd * alfa;
            return point;
        }

        public override void Move(double dx, double dy, double dz)
        {
            throw new NotImplementedException();
        }
        public override void MoveToPoint(double dx, double dy, double dz)
        {
            Matrix3D transpShift = _ShiftMatrix.GetOppositeShiftMatrix();
            transpShift.TransformPoint(A);
            transpShift.TransformPoint(B);
            transpShift.TransformPoint(C);

            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(dx, dy, dz);
            _ShiftMatrix.TransformPoint(A);
            _ShiftMatrix.TransformPoint(B);
            _ShiftMatrix.TransformPoint(C);
            _localMatrix = _RotatMatrix * _ShiftMatrix;
        }

        public override string ToString()
        {
            return "A:" + A.ToString() + "; B:" + B.ToString() + "; C:" + C.ToString() + ";";
        }

        public override void Rotate(double degAroundX, double degAroundY, double degAroundZ)
        {
            return;
        }
    }
}
