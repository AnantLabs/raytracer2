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


namespace _3dEditor
{
    
    public partial class WndBoard : Form
    {
        EditHelper _editHelp;
        Graphics _g;
        Bitmap _editorBmp;

        DrawingObject _isSelected;

        Point3D _axisC3, _axisX3, _axisY3, _axisZ3;
        bool _isDragging;

        int _scale;
        int _zoom;
        int _ZOOM_INCREMENT = 8;
        const int _SCALE_INIT = 150;
        const int _ZOOM_INIT = 150;
        /// <summary>
        /// pocatecni velikost mrizky
        /// </summary>
        const int _GRID_SIZE_INIT = 2;
        /// <summary>
        /// citlivost pro prepocitani souradnic
        /// </summary>
        int _MOUSE_SENSITIVITY = 10;

        double _INIT_DEGX = -20; double _INIT_DEGY = 210; double _INIT_DEGZ = 170;

        Point _lastMousePoint;

        List<Point3D> _pointsList;
        List<Line3D> _linesList;
        List<Line3D> _grid;

        /// <summary>
        /// vsechny objekty vykreslovane v editoru
        /// </summary>
        List<DrawingObject> _objectsToDraw;

        Point _centerPoint;
        Pen _penAxis;
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
        private bool _showCamera;

        public WndBoard()
        {
            InitializeComponent();

            
            _g = this.pictureBoard.CreateGraphics();
            _penAxis = new Pen(Color.Black, 3.0f);
            _penGrid = new Pen(Color.DarkCyan, 2.0f);
            _penObject = new Pen(Color.Chocolate, 1.5f);
            _penSelectedObject = new Pen(Color.Chocolate, 2.5f);
            _fontAxis = new Font(Font.FontFamily, 10, FontStyle.Italic);
            _editHelp = new EditHelper();
            

            this.MouseWheel += new MouseEventHandler(onBoard_MouseWheel);
            _grid = new List<Line3D>((int)Math.Pow(_GRID_SIZE_INIT + 1, 3));
            _objectsToDraw = new List<DrawingObject>(30);

            this.Update();
            this.Focus();

            Reset();
        }

        private void Reset()
        {
            this.Reset(_INIT_DEGX, _INIT_DEGY, _INIT_DEGZ, _ZOOM_INIT, _SCALE_INIT);
            
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
        private void Reset(double degreesX, double degreesY, double degreesZ, int zoom, int scale)
        {
            _objectsToDraw.Clear();
            _isDragging = false;
            _scale = scale;
            _zoom = zoom;

            this.numericUpDown1.Value = (int)degreesX;
            this.numericUpDown2.Value = (int)degreesY;
            this.numericUpDown3.Value = (int)degreesZ;

            // resetuje body na hlavnich osach
            //
            _axisC3 = new Point3D(0, 0, 0);
            _axisX3 = new Point3D(1, 0, 0);
            _axisY3 = new Point3D(0, 1, 0);
            _axisZ3 = new Point3D(0, 0, 1);

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
            _matrixForever = _matrix;

            

            this.toolStripComboBox1.SelectedIndex = _GRID_SIZE_INIT - 2;      // init nastaveni typu mrizky

            _objectsToDraw.Clear();
            //_objectsToDraw.Add(new DrawingCube(1, 1, -2));
            //_objectsToDraw.Add(new DrawingSphere(new Point3D(1, 2, 1), 1));
            //_objectsToDraw.Add(new DrawingCylinder(new Point3D(1, 2, 1), 1, 3));
            //_objectsToDraw.Add(new DrawingPlane(20, 0.5f, 45, 0, 0, null));
            //_objectsToDraw.Add(new DrawingSphere(new Point3D(2, 1, 0), 2));

            //
            //  ROTACE VSECH OBJEKTU V EDITORU
            //
            _grid = _matrix.Transform2NewLines(_grid);
            foreach (DrawingObject obj in _objectsToDraw)
            {
                obj.Rotate(_matrix);
            }

            this._matrix = EditorLib.Matrix3D.Identity;
            pictureBoard.Focus();
            Redraw();
        }

        void onBoard_MouseWheel(object sender, MouseEventArgs e)
        {
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
                //foreach (Line3D l in obj.Lines)
                //{
                //    a = l.A.To2D(_scale, _zoom, _centerPoint);
                //    b = l.B.To2D(_scale, _zoom, _centerPoint);
                //    g.DrawLine(_penObject, a, b);
                //}

                if (obj.ModelObject is DefaultShape)
                {
                    DefaultShape defSpape = (DefaultShape)obj.ModelObject;
                    Color color = defSpape.Material.Color.SystemColor();


                    if (defSpape.IsActive == false)
                        continue;

                    if (obj == _isSelected)
                    {
                        _penObject = new Pen(color, EditHelper.PenSelectedWidth);
                    }
                    else
                    {
                        _penObject = new Pen(color, EditHelper.PenNormalWidth);
                    }
                }

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
                    float xOhnisko = rad;

                    List<Point3D[]> quarts = sph.GetQuartets();
                    foreach (Point3D[] arr in quarts)
                    {
                        PointF[] pfArr = new PointF[arr.Length];
                        for (int j = 0; j < arr.Length; j++)
                        {
                            pfArr[j] = arr[j].To2D(_scale, _zoom, _centerPoint);
                        }
                        g.DrawClosedCurve(_penObject, pfArr, 1F, System.Drawing.Drawing2D.FillMode.Winding);
                        // pridame ohraniceni cestou
                        path = new GraphicsPath();
                        path.AddClosedCurve(pfArr, 0.9F);
                        editorObject.AddPath(path);
                        _editHelp.AddClickableObject(editorObject);
                    }
                }

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

                else if (obj.GetType() == typeof(DrawingCube))
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

                    List<Point3D[]> quarts = cube.GetQuarts();
                    foreach (Point3D[] arr in quarts)
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

                    List<Point3D[]> quarts = cyl.GetQuartets();
                    foreach (Point3D[] arr in quarts)
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

                    Point3D[] bottomPts = quarts[0];
                    Point3D[] upperPts = quarts[quarts.Count - 1];
                    Point3D[] poly1 = new Point3D[4];
                    poly1[0] = bottomPts[0];
                    poly1[1] = bottomPts[2];
                    poly1[2] = upperPts[2];
                    poly1[3] = upperPts[0];
                    PointF[] poly1F = new PointF[poly1.Length];
                    for (int j = 0; j < poly1.Length; j++)
                    {
                        poly1F[j] = poly1[j].To2D(_scale, _zoom, _centerPoint);
                    }

                    Point3D[] poly2 = new Point3D[4];
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
                else if (obj.GetType() == typeof(DrawingLight)) // ================ LIGHT
                {
                    if (_showLights == false)
                        continue;

                    DrawingLight light = (DrawingLight)obj;
                    Light l = (Light)light.ModelObject;
                    if (l.IsActive == false)
                        continue;
                    EditorObject editorObject = new EditorObject(light);
                    GraphicsPath path = new GraphicsPath();

                    a = light.Center.To2D(_scale, _zoom, _centerPoint);
                    float upperX = a.X - (float)( Properties.Resources.bulb_transp.Width / 2 );
                    float upperH = a.Y - (float)( Properties.Resources.bulb_transp.Height / 2 );
                    g.DrawImage(Properties.Resources.bulb_transp, new PointF(upperX, upperH));
                    path.AddRectangle(new RectangleF(upperX, upperH, Properties.Resources.bulb_transp.Width, Properties.Resources.bulb_transp.Height));
                    
                    editorObject.AddPath(path);
                    _editHelp.AddClickableObject(editorObject);
                }
            }

            pictureBoard.Image = _editorBmp;

            //
            // STATUS BAR
            //

            // ZOOM LABEL
            this.statusLabelZoom.Text = _zoom.ToString();

            // ANGLES
            double degsX = Line3D.GetDegreesX(_axisX3, new Point3D(1, 0, 0));
            double degsY = Line3D.GetDegreesX(_axisY3, new Point3D(0, 1, 0));
            double degsZ = Line3D.GetDegreesX(_axisZ3, new Point3D(0, 0, 1));
            this.statusLabelX.Text = Math.Round(degsX, 1).ToString() + "°";
            this.statusLabelY.Text = Math.Round(degsY, 1).ToString() + "°";
            this.statusLabelZ.Text = Math.Round(degsZ, 1).ToString() + "°";

        }

        private void DrawAxes(Graphics g)
        {

            PointF c = _axisC3.To2D(_scale, _zoom, _centerPoint);
            PointF x = _axisX3.To2D(_scale, _zoom, _centerPoint);
            PointF y = _axisY3.To2D(_scale, _zoom, _centerPoint);
            PointF z = _axisZ3.To2D(_scale, _zoom, _centerPoint);

            //
            // vykresli osy
            //
            g.DrawLine(_penAxis, c, x);
            g.DrawLine(_penAxis, c, y);
            g.DrawLine(_penAxis, c, z);

            g.DrawString("X", _fontAxis, Brushes.Black, x);
            g.DrawString("Y", _fontAxis, Brushes.Black, y);
            g.DrawString("Z", _fontAxis, Brushes.Black, z);
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

        private void DrawEllipse(Graphics g, List<Point3D> points3d)
        {
            PointF a = points3d[0].To2D(_scale, _zoom, _centerPoint);
            PointF b = points3d[1].To2D(_scale, _zoom, _centerPoint);

            //g.DrawEllipse(Pens.Gold, uperLeftX, upperLeftY, Width, Height);
        }

        private void onPicMouseMove(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (!pb.ClientRectangle.Contains(e.Location))
                return;
            if (!_isDragging)
                return;

            Point currPoint = e.Location;

            if (IsCloserThanPoint(_lastMousePoint, currPoint, _MOUSE_SENSITIVITY))
                return;

            double coefMove = 0.4;
            double degreesX = 0;
            double degreesY = 0;
            double degreesZ = 0;
            if (e.Button == MouseButtons.Left)
            {
                degreesX = (_lastMousePoint.X - currPoint.X) * coefMove;
                degreesY = (_lastMousePoint.Y - currPoint.Y) * coefMove;

                if (degreesX == 0.0 && degreesY == 0.0)
                {
                    this._isDragging = false;
                    return;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                //degreesY = (_lastMousePoint.X - currPoint.X) * coefMove;
                //degreesZ = (_lastMousePoint.Y - currPoint.Y) * coefMove;

                //if (degreesY == 0.0 && degreesZ == 0.0)
                //{
                //    this._isDragging = false;
                //    return;
                //}
                int xDel = (_lastMousePoint.X - currPoint.X);
                int yDel = (_lastMousePoint.Y - currPoint.Y);
                _centerPoint.X -= xDel;
                _centerPoint.Y -= yDel;

            }
            

            double radiansX = EditorMath.Degrees2Rad(degreesX);
            double radiansY = EditorMath.Degrees2Rad(degreesY);
            double radiansZ = EditorMath.Degrees2Rad(degreesZ);

            
            Matrix3D matr = Matrix3D.NewRotateByRads(-radiansY, radiansX, radiansZ);
            _matrix = _matrix * matr;
            _matrixForever = _matrixForever * _matrix;

            foreach (DrawingObject obj in _objectsToDraw)
            {
                obj.Rotate(_matrix);
            }
            if (toolBtnGrid.Checked)
                _grid = _matrix.Transform2NewLines(_grid);

            Point3D newX3d = _matrix * _axisX3;
            _axisX3 = newX3d;
            Point3D newY3d = _matrix * _axisY3;
            _axisY3 = newY3d;
            Point3D newZ3d = _matrix * _axisZ3;
            _axisZ3 = newZ3d;

            this._lastMousePoint = currPoint;
            
            this._matrix = EditorLib.Matrix3D.Identity;
            this.Redraw();
        }

        /// <summary>
        /// KLIKNUTI MYSI A VYBRANI OBJEKTU V EDITORU
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onPicMouseDown(object sender, MouseEventArgs e)
        {
            List<DrawingObject> drawingList = _editHelp.GetClickableObj(e.Location);
            if (drawingList.Count > 0)
            {
                if (drawingList[0].ModelObject is DefaultShape || drawingList[0].ModelObject is Light)
                {
                    WndScene wndsc = GetWndScene();
                    wndsc.ShowNode(drawingList[0].ModelObject);
                }
                _isSelected = drawingList[0];   // vybereme prvni ze seznamu
                labelClick.Text = "Clicked";
            }
            else
                labelClick.Text = "---";
            pictureBoard.Focus();
            this._isDragging = true;
            this._lastMousePoint = e.Location;
        }

        private void onPicMouseUp(object sender, MouseEventArgs e)
        {
            this._isDragging = false;
            this._matrix = EditorLib.Matrix3D.Identity;
        }

        private void toolBtnReset_Click(object sender, EventArgs e)
        {
            this.Reset();
            Redraw();
        }

        private void toolBtnTop_Click(object sender, EventArgs e)
        {
            this.Reset(-40, -80, 140);
            this.numericUpDown1.Value = -40;
            this.numericUpDown2.Value = -80;
            this.numericUpDown3.Value = 140;
            Redraw();
        }

        private void toolBtnSide_Click(object sender, EventArgs e)
        {
            this.Reset(-40, -80, 140);
            this.numericUpDown1.Value = -40;
            this.numericUpDown2.Value = -80;
            this.numericUpDown3.Value = 140;
            Redraw();
        }

        private void OnChanged1(object sender, EventArgs e)
        {
            ToolStripComboBox c = sender as ToolStripComboBox;
            int d = Int32.Parse(c.SelectedItem.ToString());
            _grid = EditHelper.FillGrid(d);
            _grid = _matrixForever.Transform2NewLines(_grid);
            this.pictureBoard.Focus();
            //Redraw();
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
            this.Reset(x, y, z);
        }

        /// <summary>
        /// zmena hodnoty numericUpDown prekresli editor podle daneho otoceni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onValNumChange(object sender, EventArgs e)
        {
            this.Reset((double)this.numericUpDown1.Value, (double)this.numericUpDown2.Value, (double)this.numericUpDown3.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DrawingObject obj in _objectsToDraw)
            {
                if (obj.GetType() == typeof(DrawingCube))
                {
                    DrawingCube cube = (DrawingCube)obj;
                    Matrix3D foreverPuvodni = _matrixForever.Transpose();
                    Point3D oldCenterForever = foreverPuvodni * cube.Center;

                    foreach (Point3D p in cube.Points)
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
                    Point3D oldCenterForever = foreverPuvodni * cube.Center;

                    foreach (Point3D p in cube.Points)
                    {
                        p.Posunuti(-0.2, 0, 0);
                    }
                }
            }
        }

        private WndScene GetWndScene()
        {
            ParentEditor pf = (ParentEditor)this.ParentForm;
            return pf._wndScene;
        }

        /// <summary>
        /// Prida novy objekt ze sveta raytraceru do sveta editoru
        /// </summary>
        /// <param name="shape">teleso ze sveta Raytraceru</param>
        public void AddRaytrObject(object shape)
        {
            if (shape is DefaultShape)
            {
                if (shape.GetType() == typeof(Sphere))
                {
                    Sphere sph = (Sphere)shape;
                    DrawingSphere drSphere = new DrawingSphere(sph);
                    _objectsToDraw.Add(drSphere);
                    WndScene wndScene = GetWndScene();
                    wndScene.AddItem(sph);
                }
                else if (shape.GetType() == typeof(Plane))
                {
                    Plane plane = (Plane)shape;
                    DrawingPlane drPlane = new DrawingPlane(plane);
                    _objectsToDraw.Add(drPlane);
                    WndScene wndScene = GetWndScene();
                    wndScene.AddItem(plane);
                }
                else if (shape.GetType() == typeof(Cube))
                {
                    Cube cube = (Cube)shape;
                    DrawingCube drCube = new DrawingCube(cube);
                    _objectsToDraw.Add(drCube);
                    WndScene wndScene = GetWndScene();
                    wndScene.AddItem(cube);
                }
                else if (shape.GetType() == typeof(Cylinder))
                {
                    Cylinder cylinder = (Cylinder)shape;
                    DrawingCylinder drCyl = new DrawingCylinder(cylinder);
                    _objectsToDraw.Add(drCyl);
                    WndScene wndScene = GetWndScene();
                    wndScene.AddItem(cylinder);
                }
            }
            else if (shape is Light)
            {
                Light light = (Light)shape;
                DrawingLight drLight = new DrawingLight(light);
                _objectsToDraw.Add(drLight);
                WndScene wndScene = GetWndScene();
                wndScene.AddItem(light);
            }
        }

        /// <summary>
        /// Prida scenu do editoru
        /// </summary>
        /// <param name="scene"></param>
        public void AddRaytrScene(RayTracerLib.Scene scene)
        {
            this._objectsToDraw.Clear();
            foreach (DefaultShape shape in scene.SceneObjects)
            {
                // prida novy objekt ze sveta raytraceru do sveta editoru
                AddRaytrObject(shape);
            }

            foreach (Light l in scene.Lights)
                AddRaytrObject(l);
        }

        /// <summary>
        /// Nastavi objekt sceny jako vybrany v editoru (jako by byl vybran kliknutim)
        /// </summary>
        /// <param name="shape">objekt sceny</param>
        public void SetObjectSelected(DefaultShape shape)
        {
            foreach (DrawingObject sefSh in _objectsToDraw)
            {
                if (sefSh.ModelObject == shape)
                {
                    _isSelected =  sefSh;
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
    }
}