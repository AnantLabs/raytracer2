using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracerLib
{

    /// <summary>
    /// Zakladni manipulace a pocitani s vektory pouzite v cele aplikaci
    /// </summary>
    public class Vektor
    {

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vektor() : this(0, 0, 0) { }

        public Vektor(Vektor a) : this(a.X, a.Y, a.Z) { }

        public Vektor(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vektor(double[] array)
        {
            if (array.Length != 3)
                throw new Exception("velikost vektoru musi byt 3");
            X = array[0];
            Y = array[1];
            Z = array[2];
        }

        public static Vektor ZeroVektor = new Vektor(0, 0, 0);

        public double[] GetArray()
        {
            double[] arr = { X, Y, Z };
            return arr;
        }

        public static Vektor operator +(Vektor a, Vektor b)
        {
            Vektor vysl = new Vektor();
            vysl.X = a.X + b.X;
            vysl.Y = a.Y + b.Y;
            vysl.Z = a.Z + b.Z;
            return vysl;
        }

        public void Add(Vektor a)
        {
            this.X += a.X;
            this.Y += a.Y;
            this.Z += a.Z;
        }

        public static Vektor operator -(Vektor a, Vektor b)
        {
            Vektor vysl = new Vektor();
            vysl.X = a.X - b.X;
            vysl.Y = a.Y - b.Y;
            vysl.Z = a.Z - b.Z;
            return vysl;
        }

        public static bool operator ==(Vektor a, Vektor b)
        {
            // If both are null, return true.
            if (((object)a == null) && ((object)b == null))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return (a.X == b.X && a.Y == b.Y && a.Z == b.Z);
        }

        public static bool operator !=(Vektor a, Vektor b)
        {
            return !(a == b);
        }

        public void Subtract(Vektor a)
        {
            this.X -= a.X;
            this.Y -= a.Y;
            this.Z -= a.Z;
        }

        /// <summary>
        /// skalarni soucin dvou vektoru
        /// </summary>
        public static double operator *(Vektor a, Vektor b)
        {
            return (a.X * b.X + a.Y * b.Y + a.Z * b.Z);
        }

        public double SkalarniSoucin()
        {
            return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
        }

        /// <summary>
        /// vektor nasobeny skalarem
        /// </summary>
        public static Vektor operator *(Vektor a, double c)
        {
            return new Vektor(a.X * c, a.Y * c, a.Z * c);

        }
        /// <summary>
        /// vynasobi vektor skalarem
        /// </summary>
        public void MultiplyBy(double c)
        {
            this.X *= c;
            this.Y *= c;
            this.Z *= c;
        }

        /// <summary>
        /// Vektorovy soucin
        /// </summary>
        /// <returns>vektor kolmy na oba vektory</returns>
        public static Vektor CrossProduct(Vektor a, Vektor b)
        {
            Vektor vysl = new Vektor();
            vysl.X = a.Y * b.Z - a.Z * b.Y;
            vysl.Y = a.Z * b.X - a.X * b.Z;
            vysl.Z = a.X * b.Y - a.Y * b.X;
            return vysl;
        }

        /// <summary>
        /// Norma vektoru - jeho velikost
        /// tj: sqrt(a*a)
        /// </summary>
        public static double Size(Vektor a)
        {
            double size = Math.Sqrt(a * a);
            if (size < MyMath.EPSILON)
                return 0.0;
            else
                return size;
        }

        /// <summary>
        /// Norma vektoru - jeho velikost
        /// tj: sqrt(X^2 + Y^2 + Z^2)
        /// </summary>
        public double Size()
        {
            double size = Math.Sqrt(this * this);
            if (size < MyMath.EPSILON)
                return 0.0;
            else
                return size;
        }

        /// <summary>
        /// Normalizace vektoru
        /// tj: [x/size, y/size, z/size]
        /// </summary>
        public void Normalize()
        {
            double size = Size();
            if (size < MyMath.EPSILON)
                return;
            this.X /= size;
            this.Y /= size;
            this.Z /= size;
        }

        public static void Normalize(Vektor v)
        {
            double size = v.Size();
            if (size < MyMath.EPSILON)
                return;
            v.X /= size;
            v.Y /= size;
            v.Z /= size;
        }

        /// <summary>
        /// normalizuje vektor, ale nemeni ten puvodni
        /// </summary>
        /// <param name="v">vektor, ktery chceme normalizovat; zustane stejny</param>
        /// <returns>nova instance vektoru v, ktera je normalizovana</returns>
        public static Vektor NormalizeStay(Vektor v)
        {
            double size = v.Size();
            if (size < MyMath.EPSILON)
                return v;
            Vektor n = new Vektor(v);
            n.X /= size;
            n.Y /= size;
            n.Z /= size;
            return n;
        }

        /// <summary>
        /// projekce vektoru na jiny vektor
        /// </summary>
        /// <param name="v1">vektor, na ktery promitame</param>
        /// <param name="v2">vektor, ktery promitame </param>
        /// <returns></returns>
        public static Vektor Projection(Vektor v1, Vektor v2)
        {
            double cosAngle = (v1 * v2) / (v1.Size() * v2.Size());

            Vektor proj = new Vektor(v1);
            proj = proj * v1.Size() * v2.Size() * cosAngle;

            return proj;

        }

        /// <summary>
        /// Z dvou bodu vytvori vektor (p2 - p1) smerujici od p1 do p2
        /// </summary>
        /// <param name="p1">pocatek</param>
        /// <param name="p2">konec</param>
        /// <returns>smerovy vektor</returns>
        public static Vektor ToDirectionVektor(Vektor p1, Vektor p2)
        {
            double x, y, z;
            x = p2.X - p1.X;
            y = p2.Y - p1.Y;
            z = p2.Z - p1.Z;
            return new Vektor(x, y, z);
        }

        public override string ToString()
        {
            return "[ " + X + " ; " + Y + " ; " + Z + " ]";
        }

    }
}
