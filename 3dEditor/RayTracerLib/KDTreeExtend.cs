using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mathematics;

namespace RayTracerLib
{
    public class KDTreeExtend : IOptimize
    {
                public class KDNode
        {
            
            private Cuboid Boundary;
            private List<BoundingPoint> Points;
            private KDNode[] ChildList;
            private KDNode Parrent;

            private enum Dimension { X, Y, Z }
            private Dimension MaxDim;
            /// <summary>
            /// nejvetsi dimenze kostky, neboli delka nejdelsi strany Hranice
            /// </summary>
            private double Size;
            private int Depth = 0;
            private const int MAX_DEPTH = 1024;
            private const double MIN_SIZE = 0.01;
            private const int MAX_CAPACITY = 1;

            private KDNode() { }

            public KDNode(Cuboid boundary, int depth, KDNode parrent)
            {
                Boundary = boundary;
                Depth = depth;
                Points = new List<BoundingPoint>();
                Parrent = parrent;
            }

            /// <summary>
            /// Vytvori bounding volume pro dany uzel a vsechny uzly v jeho podstrome.
            /// Bounding volume pak bude nejmensi mozny bounding volume
            /// </summary>
            public void RecreateBoundingVolumes()
            {
                if (Points.Count == 0)
                {
                    ChildList = null;
                    return;
                }

                Boundary = Points[0].BV.BV;
                foreach (BoundingPoint bp in Points)
                {
                    this.Boundary = Cuboid.Union(this.Boundary, bp.BV.BV);
                }
                if (ChildList != null)
                    for(int i=0; i<ChildList.Length; i++)
                        if (ChildList[i]!= null) ChildList[i].RecreateBoundingVolumes();
                //Boundary = Cuboid.CreateCuboid(Points.ToArray());

                //if (ChildList != null)
                //    foreach (KDNode child in ChildList)
                //        child.RecreateBoundingVolumes();
                //else Boundary = Points[0].BV.BV;
            }

            private void Split()
            {
                Cuboid newBound1 = null;
                Cuboid newBound2 = null;
                double sizePul = Size / 2.0;

                switch (MaxDim)
                {
                    case Dimension.X:
                        {
                            BoundingVolume.DimensionBoundComp = BoundingVolume.CompareDimension.X;

                            Vektor minPoint = Boundary.GetMinVectorShallow();
                            Vektor maxPoint = new Vektor(Boundary.Xmin + sizePul, Boundary.Ymax, Boundary.Zmax);
                            newBound1 = new Cuboid(minPoint, maxPoint);

                            minPoint = new Vektor(Boundary.Xmin + sizePul, Boundary.Ymin, Boundary.Zmin);
                            maxPoint = Boundary.GetMaxVectorShallow();
                            newBound2 = new Cuboid(minPoint, maxPoint);
                            break;
                        }
                    case Dimension.Y:
                        {
                            BoundingVolume.DimensionBoundComp = BoundingVolume.CompareDimension.Y;

                            Vektor minPoint = Boundary.GetMinVectorShallow();
                            Vektor maxPoint = new Vektor(Boundary.Xmax, Boundary.Ymin + sizePul, Boundary.Zmax);
                            newBound1 = new Cuboid(minPoint, maxPoint);

                            minPoint = new Vektor(Boundary.Xmin, Boundary.Ymin + sizePul, Boundary.Zmin);
                            maxPoint = Boundary.GetMaxVectorShallow();
                            newBound2 = new Cuboid(minPoint, maxPoint);
                            break;
                        }
                    case Dimension.Z:
                        {
                            BoundingVolume.DimensionBoundComp = BoundingVolume.CompareDimension.Z;

                            Vektor minPoint = Boundary.GetMinVectorShallow();
                            Vektor maxPoint = new Vektor(Boundary.Xmax, Boundary.Ymax, Boundary.Zmin + sizePul);
                            newBound1 = new Cuboid(minPoint, maxPoint);

                            minPoint = new Vektor(Boundary.Xmin, Boundary.Ymin, Boundary.Zmin + sizePul);
                            maxPoint = Boundary.GetMaxVectorShallow();
                            newBound2 = new Cuboid(minPoint, maxPoint);
                            break;
                        }
                    default: break;
                }
                // rozdeleni bodu na dve stejne velke poloviny
                List<BoundingPoint> half1 = new List<BoundingPoint>();
                List<BoundingPoint> half2 = new List<BoundingPoint>();



                foreach (BoundingPoint bp in Points)
                {
                    if (newBound1.Contains(bp.GetPoint()))
                        half1.Add(bp);
                    else//if (newBound2.Contains(bp.GetPoint()))
                        half2.Add(bp);
                }
                ChildList = new KDNode[2];
                if (half1.Count > 0)
                {
                    ChildList[0] = new KDNode(newBound1, this.Depth + 1, this);
                    ChildList[0].AddPoints(half1);
                }
                if (half2.Count > 0)
                {
                    ChildList[1] = new KDNode(newBound2, this.Depth + 1, this);
                    ChildList[1].AddPoints(half2);
                }

            }

            public bool IsLeaf()
            {
                if (ChildList == null) return true;
                return false;
            }

            public void AddPoints(List<BoundingPoint> points)
            {
                if (points.Count == 0)
                {
                    return;
                }
                //foreach (BoundingVolume bv in points)
                //    Boundary = Cuboid.Union(Boundary, bv.BV);

                double sizeX = Boundary.Xmax - Boundary.Xmin;
                double sizeY = Boundary.Ymax - Boundary.Ymin;
                double sizeZ = Boundary.Zmax - Boundary.Zmin;

                // vyber nejvetsi dimenze
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

            public bool Intersection(Vektor P0, Vektor Pd, ref List<SolidPoint> intersPts, ref List<DefaultShape> defList)
            {
                if (Points.Count == 0) return false;

                bool isInters = false;
                if (!this.IsLeaf())// && (ChildList[0] != null && ChildList[1]!=null))// || (ChildList != null && ChildList[1] != null))
                {
                    if (!Boundary.IntersectsRay(P0, Pd)) return false;
                    for(int i=0; i< 2; i++)
                    {
                        try
                        {
                            if (ChildList[i] != null)
                                isInters = ChildList[i].Intersection(P0, Pd, ref intersPts, ref defList) || isInters;
                        }
                        catch (Exception ex)
                        {
                            int asd = 0;
                        }
                    }
                }
                else
                {
                    foreach (BoundingPoint bv in Points)
                    {
                        //if (!bv.BV.BV.IntersectsRay(P0, Pd)) return false;
                        if (defList.Contains(bv.BV.Item)) continue;
                        defList.Add(bv.BV.Item);
                        //isInters = bv.BV.Item.Intersects(P0, Pd, ref intersPts) || isInters;
                    }
                }
                return isInters;
            }
        }

        private KDNode Root;
        private KDTreeExtend() { }
        public enum KdtreeType { Volumes, Points }
        private KdtreeType TreeType;
        public KDTreeExtend(List<DefaultShape> objects, KdtreeType type)
        {
            
            if (type == KdtreeType.Points)
            {
                Cuboid mainBoundary = Cuboid.CreateCuboid(objects);
                Root = new KDNode(mainBoundary, 0, null);
                List<BoundingPoint> bPointList = new List<BoundingPoint>();
                foreach (DefaultShape shape in objects)
                {
                    if (shape is Plane || !shape.IsActive) continue;
                    BoundingVolume bv = new BoundingVolume(shape);
                    //if (shape is CustomObject)
                    //{
                    //    CustomObject cust = shape as CustomObject;
                    //    foreach (Vertex vert in cust.VertexList)
                    //    {
                    //        BoundingPoint bp = new BoundingPoint(vert, bv);
                    //        bPointList.Add(bp);
                    //    }
                    //}
                    //else
                    //{
                        BoundingPoint[] points = BoundingPoint.FromBoundingVolume(bv);
                        //BoundingPoint bp = new BoundingPoint(bv.Center, bv);
                        bPointList.AddRange(points);
                    //}
                }
                Root.AddPoints(bPointList);
                Root.RecreateBoundingVolumes();
            }
            
        }

        public bool Intersection(Vektor P0, Vektor Pd, ref List<SolidPoint> intersPts)
        {
            List<DefaultShape> defList = new List<DefaultShape>();
            Root.Intersection(P0, Pd, ref intersPts, ref defList);
            bool isInters = false;
            foreach (DefaultShape ds in defList)
            {
                isInters = ds.Intersects(P0, Pd, ref intersPts) || isInters;
            }
            return isInters;
        }

    }
}
