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
        public Vektor A { get { return Vertices[0]; } private set { Vertices[0] = value; } }
        /// <summary>
        /// Pravy dolni vrchol
        /// </summary>
        public Vektor B { get { return Vertices[1]; } private set { Vertices[1] = value; } }
        /// <summary>
        /// Horni Vrchol
        /// </summary>
        public Vektor C { get { return Vertices[2]; } private set { Vertices[2] = value; } }
        Vektor[] Vertices;

        // POMOCNE PROMENNE
        // ///////////////////////////
        /// <summary>
        /// Normala roviny tvorena trojuhelnikem
        /// </summary>
        Vektor Norm;
        /// <summary>
        /// vektor s pocatkem v A
        /// </summary>
        Vektor AB, AC;

        public Triangle() : this(new Vektor(1, 0, 0), new Vektor(0, 1, 0), new Vektor(0, 0, 1)) { }

        public Triangle(Vektor a, Vektor b, Vektor c)
        {
            Set(a, b, c);
            this.Material = new Material();
        }

        public void Set(Vektor a, Vektor b, Vektor c)
        {
            IsActive = true;
            Vertices = new Vektor[3] { a, b, c };
            AB = B - A;
            AB.Normalize();
            AC = C - A;
            AC.Normalize();
            Norm = Vektor.CrossProduct(AB, AC);
            Norm.Normalize();
            _ShiftMatrix = Matrix3D.Identity;
            _RotatMatrix = Matrix3D.Identity;
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

        public override bool Intersects(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint)
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
            throw new NotImplementedException();
        }
    }
}
