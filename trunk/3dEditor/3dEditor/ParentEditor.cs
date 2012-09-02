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

namespace _3dEditor
{
    public partial class ParentEditor : Form
    {

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
            _WndBoard.Activate();

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
            _WndScene.Refresh();


            //
            // Okno s nastavenim
            //
            _WndProperties = new WndProperties();
            _WndProperties.MdiParent = this;
            _WndProperties.Location = new Point(_WndBoard.Width, _WndScene.Height);
            _WndProperties.Width = _WndScene.Width;
            _WndProperties.Height = (int)(_WndBoard.Height * (1 - _ratioSize));
            _WndProperties.Show();
            //_wndProperties.Activate();

            Form[] fs = this.MdiChildren;

            InitRayTracer();
            //LayoutMdi(MdiLayout.Cascade);
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
            _rayTracer.RScene.SceneObjects.Add(sph1);
            //_rayTracer.RScene.SceneObjects.Add(sph2);
            _rayTracer.RScene.SceneObjects.Add(cube1);
            _rayTracer.RScene.SceneObjects.Add(plane1);
            _rayTracer.RScene.SceneObjects.Add(cyl);
            sph2.IsActive = false;

            //_rayTracer.RScene.SetDefaultScene4();

            this._WndBoard.AddRaytrScene(_rayTracer.RScene);

            RayImage img = new RayImage();
            this._WndScene.AddItem(img);
            
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
            UpdateAll();
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
        }

    }
}
