using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracerLib
{
    public class Optimalizer
    {
        public enum OptimizeType { NONE, RTREE, OCTREE };
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
        }

    }
}
