using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracerLib;
using Mathematics;

namespace EditorLib
{
    public class DrawingLight : DrawingObject
    {
        /// <summary>
        /// umisteni v editoru = poloha
        /// </summary>
        public Vektor Center
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
            Points = new Vektor[1];
            this.Center = new Vektor(light.Coord.X, light.Coord.Y, light.Coord.Z);
            this.ModelObject = light;
            SetLabelPrefix("light");
        }


        public override void SetModelObject(object modelObject)
        {
            if (modelObject is Light)
            {
                Light light = (Light)modelObject;
                this.ModelObject = light;
                this.Center = new Vektor(light.Coord.X, light.Coord.Y, light.Coord.Z);
            }

        }

        public override Vektor GetCenter()
        {
            return new Vektor(this.Center);
        }
    }
}
