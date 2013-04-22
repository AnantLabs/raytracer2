using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;
using System.Threading;
using System.Runtime.Serialization;

namespace RayTracerLib
{
    [DataContract]
    public class Cone : DefaultShape
    {
        /// <summary>
        /// Vrchol kuzele
        /// </summary>
        [DataMember]
        public Vektor Peak { get; private set; }

        /// <summary>
        /// Osa kuzele - smerujici z vrcholu k podstave
        /// </summary>
        [DataMember]
        public Vektor Dir { get; private set; }

        /// <summary>
        /// Stred kuzele na podstave
        /// </summary>
        public Vektor Center { get; private set; }

        /// <summary>
        /// polomer podstravy
        /// </summary>
        [DataMember]
        public double Rad { get; private set; }

        /// <summary>
        /// vyska kuzele
        /// </summary>
        [DataMember]
        public double Height { get; private set; }

        private Vektor DirNom;
        private double S;
        Plane Bottom;

        public Cone() : this(new Vektor(1, 2, 3), new Vektor(1, 1, 1), 2, 6) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peak">vrchol kuzele</param>
        /// <param name="dir">osa kuzele (nemusi byt normalizovana)</param>
        /// <param name="rad">polomer podstavy</param>
        /// <param name="height">vyska kuzele</param>
        public Cone(Vektor peak, Vektor dir, double rad, double height)
        {

            //////_ShiftMatrix = Matrix3D.PosunutiNewMatrix(c.X, c.Y, c.Z);

            //////Vektor yAxe = new Vektor(0, 1, 0);
            //////Quaternion q = new Quaternion(new Vektor (dir), yAxe);
            //////q.Transpose();
            //////double[] degss = q.ToEulerDegs();

            //////// rotace z kvaternionu je opacne orientovana, proto minuska
            //////Matrix3D matCr = Matrix3D.NewRotateByDegrees(-degss[0], -degss[1], -degss[2]); 
            //////_RotatMatrix = matCr;
            //////_localMatrix = _RotatMatrix * _ShiftMatrix;
            //Vektor p1 = _localMatrix.Transform2NewPoint(dir);
            //p1 = _localMatrix.Transform2NewPoint(yAxe);
            //Matrix3D transp = _localMatrix.Transpose();
            //Matrix3D transpShif = _ShiftMatrix.Transpose();
            //Matrix3D transpRot = _RotatMatrix.Transpose();
            //Vektor p2 = transpShif.Transform2NewPoint(p1);
            //p2 = transpRot.Transform2NewPoint(p2);
            //_ShiftMatrix.TransformPoint(p2);

            //p1 = transp.Transform2NewPoint(p1);
            //p1 = transp.Transform2NewPoint(yAxe);
            SetLabelPrefix("cone");
            IsActive = true;
            Material = new Material();
            this.SetValues(peak, dir, rad, height);
        }

        public Cone(Cone old)
            : base(old)
        {
            this.IsActive = old.IsActive;
            this.Peak = new Vektor(old.Peak);
            this.Center = new Vektor(old.Center);
            this.DirNom = new Vektor(old.DirNom);
            this.Dir = new Vektor(old.Dir);
            this.Height = old.Height;
            this.Material = new Material(old.Material);
            this.Rad = old.Rad;
            this.S = old.S;
            Bottom = new Plane(old.Bottom);
            SetValues(Peak, Dir, Rad, Height);
        }

        public void SetValues(Vektor peak, double rad, double height, double degX, double degY, double degZ)
        {
            Peak = peak;

            Rad = rad;
            Height = height;
            _RotatMatrix = Matrix3D.NewRotateByDegrees(degX, degY, degZ);

            Dir = new Vektor(0, 1, 0);
            //_RotatMatrix.TransformPoint(Dir);
            this.DirNom = new Vektor(Dir);
            DirNom.Normalize();

            Center = Peak + DirNom * height;
            double tt = Center.Size();
            double T = Rad / Height;
            S = 1 + T * T;

            Bottom = new Plane(DirNom, -(Center * DirNom), this.Material);

            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(peak.X, peak.Y, peak.Z);
            _localMatrix = _RotatMatrix * _ShiftMatrix;
        }
        /// <summary>
        /// prenastaveni kuzele
        /// </summary>
        /// <param name="peak">vrchol kuzele</param>
        /// <param name="dir">osa kuzele (nemusi byt normalizovana)</param>
        /// <param name="rad">polomer podstavy</param>
        /// <param name="height">vyska kuzele</param>
        public void SetValues(Vektor peak, Vektor dir, double rad, double height)
        {
            Peak = peak;
            
            Rad = rad;
            Height = height;

            Dir = dir;
            this.DirNom = new Vektor(dir);
            DirNom.Normalize();

            Center = Peak + DirNom * height;
            double tt = Center.Size();
            double T = Rad / Height;
            S = 1 + T * T;

            // rovina podstavy: C*Norm = D
            // musi pak platit: C * Norm + D = 0
            // vime: C .. stred roviny
            //       Norm .. normala roviny = Dir nebo -Dir
            // chceme: D
            // D = Center * DirNorm

            Bottom = new Plane(DirNom, -(Center * DirNom), this.Material);

            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(peak.X, peak.Y, peak.Z);

            Vektor yAxe = new Vektor(0, 1, 0);
            if (dir == new Vektor(0,-1,0))
                dir.X = 0.0001;

            Quaternion q = new Quaternion(yAxe, new Vektor(dir));
            //q.Transpose();
            double[] degss = q.ToEulerDegs();

            // rotace z kvaternionu je opacne orientovana, proto minuska
            Matrix3D matCr = Matrix3D.NewRotateByDegrees(-degss[0], -degss[1], -degss[2]);
            
            double[] degss3 = matCr.GetAnglesFromMatrix();
            _RotatMatrix = matCr;
            _localMatrix = _RotatMatrix * _ShiftMatrix;
        }

        private void SetValues(Vektor peak, Matrix3D rotatMatrix, double rad, double height)
        {
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(peak.X, peak.Y, peak.Z);
            _RotatMatrix = rotatMatrix;
            _localMatrix = _RotatMatrix * _ShiftMatrix;

            Vektor p = Vektor.ZeroVektor;
            Vektor dir = new Vektor(0, 1, 0);
            _ShiftMatrix.TransformPoint(p);
            _RotatMatrix.TransformPoint(dir);
            this.SetValues(p, dir, rad, height);

            double[] degs = _RotatMatrix.GetAnglesFromMatrix();
            double[] degs2 = rotatMatrix.GetAnglesFromMatrix();

            

            //_localMatrix.TransformPoint(this.Peak);
            //_localMatrix.TransformPoint(this.Dir);
            //_localMatrix.TransformPoint(this.DirNom);
            //_localMatrix.TransformPoint(this.Center);
        }

        public override bool Intersects(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint, bool isForLight, double lightDist)
        {
            if (!IsActive)
                return false;

            if (isForLight && InterPoint.Count > 0)
            {
                foreach (SolidPoint solp in InterPoint)
                    if (lightDist > solp.T) return true;
            }

            Interlocked.Increment(ref DefaultShape.TotalTested);

            Pd.Normalize();

            bool toReturn = false;
            /////////////////////////////////////////
            // 1) prunik paprsku s podstavou

            List<SolidPoint> BasePoints = new List<SolidPoint>();
            Bottom.Intersects(P0, Pd, ref BasePoints, isForLight, lightDist);

            double planeT = 0.0;
            foreach (SolidPoint sps in BasePoints)
            {
                if (sps.T < MyMath.EPSILON) continue;
                if (MyMath.Distance2Points(sps.Coord, Center) <= this.Rad)
                {
                    sps.Shape = this;
                    sps.Material = this.Material;
                    InterPoint.Add(sps);
                    planeT = sps.T;
                    toReturn = true;
                }
            }


            /////////////////////////////////////////
            // 2) prunik paprsku s plastem

            Vektor p = P0 - Peak;

            double pp = p * p;
            double pu = p * Pd;
            double uu = Pd * Pd;
            double uw = Pd * DirNom;
            double pw = p * DirNom;

            double a = uu - S * uw * uw;
            double b = 2 * (pu - S * pw * uw);
            double c = pp - S * pw * pw;

            double discr = b * b - 4 * a * c;

            if (discr < 0.0) return toReturn;

            double discrSqrt = Math.Sqrt(discr);
            double t1 = (-b - discrSqrt) / (2 * a);
            double t2 = (-b + discrSqrt) / (2 * a);

            // vybereme mensi t, ktere je kladne
            double tMin = Math.Min(t1, t2);
            double tMax = Math.Max(t1, t2);

            double t = tMin;
            if (tMin < MyMath.EPSILON)
                t = tMax;
            if (t < MyMath.EPSILON)
                return toReturn;
            Vektor Point = P0 + Pd * t;

            //if (1 - Math.Abs(uu) < MyMath.EPSILON)//( uw > 0 && 1 - Math.Abs(uw) < 0.001))
            //{
            //    t = tMax;
            //    if (planeT > 0)
            //        if (tMax - planeT > 0)
            //        {
            //            t = tMin;
            //        }

            //    Point = P0 + Pd * t;
            //}
            //if (1 - Math.Abs(uw) > 0.0628 && tMin > 0)//0.0628
            //{
            //    t = tMin;
            //    Point = P0 + Pd * t;
            //}

            if ( uw > 0 && 1 - Math.Abs(uw) < 0.001)
            {
                t = tMax;
                Point = P0 + Pd * t;
            }
            Vektor q = Point - Peak;
            Vektor q0 = q - DirNom * (q * DirNom);
            
            //q0.Normalize();
            Vektor qt = Vektor.CrossProduct(DirNom, q0);
            Vektor len = DirNom * (q * DirNom);
            double leng = len.Size();
            if (leng > Height) return toReturn;
            //qt.Normalize();
            Vektor n = Vektor.CrossProduct(qt, q);
            n.Normalize();
            //// TED MAME DVA KUZELE, Z NICHZ MUSIME VYBRAT JEN TEN SPRAVNEJ
            // pro nej plati, ze bod pruniku lezi v kladne polorovine:
            double qw = q * DirNom;
            if (qw < 0.0 || t < MyMath.EPSILON) return toReturn;

            SolidPoint sp = new SolidPoint();
            sp.Color = this.Material.Color;
            sp.Coord = Point;
            sp.T = t;
            sp.Normal = n;
            sp.Shape = this;
            sp.Material = this.Material;

            InterPoint.Add(sp);
            return true;
        }
        public bool Intersects2(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint)
        {
            if (!IsActive) return false;

            Interlocked.Increment(ref DefaultShape.TotalTested);

            bool toReturn = false;
            /////////////////////////////////////////
            // 1) prunik paprsku s podstavou
            
            List<SolidPoint> BasePoints = new List<SolidPoint>();
            Bottom.Intersects(P0, Pd, ref BasePoints, false, 0);
            
            foreach (SolidPoint sps in BasePoints)
            {
                if (MyMath.Distance2Points(sps.Coord, Center) <= this.Rad)
                {
                    sps.Shape = this;
                    sps.Material = this.Material;
                    InterPoint.Add(sps);
                    toReturn = true;
                }
            }


            /////////////////////////////////////////
            // 2) prunik paprsku s plastem
            
            Vektor p = P0 - Peak;

            double pp = p * p;
            double pu = p * Pd;
            double uu = Pd * Pd;
            double uw = Pd * DirNom;
            double pw = p * DirNom;

            double a = uu - S * uw * uw;
            double b = 2 * (pu - S * pw * uw);
            double c = pp - S * pw * pw;

            double discr = b * b - 4 * a * c;

            if (discr < 0.0) return toReturn;

            double discrSqrt = Math.Sqrt(discr);
            double t1 = (-b - discrSqrt) / (2 * a);
            double t2 = (-b + discrSqrt) / (2 * a);

            // vybereme mensi t, ktere je kladne
            double tMin = Math.Min(t1, t2);
            double tMax = Math.Max(t1, t2);

            double t = tMin;
            if (tMin < MyMath.EPSILON)
                t = tMax;
            if (t < MyMath.EPSILON)
                return toReturn;
            //t = tMax;
            Vektor Point = P0 + Pd * t;
            double dasda = Math.Acos(uw);
            if (uw > 0 &&  1-Math.Abs(uw) < 0.001)
            {
                t = tMax;
                Point = P0 + Pd * t;
            }
            //if ( uw > 0 &&tMin * tMax < 0)// uw>0 &&Math.Acos(uw) <0.2)//(uw > 0 && 1 - Math.Abs(uw) > 0.0 && 1 - Math.Abs(uw) < 0.01)
            //{
            //    t = tMax;
            //    Point = P0 + Pd * t;
            //}
            //else if (uw < 0 && 1 - Math.Abs(uw) < 0.001)// && tMin * tMax > 0)
            //{
            //    t = tMin;
            //    Point = P0 + Pd * t;
            //}
            Vektor q = Point - Peak;
            Vektor q0 = q - DirNom * (q * DirNom);
            //q0.Normalize();
            Vektor qt = Vektor.CrossProduct(DirNom, q0);
            Vektor len = DirNom * (q * DirNom);
            double leng = len.Size();
            if (leng > Height) return toReturn;
            //qt.Normalize();
            Vektor n = Vektor.CrossProduct(qt, q);
            n.Normalize();

            //// TED MAME DVA KUZELE, Z NICHZ MUSIME VYBRAT JEN TEN SPRAVNEJ
            // pro nej plati, ze bod pruniku lezi v kladne polorovine:
            double qw = q * DirNom;
            if (qw < 0.0) return toReturn;

            SolidPoint sp = new SolidPoint();
            sp.Color = this.Material.Color;
            sp.Coord = Point;
            sp.T = t;
            sp.Normal = n;
            sp.Shape = this;
            sp.Material = this.Material;

            InterPoint.Add(sp);
            return true;
        }

        public override void Move(double dx, double dy, double dz)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Label + " {Peak=" + this.Peak + "; Dir=" + this.Dir + "}";
        }

        public override void MoveToPoint(double p1, double p2, double p3)
        {
            Vektor newPeak = new Vektor(p1, p2, p3);
            this.SetValues(newPeak, this.Dir, this.Rad, this.Height);
        }

        public override void Rotate(double degX, double degY, double degZ)
        {
            Matrix3D newRot = Matrix3D.NewRotateByDegrees(degX, degY, degZ);

            Vektor yAxe = new Vektor(0,1, 0);
            newRot.TransformPoint(yAxe);

            // 1) pretransformovat vsechny vektory do puvodniho (zakladniho) tvaru
            // nejspis to neni potreba, jelikoz je vypocitavame cele znovu
            //this.Peak = transpLoc.Transform2NewPoint(this.Peak);
            //this.Center = transpLoc.Transform2NewPoint(this.Center);
            //this.Dir = transpLoc.Transform2NewPoint(this.Dir);

            // 2) nastavit novou matici
            this._RotatMatrix = newRot;
            _localMatrix = _RotatMatrix * _ShiftMatrix;
            double[] degss2 = _RotatMatrix.GetAnglesFromMatrix();

            // 3) prenastavit objekt podle nove matice
            this.SetValues(this.Peak, yAxe, this.Rad, this.Height);
            //this.SetValues(this.Peak, this.Rad, this.Height, degX, degY, degZ);
        }

        public override DefaultShape FromDeserial()
        {
            Cone c = new Cone(this.Peak, this.Dir, this.Rad, this.Height);
            c.Label = this.Label;
            c.IsActive = this.IsActive;
            c.Material = this.Material;
            return c;
        }

    }
}
