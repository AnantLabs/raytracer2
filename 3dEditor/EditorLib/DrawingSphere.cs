using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorLib
{
    public class DrawingSphere : DrawingObject
    {
        public double Radius { get; private set; }

        public Point3D Center
        {
            get
            {
                return Points[0];
            }
            set
            {
                Points[0] = value;
            }
        }

        public DrawingSphere(Point3D center, double rad)
        {
            this.Points = new Point3D[1];
            this.Points[0] = center;
            this.Radius = rad;
            this.Lines = new List<Line3D>(0);
        }
    }
}
