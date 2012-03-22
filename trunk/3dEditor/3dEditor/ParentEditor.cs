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

        public WndBoard _wndBoard;
        public WndScene _wndScene;
        public WndProperties _wndProperties;

        public RayTracing _rayTracer;

        public ParentEditor()
        {
            InitializeComponent();
            //
            // Okno Kreslici plochy
            //
            _wndBoard = new WndBoard();
            _wndBoard.MdiParent = this;
            _wndBoard.Show();
            _wndBoard.Activate();


            //
            // Okno sceny
            //
            _wndScene = new WndScene();
            _wndScene.MdiParent = this;
            _wndScene.Location = new Point(_wndBoard.Width, _wndBoard.Location.Y);
            _wndScene.Height = _wndBoard.Height / 2;
            
            _wndScene.Show();
            _wndScene.Update();
            _wndScene.Activate();
            _wndScene.Refresh();


            //
            // Okno s nastavenim
            //
            _wndProperties = new WndProperties();
            _wndProperties.MdiParent = this;
            _wndProperties.Location = new Point(_wndBoard.Width, _wndScene.Height);
            _wndProperties.Width = _wndScene.Width;
            _wndProperties.Height = _wndBoard.Height / 2;
            _wndProperties.Show();
            //_wndProperties.Activate();

            Form[] fs = this.MdiChildren;

            InitRayTracer();
            //LayoutMdi(MdiLayout.Cascade);
        }

        private void InitRayTracer()
        {
            _rayTracer = new RayTracing();
            _rayTracer.RScene = new Scene();
            Sphere sph1 = new Sphere(new Vektor(1, 2, 1), 1);
            Sphere sph2 = new Sphere(new Vektor(-2, -1, 2), 1.5);
            Cube cube1 = new Cube(new Vektor(-3, 5, 2), new Vektor(1, 1, 1), 1);
            Plane plane1 = new Plane(new Vektor(1, 1, 0), 3);
            Cylinder cyl = new Cylinder(new Vektor(3, 2, 1), new Vektor(1, 1, 1), 1, 8);
            _rayTracer.RScene.SceneObjects.Clear();
            _rayTracer.RScene.SceneObjects.Add(sph1);
            _rayTracer.RScene.SceneObjects.Add(sph2);
            _rayTracer.RScene.SceneObjects.Add(cube1);
            _rayTracer.RScene.SceneObjects.Add(plane1);
            _rayTracer.RScene.SceneObjects.Add(cyl);

            this._wndBoard.AddRaytrScene(_rayTracer.RScene);
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
