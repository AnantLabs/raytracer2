using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{
    public interface IOptimize
    {
        bool Intersection(Vektor P0, Vektor Pd, ref List<SolidPoint> intersPts);
    }
}
