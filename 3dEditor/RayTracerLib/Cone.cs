using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{
    public class Cone : DefaultShape
    {
        /// <summary>
        /// Vrchol kuzele
        /// </summary>
        public Vektor Peak { get; private set; }

        /// <summary>
        /// Osa kuzele - smerujici z vrcholu k podstave
        /// </summary>
        public Vektor Dir { get; private set; }

        /// <summary>
        /// Stred kuzele na podstave
        /// </summary>
        public Vektor Center { get; private set; }

        /// <summary>
        /// polomer podstravy
        /// </summary>
        public double Rad { get; private set; }

        /// <summary>
        /// vyska kuzele
        /// </summary>
        public double Height { get; private set; }

        private Vektor DirNom;
        private double S;
        Plane Bottom;

        public Cone() : this(new Vektor(1, 2, 3), new Vektor(1, 1, 1), 2, 6) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c">vrchol kuzele</param>
        /// <param name="dir">osa kuzele (nemusi byt normalizovana)</param>
        /// <param name="rad">polomer podstavy</param>
        /// <param name="height">vyska kuzele</param>
        public Cone(Vektor c, Vektor dir, double rad, double height)
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

            IsActive = true;
            Material = new Material();
            this.SetValues(c, dir, rad, height);
        }

        public Cone(Cone old)
        {
            this.IsActive = old.IsActive;
            this.Peak = new Vektor(old.Peak);
            this.Center = new Vektor(old.Center);
            this.DirNom = new Vektor(old.DirNom);
            this.Height = old.Height;
            this.Material = new Material(old.Material);
            this.Rad = old.Rad;
            this.S = old.S;
        }


        /// <summary>
        /// prenastaveni kuzele
        /// </summary>
        /// <param name="c">vrchol kuzele</param>
        /// <param name="dir">osa kuzele (nemusi byt normalizovana)</param>
        /// <param name="rad">polomer podstavy</param>
        /// <param name="height">vyska kuzele</param>
        public void SetValues(Vektor c, Vektor dir, double rad, double height)
        {
            Peak = c;
            
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

            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(c.X, c.Y, c.Z);

            Vektor yAxe = new Vektor(0, 1, 0);
            Quaternion q = new Quaternion(yAxe, new Vektor(dir));
            //q.Transpose();
            double[] degss = q.ToEulerDegs();

            // rotace z kvaternionu je opacne orientovana, proto minuska
            Matrix3D matCr = Matrix3D.NewRotateByDegrees(-degss[0], -degss[1], -degss[2]);
            _RotatMatrix = matCr;
            _localMatrix = _RotatMatrix * _ShiftMatrix;

            //_ShiftMatrix = Matrix3D.PosunutiNewMatrix(c.X, c.Y, c.Z);

            //Vektor yAxe = new Vektor(0, 1, 0);

            //Quaternion q2 = new Quaternion(DirNom, yAxe);
            //q2.Transpose();
            //double[] degs2 = q2.ToEulerDegs();
            //Matrix3D m2 = q2.Matrix();
            //degs2 = m2.GetAnglesFromMatrix();
            //Vektor p2 = m2.Transform2NewPoint(yAxe);
            //p2 = q2.Rotate(DirNom);
            //p2 = q2.Rotate(yAxe);

            //Vektor crossProd = DirNom.CrossProduct(yAxe);
            //double theta = Math.Acos(DirNom * yAxe);
            //Quaternion quatern = new Quaternion(crossProd, MyMath.Radians2Deg(theta));
            //double[] degss = quatern.ToEulerDegs();
            //Matrix3D matQrt = quatern.Matrix();
            //Vektor pointTest = matQrt.Transform2NewPoint(DirNom);
            //pointTest = quatern.Rotate(DirNom);
            //pointTest = quatern.Rotate(yAxe);

            //matQrt = matQrt.Transpose();
            //pointTest = matQrt.Transform2NewPoint(yAxe);

            //Matrix3D matCr = Matrix3D.NewRotateByDegrees(-degss[0], -degss[1], -degss[2]);
            //pointTest = matCr.Transform2NewPoint(DirNom);
            ////matCr = matCr.Transpose();
            //pointTest = matCr.Transform2NewPoint(yAxe);

            ////matCr = matCr.Transpose();
            //_RotatMatrix = matCr;
            ////_RotatMatrix = q2.Matrix();
            //double[] degss2 = _RotatMatrix.GetAnglesFromMatrix();
            //degss2 = matCr.GetAnglesFromMatrix();


            //_localMatrix = _RotatMatrix * _ShiftMatrix;

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

        public override bool Intersects(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint)
        {
            if (!IsActive) return false;

            bool toReturn = false;
            /////////////////////////////////////////
            // 1) prunik paprsku s podstavou
            
            List<SolidPoint> BasePoints = new List<SolidPoint>();
            Bottom.Intersects(P0, Pd, ref BasePoints);
            
            foreach (SolidPoint sps in BasePoints)
            {
                if (MyMath.Distance2Points(sps.Coord, Center) <= this.Rad)
                {
                    sps.Shape = this;
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

            if (discr < MyMath.EPSILON) return toReturn;

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
            if (qw <= 0) return toReturn;

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
            return "Cone: Peak=" + this.Peak + "; Dir=" + this.Dir;
        }

        public override void MoveToPoint(double p1, double p2, double p3)
        {
            Vektor newPeak = new Vektor(p1, p2, p3);
            this.SetValues(newPeak, this.Dir, this.Rad, this.Height);
        }

        public override void Rotate(double degX, double degY, double degZ)
        {
            Matrix3D newRot = Matrix3D.NewRotateByDegrees(degX, degY, degZ);

            Vektor yAxe = new Vektor(0, 1, 0);
            newRot.TransformPoint(yAxe);

            // 1) pretransformovat vsechny vektory do puvodniho (zakladniho) tvaru
            // nejspis to neni potreba, jelikoz je vypocitavame cele znovu
            //this.Peak = transpLoc.Transform2NewPoint(this.Peak);
            //this.Center = transpLoc.Transform2NewPoint(this.Center);
            //this.Dir = transpLoc.Transform2NewPoint(this.Dir);

            // 2) nastavit novou matici
            this._RotatMatrix = newRot;
            _localMatrix = _RotatMatrix * _ShiftMatrix;

            // 3) prenastavit objekt podle nove matice
            this.SetValues(this.Peak, yAxe, this.Rad, this.Height);
        }
    }
}
