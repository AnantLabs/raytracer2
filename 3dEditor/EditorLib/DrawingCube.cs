using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorLib
{
    /// <summary>
    /// Krychle
    /// -- stred v Points[0]
    /// </summary>
    public class DrawingCube : DrawingObject
    {
        public Point3D Center
        {
            get { return Points[0]; }
            set { Points[0] = value; }
        }

        public DrawingCube() : this(0, 0, 0) { }
            
        public DrawingCube(double centerX, double centerY, double centerZ)
        {
            Points = new Point3D[9];

            Point3D center = new Point3D(centerX, centerY, centerZ);
            Points[0] = center;

            double sideLen = 1;
            double sideLenHalf = sideLen / 2.0;
            Point3D upper1 = new Point3D(center.X - sideLenHalf, center.Y + sideLenHalf, center.Z - sideLenHalf);
            Points[1] = upper1;
            Point3D upper2 = new Point3D(center.X + sideLenHalf, center.Y + sideLenHalf, center.Z - sideLenHalf);
            Points[2] = upper2;
            Point3D upper3 = new Point3D(center.X - sideLenHalf, center.Y + sideLenHalf, center.Z + sideLenHalf);
            Points[3] = upper3;
            Point3D upper4 = new Point3D(center.X + sideLenHalf, center.Y + sideLenHalf, center.Z + sideLenHalf);
            Points[4] = upper4;
            
            Point3D lower1 = new Point3D(center.X - sideLenHalf, center.Y - sideLenHalf, center.Z - sideLenHalf);
            Points[5] = lower1;
            Point3D lower2 = new Point3D(center.X + sideLenHalf, center.Y - sideLenHalf, center.Z - sideLenHalf);
            Points[6] = lower2;
            Point3D lower3 = new Point3D(center.X - sideLenHalf, center.Y - sideLenHalf, center.Z + sideLenHalf);
            Points[7] = lower3;
            Point3D lower4 = new Point3D(center.X + sideLenHalf, center.Y - sideLenHalf, center.Z + sideLenHalf);
            Points[8] = lower4;

            Lines = new List<Line3D>(12);
            Lines.Add(new Line3D(upper1, upper2));
            Lines.Add(new Line3D(upper1, upper3));
            Lines.Add(new Line3D(upper2, upper4));
            Lines.Add(new Line3D(upper3, upper4));

            Lines.Add(new Line3D(lower1, lower2));
            Lines.Add(new Line3D(lower1, lower3));
            Lines.Add(new Line3D(lower2, lower4));
            Lines.Add(new Line3D(lower3, lower4));

            Lines.Add(new Line3D(lower1, upper1));
            Lines.Add(new Line3D(lower2, upper2));
            Lines.Add(new Line3D(lower3, upper3));
            Lines.Add(new Line3D(lower4, upper4));
            

        }
    }
}
