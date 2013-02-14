using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Mathematics;

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

        public Pen CameraPenMain { get; set; }
        public Pen CameraPenUp { get; set; }
        public Pen CameraPenLight { get; set; }
        public Pen CameraPenEllips { get; set; }


        // styl pro bybrany objekt
        public const float PenSelectedWidth = 1.9f;
        public const float PenNormalWidth = 1.4f;

        public const String CAMERAVIEW_string = "Camera front";
        public const String CAMERAVIEW2_string = "Camera top";

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
            listCombos.Add(new ComboViewAngle("Side", 150, 340, 350));
            //listCombos.Add(new ComboViewAngle("Side +X", 315, 250, 135));
            //listCombos.Add(new ComboViewAngle("Side -X", 45, 255, 225));
            //listCombos.Add(new ComboViewAngle("Top", 270, 0, 0));
            listCombos.Add(new ComboViewAngle("Reset X", 180, 270, 0));
            listCombos.Add(new ComboViewAngle("Reset Y", 90, 0, 0));
            listCombos.Add(new ComboViewAngle("Reset Z", 0, 180, 180));
            listCombos.Add(new ComboViewAngle(CAMERAVIEW_string, 0, 0, 0));
            listCombos.Add(new ComboViewAngle(CAMERAVIEW2_string, 0, 0, 0));


            ComboViewObjects = listCombos.ToArray();

            // nastaveny pera kamery
            CameraPenMain = new Pen(Color.Black, 3);
            CameraPenMain.EndCap = LineCap.Custom;
            CameraPenMain.CustomEndCap = new AdjustableArrowCap(3f, 3f, false);
            CameraPenMain.DashStyle = DashStyle.DashDot;

            CameraPenUp = new Pen(Color.SaddleBrown, 2f);
            CameraPenUp.EndCap = LineCap.Custom;
            CameraPenUp.CustomEndCap = new AdjustableArrowCap(4f, 6f, false);
            CameraPenUp.DashStyle = DashStyle.DashDot;

            CameraPenLight = new Pen(Color.Black, 1);
            CameraPenLight.EndCap = LineCap.NoAnchor;
            CameraPenLight.DashStyle = DashStyle.DashDot;

            CameraPenEllips = new Pen(Color.Firebrick, 2);
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


            List<Vektor> points = new List<Vektor>(pointsCount);
            double delta = 2.0/points4Edge;
            for (double i = -1.0; i <= 1.0; i += delta)
            {
                for (double j = -1.0; j <= 1.0; j += delta)
                {
                    for (double k = -1.0; k <= 1.0; k += delta)
                    {
                        Vektor p = new Vektor(i, j, k);
                        points.Add(p);
                    }
                }
            }

            Vektor[] pointsList = points.ToArray();
            for (int i=0; i<pointsList.Length-1; i++)
                for (int j = i + 1; j < pointsList.Length; j++)
                {
                    if (PointsDiffers(pointsList[i], pointsList[j]))
                        grid.Add(new Line3D(new Vektor(pointsList[i]), new Vektor(pointsList[j])));

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
        public static bool PointsDiffers(Vektor p1, Vektor p2)
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
