using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mathematics;

namespace RayTracerLib
{
    /// <summary>
    /// Bod nalezici bounding volumu
    /// Musi platit, ze bounding bolume ma 8 bodu (kostka),
    /// tedy existuje 8 BoundingPointu odkazujicich na jeden BoundingVolume
    /// </summary>
    public class BoundingPoint : ISpatialPoint
    {
        public BoundingVolume BV;
        private Vektor _Point;

        public BoundingPoint(Vektor point, BoundingVolume bv)
        {
            _Point = point;
            BV = bv;
        }
        public Vektor GetPoint()
        {
            return _Point;
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
        public static int Compare(BoundingPoint c1, BoundingPoint c2)
        {
            if (c1 == null && c2 == null) return 0;
            else if (c1 != null && c2 == null) return -1;
            else if (c1 == null && c2 != null) return 1;

            Vektor center1 = c1.GetPoint();
            Vektor center2 = c2.GetPoint();

            switch (BoundingPoint.DimensionBoundComp)
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

        public static int BoundingPointComparer(BoundingPoint x, BoundingPoint y)
        {
            return BoundingPoint.Compare(x, y);
        }

        public static BoundingPoint[] FromBoundingVolume(BoundingVolume bv)
        {
            //if (bv == null) return null;
            //if (bv.BV == null) return null;

            List<BoundingPoint> points = new List<BoundingPoint>();
            Vektor[] vecs = bv.BV.Get8Points(); // dostaneme 8 vrcholu ke cuboidu
            foreach (Vektor v in vecs)
            {
                BoundingPoint bpoint = new BoundingPoint(v, bv);
                points.Add(bpoint);
            }
            return points.ToArray();
        }
    }
}
