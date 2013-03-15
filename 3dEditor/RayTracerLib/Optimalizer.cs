using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracerLib
{
    public class Optimalizer
    {
        public enum OptimizeType { NONE, RTREE, OCTREE, KDTREE, KDTREE_POINTS };
        public OptimizeType OptimType;
        public IOptimize Optimizer;

        public Optimalizer(OptimizeType type, List<DefaultShape> objects)
        {
            OptimType = type;
            if (type == OptimizeType.NONE)
            {
                Optimizer = new NaiveRaytracing(objects);
            }
            else if (type == OptimizeType.RTREE)
            {
                Optimizer = new RTree(objects);
            }
            else if(type == OptimizeType.OCTREE)
            {
                Optimizer = new Octree(objects);
            }
            else if (type == OptimizeType.KDTREE)
            {
                Optimizer = new KDTree(objects, KDTree.KdtreeType.Volumes);
            }
            else if (type == OptimizeType.KDTREE_POINTS)
            {
                Optimizer = new KDTreeExtend(objects, KDTreeExtend.KdtreeType.Points);
            }

        }

    }
}
