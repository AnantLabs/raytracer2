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
    /// libovolne orientovana krychle
    /// </summary>
    [DataContract]
    public class Cube :DefaultShape
    {

        [DataMember]
        public Vektor Dir { get; private set; }
        [DataMember]
        public Vektor Center { get; private set; }
        /// <summary>
        /// delka cele steny
        /// </summary>
        [DataMember]
        public double Size { get; private set; }


        private Vektor MinPoint = new Vektor();
        private Vektor MaxPoint = new Vektor();

        public double Xmin { get { return MinPoint.X; } private set { MinPoint.X = value; } }
        public double Ymin { get { return MinPoint.Y; } private set { MinPoint.Y = value; } }
        public double Zmin { get { return MinPoint.Z; } private set { MinPoint.Z = value; } }

        public double Xmax { get { return MaxPoint.X; } private set { MaxPoint.X = value; } }
        public double Ymax { get { return MaxPoint.Y; } private set { MaxPoint.Y = value; } }
        public double Zmax { get { return MaxPoint.Z; } private set { MaxPoint.Z = value; } }

        [DataMember]
        public double[] Degs { get; set; }
        /// <summary>
        /// polomer delky steny
        /// </summary>
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

        /// <summary>
        /// predspocitane inverzni matice
        /// </summary>
        private Matrix3D _transpRot, _transpShift;

        public Cube() : this(new Vektor(0, 0, 0), new Vektor(0, 0, 1), 1) { }

        public Cube(Vektor center, Vektor dir, double size)
        {
            SetLabelPrefix("cube");
            IsActive = true;
            this.Material = new Material();
            SetValues(center, size, 0, 0, 0);
            Dir = Vektor.ZeroVektor;
        }


        public void SetValues(Vektor center, double size, double rotX, double rotY, double rotZ)
        {
            if (size <= MyMath.EPSILON) throw new Exception("Cube can not have negative side length");
            Center = new Vektor(center);
            Size = size;
            R = size / 2;

            Xmin = -R;// center.X - R;
            Xmax = R;//center.X + R;
            Ymin = -R;// center.Y - R;
            Ymax = R;// center.Y + R;
            Zmin = -R; // center.Z - R;
            Zmax = R;// center.Z + R;

            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(Center);
            _transpShift = _ShiftMatrix.GetOppositeShiftMatrix();
            Degs = new double[3] { rotX, rotY, rotZ };
            //Degs[0] = Degs[0] % 90;
            //Degs[1] = Degs[1] % 90;
            //Degs[2] = Degs[2] % 90;
            //if (Degs[0] < 0) Degs[0] += 90;
            //if (Degs[1] < 0) Degs[1] += 90;
            //if (Degs[2] < 0) Degs[2] += 90;

            // rotace z kvaternionu je opacne orientovana, proto minuska
            _RotatMatrix = Matrix3D.NewRotateByDegrees(Degs[0], Degs[1], Degs[2]);
            _transpRot = _RotatMatrix.Transpose();
            _localMatrix = _RotatMatrix * _ShiftMatrix;
        }
        public void SetValues(Vektor center, Vektor dir, double size)
        {
            if (size <= MyMath.EPSILON) throw new Exception("Cube can not have negative side length");
            Center = new Vektor(center);
            Size = size;
            R = size / 2;
            Dir = new Vektor(dir);

            Xmin = -R;// center.X - R;
            Xmax = R;//center.X + R;
            Ymin = -R;// center.Y - R;
            Ymax = R;// center.Y + R;
            Zmin = -R; // center.Z - R;
            Zmax = R;// center.Z + R;

            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(Center);
            _transpShift = _ShiftMatrix.GetOppositeShiftMatrix();
            //Vektor yAxe = new Vektor(1, 1, 0);
            //Quaternion q = new Quaternion(yAxe, new Vektor(dir));
            //double[] degss = q.ToEulerDegs();
            //Degs = q.ToEulerDegs();
            //degss[0] = degss[0] % 180;
            //degss[1] = degss[1] % 180;
            //degss[2] = degss[2] % 180;
            //if (degss[0] < 0) degss[0] += 180;
            //if (degss[1] < 0) degss[1] += 180;
            //if (degss[2] < 0) degss[2] += 180;

            //// rotace z kvaternionu je opacne orientovana, proto minuska
            //_RotatMatrix = Matrix3D.NewRotateByDegrees(-Degs[0], -Degs[1], -Degs[2]);
            _localMatrix = _RotatMatrix * _ShiftMatrix;
        }

        public void SetValues2(Vektor center, Vektor dir, double size)
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

            //Matrix3D transpRot = _RotatMatrix.Transpose();
            //Matrix3D transpShift = _ShiftMatrix.GetOppositeShiftMatrix();
            Vektor p0 = _transpShift.Transform2NewPoint(P0);
            p0 = _transpRot.Transform2NewPoint(p0);
            Vektor pd = _transpRot.Transform2NewPoint(Pd);
            pd.Normalize();


            byte osaFar = 0;
            byte osaNear = 0;

            double tNear = Double.MinValue;
            double tFar = Double.MaxValue;
            double t1, t2;
            double temp;


            /////////////////////////////
            // osa X:
            /////////////////////////////
            if (pd.X == 0 && (p0.X < Xmin || p0.X > Xmax))
                return false;

            if (pd.X != 0)
            {
                t1 = (Xmin - p0.X) / pd.X;
                t2 = (Xmax - p0.X) / pd.X;

                // vymenime t1 a t2
                if (t1 > t2)
                {
                    temp = t1;
                    t1 = t2;
                    t2 = temp;
                }

                if (t1 > tNear)
                {
                    osaNear = 1;
                    tNear = t1;
                }

                if (t2 < tFar)
                {
                    osaFar = 1;
                    tFar = t2;
                }

                if (tNear > tFar || tFar < MyMath.EPSILON)
                    return false;
            }
            /////////////////////////////
            // osa Y:
            /////////////////////////////
            if (pd.Y == 0 && (p0.Y < Ymin || p0.Y > Ymax))
                return false;

            if (pd.Y != 0)
            {
                t1 = (Ymin - p0.Y) / pd.Y;
                t2 = (Ymax - p0.Y) / pd.Y;
                Vektor normY = new Vektor(0, -1, 0);
                if (t1 > t2)
                {
                    temp = t1;
                    t1 = t2;
                    t2 = temp;
                }

                if (t1 > tNear)
                {
                    osaNear = 2;
                    tNear = t1;
                }

                if (t2 < tFar)
                {
                    osaFar = 2;
                    tFar = t2;
                }

                if (tNear > tFar || tFar < MyMath.EPSILON)
                    return false;
            }

            /////////////////////////////
            // osa Z:
            /////////////////////////////
            if (pd.Z == 0 && (p0.Z < Zmin || p0.Z > Zmax))
                return false;

            if (pd.Z != 0)
            {

                t1 = (Zmin - p0.Z) / pd.Z;
                t2 = (Zmax - p0.Z) / pd.Z;
                if (t1 > t2)
                {
                    temp = t1;
                    t1 = t2;
                    t2 = temp;
                }

                if (t1 > tNear)
                {
                    osaNear = 3;
                    tNear = t1;
                }

                if (t2 < tFar)
                {
                    osaFar = 3;
                    tFar = t2;
                }

                if (tNear > tFar || tFar < MyMath.EPSILON)
                    return false;
            }
            Vektor norm = Vektor.ZeroVektor;
            
            Vektor point = p0 + pd * tNear;
            if (Math.Abs(point.X - MinPoint.X) < MyMath.EPSILON)
                norm.X = -1;
            else if (Math.Abs(point.Y - MinPoint.Y) < MyMath.EPSILON)
                norm.Y = -1;
            else if (Math.Abs(point.Z - MinPoint.Z) < MyMath.EPSILON)
                norm.Z = -1;
            else if (Math.Abs(point.X - MaxPoint.X) < MyMath.EPSILON)
                norm.X = 1;
            else if (Math.Abs(point.Y - MaxPoint.Y) < MyMath.EPSILON)
                norm.Y = 1;
            else if (Math.Abs(point.Z - MaxPoint.Z) < MyMath.EPSILON)
                norm.Z = 1;


            SolidPoint sp = new SolidPoint();
            sp.Color = this.Material.Color;
            //_ShiftMatrix.TransformPoint(point);
            //_RotatMatrix.TransformPoint(point);
            _localMatrix.TransformPoint(point);
            sp.Coord = point;
            //Vektor size = point - P0;
            //double len = size.Size();
            sp.T = tNear;
            sp.Normal = _transpRot.Transform2NewPoint(norm);
            sp.Normal.Normalize();
            sp.Shape = this;
            sp.Material = this.Material;
            InterPoint.Add(sp);
            return true;
        }
        public bool Intersects_OLD(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint)
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
                if (wall.Intersects(P0, Pd, ref BasePoints, false, 0))
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
                if (wall.Intersects(P0, Pd, ref BasePoints, false,0))
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
            return Label + " {Center=" + Center + "; Axis=" + Dir + "}";
        }

        public override void Rotate(double degAroundX, double degAroundY, double degAroundZ)
        {
            Matrix3D newRot = Matrix3D.NewRotateByDegrees(degAroundX, degAroundY, degAroundZ);

            Vektor yAxe = new Vektor(1, 1, 0);
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
            //this.SetValues(this.Center, yAxe, this.Size);
            this.SetValues(this.Center, this.Size, degAroundX, degAroundY, degAroundZ);
        }

        public override DefaultShape FromDeserial()
        {
            Cube cube = new Cube(this.Center, this.Dir, this.Size);
            if (this.Degs != null)
                cube.SetValues(this.Center, this.Size, this.Degs[0], this.Degs[1], this.Degs[2]);
            cube.Label = this.Label;
            cube.Material = this.Material;
            cube.IsActive = this.IsActive;
            return cube;
        }
    }
}
