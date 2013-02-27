using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{
    public class OctNode
    {
        private Cuboid Boundary;
        private List<BoundingVolume> Points;
        private OctNode[] ChildList;
        private OctNode Parrent;
        /// <summary>
        /// nejvetsi dimenze kostky, neboli delka vsech stran kostky
        /// </summary>
        private double Size;
        private int Depth = 0;
        private const double MIN_SIZE = 0.1;
        private const int MAX_DEPTH = 10;

        private OctNode() { }
        /// <summary>
        /// vytvori novy uzel v OCTREE
        /// </summary>
        /// <param name="boundary">prostorova oblast uzlu</param>
        /// <param name="depth">hloubka uzlu ve strome</param>
        public OctNode(Cuboid boundary, int depth, OctNode parrent)
        {
            Boundary = boundary;
            Depth = depth;
            Points = new List<BoundingVolume>();
            // test, if Size == boundar.Size
            
            Size = boundary.Xmax - boundary.Xmin;
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
            this.Boundary = Points[0].BV;
            foreach (BoundingVolume bv in Points)
            {
                this.Boundary = Cuboid.Union(this.Boundary, bv.BV);
            }
            if (ChildList != null)
                foreach (OctNode child in ChildList)
                    child.RecreateBoundingVolumes();
        }
        public bool AddPoint(BoundingVolume point)
        {
            if (Boundary.Contains(point.Center))
            {
                Points.Add(point);
                return true;
            }
            return false;
        }

        public void Subdivide()
        {
            // uzel nerozdelime, kdyz
            if (Points.Count < 2 ||     // pocet bodu v nem je mene nez 2
                Size <= MIN_SIZE ||     // delka steny je mensi, nez povolene minimum
                Depth >= MAX_DEPTH      // hloubka stromu je vetsi, nez povolene maximum
                ) return;

            ChildList = new OctNode[8];
            Vektor mid = Boundary.GetCenter();
            double sizeCtvrt = Size / 4;
            double sizePul = Size / 2;
            Vektor cent0 = new Vektor(mid.X - sizeCtvrt, mid.Y - sizeCtvrt, mid.Z - sizeCtvrt);
            Cuboid cub0 = new Cuboid(cent0, sizePul);
            ChildList[0] = new OctNode(cub0, Depth + 1, this);
            Vektor cent1 = new Vektor(mid.X - sizeCtvrt, mid.Y + sizeCtvrt, mid.Z - sizeCtvrt);
            Cuboid cub1 = new Cuboid(cent1, sizePul);
            ChildList[1] = new OctNode(cub1, Depth + 1, this);
            Vektor cent2 = new Vektor(mid.X + sizeCtvrt, mid.Y - sizeCtvrt, mid.Z - sizeCtvrt);
            Cuboid cub2 = new Cuboid(cent2, sizePul);
            ChildList[2] = new OctNode(cub2, Depth + 1, this);
            Vektor cent3 = new Vektor(mid.X + sizeCtvrt, mid.Y + sizeCtvrt, mid.Z - sizeCtvrt);
            Cuboid cub3 = new Cuboid(cent3, sizePul);
            ChildList[3] = new OctNode(cub3, Depth + 1, this);
            Vektor cent4 = new Vektor(mid.X - sizeCtvrt, mid.Y - sizeCtvrt, mid.Z + sizeCtvrt);
            Cuboid cub4 = new Cuboid(cent4, sizePul);
            ChildList[4] = new OctNode(cub4, Depth + 1, this);
            Vektor cent5 = new Vektor(mid.X - sizeCtvrt, mid.Y + sizeCtvrt, mid.Z + sizeCtvrt);
            Cuboid cub5 = new Cuboid(cent5, sizePul);
            ChildList[5] = new OctNode(cub5, Depth + 1, this);
            Vektor cent6 = new Vektor(mid.X + sizeCtvrt, mid.Y - sizeCtvrt, mid.Z + sizeCtvrt);
            Cuboid cub6 = new Cuboid(cent6, sizePul);
            ChildList[6] = new OctNode(cub6, Depth + 1, this);
            Vektor cent7 = new Vektor(mid.X + sizeCtvrt, mid.Y + sizeCtvrt, mid.Z + sizeCtvrt);
            Cuboid cub7 = new Cuboid(cent7, sizePul);
            ChildList[7] = new OctNode(cub7, Depth + 1, this);

            // prerozdelime body
            // jeden bod muze byt obsazen ve vice uzlech - je-li na jejich hranici
            foreach (BoundingVolume bv in Points)
            {
                foreach(OctNode child in ChildList)
                {
                    if (child.AddPoint(bv)) break;   // pri pridavani bodu je predtim zkontrolovano, zda v ni bod muze lezet
                }
            }

            foreach (OctNode child in ChildList)
                child.Subdivide();
        }

        public bool IsLeaf()
        {
            if(ChildList == null)
                return true;
            return false;
        }

        public bool IsEmpty()
        {
            return (this.Points.Count == 0);
        }

        public bool Intersection(Vektor P0, Vektor Pd, ref List<SolidPoint> intersPts)
        {
            //if (root == null) return false;
            if (Points.Count  == 0) return false;

            if (!Boundary.IntersectsRay(P0, Pd))        // neprotina-li paprsek bounding volume - konec prohledavani
                return false;

            /// UZEL JE LIST
            /// 
            // je-li uzel list, je vetsi pravdepodobnost, ze paprsek protina objekt, 
            // proto rovnou testujeme objekt uvnitr cuboidu
            bool isInters = false;
            if (!this.IsLeaf())
                foreach (OctNode child in ChildList)
                {
                    if (child != null)
                    {
                        isInters = child.Intersection(P0, Pd, ref intersPts) || isInters;
                    }
                }
            else
            {
                foreach (BoundingVolume bv in Points)
                {
                    isInters = bv.Item.Intersects(P0, Pd, ref intersPts) || isInters;
                }
            }
            return isInters;
        }

    }
    public class Octree : IOptimize
    {
        public OctNode Root;

        public Octree(List<DefaultShape> objects)
        {
            Cuboid cuboid = Cuboid.CreateCuboid(objects);
            Cuboid cube = Cuboid.CreateCubeFromCuboid(cuboid);
            Root = new OctNode(cube, 0, null);
            foreach (DefaultShape item in objects)
            {
                BoundingVolume bv = new BoundingVolume(item);
                Root.AddPoint(bv);
            }
            Root.Subdivide();
            Root.RecreateBoundingVolumes();
        }

        public bool Intersection(Vektor P0, Vektor Pd, ref List<SolidPoint> intersPts)
        {
            return this.Root.Intersection(P0, Pd, ref intersPts);
        }



    }
}
