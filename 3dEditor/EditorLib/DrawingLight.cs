using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracerLib;
using Mathematics;
using System.Drawing;

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

        public Pen PenLight;
        double len = 0.4;

        public DrawingLight() : this(new Light()) { }

        public DrawingLight(Light light)
        {
            Points = new Vektor[1];
            //SetLabelPrefix("light");
            Set(light);
        }


        public override void SetModelObject(object modelObject)
        {
            if (modelObject is Light)
            {
                Light light = (Light)modelObject;
                Set(light);
            }

        }

        private void Set(Light light)
        {
            PenLight = new Pen(light.Color.SystemColor(),2);
            this.ModelObject = light;
            List<Vektor> points = new List<Vektor>();
            Vektor center = new Vektor(light.Coord.X, light.Coord.Y, light.Coord.Z);
            points.Add(center);
            Vektor xz1 = new Vektor(center.X - len, center.Y, center.Z - len);
            Vektor xz2 = new Vektor(center.X + len, center.Y, center.Z - len);
            Vektor xz3 = new Vektor(center.X + len, center.Y, center.Z + len);
            Vektor xz4 = new Vektor(center.X - len, center.Y, center.Z + len);
            points.Add(xz1);
            points.Add(xz2);
            points.Add(xz3);
            points.Add(xz4);
            Lines = new List<Line3D>();
            Lines.Add(new Line3D(xz1, xz2));
            Lines.Add(new Line3D(xz2, xz3));
            Lines.Add(new Line3D(xz3, xz4));
            Lines.Add(new Line3D(xz4, xz1));

            xz1 = new Vektor(center.X, center.Y - len, center.Z - len);
            xz2 = new Vektor(center.X, center.Y + len, center.Z - len);
            xz3 = new Vektor(center.X, center.Y + len, center.Z + len);
            xz4 = new Vektor(center.X, center.Y - len, center.Z + len);
            points.Add(xz1);
            points.Add(xz2);
            points.Add(xz3);
            points.Add(xz4);
            Lines.Add(new Line3D(xz1, xz2));
            Lines.Add(new Line3D(xz2, xz3));
            Lines.Add(new Line3D(xz3, xz4));
            Lines.Add(new Line3D(xz4, xz1));
            Points = points.ToArray();
        }

        public override Vektor GetCenter()
        {
            return new Vektor(this.Center);
        }
    }
}
