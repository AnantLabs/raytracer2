using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using EditorLib;

namespace _3dEditor
{
    
    public partial class WndBoard : Form
    {
        Graphics _g;
        Bitmap _editorBmp;

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

        List<DrawingObject> _objectsToDraw;
        Point _centerPoint;
        Pen _penAxis;
        Pen _penGrid;
        Pen _penObject;
        Font _fontAxis;
        
        Matrix3D _matrix;
        /// <summary>
        /// Matice pocitana od zacatku
        /// </summary>
        Matrix3D _matrixForever;
        public WndBoard()
        {
            InitializeComponent();

            _g = this.pictureBoard.CreateGraphics();
            _penAxis = new Pen(Color.Black, 3.0f);
            _penGrid = new Pen(Color.DarkCyan, 2.0f);
            _penObject = new Pen(Color.Chocolate, 1.5f);
            _fontAxis = new Font(Font.FontFamily, 10, FontStyle.Italic);

            

            this.MouseWheel += new MouseEventHandler(onBoard_MouseWheel);
            _grid = new List<Line3D>((int)Math.Pow(_GRID_SIZE_INIT + 1, 3));
            _objectsToDraw = new List<DrawingObject>(30);

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

            //
            //  ROTACE VSECH OBJEKTU V EDITORU
            //
            _grid = _matrix.Transform2NewLines(_grid);
            foreach (DrawingObject obj in _objectsToDraw)
            {
                obj.Rotate(_matrix);
            }

            this.toolStripComboBox1.SelectedIndex = _GRID_SIZE_INIT - 2;      // init nastaveni typu mrizky

            _objectsToDraw.Add(new DrawingCube(1, 2, -2));
            _objectsToDraw.Add(new DrawingSphere(new Point3D(0, 0, 0), 1));
            _objectsToDraw.Add(new DrawingSphere(new Point3D(2, 1, 0), 2));
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

        private void Redraw()
        {
            this.Redraw(_g);
        }
        private void Redraw(Graphics g)
        {
            
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
                foreach (Line3D l in obj.Lines)
                {
                    a = l.A.To2D(_scale, _zoom, _centerPoint);
                    b = l.B.To2D(_scale, _zoom, _centerPoint);
                    g.DrawLine(_penObject, a, b);
                }

                if (obj.GetType() == typeof(DrawingSphere))
                {
                    DrawingSphere sph = (DrawingSphere)obj;
                    a = sph.Center.To2D(_scale, _zoom, _centerPoint);
                    
                    float rad = ((float)sph.Radius * _scale) / _scale *_zoom;

                    g.DrawEllipse(_penObject, a.X - rad, a.Y - rad, rad * 2, rad * 2);
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

        private void onPicMouseDown(object sender, MouseEventArgs e)
        {
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

        
    }
    
}
