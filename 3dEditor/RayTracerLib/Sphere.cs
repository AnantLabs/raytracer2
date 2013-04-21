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
    /// zakladni objekt sceny - koule
    /// 
    /// </summary>
    [KnownType(typeof(LabeledShape))]
    [KnownType(typeof(DefaultShape))]
    [KnownType(typeof(Vektor))]
    [DataContract]
    public class Sphere : DefaultShape
    {

        /// <summary>
        /// polomer koule
        /// </summary>
        [DataMember]
        public double R { get; set; }

        /// <summary>
        /// pocatek koule
        /// </summary>
        [DataMember]
        public Vektor Origin { get; set; }

//        public Sphere(Vektor origin, double r) : this(origin, r, new Color(0.7, 0.5, 0.3, 1.0)) { }

        public Sphere()
            : this(new Vektor(0, 0, 0), 1) { }


        public Sphere(Vektor origin, double r, bool nolabel)
        {
            IsActive = true;
            Origin = new Vektor(origin);
            R = r;
            this.Material = new Material();
            _RotatMatrix = Matrix3D.Identity;
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(Origin);
            _localMatrix = _RotatMatrix * _ShiftMatrix;
        }
        public Sphere(Vektor origin, double r)
        {
            SetLabelPrefix("sphere");
            IsActive = true;
            Origin = new Vektor(origin);
            R = r;
            this.Material = new Material();
            _RotatMatrix = Matrix3D.Identity;
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(Origin);
            _localMatrix = _RotatMatrix * _ShiftMatrix;

        }

        public Sphere(Vektor origin, double r, Colour c) : this(origin, r)
        {
            this.Material.Color = new Colour(c);
        }

        public Sphere(Sphere sp)
            : base(sp)
        {
            IsActive = sp.IsActive;
            this.R = sp.R;
            this.Origin = new Vektor(sp.Origin);
            this.Material = new Material(sp.Material);
            _localMatrix = sp._localMatrix;
            _ShiftMatrix = sp._ShiftMatrix;
            _RotatMatrix = sp._RotatMatrix;
        }



        /// <summary>
        /// Zjisti, zda paprsek protina kouli a vrati vsechny body pruniku
        /// </summary>
        /// <param name="P0">pocatek paprsku</param>
        /// <param name="P1">smerovy vektor paprsku</param>
        /// <param name="InterPoint">pripadny vysledny bod pruniku</param>
        /// <returns>true, kdyz existuje bod pruniku s paprskem</returns>
        public override bool Intersects(Vektor P0, Vektor P1, ref List<SolidPoint> InterPoint, bool isForLight, double lightDist)
        {
            if (!IsActive)
                return false;

            if (isForLight && InterPoint.Count > 0)
            {
                foreach (SolidPoint solp in InterPoint)
                    if (lightDist > solp.T) return true;
            }

            Interlocked.Increment(ref DefaultShape.TotalTested);

            Vektor P0minusOrigin = P0 - Origin;

            Vektor Pd = new Vektor(P1);
            Pd.Normalize();

            double B = P0minusOrigin * Pd;
            B *= 2;

            double C = P0minusOrigin * P0minusOrigin - R * R;

            double discr = B * B - 4 * C;

            if (discr < MyMath.EPSILON)
            {
                return false;
            }

            double odmoc = Math.Sqrt(discr);
            double t0 = (-B - odmoc) / 2;
            double t1 = (-B + odmoc) / 2;

            double jmenovatel = odmoc / 2;
            //double t0 = (-B - jmenovatel);
            //double t1 = (-B + jmenovatel);

            SolidPoint p1 = new SolidPoint();
            p1.T = t0;
            p1.Coord = P0 + Pd * t0;           // souradnice bodu pruniku
            p1.Normal = Vektor.ToDirectionVektor(Origin, p1.Coord);     // normala v bode pruniku (smerem od pocatku)
            p1.Normal.Normalize();             // nebo p1.Normal = p1 * (1/_r);
            p1.Color = new Colour(this.Material.Color);
            p1.Material = this.Material;
            p1.Shape = this;


            SolidPoint p2 = new SolidPoint();
            p2.T = t1;
            p2.Coord = P0 + Pd * t1;           // souradnice bodu pruniku
            p2.Normal = Vektor.ToDirectionVektor(Origin, p2.Coord);     // normala v bode pruniku (smerem od pocatku)
            p2.Normal.Normalize();
            p2.Color = new Colour(this.Material.Color);
            p2.Material = this.Material;
            p2.Shape = this;


            //t0 = Math.Max(t0, t1);

            //if (t0 > 0)
                InterPoint.Add(p1);
            //else
                InterPoint.Add(p2);

            return true;
        }


        /// <summary>
        /// Moves the shpere according to given differences
        /// </summary>
        /// <param name="dx">x-direction move</param>
        /// <param name="dy">y-direction move</param>
        /// <param name="dz">z-direction move</param>
        public override void Move(double dx, double dy, double dz)
        {
            //Vektor dVec = new Vektor(dx, dy, dz);
            //this.Origin += dVec;
            this.Origin.X += dx;
            this.Origin.Y += dy;
            this.Origin.Z += dz;
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(Origin);
        }

        public override void MoveToPoint(double dx, double dy, double dz)
        {
            //Vektor dVec = new Vektor(dx, dy, dz);
            //this.Origin += dVec;
            this.Origin.X = dx;
            this.Origin.Y = dy;
            this.Origin.Z = dz;
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(this.Origin);
            _localMatrix = _RotatMatrix * _ShiftMatrix;
        }

        public override string ToString()
        {
            return Label + " {Center=" + Origin + "; R=" + R + "}";
        }

        public override void Rotate(double degAroundX, double degAroundY, double degAroundZ)
        {
            _RotatMatrix = Matrix3D.NewRotateByDegrees(degAroundX, degAroundY, degAroundZ);
            _localMatrix = _RotatMatrix * _ShiftMatrix;
        }

        public override DefaultShape FromDeserial()
        {
            Sphere sph = new Sphere(this.Origin, this.R, true);
            if (this.Label != null)
                sph.Label = this.Label;
            sph.Material = this.Material;
            sph.IsActive = this.IsActive;
            return sph;
        }
    }
}
