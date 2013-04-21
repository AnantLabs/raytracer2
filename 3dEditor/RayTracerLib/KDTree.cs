using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{
    public class KDTree : IOptimize
    {
        public class KDNode
        {
            
            private Cuboid Boundary;
            private List<BoundingVolume> Points;
            private KDNode[] ChildList;
            private KDNode Parrent;

            private enum Dimension { X, Y, Z }
            private Dimension MaxDim;
            /// <summary>
            /// nejvetsi dimenze kostky, neboli delka nejdelsi strany Hranice
            /// </summary>
            private double Size;
            private int Depth = 0;
            private const int MAX_DEPTH = 20;
            private const double MIN_SIZE = 0.1;
            private const int MAX_CAPACITY = 1;

            private KDNode() { }

            public KDNode(int depth, KDNode parrent)
            {
                Depth = depth;
                Points = new List<BoundingVolume>();
                Parrent = parrent;
            }

            private void Split()
            {
                switch (MaxDim)
                {
                    case Dimension.X:
                        BoundingVolume.DimensionBoundComp = BoundingVolume.CompareDimension.X;
                        break;
                    case Dimension.Y:
                        BoundingVolume.DimensionBoundComp = BoundingVolume.CompareDimension.Y;
                        break;
                    case Dimension.Z:
                        BoundingVolume.DimensionBoundComp = BoundingVolume.CompareDimension.Z;
                        break;
                    default: break;
                }
                // setrideni bodu podle nejvetsi dimenze
                Points.Sort(BoundingVolume.BoundingVolumeComparer);
                int count = Points.Count;
                int countPul = count / 2;

                // rozdeleni bodu na dve stejne velke poloviny
                List<BoundingVolume> half1 = Points.GetRange(0, countPul);       // prvni polovina: startIndex=0, pocetPrvku=countPul
                List<BoundingVolume> half2 = Points.GetRange(countPul, countPul);// druha polovina: startIndex=countPul, pocetPrvku=countPul
                // lichy pocet prvku - druha polovina je o jeden prvek delsi - o ten posledni
                if (count % 2 == 1)
                    half2.Add(Points[count - 1]);

                ChildList = new KDNode[2];
                ChildList[0] = new KDNode(this.Depth + 1, this);
                ChildList[0].AddPoints(half1);
                ChildList[1] = new KDNode( this.Depth + 1, this);
                ChildList[1].AddPoints(half2);

            }

            public bool IsLeaf()
            {
                if (ChildList == null) return true;
                return false;
            }

            public void AddPoints(List<BoundingVolume> points)
            {
                //Boundary = Cuboid.CreateCuboid((ISpatialPoint[])points.ToArray());
                Boundary = points[0].BV;
                foreach (BoundingVolume bv in points)
                    Boundary = Cuboid.Union(Boundary, bv.BV);

                double sizeX = Boundary.Xmax - Boundary.Xmin;
                double sizeY = Boundary.Ymax - Boundary.Ymin;
                double sizeZ = Boundary.Zmax - Boundary.Zmin;

                MaxDim = Dimension.X;
                Size = sizeX;
                if (sizeY > Size)
                {
                    Size = sizeY;
                    MaxDim = Dimension.Y;
                }
                if (sizeZ > Size)
                {
                    Size = sizeZ;
                    MaxDim = Dimension.Z;
                }

                this.Points = points;
                if (Points.Count <= MAX_CAPACITY ||     // pocet bodu v uzlu je jiz dost maly
                    Size <= MIN_SIZE ||     // delka steny je mensi, nez povolene minimum
                    Depth >= MAX_DEPTH      // hloubka stromu je vetsi, nez povolene maximum
                    ) return;

                Split();
            }

            public bool Intersection(Vektor P0, Vektor Pd, ref List<SolidPoint> intersPts, bool isForLight, double lightDist)
            {
                if (Points.Count == 0) return false;

                bool isInters = false;
                if (!this.IsLeaf())
                {
                    if (!Boundary.IntersectsRay(P0, Pd)) return false;
                    foreach (KDNode child in ChildList)
                    {
                        isInters = child.Intersection(P0, Pd, ref intersPts, isForLight, lightDist) || isInters;
                    }
                }
                else
                {
                    foreach (BoundingVolume bv in Points)
                    {
                        if (!bv.BV.IntersectsRay(P0, Pd)) return false;
                        isInters = bv.Item.Intersects(P0, Pd, ref intersPts, isForLight, lightDist) || isInters;
                    }
                }
                return isInters;
            }
        }

        private KDNode Root;
        private KDTree() { }
        public enum KdtreeType { Volumes, Points }
        private KdtreeType TreeType;
        public KDTree(List<DefaultShape> objects, KdtreeType type)
        {
            Root = new KDNode(0, null);
            if (type == KdtreeType.Volumes)
            {
                Cuboid cuboid = Cuboid.CreateCuboid(objects);
                List<BoundingVolume> bounds = new List<BoundingVolume>();
                foreach (DefaultShape shape in objects)
                {
                    if (shape is Plane || !shape.IsActive) continue;
                    BoundingVolume bv = new BoundingVolume(shape);
                    bounds.Add(bv);
                }
                Root.AddPoints(bounds);
            }
            
        }

        public bool Intersection(Vektor P0, Vektor Pd, ref List<SolidPoint> intersPts, bool isForLight, double lightDist)
        {
            return Root.Intersection(P0, Pd, ref intersPts, isForLight, lightDist);
        }
    }
}
