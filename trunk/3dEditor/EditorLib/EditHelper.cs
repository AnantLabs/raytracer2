using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace EditorLib
{
    public class EditHelper
    {
        public class ComboViewAngle
        {
            public String Caption { get; private set; }
            public int degX { get; private set; }
            public int degY { get; private set; }
            public int degZ { get; private set; }
            public ComboViewAngle(string caption, int x, int y, int z)
            {
                Caption = caption;
                degX = x;
                degY = y;
                degZ = z;
            }

            public override string ToString() { return Caption; }
        }

        public int MagnifCoef { get; set; }
        public int Zoom { get; set; }

        // styl pro bybrany objekt
        public const float PenSelectedWidth = 1.9f;
        public const float PenNormalWidth = 1.4f;

        public ComboViewAngle[] ComboViewObjects { get; private set; }

        /// <summary>
        /// seznam objektu, na ktere je mozne kliknout
        /// </summary>
        private List<EditorObject> ClickableObjects { get; set; }

        public EditHelper()
        {
            MagnifCoef = 150;
            Zoom = 10;
            ClickableObjects = new List<EditorObject>();

            List<ComboViewAngle> listCombos = new List<ComboViewAngle>();
            listCombos.Add(new ComboViewAngle("Front", 150, 340, 350));
            listCombos.Add(new ComboViewAngle("Side +X", 315, 250, 135));
            listCombos.Add(new ComboViewAngle("Side -X", 45, 255, 225));
            listCombos.Add(new ComboViewAngle("Top", 270, 0, 0));
            listCombos.Add(new ComboViewAngle("Reset X", 180, 270, 0));
            listCombos.Add(new ComboViewAngle("Reset Y", 90, 0, 0));
            listCombos.Add(new ComboViewAngle("Reset Z", 0, 180, 180));

            ComboViewObjects = listCombos.ToArray();
        }

        /// <summary>
        /// vytvori mrizku
        /// </summary>
        /// <param name="points4Edge">pocet bodu mrizky, bude nasoben treti mocninou, init=1</param>
        /// <returns></returns>
        public static List<Line3D> FillGrid(int points4Edge)
        {
            if (points4Edge < 1)
                throw new ArgumentOutOfRangeException("wrong number of grid points");

            int pointsCount = points4Edge + 1;
            pointsCount = pointsCount * pointsCount * pointsCount;  // celkovy pocet bodu mrizky
            List<Line3D> grid = new List<Line3D>(pointsCount);


            List<Point3D> points = new List<Point3D>(pointsCount);
            double delta = 2.0/points4Edge;
            for (double i = -1.0; i <= 1.0; i += delta)
            {
                for (double j = -1.0; j <= 1.0; j += delta)
                {
                    for (double k = -1.0; k <= 1.0; k += delta)
                    {
                        Point3D p = new Point3D(i, j, k);
                        points.Add(p);
                    }
                }
            }

            Point3D[] pointsList = points.ToArray();
            for (int i=0; i<pointsList.Length-1; i++)
                for (int j = i + 1; j < pointsList.Length; j++)
                {
                    if (PointsDiffers(pointsList[i], pointsList[j]))
                        grid.Add(new Line3D(new Point3D(pointsList[i]), new Point3D(pointsList[j])));

                }


            return grid;
        }

        /// <summary>
        /// vrati true, kdyz se body lisi prave v jednom cisle
        /// true: -1,0,1 a -1,0,0
        /// false: -1,0,1 a -1,1,0
        /// </summary>
        /// <param name="p1">point #1</param>
        /// <param name="p2">point #2</param>
        /// <returns>true, kdyz se body lisi prave v jednom cisle</returns>
        public static bool PointsDiffers(Point3D p1, Point3D p2)
        {
            if (p1.X != p2.X)
            {
                if (p1.Y != p2.Y || p1.Z != p2.Z)
                    return false;
                else
                    return true;
            }

            if (p1.Y != p2.Y)
            {
                if (p1.X != p2.X || p1.Z != p2.Z)
                    return false;
                else
                    return true;
            }

            if (p1.Z != p2.Z)
            {
                if (p1.X != p2.X || p1.Y != p2.Y)
                    return false;
                else
                    return true;
            }
            return true;
        }

        /// <summary>
        /// odebere vsechny klikatelne objekty ze seznamu
        /// </summary>
        public void ClearAllClickableObjects()
        {
            this.ClickableObjects.Clear();
        }

        /// <summary>
        /// prida novy klikatelny objekt do seznamu
        /// </summary>
        /// <param name="editorObject"></param>
        public void AddClickableObject(EditorObject editorObject)
        {
            this.ClickableObjects.Add(editorObject);
        }

        /// <summary>
        /// vrati objekt, ktery obsahuje dany bod
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public List<DrawingObject> GetClickableObj(Point point)
        {
            List<DrawingObject> clickedList = new List<DrawingObject>();
            foreach (EditorObject obj in ClickableObjects)
            {
                if (obj.IsContained(point))
                {
                    clickedList.Add(obj.AssociatedObject);
                }
            }
            return clickedList;
        }
    }
}
