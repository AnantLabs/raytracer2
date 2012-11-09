using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTree
{
    public class Node
    {
        public int ID;
        public Cuboid MBR;
        public double Xcoord;
        public double Ycoord;
        public double Zcoord;
        public bool IsLeaf;
        public double MeanCentre;
        public double Dispersion;
        public int MaxRecords;
        public Node Parent;
    }
}
