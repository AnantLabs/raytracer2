using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{
    public class BoundingVolume
    {
        public Cuboid BV { get; private set; }
        public DefaultShape Item { get; private set; }
        public Vektor Center { get; private set; }

        public BoundingVolume(DefaultShape ds)
        {
            Item = ds;
            BV = Cuboid.CreateCuboid(ds);
            Center = BV.GetCenter();
        }
    }
}
