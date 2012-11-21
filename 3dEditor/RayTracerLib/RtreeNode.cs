using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracerLib
{
    public class RtreeNode
    {
        public int NodeID;
        public Cuboid MBR;
        //public CuboidNode MBR;
        public bool IsLeaf;
        public RtreeNode[] ChildList;
        public RtreeNode Parent { get; private set; }

        /// <summary>
        /// objekt umisteny v listu stromu
        /// </summary>
        public DefaultShape DataItem { get; private set; }

        public RtreeNode()
        {
            this.NodeID = RTree._Counter++;
            this.MBR = null;
            DataItem = null;
            IsLeaf = true;
            ChildList = new RtreeNode[RTree._MAX_SIZE];
        }
        public RtreeNode(Cuboid mbr)
        {
            this.NodeID = RTree._Counter++;
            this.MBR = mbr;
            this.MBR.CurrentNode = this;
            DataItem = null;
            IsLeaf = true;
            ChildList = new RtreeNode[RTree._MAX_SIZE];
        }
        public RtreeNode(DefaultShape item)
        {
            if (item is Plane || item is Plane2) return;
            this.NodeID = RTree._Counter++;
            this.MBR = Cuboid.CreateCuboid(item);
            this.MBR.CurrentNode = this;
            DataItem = item;
            IsLeaf = true;
            ChildList = new RtreeNode[RTree._MAX_SIZE];
        }

        public int GetChildrenSize()
        {
            int size = 0;
            for (int i = 0; i < ChildList.Length; i++)
            {
                if (ChildList[i] != null)
                    size++;
            }
            return size;
        }

        public bool TryAddChild(RtreeNode child)
        {
            int i;
            for (i = 0; i < ChildList.Length; i++)
            {
                if (ChildList[i] == null) 
                    break;
            }
            if (i == ChildList.Length) return false;

            ChildList[i] = child;
            child.Parent = this;
            this.IsLeaf = false;
            this.MBR = this.MBR + child.MBR;
            this.MBR.CurrentNode = this;
            return true;
        }


    }
}
