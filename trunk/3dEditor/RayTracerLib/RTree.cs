using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{
    public class RTree
    {
        public RtreeNode Root { get; private set; }
        /// <summary>
        /// maximalni pocet zaznamu v uzlu
        /// </summary>
        public const int _MAX_SIZE = 4;

        /// <summary>
        /// citac uzlu
        /// </summary>
        public static int _Counter = 0;

        public RTree()
        {

        }
        public RTree(DefaultShape[] objects)
        {
            List<RtreeNode> nodes = new List<RtreeNode>();
            foreach (DefaultShape obj in objects)
            {
                if (obj is Plane || obj is Plane2) continue;

                RtreeNode node = new RtreeNode(obj);
                nodes.Add(node);
            }

            Cuboid[] cuboids = new Cuboid[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
                cuboids[i] = nodes[i].MBR;

            this.Root = STR3DPack(cuboids);
        }

        /// <summary>
        /// Najde vsechny body pruniku ve strome obsahujicim vsechny objekty v listech.
        /// Prochazi strom od korene a testuje, zda paprsek protina kazdy uzel - neboli Cuboid uzlu.
        /// Kdyz paprsek protne Cuboid uzlu, prochazime bud dal strom do potomku daneho uzlu.
        /// Je-li uzel jiz list, otestujeme, zda paprsek protina i objekt jemu prirazeny.
        /// </summary>
        /// <param name="P0">pocatek paprsku</param>
        /// <param name="Pd">smer paprsku</param>
        /// <param name="intersPts">seznam pruseciku paprsku s objekty</param>
        /// <returns>true, kdyz paprsek protne uzel a jeho prislusny objekt</returns>
        public bool TestIntersection(Vektor P0, Vektor Pd, ref List<SolidPoint> intersPts)
        {
            return TestIntersection(this.Root, P0, Pd, ref intersPts);
        }

        /// <summary>
        /// Najde vsechny body pruniku ve strome obsahujicim vsechny objekty v listech.
        /// Prochazi strom od korene a testuje, zda paprsek protina kazdy uzel - neboli Cuboid uzlu.
        /// Kdyz paprsek protne Cuboid uzlu, prochazime bud dal strom do potomku daneho uzlu.
        /// Je-li uzel jiz list, otestujeme, zda paprsek protina i objekt jemu prirazeny.
        /// </summary>
        /// <param name="root">koren stromu, odkud se ma zacit prohledavat</param>
        /// <param name="P0">pocatek paprsku</param>
        /// <param name="Pd">smer paprsku</param>
        /// <param name="intersPts">seznam pruseciku paprsku s objekty</param>
        /// <returns>true, kdyz paprsek protne uzel a jeho prislusny objekt</returns>
        public bool TestIntersection(RtreeNode root, Vektor P0, Vektor Pd, ref List<SolidPoint> intersPts)
        {
            if (root == null) return false;

            //// JE OTAZKA, ZDA V LISTU DRIVE TESTOVAT PRUNIK S CUBOIDEM, NEBO PRUNIK S DATA OBJEKTEM
            //// 1) nejdrive objekt: prave C testu slozitejsich
            //// 2) nejdrive cuboid: minimalne C testu jednodussich + maximalne C testu slozitejsich = celkem maximalne 2*C testu
            //// byla zvolena prvni varianta jako efektivnejsi


            if (!root.MBR.IntersectsRay(P0, Pd))        // neprotina-li paprsek cuboid - konec prohledavani
                return false;

            /// UZEL JE LIST
            /// 
            // je-li uzel list, je vetsi pravdepodobnost, ze paprsek protina objekt, 
            // proto rovnou testujeme objekt uvnitr cuboidu
            if (root.IsLeaf)                            // je-li uzel list, otestujeme, zda paprsek protina prirazeny objekt
                return root.DataItem.Intersects(P0, Pd, ref intersPts);


            /// UZEL NENI LIST
            /// 
            // nejdrive otestujeme prunik paprsku s cuboiudem uzlu



            bool isInters = false;
            foreach (RtreeNode child in root.ChildList) // neni-li uzel list, zavolame rekurzivne na vsechny potomky uzlu
            {
                if (child == null) continue;
                isInters = TestIntersection(child, P0, Pd, ref intersPts) || isInters;
            }
            return isInters;
        }

        public RtreeNode STR3DPack(Cuboid[] cuboids)
        {
            int dim = 3;
            int N = cuboids.Length;         // pocet zaznamu na ulozeni
            int C = _MAX_SIZE;              // maximalni pocet zaznamu v jenom uzlu stromu
            int L = (int)Math.Ceiling((double)N / C);   // s ... pocet uzlu potrebnych k ulozeni vsech zaznamu
            int SlabsCount = (int)Math.Ceiling(Math.Pow(L, 1.0 / dim));     // pocet rezu v aktualni dimenzi

            Cuboid[][][] slabs = CreateSlabs3D(cuboids, dim);

            RtreeNode parent = null;
            List<RtreeNode> nodeList = new List<RtreeNode>();
            for (int i = 0; i < slabs.Length; i++)
            {
                for (int j = 0; j < slabs[i].Length; j++)
                {
                    parent = new RtreeNode();
                    for (int k = 0; k < slabs[i][j].Length; k++)
                    {
                        if (slabs[i][j][k] == null)
                        {
                            break;
                        }

                        //RtreeNode node = new RtreeNode(slabs[i][j][k]);
                        RtreeNode node = slabs[i][j][k].CurrentNode;
                        if (!parent.TryAddChild(node))
                        {
                            nodeList.Add(parent);
                            parent = new RtreeNode();
                            parent.TryAddChild(node);
                        }
                    }
                    if (parent.GetChildrenSize() > 0)
                        nodeList.Add(parent);
                }
            }

            if (nodeList.Count > 1)
            {
                List<Cuboid> nextLevCubs = new List<Cuboid>();
                foreach (RtreeNode node in nodeList)
                {
                    nextLevCubs.Add(node.MBR);
                }
                return STR3DPack(nextLevCubs.ToArray());
            }
            else return nodeList[0];
        }

        private Cuboid[][][] CreateSlabs3D(Cuboid[] cuboids, int dim)
        {
            int N = cuboids.Length;         // pocet zaznamu na ulozeni
            int C = _MAX_SIZE;              // maximalni pocet zaznamu v jenom uzlu stromu
            int L = (int)Math.Ceiling((double)N / C);   // s ... pocet uzlu potrebnych k ulozeni vsech zaznamu
            int SlabsCount = (int)Math.Ceiling(Math.Pow(L, 1.0 / dim));     // pocet rezu v aktualni dimenzi

            int slabSize = (int)Math.Ceiling(Math.Pow(L, (dim - 1.0) / dim));  // pocet vsech zaznamu v aktualni dimenzi
            slabSize = slabSize * C;

            Cuboid.CompareOrder = Cuboid.CuboidComparerType.Xorder;
            Array.Sort<Cuboid>(cuboids, Cuboid.CuboidComparer);                // serazeni zaznamu podle aktualni dimenze

            Cuboid[][][] slabs = new Cuboid[SlabsCount][][];
           
            for (int i = 0; i < SlabsCount; i++)
            {
                Cuboid[] actualCubs = new Cuboid[slabSize];
                for (int j = 0; j < slabSize; j++)
                {
                    if (i * slabSize + j >= N)
                        break;
                    actualCubs[j] = cuboids[i * slabSize + j];
                }
                Cuboid[][] sortedSlabs = CreateSlabs2D(actualCubs, dim - 1);
                slabs[i] = sortedSlabs;
            }
            return slabs;
        }

        private Cuboid[][] CreateSlabs2D(Cuboid[] cuboids, int dim)
        {
            int N = cuboids.Length;         // pocet zaznamu na ulozeni
            int C = _MAX_SIZE;              // maximalni pocet zaznamu v jenom uzlu stromu
            int L = (int)Math.Ceiling((double)N / C);   // s ... pocet uzlu potrebnych k ulozeni vsech zaznamu
            int SlabsCount = (int)Math.Ceiling(Math.Pow(L, 1.0 / dim));     // pocet rezu v aktualni dimenzi

            int slabSize = (int)Math.Ceiling(Math.Pow(L, (dim - 1.0) / dim));  // pocet vsech zaznamu v aktualni dimenzi
            slabSize = slabSize * C;

            Cuboid.CompareOrder = Cuboid.CuboidComparerType.Yorder;
            Array.Sort<Cuboid>(cuboids, Cuboid.CuboidComparer);                // serazeni zaznamu podle aktualni dimenze

            Cuboid[][] slabs = new Cuboid[SlabsCount][];
            for (int i = 0; i < SlabsCount; i++)
            {
                Cuboid[] actualCubs = new Cuboid[slabSize];
                for (int j = 0; j < slabSize; j++)
                {
                    if (i * slabSize + j >= N)
                        break;
                    actualCubs[j] = cuboids[i * slabSize + j];
                }
                Cuboid.CompareOrder = Cuboid.CuboidComparerType.Zorder;
                Array.Sort<Cuboid>(actualCubs, Cuboid.CuboidComparer);
                slabs[i] = actualCubs;
            }
            return slabs;
        }


        public static Sphere[] CreateRndSpheres(int count)
        {
            Random rnd = new Random();
            Sphere[] sphs = new Sphere[count];
            for (int i = 0; i < count; i++)
            {
                sphs[i] = new Sphere(new Vektor(rnd.Next(-30, 30), rnd.Next(-30, 30), rnd.Next(-30, 30)), rnd.Next(10));
            }
            return sphs;
        }

        public static Cube[] CreateRndCubes(int count)
        {
            Random rnd = new Random();
            Cube[] cubes = new Cube[count];
            for (int i = 0; i < count; i++)
            {
                Vektor rndCenter = new Vektor(rnd.Next(-30, 30), rnd.Next(-30, 30), rnd.Next(-30, 30));
                Vektor rndDir = new Vektor(rnd.Next(-30, 30), rnd.Next(-30, 30), rnd.Next(-30, 30));
                cubes[i] = new Cube(rndCenter, rndDir, rnd.Next(5));
            }
            return cubes;
        }

        public static Cylinder[] CreateRndCyls(int count)
        {
            Random rnd = new Random();
            Cylinder[] cyls = new Cylinder[count];
            for (int i = 0; i < count; i++)
            {
                Vektor rndCenter = new Vektor(rnd.Next(-30, 30), rnd.Next(-30, 30), rnd.Next(-30, 30));
                Vektor rndDir = new Vektor(rnd.Next(-30, 30), rnd.Next(-30, 30), rnd.Next(-30, 30));
                cyls[i] = new Cylinder(rndCenter, rndDir, rnd.Next(10), rnd.Next(5));
            }
            return cyls;
        }

        public static bool Testing1()
        {
            Cuboid c1 = new Cuboid(new Vektor(2, -2, 2), 1);
            Cuboid c2 = new Cuboid(new Vektor(-2, 2, -2), 1);
            Cuboid c3 = null;
            Cuboid.CompareOrder = Cuboid.CuboidComparerType.Xorder;
            Cuboid[] cs = new Cuboid[] { null, c1, c2, c3, null, null, null, null };
//            Array.Sort<Cuboid>(cs);
            Array.Sort<Cuboid>(cs, Cuboid.CuboidComparer);


            Cuboid.CompareOrder = Cuboid.CuboidComparerType.Yorder;
            Array.Sort<Cuboid>(cs, Cuboid.CuboidComparer);
            Cuboid.CompareOrder = Cuboid.CuboidComparerType.Zorder;
            Array.Sort<Cuboid>(cs, Cuboid.CuboidComparer);

            int count = 55;
            Sphere[] sphs = RTree.CreateRndSpheres(count);
            Cube[] cubes = RTree.CreateRndCubes(count);
            Cylinder[] cyls = RTree.CreateRndCyls(count);

            int total = count * 3;
            List<RtreeNode> nodes = new List<RtreeNode>();
            for (int i = 0; i < count; i++)
            {
                nodes.Add(new RtreeNode(sphs[i]));
                nodes.Add(new RtreeNode(cubes[i]));
                nodes.Add(new RtreeNode(cyls[i]));
            }

            Cuboid[] cuboids = new Cuboid[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
                cuboids[i] = nodes[i].MBR;
            RTree rtree = new RTree();
            rtree.Root = rtree.STR3DPack(cuboids);

            if (rtree.Root == null)
                return false;

            int nodesInTree = rtree.GetNumberNodes(rtree.Root);
            int leavesInTree = rtree.GetNumberLeaves(rtree.Root);
            if (leavesInTree != total)
                return false;

            return true;
        }

        public static bool Testing2()
        {
            Sphere sph1 = new Sphere(new Vektor(-1, -1, -1), 1);
            Sphere sph2 = new Sphere(new Vektor(1, 1, 1), 1);
            Sphere sph3 = new Sphere(new Vektor(3, 3, 3), 1);
            Sphere sph4 = new Sphere(new Vektor(5, 5, 5), 1);
            Sphere sph5 = new Sphere(new Vektor(7, 7, 7), 1);

            int count = 20;
            List<RtreeNode> nodes = new List<RtreeNode>();
            nodes.Add(new RtreeNode(sph1));
            Cuboid totalCuboid = nodes[0].MBR;
            for (int i = 0; i < count; i++)
            {
                int sgn = (int)Math.Pow(-1, i);
                Sphere sph = new Sphere(new Vektor(i, sgn * i, i), 1);
                nodes.Add(new RtreeNode(sph));
                totalCuboid += nodes[i + 1].MBR;
            }

            RtreeNode node1 = new RtreeNode(sph1);
            RtreeNode node2 = new RtreeNode(sph2);
            RtreeNode node3 = new RtreeNode(sph3);
            RtreeNode node4 = new RtreeNode(sph4);
            RtreeNode node5 = new RtreeNode(sph5);

            Cuboid resCuboid = node1.MBR + node2.MBR + node3.MBR + node4.MBR + node5.MBR;
            RTree tree = new RTree();
            RtreeNode root = tree.STR3DPack(new Cuboid[] { node1.MBR, node2.MBR, node3.MBR, node4.MBR, node5.MBR });

            Cuboid[] cuboids = new Cuboid[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
                cuboids[i] = nodes[i].MBR;
            RtreeNode root2 = tree.STR3DPack(cuboids);
            
            return true;
        }

        /// <summary>
        /// Vrati pocet listu ve strome a otestuje, zda jsou opravdu listy, tedy nemaji potomky
        /// </summary>
        /// <param name="root">startovni uzel</param>
        /// <returns>pocet listu</returns>
        public int GetNumberLeaves(RtreeNode root)
        {
            if (root == null)
                return 0;

            int count = 0;
            if (root.IsLeaf)
                count = 1;

            for (int i = 0; i < root.ChildList.Length; i++)
            {
                if (root.ChildList[i] != null)
                    count += GetNumberLeaves(root.ChildList[i]);
            }
            return count;
        }

        /// <summary>
        /// Vrati pocet uzlu ve strome
        /// </summary>
        /// <param name="root">startovni uzel</param>
        /// <returns>pocet vsechn uzlu</returns>
        public int GetNumberNodes(RtreeNode root)
        {
            if (root == null)
                return 0;

            int count = 1;
            for (int i = 0; i < root.ChildList.Length; i++)
            {
                if (root.ChildList[i] != null)
                    count += GetNumberNodes(root.ChildList[i]);
            }
            return count;
        }
    }
}
