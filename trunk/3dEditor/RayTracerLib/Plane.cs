using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;
using System.Threading;
using System.Runtime.Serialization;

namespace RayTracerLib
{


    /// <summary>
    /// Rovnice roviny:
    /// A*x + B*y + C*z + D = 0  je rovnice vsech bodu na rovine
    /// 
    /// Point * Normal = -D;
    /// Point .. libovolny bod na rovine
    /// Normal .. normala roviny
    /// D .. vzdalenost roviny od pocatku
    /// </summary>
    [DataContract]
    public class Plane : DefaultShape
    {
        private Vektor DirNom;
        private double p;

        /// <summary>
        /// Smer roviny - jeji normala
        /// </summary>
        [DataMember]
        public Vektor Normal { get; set; }
        public Vektor Pocatek { get; set; }

        /// <summary>
        /// rovnice hranicnich primek
        /// bod, smer
        /// </summary>
        private List<KeyValuePair<Vektor, Vektor>> BoundVectors { get;  set; }

        /// <summary>
        /// Vzdalenost roviny od pocatku
        /// </summary>
        [DataMember]
        public double D { get; set; }

        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }
        public double MinZ { get; set; }
        public double MaxZ { get; set; }

        public Plane() : this(new Vektor(0, 1, 0), 1) { }
        public Plane(Vektor normal, double d)
        {
            IsActive = true;
            this.SetValues(normal, d);
        }

        public Plane(Vektor normal, double d, Colour c) : this(normal, d)
        {
            Material.Color = new Colour(c);
        }

        public Plane(Vektor normal, double d, Material m) : this (normal, d)
        {
            Material = new Material(m);
        }

        public Plane(Plane oldPlane)
        {
            IsActive = oldPlane.IsActive;
            this.Normal = new Vektor(oldPlane.Normal);
            this.D = oldPlane.D;
            SetValues(Normal, D);
            this.Material = new Material(oldPlane.Material);
        }

        public void SetValues(Vektor normal, double d)
        {
            Normal = normal;
            
            DirNom = new Vektor(normal);
            DirNom.Normalize();

            D = d;
            this.Material = new Material();
            this.Pocatek = new Vektor(0, 0, -10);
            MinX = MinY = MinZ = Double.NegativeInfinity;
            MaxX = MaxY = MaxZ = Double.PositiveInfinity;

            CreateBoundVektors();

            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(0, -d, 0);
            Vektor yAxe = new Vektor(0, 1, 0);
            Quaternion q = new Quaternion(yAxe, new Vektor(normal));
            double[] degss = q.ToEulerDegs();

            // rotace z kvaternionu je opacne orientovana, proto minuska
            Matrix3D matCr = Matrix3D.NewRotateByDegrees(-degss[0], -degss[1], -degss[2]);
            _RotatMatrix = matCr;
            _localMatrix = _RotatMatrix * _ShiftMatrix;
        }


        /// <summary>
        /// t = -(D + P0 * Normal) / (Pd * Normal)
        /// </summary>
        /// <param name="P0"></param>
        /// <param name="Pd"></param>
        /// <param name="InterPoint"></param>
        /// <returns></returns>
        public override bool Intersects(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint)
        {
            if (!IsActive)
                return false;

            Interlocked.Increment(ref DefaultShape.TotalTested);

            Vektor normal = DirNom;
            Pd.Normalize();

            double Vd = normal * Pd;

            // paprsek rovinu neprotina: - kdyz Vd==0, pak paprsek neprotina
            if (Math.Abs(Vd) < MyMath.EPSILON)
                return false;

            // spravna verze (spocitano):
            double V0 = - (normal * P0 + D);  // asi je tahle verze spise vic dobre (funguje pro krychli)

            //double V0 = D - normal * P0;        // asi je tahle verzi opacne (funguje pro valec)
            double t = V0 / Vd;

            // paprsek protina rovinu zezadu
            if (t < 0.0)
                return false;

            Vektor Bounds = P0 + Pd * t;
            if (Bounds.X < MinX || Bounds.X > MaxX ||
                Bounds.Y < MinY || Bounds.Y > MaxY ||
                Bounds.Z < MinZ || Bounds.Z > MaxZ)
                return false;

            SolidPoint sp = new SolidPoint();
            sp.T = t;
            sp.Coord = P0 + Pd * t;           // souradnice bodu pruniku
            if (Vd < 0)
                sp.Normal = new Vektor(normal);
            else
                sp.Normal = Vektor.ZeroVektor - normal;

            sp.Normal.Normalize();
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

        public override string ToString()
        {
            return "Plane: " + Normal + "; D=" + Math.Round(this.D, 1);
        }

        public void CreateBoundVektors()
        {

            
            double[] Xs = new double[2];
            Xs[0] = Double.IsNegativeInfinity(this.MinX) ? -Double.MaxValue : this.MinX;
            Xs[1] = Double.IsPositiveInfinity(this.MaxX) ? Double.MaxValue : this.MaxX;
            double[] Ys = new double[2];
            Ys[0] = Double.IsNegativeInfinity(this.MinY) ? -Double.MaxValue : this.MinY;
            Ys[1] = Double.IsPositiveInfinity(this.MaxY) ? Double.MaxValue : this.MaxY;
            double[] Zs = new double[2];
            Zs[0] = Double.IsNegativeInfinity(this.MinZ) ? -Double.MaxValue : this.MinZ;
            Zs[1] = Double.IsPositiveInfinity(this.MaxZ) ? Double.MaxValue : this.MaxZ;

            Pocatek = new Vektor(Xs[0] + Xs[1], Ys[0] + Ys[1], Zs[0] + Zs[1]);
            Pocatek = Pocatek * (1.0 / 2.0);

            //double diff = Pocatek * this.Normal + this.D;


            List<Vektor> okraje = new List<Vektor>();
            for(int x=0; x<2; x++)
                for (int y=0; y<2; y++)
                    for (int z = 0; z < 2; z++)
                    {
                        Vektor v = new Vektor(Xs[x], Ys[y], Zs[z]);
                        if (v * Normal + D < MyMath.EPSILON)
                            okraje.Add(v);
                    }
            List<KeyValuePair<Vektor, Vektor>> boundVectors = new List<KeyValuePair<Vektor, Vektor>>();
            Vektor v1 = okraje[0];

            for (int i=0; i<okraje.Count-1; i++)
                for (int j = i+1; j < okraje.Count; j++)
                {
                    if (IsDifferenceInOne(okraje[i], okraje[j]))
                    {
                        Vektor dir = okraje[i] - okraje[j];
                        dir.Normalize();
                        KeyValuePair<Vektor, Vektor> hranice = new KeyValuePair<Vektor, Vektor>(okraje[i], dir);
                        boundVectors.Add(hranice);
                    }
                }
            KeyValuePair<Vektor, Vektor> v2 = boundVectors[0];
            this.BoundVectors = boundVectors;
        }

        /// <summary>
        /// zjisti, zda se vektory lisi prave v jedne slozce
        /// </summary>
        /// <param name="v1">vektor c1</param>
        /// <param name="v2">vektor c2</param>
        /// <returns>true, kdyz jsou vektory ve dvou libovolnych slozkach stejne</returns>
        private bool IsDifferenceInOne(Vektor v1, Vektor v2)
        {
            if (v1.X == v2.X && (v1.Y == v2.Y || v1.Z == v2.Z))
                return true;
            if (v1.X != v2.X && v1.Y == v2.Y && v1.Z == v2.Z)
                return true;

            return false;
        }

        public Vektor GetNearestPoint(Vektor point, out double dist)
        {
            if (BoundVectors == null)
                CreateBoundVektors();
            Vektor nearest = point;
            dist = 0;
            for (int i = 0; i < BoundVectors.Count; i++)
            {
                KeyValuePair<Vektor,Vektor> hranice = BoundVectors[i];
                Vektor p = hranice.Key;
                Vektor dir = hranice.Value;
                double len = MyMath.DistancePointAndLine(point, p, dir);
                if (len > dist)
                {
                    dist = len;
                    nearest = MyMath.NearestPoint(point, p, dir);
                }
            }
            return nearest;
        }

        public override void MoveToPoint(double dx, double dy, double dz)
        {
            this.D = dy;
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(0, -this.D, 0);
            _localMatrix = _RotatMatrix * _ShiftMatrix;
        }
        public override void Rotate(double degX, double degY, double degZ)
        {
            Matrix3D newRot = Matrix3D.NewRotateByDegrees(degX, degY, degZ);

            Vektor yAxe = new Vektor(0, 1, 0);
            newRot.TransformPoint(yAxe);
            this._RotatMatrix = newRot;
            _localMatrix = _RotatMatrix * _ShiftMatrix;
            this.SetValues(yAxe, this.D);
        }

        //public bool Intersects2(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint)
        //{

        //    Vektor normal = new Vektor(Normal);
        //    //normal.Normalize();
        //    //Pd.Normalize();

        //    double Vd = normal * Pd;

        //    Vektor V0mP0 = Pocatek - P0;
        //    double cit = V0mP0 * normal;

        //    // paprsek rovinu neprotina: - kdyz Vd==0, pak paprsek splyva
        //    if (Math.Abs(Vd) < MyMath.EPSILON)
        //        return false;

        //    double t = cit / Vd;

        //    // paprsek protina rovinu za pocatkem
        //    if (t < MyMath.EPSILON)
        //        return false;

        //    SolidPoint sp = new SolidPoint();
        //    sp.T = t;
        //    sp.Coord = P0 + Pd * t;           // souradnice bodu pruniku
        //    if (Vd < 0)
        //        sp.Normal = new Vektor(normal);
        //    else
        //        sp.Normal = Vektor.ZeroVektor - normal;

        //    sp.Normal.Normalize();
        //    sp.Color = this.Material.Color;
        //    sp.Material = this.Material;


        //    InterPoint.Add(sp);

        //    return true;
        //}


    }
}
