using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EditorLib;
using RayTracerLib;
using System.Runtime.InteropServices;

namespace _3dEditor
{
    public partial class ParentEditor : Form
    {
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;

        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;

        private const int SB_LINELEFT = 0;
        private const int SB_LINERIGHT = 1;
        private const int SB_PAGELEFT = 2;
        private const int SB_PAGERIGHT = 3;
        private const int SB_THUMBPOSITION = 4;
        private const int SB_THUMBTRACK = 5;
        private const int SB_LEFT = 6;
        private const int SB_RIGHT = 7;
        private const int SB_ENDSCROLL = 8;

        private const int SIF_TRACKPOS = 0x10;
        private const int SIF_RANGE = 0x1;
        private const int SIF_POS = 0x4;
        private const int SIF_PAGE = 0x2;
        private const int SIF_ALL = SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS;

        public WndBoard _WndBoard { get; private set; }
        public WndScene _WndScene { get; private set; }
        public WndProperties _WndProperties { get; private set; }

        public RayTracing _rayTracer;

        const double _ratioSize = 1.0 / 2.4;    // pomer vysek oken> hlavni okno: okno sceny
        public ParentEditor()
        {
            InitializeComponent();
            //
            // Okno Kreslici plochy
            //
            _WndBoard = new WndBoard();
            _WndBoard.MdiParent = this;
            _WndBoard.Show();
            _WndBoard.Invalidate();

            //
            // Okno sceny
            //
            _WndScene = new WndScene();
            _WndScene.MdiParent = this;
            _WndScene.Location = new Point(_WndBoard.Width, _WndBoard.Location.Y);
            _WndScene.Height = (int)(_WndBoard.Height * _ratioSize);
            
            _WndScene.Show();
            _WndScene.Update();
            _WndScene.Activate();
            _WndScene.Invalidate();


            //
            // Okno s nastavenim
            //
            _WndProperties = new WndProperties();
            _WndProperties.MdiParent = this;
            _WndProperties.Location = new Point(_WndBoard.Width, _WndScene.Height);
            _WndProperties.Width = _WndScene.Width;
            _WndProperties.Height = (int)(_WndBoard.Height * (1 - _ratioSize));
            _WndProperties.Show();
            _WndProperties.Invalidate();
            //_WndScene.Paint += new PaintEventHandler

            Form[] fs = this.MdiChildren;

            VScroll = true;
            VScrollBar vs = new VScrollBar();
            vs.Parent = this;
            vs.Scroll += new ScrollEventHandler(this.onScroll);
            vs.Dock = DockStyle.Right;
            Controls.Add(vs);

            
            InitRayTracer();
            //LayoutMdi(MdiLayout.Cascade);
        }

        protected override void WndProc(ref Message m)
        {

            int i2 = 2;
            //ScrollableControl scc = new ScrollableControl();
            
            // Add event handlers for the OnScroll and OnValueChanged events.
            
            //vs.ValueChanged += new EventHandler(this.vScrollBar1_ValueChanged); 

            //VScrollProperties vsp = new VScrollProperties(ScrVScroll);
            switch (m.Msg)
            {
                case WM_VSCROLL:
                    int i = 2;
                    //ScrollInfoStruct si = new ScrollInfoStruct();
                    //si.fMask = SIF_ALL;
                    //si.cbSize = (uint)Marshal.SizeOf(si);
                    //GetScrollInfo(msg.HWnd, SB_VERT, ref si);
                    //if (msg.WParam.ToInt32() == SB_ENDSCROLL)
                    //{
                    //    ScrollEventArgs sargs = new ScrollEventArgs(ScrollEventType.EndScroll, si.nPos);
                    //    onScroll(this, sargs);
                    //}
                    break;
            }
            base.WndProc(ref m);
        }

        private void InitRayTracer()
        {
            _rayTracer = new RayTracing();
            _rayTracer.RScene = new Scene();
            Sphere sph1 = new Sphere(new Vektor(1, 2, 1), 1, new Colour(1, 0.5, 0.1, 1));
            Sphere sph2 = new Sphere(new Vektor(-2, -1, 2), 1.5);
            Cube cube1 = new Cube(new Vektor(-3, 5, 2), new Vektor(1, 1, 1), 1);
            Plane plane1 = new Plane(new Vektor(1, 1, 0), 3);
            Cylinder cyl = new Cylinder(new Vektor(3, 2, 1), new Vektor(1, 1, 1), 1, 8);
            _rayTracer.RScene.SceneObjects.Clear();
            //_rayTracer.RScene.SceneObjects.Add(sph1);
            //_rayTracer.RScene.SceneObjects.Add(sph2);
            _rayTracer.RScene.SceneObjects.Add(cube1);
            _rayTracer.RScene.SceneObjects.Add(plane1);
            //_rayTracer.RScene.SceneObjects.Add(cyl);
            sph2.IsActive = false;

            //_rayTracer.RScene.SetDefaultScene4();

            this._WndBoard.AddRaytrScene(_rayTracer.RScene);

            RayImage img = new RayImage(1, new Colour(1, 0, 0, 0), false);
            this._WndScene.AddItem(img);

            DrawingAnimation drAnim = new DrawingAnimation();
            this._WndBoard.AddAnimation(drAnim);
            
        }

        private void onMDIChildActivate(object sender, EventArgs e)
        {
        }

        private void onShown(object sender, EventArgs e)
        {
            // zaktivuje hlavni plochu na kresleni
            this.MdiChildren[0].Activate();
            this.MdiChildren[1].Activate();
            this.MdiChildren[2].Activate();
            this.MdiChildren[0].Focus();
            this.MdiChildren[1].Focus();
            this.MdiChildren[2].Focus();
            this.MdiChildren[0].Invalidate();
            this.MdiChildren[1].Invalidate();
            this.MdiChildren[2].Invalidate(); 
            //UpdateAll();
            //_wndBoard.Activate();
            //this.MdiChildren[0].Activate();
            //nebo:
            //for (int i = 0; i < MdiChildren.Length; i++ )
            //{
            //    this.MdiChildren[i].Activate();
            //}
        }

        private void onPaint(object sender, PaintEventArgs e)
        {
            //UpdateAll();
            ////_WndBoard.toolStrip1.Invalidate(true);
            //this.MdiChildren[0].Activate();//.Invalidate(true);
            //this.MdiChildren[1].Activate();//.Invalidate(true);
            //this.MdiChildren[2].Activate();// Invalidate(true);
        }

        private void onScroll(object sender, ScrollEventArgs e)
        {
            UpdateAll();
        }

        private void UpdateAll()
        {
            //this.MdiChildren[0].Activate();
            this.MdiChildren[0].Update();
            //this.MdiChildren[1].Activate();
            this.MdiChildren[1].Update();
            //this.MdiChildren[2].Activate();
            this.MdiChildren[2].Update();
            this.MdiChildren[0].Activate();
            this.MdiChildren[1].Activate();
            this.MdiChildren[2].Activate();
            this.MdiChildren[0].Focus();
            this.MdiChildren[1].Focus();
            this.MdiChildren[2].Focus();
            this.MdiChildren[0].Invalidate(true);
            this.MdiChildren[1].Invalidate(true);
            this.MdiChildren[2].Invalidate(true);
            this.Invalidate(true);
        }

        private void onScroll1(object sender, ScrollEventArgs e)
        {
            int i123 = 0;
        }

        private void onClick(object sender, EventArgs e)
        {
            int a = 0;
        }

        private void onMouse(object sender, MouseEventArgs e)
        {
            int a = 0;
        }

        private void onMDIActive(object sender, EventArgs e)
        {
            //this.MdiChildren[0].Invalidate(true);
            //this.MdiChildren[1].Invalidate(true);
            //this.MdiChildren[2].Invalidate(true);
            //this.Invalidate(true);
        }

    }
}
