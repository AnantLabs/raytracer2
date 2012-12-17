using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracerLib
{
    /**
 * AABB
 * The axis-aligned minimum bounding box for a given point set is its minimum bounding box subject 
 * to the constraint that the edges of the box are parallel to the (Cartesian) coordinate axes.
 * It is simply the Cartesian product of N intervals each of which is defined by 
 * the minimal and maximal value of the corresponding coordinate for the points in S.
 * */
    public class Cuboid
    {
        /// <summary>
        /// uzel stromu, kteremu Cuboid nalezi
        /// </summary>
        public RtreeNode CurrentNode { get; set; }

        public enum CuboidComparerType { Xorder, Yorder, Zorder, Volume }

        /// <summary>
        /// zpusob porovnavani dvou Cuboidu
        /// </summary>
        public static CuboidComparerType CompareOrder = CuboidComparerType.Xorder;

        private Vektor MinPoint = new Vektor();
        private Vektor MaxPoint = new Vektor();

        public double Xmin { get { return MinPoint.X; } private set { MinPoint.X = value; } }
        public double Ymin { get { return MinPoint.Y; } private set { MinPoint.Y = value; } }
        public double Zmin { get { return MinPoint.Z; } private set { MinPoint.Z = value; } }

        public double Xmax { get { return MaxPoint.X; } private set { MaxPoint.X = value; } }
        public double Ymax { get { return MaxPoint.Y; } private set { MaxPoint.Y = value; } }
        public double Zmax { get { return MaxPoint.Z; } private set { MaxPoint.Z = value; } }

        /// <summary>
        /// odchylka pro testu pruniku s primkou
        /// </summary>
        private double Epsilon = 0.0001;

        /// <summary>
        /// Vytvori rovnostranny Cuboid zadany stredem a delkou steny
        /// </summary>
        /// <param name="center">stred Cuboidu</param>
        /// <param name="size">delka steny</param>
        public Cuboid(Vektor center, double size)
        {
            double spul = size / 2;
            Xmin = center.X - spul;
            Xmax = center.X + spul;
            Ymin = center.Y - spul;
            Ymax = center.Y + spul;
            Zmin = center.Z - spul;
            Zmax = center.Z + spul;
            TestMinMax();
        }
        public Cuboid() : this(1) { }

        /// <summary>
        /// Vytvori rovnostranny Cuboid
        /// </summary>
        /// <param name="size">delka strany</param>
        public Cuboid(double size)
        {
            double sizepul = size / 2;
            Xmin = Ymin = Zmin = -sizepul;
            Xmax = Ymax = Zmax = sizepul;
            TestMinMax();
        }

        public Cuboid(Vektor minPoint, Vektor maxPoint)
        {
            MinPoint = new Vektor(minPoint);
            MaxPoint = new Vektor(maxPoint);
            TestMinMax();
        }

        public Cuboid(Cuboid oldCub)
        {
            this.MinPoint = new Vektor(oldCub.MinPoint);
            this.MaxPoint = new Vektor(oldCub.MaxPoint);
            this.Epsilon = oldCub.Epsilon;
        }

        private void TestMinMax()
        {
            if (Xmin > Xmax || Ymin > Ymax || Zmin > Zmax)
                throw new Exception("Spatne nastavene minimalni a maximalni hodnoty Cuboidu");
        }
        
        /// <summary>
        /// Stred Cuboidu
        /// </summary>
        public Vektor GetCenter()
        {
            double size = Xmax - Xmin;
            if (size < 0)
            {
                int o = 0;
            }
            double sizePul = size / 2;
            Vektor c = new Vektor(Xmin + sizePul, Ymin + sizePul, Zmin + sizePul);
            return c;

        }

        /// <summary>
        /// Objem Cuboidu
        /// </summary>
        public double GetVolume()
        {
            double x = Xmax - Xmin;
            double y = Ymax - Ymin;
            double z = Zmax - Zmin;
            double vol = x * y * z;
            return vol;
        }

        /// <summary>
        /// vrati vsechny rohy Cuboidu
        /// </summary>
        /// <returns>pole velikosti 8</returns>
        public Vektor[] GetAllCorners()
        {
            Vektor[] corners = new Vektor[8];
            corners[0] = new Vektor(Xmin, Ymin, Zmin);
            corners[1] = new Vektor(Xmin, Ymin, Zmax);
            corners[2] = new Vektor(Xmin, Ymax, Zmin);
            corners[3] = new Vektor(Xmin, Ymax, Zmax);

            corners[4] = new Vektor(Xmax, Ymin, Zmin);
            corners[5] = new Vektor(Xmax, Ymin, Zmax);
            corners[6] = new Vektor(Xmax, Ymax, Zmin);
            corners[7] = new Vektor(Xmax, Ymax, Zmax);

            return corners;

        }

        /// <summary>
        /// zjisti, zda je Cuboid rovnostranny
        /// </summary>
        /// <returns></returns>
        public bool IsEqualSided()
        {
            bool eq1 = ((Xmax - Xmin) == (Ymax - Ymin)) && ((Ymax - Ymin) == (Zmax - Zmin));
            return eq1;
        }

        /// <summary>
        /// zjisti, zda aktualni Cuboid obsahuje cely Cuboid zadany parametrem
        /// </summary>
        /// <param name="cuboid">Cuboid, ktery ma byt uvnitr</param>
        /// <returns>true, kdyz je zadany Cuboid uvnitr aktualni instance</returns>
        public bool Contains(Cuboid cuboid)
        {
            // pro kazdy roh rozhodni, zda je uvnitr druheho Cuboidu, kdyz neni uvnitr, vrat hned false
            Vektor[] corners = cuboid.GetAllCorners();
            foreach (Vektor corner in corners)
            {
                if (corner.X < this.Xmin || corner.X > this.Xmin ||
                    corner.Y < this.Ymin || corner.Y > this.Ymax ||
                    corner.Z < this.Zmin || corner.Z > this.Zmax)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// zjisti, zda tento Cuboid je v pruniku se zadanym druhym Cuboidem
        /// </summary>
        /// <param name="cuboid">druhy Cuboid</param>
        /// <returns>true, kdyz se Cuboidy protinaji</returns>
        public bool IsIntersectedBy(Cuboid c2)
        {
            return Cuboid.AreIntersected(this, c2);
        }

        /// <summary>
        /// zjisti, zda jsou dva Cuboidy v pruniku
        /// </summary>
        /// <returns>true, kdyz se Cuboidy protinaji</returns>
        public static bool AreIntersected(Cuboid c1, Cuboid c2)
        {
            // vyber jeden Cuboid a pro kazdy jeho roh rozhodni, zda je uvnitr druheho Cuboidu
            Vektor[] corners = c2.GetAllCorners();
            foreach (Vektor corner in corners)
            {
                if ((corner.X > c1.Xmin && corner.X < c1.Xmin) && 
                    (corner.Y > c1.Ymin && corner.Y < c1.Ymax) && 
                    (corner.Z > c1.Zmin && corner.Z < c1.Zmax))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// k zadanemu objektu vytvori jeho obalku - CUBOID
        /// </summary>
        /// <param name="item">objekt v RayTracingu</param>
        /// <returns>Cuboid daneho objektu</returns>
        public static Cuboid CreateCuboid(DefaultShape item)
        {
            Cuboid cluster = null;

            if (item is Sphere)
            {
                Sphere sph = (Sphere)item;
                cluster = new Cuboid(sph.Origin, sph.R * 2);
            }
            else if (item is Cube)
            {
                Cube cube = (Cube)item;
                double r = cube.Size / 2;
                double u = Math.Sqrt(r * r + r * r);
                double d = Math.Sqrt(r * r + u * u);        // vzdalenost stredu krychle od jednoho rohu - polovicni uhlopricka // Pythagorova veta
                cluster = new Cuboid(cube.Center, d * 2);
            }
            else if (item is Cylinder)
            {
                Cylinder cyl = (Cylinder)item;
                double size = cyl.H / 2 > cyl.R ? cyl.H / 2 : cyl.R;
                cluster = new Cuboid(cyl.Center, size * 2);
            }
            else if (item is Triangle)
            {
                Triangle tr = (Triangle)item;
                double minX = Math.Min(tr.A.X, tr.B.X);
                minX = Math.Min(minX, tr.C.X);
                double minY = Math.Min(tr.A.Y, tr.B.Y);
                minY = Math.Min(minY, tr.C.Y);
                double minZ = Math.Min(tr.A.Z, tr.B.Z);
                minZ = Math.Min(minZ, tr.C.Z);

                double maxX = Math.Max(tr.A.X, tr.B.X);
                maxX = Math.Max(maxX, tr.C.X);
                double maxY = Math.Max(tr.A.Y, tr.B.Y);
                maxY = Math.Max(maxY, tr.C.Y);
                double maxZ = Math.Max(tr.A.Z, tr.B.Z);
                maxZ = Math.Max(maxZ, tr.C.Z);

                cluster = new Cuboid(new Vektor(minX, minY, minZ), new Vektor(maxX, maxY, maxZ));
            }
            else if (item is Cone)
            {
                Cone cone = (Cone)item;

                Vektor point1 = cone.C;
                Vektor point2 = cone.Dir + new Vektor(cone.Rad, cone.Rad, cone.Rad);
                Vektor point3 = cone.Dir - new Vektor(cone.Rad, cone.Rad, cone.Rad);

                double minX = Math.Min(point1.X, point2.X);
                minX = Math.Min(minX, point3.X);
                double minY = Math.Min(point1.Y, point2.Y);
                minY = Math.Min(minY, point3.Y);
                double minZ = Math.Min(point1.Z, point2.Z);
                minZ = Math.Min(minZ, point3.Z);

                double maxX = Math.Max(point1.X, point2.X);
                maxX = Math.Max(maxX, point3.X);
                double maxY = Math.Max(point1.Y, point2.Y);
                maxY = Math.Max(maxY, point3.Y);
                double maxZ = Math.Max(point1.Z, point2.Z);
                maxZ = Math.Max(maxZ, point3.Z);

                cluster = new Cuboid(new Vektor(minX, minY, minZ), new Vektor(maxX, maxY, maxZ));
            }
            else if (item is Plane)
            {
                //cluster = new Cuboid(1);
            }
            return cluster;
        }

        /// <summary>
        /// slije dva cuboidy dohromady
        /// </summary>
        /// <returns>novy Cuboid obsahujici oba Cuboidy</returns>
        public static Cuboid Union(Cuboid c1, Cuboid c2)
        {

            if (c1 == null) return new Cuboid(c2);
            if (c2 == null) return new Cuboid(c1);
            double minx = Math.Min(c1.Xmin, c2.Xmin);
            double miny = Math.Min(c1.Ymin, c2.Ymin);
            double minz = Math.Min(c1.Zmin, c2.Zmin); 

            double maxx = Math.Max(c1.Xmax, c2.Xmax);
            double maxy = Math.Max(c1.Ymax, c2.Ymax);
            double maxz = Math.Max(c1.Zmax, c2.Zmax);

            Cuboid cub = new Cuboid(new Vektor(minx, miny, minz), new Vektor(maxx, maxy, maxz));
            return cub;
        }

        /// <summary>
        /// slije dva cuboidy dohromady
        /// </summary>
        /// <returns>novy Cuboid obsahujici oba Cuboidy</returns>
        public static Cuboid operator+(Cuboid c1, Cuboid c2)
        {
            return Cuboid.Union(c1, c2);
        }

        /// <summary>
        /// zjisti, zda paprsek protina dany Cuboid
        /// </summary>
        /// <param name="P0"></param>
        /// <param name="Pd"></param>
        /// <returns></returns>
        public bool IntersectsRay(Vektor P0, Vektor Pd)
        {
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
            if (Pd.X == 0 && (P0.X < Xmin || P0.X > Xmax))
                return false;

            if (Pd.X != 0)
            {
                t1 = (Xmin - P0.X) / Pd.X;
                t2 = (Xmax - P0.X) / Pd.X;

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
            if (Pd.Y == 0 && (P0.Y < Ymin || P0.Y > Ymax))
                return false;

            if (Pd.Y != 0)
            {
                t1 = (Ymin - P0.Y) / Pd.Y;
                t2 = (Ymax - P0.Y) / Pd.Y;
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
            if (Pd.Z == 0 && (P0.Z < Zmin || P0.Z > Zmax))
                return false;

            if (Pd.Z != 0)
            {

                t1 = (Zmin - P0.Z) / Pd.Z;
                t2 = (Zmax - P0.Z) / Pd.Z;
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

            return true;
        }

        /// <summary>
        /// porovna dva Cuboidy dle jejich stredu
        /// pro porovnavani se ridi nastavenou hodnotou CompareOrder typu CuboidComparerType,
        /// zda se porovna podle X, Y, Z souradnicich, nebo podle objemu
        /// </summary>
        /// <param name="other">druhy Cuboid k porovnani</param>
        /// <returns>-1 | 0 | 1</returns>
        public static int Compare(Cuboid c1, Cuboid c2)
        {
            if (c1 == null && c2 == null) return 0;
            else if (c1 != null && c2 == null) return -1;
            else if (c1 == null && c2 != null) return 1;

            Vektor center1 = c1.GetCenter();
            Vektor center2 = c2.GetCenter();

            switch (Cuboid.CompareOrder)
            {
                case CuboidComparerType.Xorder:
                    return center1.X.CompareTo(center2.X);

                case CuboidComparerType.Yorder:
                    return center1.Y.CompareTo(center2.Y);

                case CuboidComparerType.Zorder:
                    return center1.Z.CompareTo(center2.Z);

                case CuboidComparerType.Volume:
                    double vol1 = c1.GetVolume();
                    double vol2 = c2.GetVolume();
                    return vol1.CompareTo(vol2);
            }

            return 0;
        }

        public static int CuboidComparer(Cuboid x, Cuboid y)
        {
            return Cuboid.Compare(x, y);
        }

        public override string ToString()
        {
            return "MIN: " + this.MinPoint + "; MAX: " + this.MaxPoint;
        }

        public static bool TestCuboidRayFromInside()
        {
            Cuboid c = new Cuboid(new Vektor(0, 0, 0), 5);
            Vektor P0 = new Vektor(1, 1, 1);
            Vektor Pd = new Vektor(1, 0, 0);
            bool iSintersected = c.IntersectsRay(P0, Pd);
            iSintersected = c.IntersectsRay(P0, new Vektor(1, 1, 1));
            iSintersected = c.IntersectsRay(P0, new Vektor(0, -1, 1));
            return iSintersected;
        }
    }
}
