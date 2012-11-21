using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracerLib
{
    /// <summary>
    /// Trida stejna jako Cuboid, jen bude obsahovat ukazatel na Uzel Rstromu, ve kterem je ulozen a jemuz prislusi
    /// </summary>
    public class CuboidNode : Cuboid
    {
        RtreeNode CurrtentNode { get; set; }

        public CuboidNode(RtreeNode cuboid)
        {

        }
    }
}
