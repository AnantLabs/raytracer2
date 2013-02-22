using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Drawing2D;

using EditorLib;
using RayTracerLib;
using Mathematics;


namespace _3dEditor
{
  
    public partial class WndBoard : Form
    {

        EditHelper _editHelp;
        Graphics _g;
        Bitmap _editorBmp;
        Scene _currentScene;
        MenuDrawItemFlowLayout _selectedMenu;

        public Matrix3D RotationMatrix
        {
            get
            {
                return _matrixForever;
            }
            private set { }
        }

        /// <summary>
        /// indikuje, zda se ma updatovat a prekreslit vse v editoru
        /// </summary>
        bool _updateAll;
        /// <summary>
        /// vybrany objekt kliknutim
        /// </summary>
        public DrawingObject _Selected { get; private set; }

        Vektor _axisC3, _axisX3, _axisY3, _axisZ3, _axisXY;
        public static Vektor _POV;
        /// <summary>
        /// zda mame stisknute nejake (L | P) tlacitko mysi a zda posouvame mysi po platne
        /// </summary>
        bool _isDragging;

        /// <summary>
        /// zda posouvame objekt mysi
        /// </summary>
        bool _isTransforming;

        bool _canRotationChange;
        bool _canRotationChange2;


        int _scale;
        int _zoom;
        int _ZOOM_INCREMENT = 5;
        const int _SCALE_INIT = 150;
        const int _ZOOM_INIT = 70;
        /// <summary>
        /// pocatecni velikost mrizky
        /// </summary>
        const int _GRID_SIZE_INIT = 2;
        /// <summary>
        /// citlivost pro prepocitani souradnic
        /// </summary>
        int _MOUSE_SENSITIVITY = 12;

        /// <summary>
        /// koeficient pri rotaci editoru mysi
        /// </summary>
        double _coefMove = 0.4;

        double _INIT_DEGX = -20; double _INIT_DEGY = 210; double _INIT_DEGZ = 170;

        Point _lastMousePoint;

        List<Vektor> _pointsList;
        List<Line3D> _linesList;
        List<Line3D> _grid;

        /// <summary>
        /// vsechny objekty vykreslovane v editoru
        /// </summary>
        List<DrawingObject> _objectsToDraw;

        /// <summary>
        /// stred promitaci roviny v platne
        /// </summary>
        Point _centerPoint;
        /// <summary>
        /// Pero pro osy s kladnymi hodnotami
        /// </summary>
        Pen _penAxis;
        /// <summary>
        /// Pero pro osy se zapornymi hodnotami
        /// </summary>
        Pen _penAxisMinus;
        Pen _penGrid;
        Pen _penObject;
        Pen _penSelectedObject;
        Font _fontAxis;



        Matrix3D _matrix;
        /// <summary>
        /// Matice pocitana od zacatku
        /// </summary>
        Matrix3D _matrixForever;

        /// <summary>
        /// zda se v editoru zobrazi svetla
        /// </summary>
        private bool _showLights;   
        /// <summary>
        /// zda se v editoru zobrazi kamera
        /// </summary>
        private bool _showCamera = true;

        public WndBoard()
        {
            InitializeComponent();

            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            _g = this.pictureBoard.CreateGraphics();
            _penAxis = new Pen(Color.Black, 3.0f);
            _penAxisMinus = new Pen(Color.Black, 1.0f);

            _penGrid = new Pen(Color.DarkCyan, 2.0f);
            _penObject = new Pen(Color.Chocolate, 1.5f);
            _penSelectedObject = new Pen(Color.Chocolate, 2.5f);
            _fontAxis = new Font(Font.FontFamily, 10, FontStyle.Italic);
            _editHelp = new EditHelper();
            

            this.MouseWheel += new MouseEventHandler(onBoard_MouseWheel);
            _grid = new List<Line3D>((int)Math.Pow(_GRID_SIZE_INIT + 1, 3));
            _objectsToDraw = new List<DrawingObject>(30);

            _updateAll = false;

            toolsComboViewAngle.Items.Clear();
            toolsComboViewAngle.Items.AddRange(_editHelp.ComboViewObjects);
            //this.toolsComboViewAngle.SelectedIndex = 0;

            this.Update();
            this.Focus();

            this.Invalidated += new InvalidateEventHandler(WndBoard_Invalidated);

            // nastaveni obsluhy udalosti k menu vybratelnych objektu kliknutim
            this.drawItemFlowLayout1.OnMyEnter += new MenuDrawItemFlowLayout.MenuDrawingItemEventHandler(OnShowItemFromControlMenu);
            this.drawItemFlowLayout1.OnMyClick += new MenuDrawItemFlowLayout.MenuDrawingItemEventHandler(OnClickItemFromControlMenu);

            Reset();
        }

        void WndBoard_Invalidated(object sender, InvalidateEventArgs e)
        {
            this.toolStrip1.Invalidate(true);
            this.toolStrip1.Refresh();
        }

        private void Reset()
        {
            this.Reset(_INIT_DEGX, _INIT_DEGY, _INIT_DEGZ, _ZOOM_INIT, _SCALE_INIT);
            
        }

        public void SetSelectedDrawingObject(DrawingObject drobSelected)
        {
            _Selected = drobSelected;
        }
        /// <summary>
        /// resetuje, ale ponecha hodnoty scale a zoom
        /// </summary>
        /// <param name="degreesX"></param>
        /// <param name="degreesY"></param>
        /// <param name="degreesZ"></param>
        private void Reset(double degreesX, double degreesY, double degreesZ)
        {
            this.Reset(degreesX, degreesY, degreesZ, _zoom, _scale);
        }
        private void Reset(Matrix3D m)
        {
            RotateWholeEditor(m);
        }
        private void Reset(double degreesX, double degreesY, double degreesZ, int zoom, int scale)
        {
            _objectsToDraw.Clear();
            _isDragging = false;
            _scale = scale;
            _zoom = zoom;
            _canRotationChange2 = true;
            

            // resetuje body na hlavnich osach
            //
            _axisC3 = new Vektor(0, 0, 0);
            _axisX3 = new Vektor(2, 0, 0);
            _axisY3 = new Vektor(0, 2, 0);
            _axisZ3 = new Vektor(0, 0, 2);
            _axisXY = new Vektor(_axisX3.X, _axisY3.Y, _axisX3.Z);

            _POV = new Vektor(0, 0, -100);
            //_POV.Scale(50, 50, 50);
            // vyplni potrebne primky
            //
            Line3D l1 = new Line3D(_axisC3, _axisX3);
            Line3D l2 = new Line3D(_axisC3, _axisY3);
            Line3D l3 = new Line3D(_axisC3, _axisZ3);

            _grid = EditHelper.FillGrid(_GRID_SIZE_INIT-1);
            
            _centerPoint = new Point(this.pictureBoard.Width / 2, this.pictureBoard.Height / 2);

            _matrix = new Matrix3D();

            // otocime osy na puvodni stupne
            //
            _matrix.SetOnDegrees(degreesX, degreesY, degreesZ);
            _axisC3 = _matrix.Transform2NewPoint(_axisC3);
            _axisX3 = _matrix.Transform2NewPoint(_axisX3);
            _axisY3 = _matrix.Transform2NewPoint(_axisY3);
            _axisZ3 = _matrix.Transform2NewPoint(_axisZ3);
            _axisXY = _matrix.Transform2NewPoint(_axisXY);
            Matrix3D trp = _matrix.Transpose();
            _POV = trp * _POV;
            _matrixForever = _matrix;

            

            _objectsToDraw.Clear();
            //_objectsToDraw.Add(new DrawingCube(1, 1, -2));
            //_objectsToDraw.Add(new DrawingSphere(new Vektor(1, 2, 1), 1));
            //_objectsToDraw.Add(new DrawingCylinder(new Vektor(1, 2, 1), 1, 3));
            //_objectsToDraw.Add(new DrawingPlane(20, 0.5f, 45, 0, 0, null));
            //_objectsToDraw.Add(new DrawingSphere(new Vektor(2, 1, 0), 2));

            //
            //  ROTACE VSECH OBJEKTU V EDITORU
            //
            _matrix.TransformLines(_grid);
            foreach (DrawingObject obj in _objectsToDraw)
            {
                obj.ApplyRotationMatrix(_matrix);
            }

            this._matrix = Matrix3D.Identity;

            this.toolsComboGridSize.SelectedIndex = _GRID_SIZE_INIT - 2;      // init nastaveni typu mrizky
            this.toolsComboViewAngle.SelectedIndex = 0;
            
            //this.numericUpDown1.Value = (int)degreesX;
            //this.numericUpDown2.Value = (int)degreesY;
            //this.numericUpDown3.Value = (int)degreesZ;

            pictureBoard.Focus();
            Redraw();
        }

        void onBoard_MouseWheel(object sender, MouseEventArgs e)
        {
            this.drawItemFlowLayout1.Visible = false;
            if (!this.pictureBoard.ClientRectangle.Contains(e.Location))
                return;
            if (e.Delta > 0)
                _zoom += _ZOOM_INCREMENT;
            else
            {
                _zoom -= _ZOOM_INCREMENT;
                if (_zoom < 0)
                    _zoom = 0;
            }

        }

        private void onPaintBoard(object sender, PaintEventArgs e)
        {
            this.Redraw(e.Graphics);
        }

        public void Redraw()
        {
            this.Redraw(_g);
        }
        private void Redraw(Graphics g)
        {
            _editHelp.ClearAllClickableObjects();

            _editorBmp = new Bitmap(pictureBoard.Width, pictureBoard.Height);
            g = Graphics.FromImage(_editorBmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;      // ANTIALIASING!!!


            // ================================ DRAWING:
            // =========================================
            // vykresli vsechny primky
            //
            if (toolBtnGrid.Checked)
                DrawGrid(g, _grid);

            // osy:
            if (toolBtnAxes.Checked)
                DrawAxes(g);

            // objekty:
            PointF a, b;
            foreach (DrawingObject obj in _objectsToDraw)
            {
                if (obj.ModelObject is DefaultShape)
                {
                    DefaultShape defSpape = (DefaultShape)obj.ModelObject;

                    if (defSpape.IsActive == false)
                        continue;

                    Color color = defSpape.Material.Color.SystemColor();

                    if (obj == _Selected)
                    {
                        //_penObject = new Pen(color, EditHelper.PenSelectedWidth);
                        _penObject.Color = color;
                        _penObject.Width = EditHelper.PenSelectedWidth; 
                    }
                    else
                    {
                        //_penObject = new Pen(color, EditHelper.PenNormalWidth);
                        _penObject.Color = color;
                        _penObject.Width = EditHelper.PenNormalWidth;
                    }
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////
                if (obj.GetType() == typeof(DrawingSphere)) // ================= SPHERE
                {
                    DrawingSphere sph = (DrawingSphere)obj;
                    EditorObject editorObject = new EditorObject(sph);
                    GraphicsPath path;

                    foreach (Line3D l in sph.Lines)
                    {
                        a = l.A.To2D(_scale, _zoom, _centerPoint);
                        b = l.B.To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(_penObject, a, b);
                    }

                    a = sph.Center.To2D(_scale, _zoom, _centerPoint);
                    float rad = ((float)sph.Radius * _scale) / _scale *_zoom;

                    Vektor[] points = sph.GetDrawingPoints();

                    for (int i = 0; i < points.Length - 2; i++)
                    {
                        PointF pf1 = points[i].To2D(_scale, _zoom, _centerPoint);
                        PointF pf2 = points[i+1].To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(_penObject, pf1, pf2);
                    }

                    path = new GraphicsPath();
                    path.AddEllipse(a.X - rad, a.Y - rad, 2*rad, 2*rad);
                    editorObject.AddPath(path);
                    _editHelp.AddClickableObject(editorObject);                        
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////
                else if (obj.GetType() == typeof(DrawingPlane)) // ================= PLANE
                {
                    DrawingPlane plane = (DrawingPlane)obj;
                    EditorObject editorObject = new EditorObject(plane);
                    GraphicsPath path = new GraphicsPath();

                    foreach (Line3D l in plane.Lines)
                    {
                        a = l.A.To2D(_scale, _zoom, _centerPoint);
                        b = l.B.To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(_penObject, a, b);
                    }

                    Line3D l1 = plane.Lines[0];
                    Line3D l2 = plane.Lines[plane.Lines.Count/2 - 1];
                    PointF[] pfArr = new PointF[4];
                    pfArr[0] = l1.A.To2D(_scale, _zoom, _centerPoint);
                    pfArr[1] = l1.B.To2D(_scale, _zoom, _centerPoint);
                    pfArr[2] = l2.B.To2D(_scale, _zoom, _centerPoint);
                    pfArr[3] = l2.A.To2D(_scale, _zoom, _centerPoint);
                    path.AddPolygon(pfArr);
                    editorObject.AddPath(path);
                    _editHelp.AddClickableObject(editorObject);
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////
                else if (obj.GetType() == typeof(DrawingCube))      //=============== CUBE
                {
                    DrawingCube cube = (DrawingCube)obj;
                    EditorObject editorObject = new EditorObject(cube);
                    GraphicsPath path = new GraphicsPath();

                    foreach (Line3D l in cube.Lines)
                    {
                        a = l.A.To2D(_scale, _zoom, _centerPoint);
                        b = l.B.To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(_penObject, a, b);
                    }

                    List<Vektor[]> quarts = cube.GetQuarts();
                    foreach (Vektor[] arr in quarts)
                    {
                        PointF[] pfArr = new PointF[arr.Length];
                        for (int j = 0; j < arr.Length; j++)
                        {
                            pfArr[j] = arr[j].To2D(_scale, _zoom, _centerPoint);
                        }
                        // pridame ohraniceni cestou
                        path = new GraphicsPath();
                        path.AddPolygon(pfArr);
                        editorObject.AddPath(path);
                    }
                    _editHelp.AddClickableObject(editorObject);
                }
                    /////////////////////////////////////////////////////////////////////////////////////////////////
                else if (obj.GetType() == typeof(DrawingCylinder)) // =============== CYLINDER
                {
                    DrawingCylinder cyl = (DrawingCylinder)obj;
                    EditorObject editorObject = new EditorObject(cyl);
                    GraphicsPath path;

                    foreach (Line3D l in cyl.Lines)
                    {
                        a = l.A.To2D(_scale, _zoom, _centerPoint);
                        b = l.B.To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(_penObject, a, b);
                    }

                    List<Vektor[]> quarts = cyl.GetQuartets();
                    foreach (Vektor[] arr in quarts)
                    {
                        PointF[] pfArr = new PointF[arr.Length];
                        for (int j = 0; j < arr.Length; j++)
                        {
                            pfArr[j] = arr[j].To2D(_scale, _zoom, _centerPoint);
                        }
                        // pridame ohraniceni cestou
                        path = new GraphicsPath();
                        path.AddClosedCurve(pfArr, 0.9F);
                        editorObject.AddPath(path);

                        g.DrawClosedCurve(_penObject, pfArr, 0.9F, System.Drawing.Drawing2D.FillMode.Winding);
                    }

                    Vektor[] bottomPts = quarts[0];
                    Vektor[] upperPts = quarts[quarts.Count - 1];
                    Vektor[] poly1 = new Vektor[4];
                    poly1[0] = bottomPts[0];
                    poly1[1] = bottomPts[2];
                    poly1[2] = upperPts[2];
                    poly1[3] = upperPts[0];
                    PointF[] poly1F = new PointF[poly1.Length];
                    for (int j = 0; j < poly1.Length; j++)
                    {
                        poly1F[j] = poly1[j].To2D(_scale, _zoom, _centerPoint);
                    }

                    Vektor[] poly2 = new Vektor[4];
                    poly2[0] = bottomPts[1];
                    poly2[1] = bottomPts[3];
                    poly2[2] = upperPts[3];
                    poly2[3] = upperPts[1];
                    PointF[] poly2F = new PointF[poly2.Length];
                    for (int j = 0; j < poly2.Length; j++)
                    {
                        poly2F[j] = poly2[j].To2D(_scale, _zoom, _centerPoint);
                    }

                    g.DrawPolygon(_penObject, poly1F);
                    g.DrawPolygon(_penObject, poly2F);
                    path = new GraphicsPath();
                    path.AddPolygon(poly1F);
                    editorObject.AddPath(path);
                    path = new GraphicsPath();
                    path.AddPolygon(poly2F);
                    editorObject.AddPath(path);

                    _editHelp.AddClickableObject(editorObject);
                }

                    /////////////////////////////////////////////////////////////////////////////////////////////////
                else if (obj.GetType() == typeof(DrawingTriangle)) // =============== TRIANGLE
                {
                    DrawingTriangle drTiangl = (DrawingTriangle)obj;
                    EditorObject editorObject = new EditorObject(drTiangl);
                    GraphicsPath path;


                    foreach (Line3D l in drTiangl.Lines)
                    {
                        a = l.A.To2D(_scale, _zoom, _centerPoint);
                        b = l.B.To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(_penObject, a, b);
                    }

                    Vektor[] points3 = drTiangl.GetDrawingPoints();
                    PointF[] pointsF = new PointF[3];
                    for (int i = 0; i < points3.Length; i++)
                    {
                        pointsF[i] = points3[i].To2D(_scale, _zoom, _centerPoint);
                    }
                    g.FillPolygon(drTiangl.FillBrush, pointsF);




                    if (_Selected == obj)
                    {
                        _penObject.Color = Color.Black;

                        Font myfont = EditHelper.FontTriangVert;
                        Rectangle rec = EditHelper.RecTriangVert;
                        Brush brush = EditHelper.BrushTriangVertRect;

                        g.DrawPolygon(Pens.Black, pointsF);
                        rec.Location = new Point((int)pointsF[0].X, (int)pointsF[0].Y);
                        g.FillRectangle(brush, rec);
                        g.DrawString("A", myfont, Brushes.Black, pointsF[0]);
                        rec.Location = new Point((int)pointsF[1].X, (int)pointsF[1].Y);
                        g.FillRectangle(brush, rec);
                        g.DrawString("B", myfont, Brushes.Black, pointsF[1]);
                        rec.Location = new Point((int)pointsF[2].X, (int)pointsF[2].Y);
                        g.FillRectangle(brush, rec);
                        g.DrawString("C", myfont, Brushes.Black, pointsF[2]);
                    }


                    path = new GraphicsPath();
                    path.AddPolygon(pointsF);
                    editorObject.AddPath(path);
                    _editHelp.AddClickableObject(editorObject);
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////
                else if (obj.GetType() == typeof(DrawingCustom)) // =============== C U S T O M     O B J E C T
                {
                    DrawingCustom drCust = (DrawingCustom)obj;
                    EditorObject editorObjectCustom =  new EditorObject(drCust);
                    EditorObject editorObjectTrianglFace;
                    GraphicsPath path;

                    Vektor[] points3;
                    PointF[] pointsF;

                    if (_Selected == obj)
                    {
                        _penObject.Color = Color.Black;
                    }
                    foreach (Line3D l in drCust.Lines)
                    {
                        a = l.A.To2D(_scale, _zoom, _centerPoint);
                        b = l.B.To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(_penObject, a, b);
                    }

                    foreach (DrawingTriangle drTriang in drCust.DrawingFacesList)
                    {
                        points3 = drTriang.GetDrawingPoints();
                        pointsF = new PointF[3];
                        for (int i = 0; i < points3.Length; i++)
                        {
                            pointsF[i] = points3[i].To2D(_scale, _zoom, _centerPoint);
                        }
                        //g.FillPolygon(drTriang.FillBrush, pointsF);
                        Font myfont = EditHelper.FontTriangVert;
                        Rectangle rec = EditHelper.RecTriangVert;
                        Brush brush = EditHelper.BrushTriangVertRect;

                        if (drCust.ShowFilled)
                        {
                            g.FillPolygon(drTriang.FillBrush, pointsF);
                        }
                        else if (drTriang == _Selected)
                        {
                            g.FillPolygon(drTriang.FillBrush, pointsF);
                            g.DrawPolygon(Pens.Black, pointsF);

                            //rec = new Rectangle(new Point((int)pointsF[0].X, (int)pointsF[0].Y), new Size(10, 10));
                            rec.Location = new Point((int)pointsF[0].X, (int)pointsF[0].Y);
                            g.FillRectangle(brush, rec);
                            g.DrawString("A", myfont, Brushes.Black, pointsF[0]);
                            rec.Location = new Point((int)pointsF[1].X, (int)pointsF[1].Y);
                            g.FillRectangle(brush, rec);
                            g.DrawString("B", myfont, Brushes.Black, pointsF[1]);
                            rec.Location = new Point((int)pointsF[2].X, (int)pointsF[2].Y);
                            g.FillRectangle(brush, rec);
                            g.DrawString("C", myfont, Brushes.Black, pointsF[2]);
                        }
                        path = new GraphicsPath();
                        path.AddPolygon(pointsF);
                        editorObjectTrianglFace = new EditorObject(drTriang);
                        editorObjectTrianglFace.AddPath(path);
                        _editHelp.AddClickableObject(editorObjectTrianglFace);

                        editorObjectCustom.AddPath(path);

                        
                    }
                    // zobrazeni vybraneho trojuhelniku nade vsemi ostatnimi - proto az na konec se vykresli
                    if (_Selected is DrawingFacet)
                    {
                        foreach (DrawingTriangle drTriang in drCust.DrawingFacesList)
                        {
                            if (drTriang == _Selected)
                            {
                                points3 = drTriang.GetDrawingPoints();
                                pointsF = new PointF[3];
                                for (int i = 0; i < points3.Length; i++)
                                {
                                    pointsF[i] = points3[i].To2D(_scale, _zoom, _centerPoint);
                                }
                                //g.FillPolygon(drTriang.FillBrush, pointsF);
                                Font myfont = EditHelper.FontTriangVert;
                                Rectangle rec = EditHelper.RecTriangVert;
                                Brush brush = EditHelper.BrushTriangVertRect;

                                g.FillPolygon(drTriang.FillBrush, pointsF);
                                g.DrawPolygon(Pens.Black, pointsF);

                                //rec = new Rectangle(new Point((int)pointsF[0].X, (int)pointsF[0].Y), new Size(10, 10));
                                rec.Location = new Point((int)pointsF[0].X, (int)pointsF[0].Y);
                                g.FillRectangle(brush, rec);
                                g.DrawString("A", myfont, Brushes.Black, pointsF[0]);
                                rec.Location = new Point((int)pointsF[1].X, (int)pointsF[1].Y);
                                g.FillRectangle(brush, rec);
                                g.DrawString("B", myfont, Brushes.Black, pointsF[1]);
                                rec.Location = new Point((int)pointsF[2].X, (int)pointsF[2].Y);
                                g.FillRectangle(brush, rec);
                                g.DrawString("C", myfont, Brushes.Black, pointsF[2]);
                            }
                        }
                    }
                    //path = new GraphicsPath();
                    //path.AddPolygon(pointsF);
                    //editorObject.AddPath(path);
                    //_editHelp.AddClickableObject(editorObject);
                    _editHelp.AddClickableObject(editorObjectCustom);
                }
                    /////////////////////////////////////////////////////////////////////////////////////////////////
                else if (obj.GetType() == typeof(DrawingCone)) // =============== CONE
                {
                    DrawingCone drCone = (DrawingCone)obj;
                    EditorObject editorObject = new EditorObject(drCone);
                    GraphicsPath path;

                    Vektor[] basePoints = drCone.GetBasePoints();
                    PointF[] pointsF = new PointF[basePoints.Length];
                    for (int i = 0; i < pointsF.Length; i++)
                    {
                        pointsF[i] = basePoints[i].To2D(_scale, _zoom, _centerPoint);
                    }
                    g.DrawClosedCurve(_penObject, pointsF);
                    path = new GraphicsPath();
                    path.AddClosedCurve(pointsF);
                    editorObject.AddPath(path);

                    PointF start,end;
                    PointF previous = drCone.Lines[0].A.To2D(_scale, _zoom, _centerPoint);
                    foreach (Line3D line in drCone.Lines)
                    {
                        start = line.A.To2D(_scale, _zoom, _centerPoint);
                        end = line.B.To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(_penObject, start, end);

                        path = new GraphicsPath();
                        path.AddPolygon(new PointF[] { start, end, previous });
                        editorObject.AddPath(path);

                        previous = start;
                    }
                    _editHelp.AddClickableObject(editorObject);
                }

                /////////////////////////////////////////////////////////////////////////////////////////////////
                else if (obj.GetType() == typeof(DrawingLight)) // ================ LIGHT
                {
                    if (_showLights == false)           // neni-li zaskrtnuta volba zobrazeni svetel, konec
                        continue;

                    DrawingLight drLight = (DrawingLight)obj;
                    Light l = (Light)drLight.ModelObject;
                    if (l.IsActive == false)
                        continue;
                    EditorObject editorObject = new EditorObject(drLight);
                    GraphicsPath path = new GraphicsPath();

                    a = drLight.Center.To2D(_scale, _zoom, _centerPoint);
                    float upperX = a.X - (float)(Properties.Resources.bulb_transp.Width / 2);
                    float upperY = a.Y - (float)(Properties.Resources.bulb_transp.Height / 2);
                    g.DrawImage(Properties.Resources.bulb_transp, new PointF(upperX, upperY));
                    path.AddRectangle(new RectangleF(upperX, upperY, Properties.Resources.bulb_transp.Width, Properties.Resources.bulb_transp.Height));

                    editorObject.AddPath(path);
                    _editHelp.AddClickableObject(editorObject);
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////
                else if (obj.GetType() == typeof(DrawingCamera))    // ============ CAMERA
                {
                    if (_showCamera == false)       // neni-li zaskrtnuta volba zobrazeni kamery, konec
                        continue;

                    DrawingCamera drCam = (DrawingCamera)obj;
                    Camera cam = (Camera)drCam.ModelObject;
                    EditorObject editorObject = new EditorObject(drCam);
                    GraphicsPath path = new GraphicsPath();

                    a = drCam.Center.To2D(_scale, _zoom, _centerPoint);
                    float upperX = a.X - (float)(Properties.Resources.camera.Width / 2);
                    float upperY = a.Y - (float)(Properties.Resources.camera.Height / 2);
                    g.DrawImage(Properties.Resources.camera, new PointF(upperX, upperY));
                    path.AddRectangle(new RectangleF(upperX, upperY, Properties.Resources.camera.Width, Properties.Resources.camera.Height));

                    editorObject.AddPath(path);
                    _editHelp.AddClickableObject(editorObject);


                    Line3D line;

                    // podstava
                    PointF[] pointsF = new PointF[4];
                    for (int i = 2; i < 6; i++)
                    {
                        pointsF[i - 2] = drCam.Points[i].To2D(_scale, _zoom, _centerPoint);
                    }
                    g.DrawClosedCurve(_editHelp.CameraPenEllips, pointsF, 1F, System.Drawing.Drawing2D.FillMode.Winding);

                    // cara ze stredu kamery do stredu elipsy -- osa kuzele
                    // tato cara ma i sipku ukazujici smer kamery
                    line = drCam.Lines[0];
                    a = line.A.To2D(_scale, _zoom, _centerPoint);
                    b = line.B.To2D(_scale, _zoom, _centerPoint);
                    g.DrawLine(_editHelp.CameraPenMain, a, b);

                    // cary podstavy elipsy
                    // tenci cary bez sipky
                    if (drCam.ShowCross)
                    {
                        // prvni line
                        line = drCam.Lines[1];
                        a = line.A.To2D(_scale, _zoom, _centerPoint);
                        b = line.B.To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(_editHelp.CameraPenUp, a, b);

                        // druha line
                        line = drCam.Lines[2];
                        a = line.A.To2D(_scale, _zoom, _centerPoint);
                        b = line.B.To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(_editHelp.CameraPenLight, a, b);
                    }
                    /// cary steny:
                    /// 
                    // prvni par:
                    if (drCam.ShowSide1)
                    {
                        line = drCam.Lines[3];
                        a = line.A.To2D(_scale, _zoom, _centerPoint);
                        b = line.B.To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(_editHelp.CameraPenLight, a, b);

                        line = drCam.Lines[5];
                        a = line.A.To2D(_scale, _zoom, _centerPoint);
                        b = line.B.To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(_editHelp.CameraPenLight, a, b);

                    }

                    // druhy par
                    if (drCam.ShowSide2)
                    {
                        line = drCam.Lines[4];
                        a = line.A.To2D(_scale, _zoom, _centerPoint);
                        b = line.B.To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(_editHelp.CameraPenLight, a, b);

                        line = drCam.Lines[6];
                        a = line.A.To2D(_scale, _zoom, _centerPoint);
                        b = line.B.To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(_editHelp.CameraPenLight, a, b);
                    }




                }
                /////////////////////////////////////////////////////////////////////////////////////////////////
                else if (obj.GetType() == typeof(DrawingAnimation))     // ========== ANIMATION
                {
                    DrawingAnimation drAnim = (DrawingAnimation)obj;
                    if (drAnim.ShowAnimation == false)       // neni-li volba zobrazeni animace, nezobrazujeme
                        continue;

                    EditorObject editorObject = new EditorObject(drAnim);
                    GraphicsPath path;

                    Line3D lineA = drAnim.AxisA;
                    a = lineA.A.To2D(_scale, _zoom, _centerPoint);
                    b = lineA.B.To2D(_scale, _zoom, _centerPoint);
                    g.DrawLine(DrawingAnimation.AxisAPen, a, b);
                    Line3D lineB = drAnim.AxisB;
                    a = lineB.A.To2D(_scale, _zoom, _centerPoint);
                    b = lineB.B.To2D(_scale, _zoom, _centerPoint);
                    g.DrawLine(DrawingAnimation.AxisBPen, a, b);
                    //foreach (Line3D l in drAnim.Lines)
                    //{
                    //    a = l.A.To2D(_scale, _zoom, _centerPoint);
                    //    b = l.B.To2D(_scale, _zoom, _centerPoint);
                    //    g.DrawLine(_penObject, a, b);
                    //}

                    a = drAnim.Center.To2D(_scale, _zoom, _centerPoint);
                    // nakreslit stred elipsy
                    //g.DrawLine(_penObject, a, PointF.Subtract(a, new Size(2, 2)));
                    //float rad = ((float)sph.Radius * _scale) / _scale * _zoom;

                    Vektor[] points = drAnim.GetDrawingPoints();
                    path = new GraphicsPath();

                    // vybrana animace ma jine PERO
                    Pen penElips = DrawingAnimation.EllipsePen;
                    if (drAnim == _Selected)
                        penElips = DrawingAnimation.EllipseSelectedPen;

                    for (int i = 0; i < points.Length - 1; i++)
                    {
                        PointF pf1 = points[i].To2D(_scale, _zoom, _centerPoint);
                        PointF pf2 = points[i + 1].To2D(_scale, _zoom, _centerPoint);
                        g.DrawLine(penElips, pf1, pf2);
                        path.AddLine(pf1, pf2);
                    }



                    //path.AddEllipse(a.X - rad, a.Y - rad, 2 * rad, 2 * rad);
                    editorObject.AddPath(path);
                    _editHelp.AddClickableObject(editorObject);

                }

            }

            pictureBoard.Image = _editorBmp;

            // ///////////////////////////////////////////////////////////////
            // STATUS BAR
            // ///////////////////////////////////////////////////////////////

            // ZOOM LABEL
            this.statusLabelZoom.Text = _zoom.ToString();

            // ANGLES
            double[] angles = _matrixForever.GetAnglesFromMatrix();
            for (int i = 0; i < angles.Length; i++)
            {
                if (angles[i] > 359)
                    angles[i] = angles[i] % 360;
                else if (angles[i] < 0)
                    angles[i] = 360 + angles[i];
            }

            double degsX = Math.Round(angles[0], 1);
            double degsY = Math.Round(angles[1], 1);
            double degsZ = Math.Round(angles[2], 1);

            this.statusLabelX.Text = degsX.ToString() + "°";
            this.statusLabelY.Text = degsY.ToString() + "°";
            this.statusLabelZ.Text = degsZ.ToString() + "°";

            if (_canRotationChange2)
            {
                _canRotationChange = false;
                this.numericUpDown1.Value = (decimal)degsX;
                this.numericUpDown2.Value = (decimal)degsY;
                this.numericUpDown3.Value = (decimal)degsZ;
                _canRotationChange = true;
            }

            //////////////////////////////////////////////////////////////
            // UPDATING REST OF EDITOR
            // ///////////////////////////////////////////////////////////
            
            // aktualizujeme seznam objektu
            if (_updateAll)
            {
                WndScene sc = GetWndScene();
                sc.UpdateRecords();
                _updateAll = false;
            }


            //DrawShit();
        }

        private void DrawAxes(Graphics g)
        {

            PointF c = _axisC3.To2D(_scale, _zoom, _centerPoint);
            PointF x = _axisX3.To2D(_scale, _zoom, _centerPoint);
            PointF y = _axisY3.To2D(_scale, _zoom, _centerPoint);
            PointF z = _axisZ3.To2D(_scale, _zoom, _centerPoint);

            PointF xm = (_axisC3 - _axisX3).To2D(_scale, _zoom, _centerPoint);
            PointF ym = (_axisC3 - _axisY3).To2D(_scale, _zoom, _centerPoint);
            PointF zm = (_axisC3 - _axisZ3).To2D(_scale, _zoom, _centerPoint);

            //
            // vykresli osy
            //
            g.DrawLine(_penAxis, c, x);
            g.DrawLine(_penAxis, c, y);
            g.DrawLine(_penAxis, c, z);

            g.DrawLine(_penAxisMinus, c, xm);
            g.DrawLine(_penAxisMinus, c, ym);
            g.DrawLine(_penAxisMinus, c, zm);

            g.DrawString("X", _fontAxis, Brushes.Black, x);
            g.DrawString("Y", _fontAxis, Brushes.Black, y);
            g.DrawString("Z", _fontAxis, Brushes.Black, z);


            PointF xy = _axisXY.To2D(_scale, _zoom, _centerPoint);
            //g.FillPolygon(Brushes.Tomato, new PointF[] { c, x, xy, y, c}, FillMode.Alternate);
        }

        private void DrawGrid(Graphics g, List<Line3D> lines)
        {
            PointF a, b;
            foreach (Line3D l in lines)
            {
                a = l.A.To2D(_scale, _zoom, _centerPoint);
                b = l.B.To2D(_scale, _zoom, _centerPoint);
                g.DrawLine(_penGrid, a, b);
            }
        }

        private void onPicMouseUp(object sender, MouseEventArgs e)
        {
            int diff = e.X - _lastMousePoint.X + e.Y - _lastMousePoint.Y;
            if (diff == 0 && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                List<DrawingObject> drawingList = _editHelp.GetClickableObj(e.Location);
                // je-li vybranych kliknutim vice objektu, zobrazime nabidku
                if (drawingList.Count > 1)
                {
                    this.drawItemFlowLayout1.AddItems(drawingList.ToArray());
                    this.drawItemFlowLayout1.Location = new Point(e.X, e.Y);
                    this.drawItemFlowLayout1.Visible = true;
                }
                else
                {
                    this.drawItemFlowLayout1.Visible = false;
                    if (drawingList.Count == 0)
                    {
                        WndScene wndSc = GetWndScene();
                        RayImage img = wndSc.GetSelectedImage();
                        wndSc.ShowNode(img);
                        pictureBoard.Focus();
                        
                    } 
                }
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                this.drawItemFlowLayout1.Visible = false;
            this._isDragging = false;
            _isTransforming = false;
            this._matrix = Matrix3D.Identity;
        }

        /// <summary>
        /// KLIKNUTI MYSI A VYBRANI OBJEKTU V EDITORU
        /// </summary>
        private void onPicMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && _Selected == null)
                _Selected = null;
            List<DrawingObject> drawingList = _editHelp.GetClickableObj(e.Location);
            if (drawingList.Count > 0)
            {
                bool wasSelectedBefore = false;
                foreach (DrawingObject drObj in drawingList)
                {
                    if (_Selected == drObj)
                    {
                        WndScene wndsc = GetWndScene();
                        wndsc.ShowNode(drObj);
                        wasSelectedBefore = true;
                        break;
                    }
                }
                if (!wasSelectedBefore && drawingList[0] is DrawingObject)
                {
                    WndScene wndsc = GetWndScene();
                    _Selected = drawingList[drawingList.Count - 1];   // vybereme posledni ze seznamu - je nejbliz pozorovateli
                    wndsc.ShowNode(_Selected);
                }
                labelClick.Text = "Mouse Down";
            }
            else
            {
                _Selected = null;   // otazka, zda po kliknuti do prazdneho prostoru, zobrazit vlastnosti
                labelClick.Text = "---";
            }
            pictureBoard.Focus();
            if (e.Button == MouseButtons.Left)
                this._isDragging = true;
            else if (e.Button == MouseButtons.Right && _Selected == null)
                this._isDragging = true;
            else if (e.Button == MouseButtons.Right && _Selected != null)
                this._isTransforming = true;

            drawItemFlowLayout1.Visible = false;
            this._lastMousePoint = e.Location;
        }

        private void onPicMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging && !_isTransforming)
                return;

            PictureBox pb = sender as PictureBox;
            if (!pb.ClientRectangle.Contains(e.Location))
                return;

            Point currPoint = e.Location;

            if (IsCloserThanPoint(_lastMousePoint, currPoint, _MOUSE_SENSITIVITY))
                return;

            if (e.Button == MouseButtons.Left)
            {
                RotateWholeEditor(currPoint);
            }
            else if (e.Button == MouseButtons.Right)
            {

                if (_isTransforming && _Selected!= null)
                {
                    // pretransformovat z 2D do 3D
                    // potrebujeme ale Z souradnici modelovaneho objektu, ktery posouvame
                    //double z = _Selected.ModelObject.POINTS.Z
                    int z = _zoom;
                    int s = _scale;
                    double xDel = ((double)(_lastMousePoint.X - currPoint.X)) / _zoom;
                    double yDel = ((double)(_lastMousePoint.Y - currPoint.Y)) / _zoom;
                    //foreach (Vektor p in _Selected.Points)
                    //{
                    //    p.Posunuti(-xDel, -yDel, 0);
                    //}

                    Matrix3D shift2DMatrix = Matrix3D.PosunutiNewMatrix(-xDel, -yDel, 0); // matice posunuti vzhledem k mysi - 2D
                    if (_Selected.ModelObject is DefaultShape)
                    {
                        DefaultShape ds = _Selected.ModelObject as DefaultShape;

                        //shift2DMatrix.TransformPoints(_Selected.Points);
                            //foreach (Vektor p in _Selected.Points)
                            //{
                            //    p.Posunuti(-xDel, -yDel, 0);    // potreba nahradit zpusobem s matici a nasobeni rotacni matice
                            //}
                        if (ds is Sphere)
                        {
                            DrawingSphere drawSphere = _Selected as DrawingSphere;
                            Matrix3D transp = this._matrixForever.Transpose();
                            // zjistime souradnice bodu v editoru - kam se v editoru dostane 2D posunutim hlavni posouvaci bod - stred
                            Vektor center = shift2DMatrix.Transform2NewPoint(drawSphere.Center);
                            // zjistime svetove souradnice bodu z bodu editoru
                            Vektor centerTransp = transp.Transform2NewPoint(center);
                            // presuneme vsechny body editoru do puvodniho tvaru - svetove souradnice
                            transp.TransformPoints(drawSphere.Points);
                            // posuneme body podle noveho bodu posunuti
                            drawSphere.Move(centerTransp.X, centerTransp.Y, centerTransp.Z);
                            // transformujeme body do editoru
                            _matrixForever.TransformPoints(drawSphere.Points);
                        }
                        else if (ds is Cube)
                        {
                            DrawingCube drCube = _Selected as DrawingCube;
                            Matrix3D transp = this._matrixForever.Transpose();
                            Vektor center = shift2DMatrix.Transform2NewPoint(drCube.Center);
                            Vektor centerTransp = transp.Transform2NewPoint(center);
                            transp.TransformPoints(drCube.Points);
                            drCube.Move(centerTransp.X, centerTransp.Y, centerTransp.Z);
                            _matrixForever.TransformPoints(drCube.Points);
                        }
                        else if (ds is Cylinder)
                        {
                            DrawingCylinder drCyl = _Selected as DrawingCylinder;
                            Matrix3D transp = this._matrixForever.Transpose();
                            Vektor center = shift2DMatrix.Transform2NewPoint(drCyl.Center);
                            Vektor centerTransp = transp.Transform2NewPoint(center);
                            transp.TransformPoints(drCyl.Points);
                            drCyl.Move(centerTransp.X, centerTransp.Y, centerTransp.Z);
                            _matrixForever.TransformPoints(drCyl.Points);
                        }
                        else if (ds is Cone)
                        {
                            DrawingCone drCone = _Selected as DrawingCone;
                            Matrix3D transp = this._matrixForever.Transpose();
                            Vektor center = shift2DMatrix.Transform2NewPoint(drCone.Center);
                            Vektor centerTransp = transp.Transform2NewPoint(center);
                            transp.TransformPoints(drCone.Points);
                            drCone.Move(centerTransp.X, centerTransp.Y, centerTransp.Z);
                            _matrixForever.TransformPoints(drCone.Points);
                        }
                        else if (ds is Triangle)
                        {
                            DrawingTriangle drTriangl = _Selected as DrawingTriangle;
                            Matrix3D transp = this._matrixForever.Transpose();
                            Vektor center = shift2DMatrix.Transform2NewPoint(drTriangl.Center);
                            Vektor centerTransp = transp.Transform2NewPoint(center);
                            transp.TransformPoints(drTriangl.Points);
                            Vektor diff = drTriangl.Center - centerTransp;
                            drTriangl.Move(diff.X, diff.Y, diff.Z);
                            _matrixForever.TransformPoints(drTriangl.Points);
                            //drTriangl.Move(centerTransp.X, centerTransp.Y, centerTransp.Z);
                            //drTriangl.Move(-diff.X, -diff.Y, -diff.Z);
                            //Matrix3D shift3D = Matrix3D.PosunutiNewMatrix(-diff.X, -diff.Y, -diff.Z);
                            //shift3D.TransformPoints(drTriangl.Points);

                            //for (int i = 0; i < pointsOld.Length; i++)
                            //{
                            //    Vektor center = shift2DMatrix.Transform2NewPoint(pointsOld[i]);
                            //    Vektor centerTransp = transp.Transform2NewPoint(center);
                            //    pointsNew[i] = centerTransp;
                            //}
                            //if (pointsNew.Length == 3)
                            //    triangl.Set(new Vektor(pointsNew[0].X, pointsNew[0].Y, pointsNew[0].Z),
                            //        new Vektor(pointsNew[1].X, pointsNew[1].Y, pointsNew[1].Z),
                            //        new Vektor(pointsNew[2].X, pointsNew[2].Y, pointsNew[2].Z));
                            //drTriangl.SetModelObject(triangl);
                        }
                        else if (ds is CustomObject)
                        {
                            DrawingCustom drCust = _Selected as DrawingCustom;
                            Matrix3D transp = this._matrixForever.Transpose();
                            Vektor center = shift2DMatrix.Transform2NewPoint(drCust.Center);
                            Vektor centerTransp = transp.Transform2NewPoint(center);
                            transp.TransformPoints(drCust.Points);
                            Vektor diff = drCust.Center - centerTransp;
                            drCust.Move(diff.X, diff.Y, diff.Z);
                            //drCust.MoveDiff2(centerTransp.X, centerTransp.Y, centerTransp.Z);
                            //drCust.Move(centerTransp.X, centerTransp.Y, centerTransp.Z);
                            _matrixForever.TransformPoints(drCust.Points);
                            //foreach (DrawingTriangle drTr in drCust.DrawingFacesList)
                            //{
                            //    center = shift2DMatrix.Transform2NewPoint(drTr.Center);
                            //    centerTransp = transp.Transform2NewPoint(center);
                            //    transp.TransformPoints(drTr.Points);
                            //    diff = drTr.Center - centerTransp;
                            //    drTr.Move(diff.X, diff.Y, diff.Z);
                            //    _matrixForever.TransformPoints(drTr.Points);
                            //}
                        }
                        // aktualizace seznamu objektu ve scene
                        WndScene wnd = GetWndScene();
                        wnd.UpdateRecords();
                    }
                    else if (_Selected.ModelObject is Camera)
                    {
                        DrawingCamera drCam = _Selected as DrawingCamera;
                        Camera cam = _Selected.ModelObject as Camera;
                        shift2DMatrix.TransformPoints(_Selected.Points);
                        //foreach (Vektor p in _Selected.Points)
                        //{
                        //    p.Posunuti(-xDel, -yDel, 0);
                        //}
                        Matrix3D transp = this._matrixForever.Transpose();
                        Vektor centerTransp = transp.Transform2NewPoint(drCam.Center);
                        cam.MoveToPoint(centerTransp.X, centerTransp.Y, centerTransp.Z);

                        WndScene wnd = GetWndScene();
                        wnd.UpdateRecords();
                    }

                    else if (_Selected.ModelObject is Light)
                    {
                        DrawingLight drLight = _Selected as DrawingLight;
                        Light light = _Selected.ModelObject as Light;
                        shift2DMatrix.TransformPoints(_Selected.Points);
                        //foreach (Vektor p in _Selected.Points)
                        //{
                        //    p.Posunuti(-xDel, -yDel, 0);
                        //}
                        Matrix3D transp = this._matrixForever.Transpose();
                        Vektor centerTransp = transp.Transform2NewPoint(drLight.Center);
                        light.MoveToPoint(centerTransp.X, centerTransp.Y, centerTransp.Z);

                        WndScene wnd = GetWndScene();
                        wnd.UpdateRecords();

                    }
                    else if (_Selected is DrawingAnimation)
                    {
                        DrawingAnimation drAnim = _Selected as DrawingAnimation;
                        shift2DMatrix.TransformPoints(_Selected.Points);
                        //foreach (Vektor p in _Selected.Points)
                        //{
                        //    p.Posunuti(-xDel, -yDel, 0);
                        //}
                        Matrix3D transp = this._matrixForever.Transpose();
                        Vektor centerTransp = transp.Transform2NewPoint(drAnim.Center);
                        drAnim.CenterWorld = centerTransp;
                        WndScene wnd = GetWndScene();
                        wnd.UpdateRecords();
                    }
                    
                }

                if (_isDragging && !_isTransforming)
                {
                    int xDel = (_lastMousePoint.X - currPoint.X);
                    int yDel = (_lastMousePoint.Y - currPoint.Y);
                    _centerPoint.X -= xDel;
                    _centerPoint.Y -= yDel;
                }
            }


            this._lastMousePoint = currPoint;
            this._matrix = Matrix3D.Identity;
            this.Redraw();
        }



        /// <summary>
        /// Rotuje celym editorem
        /// Neposouva zadny realny objekt ve svete sceny
        /// Pouze zobrazuje libovolne nahledy
        /// </summary>
        /// <param name="currePoint">bod, kde byla naposled mys se stisknutym tlacitkem</param>
        private void RotateWholeEditor(Point currePoint)
        {
            double degreesX = 0;
            double degreesY = 0;
            double degreesZ = 0;

            degreesX = (_lastMousePoint.X - currePoint.X) * _coefMove;
            degreesY = (_lastMousePoint.Y - currePoint.Y) * _coefMove;

            if (degreesX == 0.0 && degreesY == 0.0)
            {
                this._isDragging = false;
                return;
            }

            double radiansX = MyMath.Degrees2Rad(degreesX);
            double radiansY = MyMath.Degrees2Rad(degreesY);
            double radiansZ = MyMath.Degrees2Rad(degreesZ);


            Matrix3D matr = Matrix3D.NewRotateByRads(-radiansY, radiansX, radiansZ);
            _matrix = _matrix * matr;
            _matrixForever = _matrixForever * _matrix;

            foreach (DrawingObject obj in _objectsToDraw)
            {
                obj.ApplyRotationMatrix(_matrix);
            }
            if (toolBtnGrid.Checked)
                _matrix.TransformLines(_grid);

            Vektor newX3d = _matrix * _axisX3;
            _axisX3 = newX3d;
            Vektor newY3d = _matrix * _axisY3;
            _axisY3 = newY3d;
            Vektor newZ3d = _matrix * _axisZ3;
            _axisZ3 = newZ3d;
            Vektor newXY = _matrix * _axisXY;
            _axisXY = newXY;

            DrawObjectComparer dcomp = new DrawObjectComparer(new Vektor(0, 0, -100), _matrixForever, DrawObjectComparer.SortType.DESC);
            _objectsToDraw.Sort(0, _objectsToDraw.Count, dcomp);

            this._lastMousePoint = currePoint;

            this._matrix = Matrix3D.Identity;
            _canRotationChange2 = true;
        }

        
        /// <summary>
        /// provnava objekty podle vzdalenosti od bodu _POV
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        //private static int CompareObjectsToDraw(DrawingObject x, DrawingObject y)
        //{

        //    foreach (DrawingObject drob in drawingList)
        //    {
        //        Vektor vec = drob.GetCenter();
        //        transp.TransformPoint(vec);
        //        vec = _POV - vec;
        //        double len = vec.Size();
        //        if (len < minlen)
        //        {
        //            minlen = len;
        //            closestObj = drob;
        //        }
        //}
        /// <summary>
        /// nastavi rotaci editoru podle zadane matice
        /// nevyuziva predchozi nastaveni. 
        /// Nastaveni celeho editoru na presne uhly pomoci zadane matice
        /// </summary>
        /// <param name="rotationMatrix">rotacni matice definujici koncovou podobu editoru</param>
        private void RotateWholeEditor(Matrix3D rotationMatrix)
        {
            // musime vyuzit inverzni matici k soucasne globalni rotacni matici
            // inverzni matice je matice transponovana
            Matrix3D transp = this._matrixForever.Transpose();
            foreach (DrawingObject obj in _objectsToDraw)
            {
                obj.ApplyRotationMatrix(transp);
                obj.ApplyRotationMatrix(rotationMatrix);
            }
            if (toolBtnGrid.Checked)
            {
                transp.TransformLines(_grid);
                rotationMatrix.TransformLines(_grid);
            }

            Vektor newX3d = rotationMatrix * (transp * _axisX3);
            _axisX3 = newX3d;
            Vektor newY3d = rotationMatrix * (transp * _axisY3);
            _axisY3 = newY3d;
            Vektor newZ3d = rotationMatrix * (transp * _axisZ3);
            _axisZ3 = newZ3d;
            Vektor newXY = rotationMatrix * (transp * _axisXY);
            _axisXY = newXY;

            this._matrixForever = rotationMatrix;

            DrawObjectComparer dcomp = new DrawObjectComparer(new Vektor(0, 0, -100), _matrixForever, DrawObjectComparer.SortType.DESC);
            _objectsToDraw.Sort(0, _objectsToDraw.Count, dcomp);

        }
        

        /// <summary>
        /// ze seznamu vrati nejblizzsi objektu
        /// </summary>
        /// <param name="drawingList"></param>
        /// <returns></returns>
        private DrawingObject GetClosestDrawingObj(List<DrawingObject> drawingList)
        {
            double minlen = Double.MaxValue;
            DrawingObject closestObj = null;
            
            Vektor pov = new Vektor(0, 0, -100);
            Matrix3D transp = _matrixForever.Transpose();
            transp.TransformPoint(pov);
            foreach (DrawingObject drob in drawingList)
            {
                Vektor vec = drob.GetCenter();
                transp.TransformPoint(vec);
                vec = pov - vec;
                double len = vec.Size();
                if (len < minlen)
                {
                    minlen = len;
                    closestObj = drob;
                }
            }
            return closestObj;
        }

        /// <summary>
        /// ulozi do souboru obrazek pozadi okna properties
        /// </summary>
        private void DrawShit()
        {
            int w = 800;
            int h = 400;

            Bitmap bmp = new Bitmap(w,h);
            Graphics gr = Graphics.FromImage(bmp);
            gr.Clear(Color.White);

            int inc = 22;
            for (int i = 0; i < h; i += 2*inc)
            {
                gr.FillRectangle(Brushes.Gainsboro, new Rectangle(0, i, w, h));
                gr.FillRectangle(Brushes.WhiteSmoke, new Rectangle(0, i + inc, w, h));
            }
            bmp.Save("C:\\back.png", System.Drawing.Imaging.ImageFormat.Png);
        }



        private void toolBtnReset_Click(object sender, EventArgs e)
        {
            //this.Reset();
            //Redraw();
        }

        private void toolBtnTop_Click(object sender, EventArgs e)
        {
            Matrix3D m = Matrix3D.NewRotateByDegrees(270, 0, 0);
            RotateWholeEditor(m);
        }

        private void toolBtnSide_Click(object sender, EventArgs e)
        {
            Matrix3D m = Matrix3D.NewRotateByDegrees(340, 250, 160);
            RotateWholeEditor(m);
        }

        private void OnChangedComboGrid(object sender, EventArgs e)
        {
            ToolStripComboBox c = sender as ToolStripComboBox;
            int d = Int32.Parse(c.SelectedItem.ToString());
            _grid = EditHelper.FillGrid(d);
            _matrixForever.TransformLines(_grid);
            this.pictureBoard.Focus();
            //Redraw();
        }

        private void OnChangedComboAngleView(object sender, EventArgs e)
        {
            if (_currentScene == null) return;
            ToolStripComboBox c = sender as ToolStripComboBox;
            EditHelper.ComboViewAngle obj = (EditHelper.ComboViewAngle)c.SelectedItem;
            Matrix3D m = Matrix3D.Identity;

            if (obj.Caption == EditHelper.CAMERAVIEW_string)
            {
                // VPRED
                Vektor dirNorm = new Vektor(_currentScene.Camera.Norm);
                dirNorm.Normalize();
                Vektor z = new Vektor(0, 0, 1);
                Quaternion q = new Quaternion(dirNorm, z);
                //double[] degss1 = q.ToEulerDegs();
                //Matrix3D m1 = Matrix3D.NewRotateByDegrees(-degss1[0], -degss1[1], -degss1[2]);
                Matrix3D m1 = q.Matrix();

                Matrix3D m1Transp = new Matrix3D(m1);
                m1Transp.Transpose();

                // NAHORU
                Vektor y = new Vektor(0, -1, 0);

                Vektor up = new Vektor(_currentScene.Camera.Up);
                up.Normalize();
                Matrix3D m1transp = new Matrix3D(m1);
                m1transp.Transpose();
                Vektor up2 = m1transp.Transform2NewPoint(up);
                up2.Normalize();

                q = new Quaternion(up2, y);
                //double[] degss2 = q.ToEulerDegs();
                //Matrix3D m2 = Matrix3D.NewRotateByDegrees(-degss2[0], -degss2[1], -degss2[2]);
                Matrix3D m2 = q.Matrix();
                m = m1 * m2;
            }
            else if (obj.Caption == EditHelper.CAMERAVIEW2_string)
            {
                // VPRED
                Vektor dirNorm = new Vektor(_currentScene.Camera.Norm);
                dirNorm.Normalize();
                Vektor z = new Vektor(0, -1, 0);
                Quaternion q = new Quaternion(dirNorm, z);
                //double[] degss1 = q.ToEulerDegs();
                //Matrix3D m1 = Matrix3D.NewRotateByDegrees(-degss1[0], -degss1[1], -degss1[2]);
                Matrix3D m1 = q.Matrix();

                Matrix3D m1Transp = new Matrix3D(m1);
                m1Transp.Transpose();

                // NAHORU
                Vektor y = new Vektor(0, 0, -1);

                Vektor up = new Vektor(_currentScene.Camera.Up);
                up.Normalize();
                Matrix3D m1transp = new Matrix3D(m1);
                m1transp.Transpose();
                Vektor up2 = m1transp.Transform2NewPoint(up);
                up2.Normalize();

                q = new Quaternion(up2, y);
                //double[] degss2 = q.ToEulerDegs();
                //Matrix3D m2 = Matrix3D.NewRotateByDegrees(-degss2[0], -degss2[1], -degss2[2]);
                Matrix3D m2 = q.Matrix();
                m = m1 * m2;
            }
            else
            {
                m = Matrix3D.NewRotateByDegrees(obj.degX, obj.degY, obj.degZ);
            }
            RotateWholeEditor(m);
        }
        
        /// <summary>
        /// zjisti, zda jsou dva body blizko sobe zahrnujic X i Y souradnice
        /// </summary>
        /// <param name="p1">bod 1</param>
        /// <param name="p2">bod 2</param>
        /// <param name="maxVal">maximalni povolena vzdalenost</param>
        /// <returns>true, kdyz jsou bliz, nez je maxVal</returns>
        private bool IsCloserThanPoint(Point p1, Point p2, int maxVal)
        {
            int xDelta = Math.Abs(p1.X - p2.X);
            int yDelta = Math.Abs(p1.Y - p2.Y);
            if (xDelta + yDelta < maxVal)
                return true;
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double x = Double.Parse(this.statusLabelX.Text.Substring(0,this.statusLabelX.Text.Length - 1));
            double y = Double.Parse(this.statusLabelY.Text.Substring(0, this.statusLabelY.Text.Length - 1));
            double z = Double.Parse(this.statusLabelZ.Text.Substring(0, this.statusLabelZ.Text.Length - 1));
            Matrix3D m = Matrix3D.NewRotateByDegrees(x, y, z);
            RotateWholeEditor(m);
        }

        /// <summary>
        /// zmena hodnoty numericUpDown prekresli editor podle daneho otoceni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onValNumChange(object sender, EventArgs e)
        {
            if (!_canRotationChange) return;

            NumericUpDown num = sender as NumericUpDown;
            if (num.Value > 359)
                num.Value = num.Value % 360;
            else if (num.Value < 0)
                num.Value = 360 + num.Value;

            double x = (double)this.numericUpDown1.Value;
            double y = (double)this.numericUpDown2.Value;
            double z = (double)this.numericUpDown3.Value;
            Matrix3D m = Matrix3D.NewRotateByDegrees(x, y, z);
            _canRotationChange2 = false;
            RotateWholeEditor(m);
            //_canRotationChange2 = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DrawingObject obj in _objectsToDraw)
            {
                if (obj.GetType() == typeof(DrawingCube))
                {
                    DrawingCube cube = (DrawingCube)obj;
                    Matrix3D foreverPuvodni = _matrixForever.Transpose();
                    Vektor oldCenterForever = foreverPuvodni * cube.Center;

                    foreach (Vektor p in cube.Points)
                    {
                        p.Posunuti(0.2, 0, 0);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (DrawingObject obj in _objectsToDraw)
            {
                if (obj.GetType() == typeof(DrawingCube))
                {
                    DrawingCube cube = (DrawingCube)obj;
                    Matrix3D foreverPuvodni = _matrixForever.Transpose();
                    Vektor oldCenterForever = foreverPuvodni * cube.Center;

                    foreach (Vektor p in cube.Points)
                    {
                        p.Posunuti(-0.2, 0, 0);
                    }
                }
            }
        }

        private WndScene GetWndScene()
        {
            ParentEditor pf = (ParentEditor)this.ParentForm;
            return pf._WndScene;
        }

        public void RemoveRaytrObject(object shape)
        {
            if (shape is RayImage)
            {

            }
            else if (shape is DrawingAnimation)
            {
                _objectsToDraw.Remove((DrawingObject)shape);
            }
            else if (shape is DrawingObject)
            {
                DrawingObject drObj = (DrawingObject)shape;
                _currentScene.RemoveObject(drObj.ModelObject);
                _objectsToDraw.Remove(drObj);
            }
        }
        /// <summary>
        /// Prida novy objekt ze sveta raytraceru do sveta editoru
        /// </summary>
        /// <param name="shape">teleso ze sveta Raytraceru (object, svetlo, camera)</param>
        public void AddRaytrObject(object shape)
        {
            if (shape is DefaultShape)
            {
                if (shape.GetType() == typeof(Sphere))
                {
                    Sphere sph = (Sphere)shape;
                    DrawingSphere drSphere = new DrawingSphere(sph);
                    drSphere.ApplyRotationMatrix(_matrixForever); // nastaveni do souradnic editoru
                    _objectsToDraw.Add(drSphere);
                    WndScene wndScene = GetWndScene();
                    wndScene.AddItem(drSphere);
                    wndScene.ShowNode(drSphere);
                }
                else if (shape.GetType() == typeof(Plane))
                {
                    Plane plane = (Plane)shape;
                    DrawingPlane drPlane = new DrawingPlane(plane);
                    drPlane.ApplyRotationMatrix(_matrixForever); // nastaveni do souradnic editoru
                    _objectsToDraw.Add(drPlane);
                    WndScene wndScene = GetWndScene();
                    wndScene.AddItem(drPlane);
                    wndScene.ShowNode(drPlane);
                }
                else if (shape.GetType() == typeof(Cube))
                {
                    Cube cube = (Cube)shape;
                    DrawingCube drCube = new DrawingCube(cube);
                    drCube.ApplyRotationMatrix(_matrixForever); // nastaveni do souradnic editoru
                    _objectsToDraw.Add(drCube);
                    WndScene wndScene = GetWndScene();
                    wndScene.AddItem(drCube);
                    wndScene.ShowNode(drCube);
                }
                else if (shape.GetType() == typeof(Cylinder))
                {
                    Cylinder cylinder = (Cylinder)shape;
                    DrawingCylinder drCyl = new DrawingCylinder(cylinder);
                    drCyl.ApplyRotationMatrix(_matrixForever); // nastaveni do souradnic editoru
                    _objectsToDraw.Add(drCyl);
                    WndScene wndScene = GetWndScene();
                    wndScene.AddItem(drCyl);
                    wndScene.ShowNode(drCyl);
                }
                else if (shape.GetType() == typeof(Triangle))
                {
                    Triangle triangle = (Triangle)shape;
                    DrawingTriangle drTriangl = new DrawingTriangle(triangle);
                    drTriangl.ApplyRotationMatrix(_matrixForever); // nastaveni do souradnic editoru
                    _objectsToDraw.Add(drTriangl);
                    WndScene wndScene = GetWndScene();
                    wndScene.AddItem(drTriangl);
                    wndScene.ShowNode(drTriangl);
                }
                else if (shape.GetType() == typeof(Cone))
                {
                    Cone cone = (Cone)shape;
                    DrawingCone drCone = new DrawingCone(cone);
                    drCone.ApplyRotationMatrix(_matrixForever); // nastaveni do souradnic editoru
                    _objectsToDraw.Add(drCone);
                    WndScene wndScene = GetWndScene();
                    wndScene.AddItem(drCone);
                    wndScene.ShowNode(drCone);
                }
                else if (shape.GetType() == typeof(CustomObject))
                {
                    CustomObject custom = (CustomObject)shape;
                    DrawingCustom drCust = new DrawingCustom(custom);
                    drCust.ApplyRotationMatrix(_matrixForever); // nastaveni do souradnic editoru
                    _objectsToDraw.Add(drCust);
                    WndScene wndScene = GetWndScene();
                    wndScene.AddItem(drCust);
                    wndScene.ShowNode(drCust);
                }
            }
            else if (shape is Light)
            {
                Light light = (Light)shape;
                DrawingLight drLight = new DrawingLight(light);
                drLight.ApplyRotationMatrix(_matrixForever); // nastaveni do souradnic editoru
                _objectsToDraw.Add(drLight);
                WndScene wndScene = GetWndScene();
                wndScene.AddItem(drLight);
                wndScene.ShowNode(drLight);
            }
            else if (shape is Camera)
            {
                Camera cam = (Camera)shape;
                DrawingCamera drCam = new DrawingCamera(cam);
                drCam.ApplyRotationMatrix(_matrixForever);
                _objectsToDraw.Add(drCam);
                WndScene wndScene = GetWndScene();
                wndScene.AddItem(drCam);
            }

        }

        public void AddAnimation(DrawingAnimation drAnim)
        {
            drAnim.ApplyRotationMatrix(_matrixForever);
            _objectsToDraw.Add(drAnim);
            WndScene wndScene = GetWndScene();
            wndScene.AddItem(drAnim);
            wndScene.ShowNode(drAnim);
        }

        public void AddCustomObject(DrawingCustom drCust)
        {
            drCust.ApplyRotationMatrix(_matrixForever);
            _objectsToDraw.Add(drCust);
            WndScene wndScene = GetWndScene();
            wndScene.AddItem(drCust);
            wndScene.ShowNode(drCust);
        }

        /// <summary>
        /// Vyprazdni a znovunaplni cely seznam objektu sceny
        /// </summary>
        public void FillWndScene()
        {
            WndScene wndScene = GetWndScene();
            wndScene.ClearAll();
            foreach (DrawingObject drobj in _objectsToDraw)
            {
                wndScene.AddItem(drobj);
            }
        }
        
        /// <summary>
        /// Prida scenu do editoru. I do vsech oken celeho editoru.
        /// </summary>
        /// <param name="scene"></param>
        public void AddRaytrScene(RayTracerLib.Scene scene)
        {
            _currentScene = scene;
            this._objectsToDraw.Clear();
            foreach (DefaultShape shape in scene.SceneObjects)
            {
                // prida novy objekt ze sveta raytraceru do sveta editoru
                AddRaytrObject(shape);
            }

            foreach (Light l in scene.Lights)
                AddRaytrObject(l);

            // nakonec pridame kameru
            AddRaytrObject(scene.Camera);
        }

        /// <summary>
        /// Nastavi objekt sceny jako vybrany v editoru (jako by byl vybran kliknutim)
        /// </summary>
        /// <param name="shape">objekt sceny</param>
        public void SetObjectSelected(DrawingObject shape)
        {
            foreach (DrawingObject sefSh in _objectsToDraw)
            {
                if (sefSh == shape)
                {
                    _Selected =  sefSh;
                }
            }
        }

        /// <summary>
        /// zobrazeni svetel v editoru?
        /// </summary>
        private void OnShowLights(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            this._showLights = btn.Checked;
        }

        private void onShowCamera(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            this._showCamera = btn.Checked;
        }

        /// <summary>
        /// udalost dvojkliku na kreslici platno.
        /// Je-li osetrena i udalost OnMouseDown, pak ta bude osetrena pred touto udalosti
        /// 
        /// Ucel: vybrani objektu k jeho transformaci. Tedy krome toho, ze se zobrazi jeho vlastnosti
        /// v okne Properties, tak se u objektu zobrazi nabidka na mozne transformace: 
        ///     POSUN, ROTACE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Clicks < 2)   // neni-li doubleclick, konec
                return;

            if (_Selected == null)   // nebyl-li vybran zadny kreslici objekt v mouseDown, pak konec
                return;

            _isDragging = false; // nepresouvame objekt;

            labelClick.Text = "Double Click";

            Vektor zpoint = new Vektor(0, 0, 1);
            this._matrixForever.TransformPoint(zpoint);
            Vektor zpointNorm = new Vektor(zpoint);
            zpointNorm.Normalize();
            
            PointF pf1 = _Selected.Points[0].To2D(_scale, _zoom, _centerPoint);
            Vektor p3d = Vektor.To3D_From2D(pf1, zpoint.Z, _scale, _zoom, _centerPoint);
            int asd = 2;

            //MoveSelectedObject();

        }

        private void MoveSelectedObject(double dx, double dy, double dz)
        {
            DrawingObject drawObj = _Selected;
            DefaultShape modelObj = null;
            if (drawObj.ModelObject is DefaultShape)
                modelObj = (DefaultShape)drawObj.ModelObject;

            Matrix3D matrixCurrent = this._matrix;
            Matrix3D matrixForever = this._matrixForever;
            Matrix3D transposed = matrixForever.Transpose();

            modelObj.Move(dx, dy, dz);
            drawObj.SetModelObject(modelObj);

            
            drawObj.ApplyRotationMatrix(matrixForever);

            _Selected = drawObj;
            _updateAll = true;
        }

        private void btnL_Click(object sender, EventArgs e)
        {
            this.MoveSelectedObject(-0.5, 0, 0);
        }

        private void btnR_Click(object sender, EventArgs e)
        {
            this.MoveSelectedObject(0.5, 0, 0);
        }
        private void btnU_Click(object sender, EventArgs e)
        {
            this.MoveSelectedObject(0, -0.5, 0);
        }

        private void btnD_Click(object sender, EventArgs e)
        {
            this.MoveSelectedObject(0, 0.5, 0);
        }

        private void btnZPlus_Click(object sender, EventArgs e)
        {
            this.MoveSelectedObject(0, 0, 0.5);
        }

        private void btnZMinus_Click(object sender, EventArgs e)
        {
            this.MoveSelectedObject(0, 0, -0.5);
        }

        private void toolBtnFront_Click(object sender, EventArgs e)
        {
            Matrix3D m = Matrix3D.NewRotateByDegrees(170, 350, 0);
            RotateWholeEditor(m);
        }

        /// <summary>
        /// Obsluha udalosti pri presunu mysi na polozku nabidky kliknutelnych objektu.
        /// Zobrazi se vybrany objekt v editoru, ale menu zustane otevrene
        /// </summary>
        private void OnShowItemFromControlMenu(object sender, MenuDrawingItemArg e)
        {
            _Selected = e.ObjectToDraw;
            //WndScene wndsc = GetWndScene();
            //wndsc.ShowNode(e.ObjectToDraw);
        }

        /// <summary>
        /// Obsluha udalosti pri kliknuti mysi na polozku nabidky kliknutelnych objektu.
        /// Zobrazi se vybrany objekt v editoru a zaroven se zneviditelni menu (jako by se zavrelo)
        /// </summary>
        private void OnClickItemFromControlMenu(object sender, MenuDrawingItemArg e)
        {
            _Selected = e.ObjectToDraw;
            drawItemFlowLayout1.Visible = false;
            WndScene wndsc = GetWndScene();
            wndsc.ShowNode(_Selected);
            pictureBoard.Focus();
        }

        /// <summary>
        /// Pri deaktivaci okna. Je vhodne zrusit nabidku kliknutych objektu.
        /// </summary>
        private void OnDeactivate(object sender, EventArgs e)
        {
            // zruseni nabidky, aby nedoslo ke kolizi mezi zobrazovanym objektem ve Vlastnostech a vybranym objektem v Boardu
            this.drawItemFlowLayout1.Visible = false;
        }

        
    }
}