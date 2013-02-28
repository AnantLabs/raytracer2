using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{
    public class BoundingVolume : ISpatialPoint
    {
        /// <summary>
        /// oblast ohranicujici objekt
        /// </summary>
        public Cuboid BV { get; private set; }
        /// <summary>
        /// objekt ohraniceny touto oblasti
        /// </summary>
        public DefaultShape Item { get; private set; }
        /// <summary>
        /// stred oblasti
        /// </summary>
        public Vektor Center { get; private set; }

        public BoundingVolume(DefaultShape ds)
        {
            Item = ds;
            BV = Cuboid.CreateCuboid(ds);
            Center = BV.GetCenter();
        }

        public enum CompareDimension { X, Y, Z }
        public static CompareDimension DimensionBoundComp;
        /// <summary>
        /// porovna dva Cuboidy dle jejich stredu
        /// pro porovnavani se ridi nastavenou hodnotou CompareOrder typu CuboidComparerType,
        /// zda se porovna podle X, Y, Z souradnicich, nebo podle objemu
        /// </summary>
        /// <param name="other">druhy Cuboid k porovnani</param>
        /// <returns>-1 | 0 | 1</returns>
        public static int Compare(BoundingVolume c1, BoundingVolume c2)
        {
            if (c1 == null && c2 == null) return 0;
            else if (c1 != null && c2 == null) return -1;
            else if (c1 == null && c2 != null) return 1;

            Vektor center1 = c1.Center;
            Vektor center2 = c2.Center;

            switch (BoundingVolume.DimensionBoundComp)
            {
                case CompareDimension.X:
                    return center1.X.CompareTo(center2.X);

                case CompareDimension.Y:
                    return center1.Y.CompareTo(center2.Y);

                case CompareDimension.Z:
                    return center1.Z.CompareTo(center2.Z);
            }

            return 0;
        }

        public static int BoundingVolumeComparer(BoundingVolume x, BoundingVolume y)
        {
            return BoundingVolume.Compare(x, y);
        }

        public Vektor GetPoint()
        {
            return Center;
        }
    }
}
