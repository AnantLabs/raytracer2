using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{
    public class Box : DefaultShape
    {
        /// <summary>
        /// minimalni rozmer - minimalni hranice
        /// </summary>
        private Vektor ExtentMin { get; set; }

        private double Epsilon = 0.0001;

        /// <summary>
        /// maximalni rozmer - maximalni hranice
        /// </summary>
        private Vektor ExtentMax { get; set; }

        public double Size { get; private set; }
        /// <summary>
        /// stred krychle
        /// </summary>
        public Vektor Center { get; private set; }

        public Box(Vektor center, double size)
        {
            IsActive = true;
            this.Material = new Material();
            SetCenterSize(center, size);
        }

        public Box(Box old)
        {
            IsActive = old.IsActive;
            ExtentMin = new Vektor(old.ExtentMin);
            ExtentMax = new Vektor(old.ExtentMax);
            Size = old.Size;
            Center = new Vektor(old.Center);
            Material = new Material(old.Material);
        }

        public void SetCenterSize(Vektor center, double size)
        {
            Center = center;
            Size = size;

            double pulext = size / 2;
            ExtentMin = new Vektor(Center.X - pulext, Center.Y - pulext, Center.Z - pulext);
            ExtentMax = new Vektor(Center.X + pulext, Center.Y + pulext, Center.Z + pulext);
        }

        public override bool Intersects(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint)
        {
            if (!IsActive)
                return false;

            // pro kazdou osu overime prunik s paprskem

            byte osaFar = 0;
            byte osaNear = 0;

            double tNear = Double.MinValue;
            double tFar = Double.MaxValue;
            double t1, t2;
            double temp;

            /////////////////////////////
            // osa X:
            /////////////////////////////
            if (Pd.X == 0 && (P0.X < ExtentMin.X || P0.X > ExtentMax.X))
                return false;

            if (Pd.X != 0)
            {
                t1 = (ExtentMin.X - P0.X) / Pd.X;
                t2 = (ExtentMax.X - P0.X) / Pd.X;
                
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

                if (tNear > tFar || tFar < Epsilon)
                    return false;
            }
            /////////////////////////////
            // osa Y:
            /////////////////////////////
            if (Pd.Y == 0 && (P0.Y < ExtentMin.Y || P0.Y > ExtentMax.Y))
                return false;

            if (Pd.Y != 0)
            {
                t1 = (ExtentMin.Y - P0.Y) / Pd.Y;
                t2 = (ExtentMax.Y - P0.Y) / Pd.Y;
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

                if (tNear > tFar || tFar < Epsilon)
                    return false;
            }

            /////////////////////////////
            // osa Z:
            /////////////////////////////
            if (Pd.Z == 0 && (P0.Z < ExtentMin.Z || P0.Z > ExtentMax.Z))
                return false;

            if (Pd.Z != 0)
            {

                t1 = (ExtentMin.Z - P0.Z) / Pd.Z;
                t2 = (ExtentMax.Z - P0.Z) / Pd.Z;
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

                if (tNear > tFar || tFar < Epsilon)
                    return false;
            }
            SolidPoint sp = new SolidPoint();
            sp.Material = this.Material;
            sp.Color = sp.Material.Color;

            if (tNear > Epsilon)
            {
                sp.T = tNear;
                if (osaNear == 1)
                    sp.Normal = new Vektor(-1, 0, 0);
                else if (osaNear == 2)
                    sp.Normal = new Vektor(0, 1, 0);
                else if (osaNear == 3)
                    sp.Normal = new Vektor(0, 0, 1);

                sp.Coord = P0 + Pd * sp.T;
                InterPoint.Add(sp);
            }
            if (tFar>Epsilon)
            {
                sp = new SolidPoint();
                sp.T = tFar;
                if (osaFar == 1)
                    sp.Normal = new Vektor(1, 0, 0);
                else if (osaFar == 2)
                    sp.Normal = new Vektor(0, -1, 0);
                else if (osaFar == 3)
                    sp.Normal = new Vektor(0, 0, -1);

                
                sp.Material = this.Material;
                sp.Color = sp.Material.Color;
                sp.Coord = P0 + Pd * sp.T;
                InterPoint.Add(sp);
            }

            //sp.Coord = P0 + Pd * sp.T;

            //InterPoint.Add(sp);

            return true;
        }

        public override void Move(double dx, double dy, double dz)
        {
            throw new NotImplementedException();
        }

        public override void MoveToPoint(double dx, double dy, double dz)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Box: [" + Center.X + "; " + Center.Y + "; " + Center.Z + "]";
        }

        public override void Rotate(double degAroundX, double degAroundY, double degAroundZ)
        {
            throw new NotImplementedException();
        }
    }
}
