using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracerLib;
using Mathematics;

namespace EditorLib
{
    public class DrawingFacet : DrawingTriangle
    {
        private RayTracerLib.Triangle trian;
        public DrawingCustom DrCustObject { get; private set; }

        public DrawingFacet() : base() { }
        public DrawingFacet(RayTracerLib.Triangle trian, DrawingCustom drCust) : base(trian)
        {
            // TODO: Complete member initialization
            this.trian = trian;
            DrCustObject = drCust;
            SetLabelPrefix("facet");
        }
    }
}
