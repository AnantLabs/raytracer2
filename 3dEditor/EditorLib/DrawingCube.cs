using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RayTracerLib;

namespace EditorLib
{
    /// <summary>
    /// Krychle
    /// -- stred v Points[0]
    /// </summary>
    public class DrawingCube : DrawingDefaultShape
    {

        public Point3D Center
        {
            get { return Points[0]; }
            set { Points[0] = value; }
        }

        public DrawingCube() : this(0, 0, 0) { }

        public DrawingCube(RayTracerLib.Cube cube)
        {
            this.SetModelObject(cube);
        }

        public DrawingCube(double centerX, double centerY, double centerZ)
        {
            Cube cube = new Cube(new Vektor(centerX, centerY, centerZ), new Vektor(1, 0, 0), 1);
            cube.Material = new Material();
            cube.Material.Color = new Colour(1, 0.5, 0.1, 1);
            this.SetModelObject(cube);
        }

        private void Set(Point3D center, double sideLen)
        {
            Points = new Point3D[9];

            Points[0] = center;

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

        /// <summary>
        /// vrati 6 ctveric pro polygony
        /// </summary>
        /// <returns></returns>
        public List<Point3D[]> GetQuarts()
        {
            List<Point3D[]> list = new List<Point3D[]>();

            Point3D[] sez1 = new Point3D[4];
            sez1[0] = Points[1];
            sez1[1] = Points[2];
            sez1[2] = Points[4];
            sez1[3] = Points[3];
            list.Add(sez1);

            Point3D[] sez2 = new Point3D[4];
            sez2[0] = Points[5];
            sez2[1] = Points[6];
            sez2[2] = Points[8];
            sez2[3] = Points[7];
            list.Add(sez2);

            Point3D[] sez3 = new Point3D[4];
            sez3[0] = Points[1];
            sez3[1] = Points[2];
            sez3[2] = Points[6];
            sez3[3] = Points[5];
            list.Add(sez3);

            Point3D[] sez4 = new Point3D[4];
            sez4[0] = Points[3];
            sez4[1] = Points[4];
            sez4[2] = Points[8];
            sez4[3] = Points[7];
            list.Add(sez4);

            Point3D[] sez5 = new Point3D[4];
            sez5[0] = Points[1];
            sez5[1] = Points[3];
            sez5[2] = Points[7];
            sez5[3] = Points[5];
            list.Add(sez5);

            Point3D[] sez6 = new Point3D[4];
            sez6[0] = Points[2];
            sez6[1] = Points[4];
            sez6[2] = Points[8];
            sez6[3] = Points[6];
            list.Add(sez6);

            return list;
        }


        public void SetModelObject(RayTracerLib.Cube cube)
        {
            this.ModelObject = cube;
            double sideLen = cube.Size;
            Point3D center = new Point3D(cube.Center.X, cube.Center.Y, cube.Center.Z);
            this.Set(center, sideLen);
        }
    }
}
