using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;
using System.Threading;

namespace RayTracerLib
{

    /// <summary>
    /// libovolne orientovana krychle
    /// </summary>
    public class Cube :DefaultShape
    {

        public Vektor Dir { get; private set; }
        public Vektor Center { get; private set; }
        public double Size { get; private set; }


        private double R;

        /// <summary>
        /// normalizovane vektor Dir
        /// </summary>
        private Vektor[] Dirs;

        /// <summary>
        /// koeficient pro vzdalenost bodu k nejlbizsi stene, zda je bod uvnitr krychle
        /// </summary>
        private double CoefT = 0.001;

        /// <summary>
        /// steny krychle
        /// </summary>
        private List<Plane> Planes;

        /// <summary>
        /// steny krychle s opacnymi normalami (normaly dovnitr)
        /// </summary>
        private List<Plane> PlanesOpp;


        public Cube() : this(new Vektor(0, 0, 0), new Vektor(0, 0, 1), 1) { }

        public Cube(Vektor center, Vektor dir, double size)
        {
            IsActive = true;
            this.Material = new Material();
            SetValues(center, dir, size);
        }



        public void SetValues(Vektor center, Vektor dir, double size)
        {
            Center = new Vektor(center);
            Size = size;
            R = size/2;
            Dir = new Vektor(dir);

            // Dir1, Dir2, Dir3 ... osy krychle na sebe kolme
            Vektor Dir1 = new Vektor(dir);
            Dir1.Normalize();

            Vektor Dir2 = new Vektor(-Dir1.Y, Dir1.X, Dir1.Z);
            Dir2 = Vektor.CrossProduct(Dir1, Dir2);
            Dir2.Normalize();

            double prod = Dir1 * Dir2;
            if (Math.Abs(prod) > MyMath.EPSILON)
                throw new Exception("Nevytvoren kolmy vektor");

            Vektor Dir3 = Vektor.CrossProduct(Dir1, Dir2);
            Dir3.Normalize();

            prod = Dir1 * Dir3;
            if (Math.Abs(prod) > MyMath.EPSILON)
                throw new Exception("Nevytvoren kolmy vektor");

            // pole vsech os
            Dirs = new Vektor[3];
            Dirs[0] = Dir1;
            Dirs[1] = Dir2;
            Dirs[2] = Dir3;

            // steny krychle
            Planes = new List<Plane>();

            Vektor cent;
            Vektor norm;
            Plane plane;
            double d;


            // pro kazdou osu urcime dve steny
            // stena = stredKrychle +- osa*R
            foreach (Vektor direction in Dirs)
            {
                cent = this.Center + direction * R;
                norm = new Vektor(direction);
                d = -(cent * norm);
                plane = new Plane(norm, d, this.Material);
                Planes.Add(plane);

                cent = this.Center - direction * R;
                norm = new Vektor(Vektor.ZeroVektor - direction);
                d = -(cent * norm);
                plane = new Plane(norm, d, this.Material);
                Planes.Add(plane);
            }

            PlanesOpp = new List<Plane>();
            // pro kazdou osu urcime dve steny
            // stena = stredKrychle +- osa*R
            foreach (Vektor direction in Dirs)
            {
                cent = this.Center + direction * R;
                norm = new Vektor(Vektor.ZeroVektor - direction);
                d = -(cent * norm);
                plane = new Plane(norm, d, this.Material);
                PlanesOpp.Add(plane);

                cent = this.Center - direction * R;
                norm = new Vektor(direction);
                d = -(cent * norm);
                plane = new Plane(norm, d, this.Material);
                PlanesOpp.Add(plane);
            }



            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(Center);
            Vektor yAxe = new Vektor(0, 1, 0);
            Quaternion q = new Quaternion(yAxe, new Vektor(dir));
            double[] degss = q.ToEulerDegs();

            // rotace z kvaternionu je opacne orientovana, proto minuska
            _RotatMatrix = Matrix3D.NewRotateByDegrees(-degss[0], -degss[1], -degss[2]);
            _localMatrix = _RotatMatrix * _ShiftMatrix;
        }

        /// <summary>
        /// zjisti, zda je bod umisten mezi vsemi stenami.
        /// Tj. zda bod lezi za kazdou z nich
        /// 
        /// kdyz pro kazdou stenu:
        ///     normala*bod + Dsteny < 0
        /// </summary>
        /// <param name="point"></param>
        /// <param name="walls"></param>
        /// <returns></returns>
        private bool IsInside(Vektor point, List<Plane> walls)
        {
            if (walls.Count < 6)
                throw new Exception("nemam 6 sten");

            double dstc = 0;


            foreach (Plane wall in walls)
            {
                dstc = wall.Normal * point + wall.D;

                // je-li (dsc<0), lezi bod za stenou (ve smeru vnitrku krychle)
                if (dstc > MyMath.EPSILON)
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Intersects(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint)
        {
            if (!IsActive)
                return false;

            Interlocked.Increment(ref DefaultShape.TotalTested);

            // 1) spocitani pruniku s prvni dvojici sten
            // rovina jedne podstavy: C*Norm = D
            // vime: C .. stred roviny
            //       Norm .. normala roviny = Dir nebo -Dir
            // chceme: D
            bool intersected = false;

            List<SolidPoint> BasePoints;
            List<SolidPoint> vysl = new List<SolidPoint>();

            // maximalne jeden prunik se stenou z Planes
            foreach (Plane wall in Planes)
            {
                BasePoints = new List<SolidPoint>();
                if (wall.Intersects(P0, Pd, ref BasePoints))
                    foreach (SolidPoint sps in BasePoints)
                    {

                        Vektor farerPoint = sps.Coord - sps.Normal * CoefT;
                        if (IsInside(farerPoint, Planes))
                        {
                            sps.Material = new Material(this.Material);
                            vysl.Add(sps);
                            intersected = true;
                            break;
                        }
                    }
                if (intersected)
                    break;
            }


            foreach (Plane wall in PlanesOpp)
            {
                BasePoints = new List<SolidPoint>();
                if (wall.Intersects(P0, Pd, ref BasePoints))
                    foreach (SolidPoint sps in BasePoints)
                    {

                        Vektor farerPoint = sps.Coord + sps.Normal * CoefT;
                        if (IsInside(farerPoint, Planes))
                        {
                            sps.Normal = Vektor.ZeroVektor - sps.Normal;
                            sps.Material = new Material(this.Material);
                            vysl.Add(sps);
                            intersected = true;
                        }
                    }
            }

            if (vysl.Count > 0)
            {
                foreach (SolidPoint sp in vysl)
                {
                    if (sp.T > 0 && sp.T * sp.T > MyMath.EPSILON)
                        InterPoint.Add(sp);
                }
                //InterPoint.AddRange(vysl);
                //SolidPoint s = InterPoint.Find(sp => sp.T > 0);
            }
            return intersected;
        }

        public override void Move(double dx, double dy, double dz)
        {
            throw new NotImplementedException();
        }

        public override void MoveToPoint(double dx, double dy, double dz)
        {
            //Vektor dVec = new Vektor(dx, dy, dz);
            //this.Origin += dVec;
            this.Center.X = dx;
            this.Center.Y = dy;
            this.Center.Z = dz;
            SetValues(Center, Dir, Size);
        }

        public override string ToString()
        {
            return "Cube: Center=" + Center + "; Axis=" + Dir;
        }

        public override void Rotate(double degAroundX, double degAroundY, double degAroundZ)
        {
            Matrix3D newRot = Matrix3D.NewRotateByDegrees(degAroundX, degAroundY, degAroundZ);

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
            this.SetValues(this.Center, yAxe, this.Size);
        }
    }
}
