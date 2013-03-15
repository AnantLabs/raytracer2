using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracerLib
{
    public class NaiveRaytracing : IOptimize
    {
        private List<DefaultShape> DefaultShapes;

        public NaiveRaytracing(List<DefaultShape> defaultShapes)
        {
            DefaultShapes = defaultShapes;
        }
        public bool Intersection(Mathematics.Vektor P0, Mathematics.Vektor Pd, ref List<SolidPoint> intersPts)
        {
            bool isInters = false;
            foreach (DefaultShape df in DefaultShapes)
            {
                if (df is Plane) continue;
                isInters = df.Intersects(P0, Pd, ref intersPts) || isInters;
            }
            return isInters;
        }
    }
}
