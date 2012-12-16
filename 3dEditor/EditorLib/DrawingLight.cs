using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracerLib;

namespace EditorLib
{
    public class DrawingLight : DrawingObject
    {
        /// <summary>
        /// umisteni v editoru = poloha
        /// </summary>
        public Point3D Center
        {
            get
            {
                return Points[0];
            }
            private set
            {
                Points[0] = value;
            }
        }


        public DrawingLight() : this(new Light()) { }

        public DrawingLight(Light light)
        {
            Points = new Point3D[1];
            this.Center = new Point3D(light.Coord.X, light.Coord.Y, light.Coord.Z);
            this.ModelObject = light;
        }

        public override void SetModelObject(object modelObject)
        {
            if (modelObject is Light)
            {
                Light light = (Light)modelObject;
                this.ModelObject = light;
                this.Center = new Point3D(light.Coord.X, light.Coord.Y, light.Coord.Z);
            }

        }

        public override Point3D GetCenter()
        {
            return new Point3D(this.Center);
        }
    }
}
