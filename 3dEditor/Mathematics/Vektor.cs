using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;

namespace Mathematics
{

    /// <summary>
    /// Zakladni manipulace a pocitani s vektory pouzite v cele aplikaci
    /// </summary>
    [DataContract(Namespace="")]
    public class Vektor
    {

        [DataMember(Name="X")]
        public double X { get; set; }
        [DataMember(Name = "Y")]
        public double Y { get; set; }
        [DataMember(Name = "Z")]
        public double Z { get; set; }
        [DataMember(Name = "ZZ")]
        public double ZZ { get; set; }

        public double this[int i]
        {
            get
            {
                if (i == 0) return X;
                if (i == 1) return Y;
                if (i == 2) return Z;
                if (i == 3) return ZZ;
                return 0;
            }
            set
            {
                if (i == 0) X = value;
                if (i == 1) Y = value;
                if (i == 2) Z = value;
                if (i == 3) ZZ = value;
            }
        }

        /// <summary>
        /// (1,1,1)
        /// </summary>
        public static Vektor OneVektor { get { return new Vektor(1, 1, 1); } }

        /// <summary>
        /// (0,0,0)
        /// </summary>
        public static Vektor ZeroVektor { get { return new Vektor(0, 0, 0); } }

        public Vektor() : this(0, 0, 0) { }

        public Vektor(Vektor a) : this(a.X, a.Y, a.Z, a.ZZ) { }

        public Vektor(double x, double y, double z) : this(x, y, z, 1) { }

        public Vektor(double x, double y, double z, double zz)
        {
            this.X = x; this.Y = y; this.Z = z; ZZ = zz;
        }

        /// <summary>
        /// (constanta, constanta, constanta)
        /// </summary>
        /// <param name="constanta"></param>
        public Vektor(double constanta) : this(constanta, constanta, constanta) { }

        public Vektor(double[] array)
        {
            if (array.Length == 3)
            {
                X = array[0];
                Y = array[1];
                Z = array[2];
                ZZ = 1;
            }
            else if (array.Length == 4)
            {
                X = array[0];
                Y = array[1];
                Z = array[2];
                ZZ = array[3];
            }
            else throw new Exception("velikost vektoru musi byt 3 nebo 4");
        }
        /// <summary>
        /// nastavi souradnice podle zadaneho vektoru
        /// Nevytvori pri tom novy vektor
        /// </summary>
        /// <param name="a">vzorovy vektor</param>
        public void Set(Vektor a)
        {
            this.X = a.X;
            this.Y = a.Y;
            this.Z = a.Z;
            this.ZZ = a.ZZ;
        }

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
        public Vektor MultiplyBy(double c)
        {
            this.X *= c;
            this.Y *= c;
            this.Z *= c;
            return this;
        }

        public Vektor CrossProduct(Vektor a)
        {
            return Vektor.CrossProduct(this, a);
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
            double size = Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
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
            double size = Math.Sqrt( X * X + Y * Y + Z * Z);
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

        public void MultiplyByMatrix(Matrix3D matrix)
        {
            Vektor newPoint = matrix * this;
            this.X = newPoint.X;
            this.Y = newPoint.Y;
            this.Z = newPoint.Z;
            this.ZZ = newPoint.ZZ;
        }
        /// <summary>
        /// vypocita uhel mezi dvema vektory
        /// </summary>
        /// <param name="vec">druhy vektor</param>
        /// <returns>uhel ve stupnich</returns>
        public double AngleDeg(Vektor vec)
        {
            double angle = this * vec;
            angle = Math.Acos(angle);
            angle = MyMath.Radians2Deg(angle);
            return angle;
        }

        /// <summary>
        /// vrati kolmy vektor
        /// </summary>
        /// <returns></returns>
        public Vektor GetOrthogonal()
        {
            Vektor orto = new Vektor(this.Z, this.Z, -this.X - this.Y);
            if (Math.Abs(orto * this) > MyMath.EPSILON)
            {
                orto = new Vektor(-this.Y - this.Z, this.X, this.X);
                if (Math.Abs(orto * this) > MyMath.EPSILON)
                {
                    orto = new Vektor(this.Y, -this.X - this.Z, this.Y);
                }
            }
            return orto;
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

        public PointF To2D()
        {
            return new PointF((float)X, (float)Y);
        }

        /// <summary>
        /// Delegat na projekci
        /// </summary>
        /// <param name="vector">bod, ktery je promitan</param>
        /// <param name="scale"></param>
        /// <param name="zoom"></param>
        /// <param name="centerPoint"></param>
        public delegate PointF DelegateTo2D(Vektor vector, int scale, int zoom, Point centerPoint);
        public static DelegateTo2D To2DProj = new DelegateTo2D(To2D_Orto);

        public static PointF To2D_Persp(Vektor vector, int scale, int zoom, Point centerPoint)
        {
            double maxZ = 30.0;
            double zz = vector.Z;
            if (-vector.Z > maxZ)
                zz = -maxZ + 1;
            else if (vector.Z > maxZ)
                zz = maxZ - 1;
            Matrix3D matr = new Matrix3D(new Vektor(zoom, 0, 0, 0), new Vektor(0, zoom, 0, 0), new Vektor(0, 0, 0, 0), new Vektor(0, 0, 1.0 / maxZ, 1));
            Vektor vec = new Vektor(vector.X, vector.Y, zz, vector.ZZ);
            matr.TransformPoint(vec);
            if (vec.ZZ == 0.0)
            {
                vec.ZZ = 0.01;
            }
            PointF point = new PointF((float)(vec.X / vec.ZZ), (float)(vec.Y / vec.ZZ));
            point.X = point.X + centerPoint.X;
            point.Y = point.Y + centerPoint.Y;
            return point;
        }

        public static PointF To2D_Orto(Vektor vector, int scale, int zoom, Point centerPoint)
        {
            PointF point = new PointF((float)vector.X, (float)vector.Y);
            float zFloat = (float)vector.Z;
            float divide = (zFloat + scale) / zoom;
            float temp = scale / divide;
            point.X = point.X * temp + centerPoint.X;
            point.Y = point.Y * temp + centerPoint.Y;
            return point;
        }
        /// <summary>
        /// Perspektivni promitani
        /// </summary>
        public PointF To2D1(int scale, int zoom, Point centerPoint)
        {
            double maxZ = 20.0;
            double zz = Z;
            if (-Z > maxZ)
                zz = -maxZ + 1;
            Matrix3D matr = new Matrix3D(new Vektor(zoom, 0, 0, 0), new Vektor(0, zoom, 0, 0), new Vektor(0, 0, 0, 0), new Vektor(0, 0, 1.0/maxZ, 1));
            Vektor vec = new Vektor(X , Y , zz , ZZ);
            matr.TransformPoint(vec);
            PointF point = new PointF((float)(vec.X / vec.ZZ), (float)(vec.Y / vec.ZZ));
            point.X = point.X  + centerPoint.X;
            point.Y = point.Y  + centerPoint.Y;
            return point;
        }
        /// <summary>
        /// ortogonalni promitani
        /// </summary>
        public PointF To2D(int scale, int zoom, Point centerPoint)
        {
            return To2DProj(this, scale, zoom, centerPoint);
        }
        public static Vektor To3D_From2D(PointF p2d, double z, int scale, int zoom, Point centerPoint)
        {
            Vektor point = new Vektor(p2d.X, p2d.Y, 0);
            float divide = ((float)z + scale) / zoom;
            point.X = (point.X - centerPoint.X) * divide / scale;
            point.Y = (point.Y - centerPoint.Y) * divide / scale;
            point.Z = z;
            return point;
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

        public void Otoceni(double xRad, double yRad, double zRad)
        {
            Matrix3D matrix = Matrix3D.NewRotateByRads(xRad, yRad, zRad);
            Vektor p = matrix * this;
            X = p.X;
            Y = p.Y;
            Z = p.Z;
        }
        public void Posunuti(double Px, double Py, double Pz)
        {
            Vektor p = Matrix3D.Posunuti(this, Px, Py, Pz);
            X = p.X;
            Y = p.Y;
            Z = p.Z;
        }


        public void Scale(double Sx, double Sy, double Sz)
        {
            Matrix3D m = Matrix3D.ScalingNewMatrix(Sx, Sy, Sz);
            this.MultiplyByMatrix(m);
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
            return "[ " + Math.Round(X, 1) + " ; " + Math.Round(Y, 1) + " ; " + Math.Round(Z, 1) + " ]";
        }

    }
}
