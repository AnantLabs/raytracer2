using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _3dEditor
{
    public partial class Editor : Form
    {

        public Editor()
        {
            InitializeComponent();
            //
            // Okno Kreslici plochy
            //
            WndBoard b = new WndBoard();
            b.MdiParent = this;
            b.Show();

            //
            // Okno sceny
            //
            WndScene s = new WndScene();
            s.MdiParent = this;
            s.Location = new Point(b.Width, b.Location.Y);
            s.Height = b.Height / 2;
            s.Show();

            //
            // Okno s nastavenim
            //
            WndProperties p = new WndProperties();
            p.MdiParent = this;
            p.Location = new Point(b.Width, s.Height);
            p.Width = s.Width;
            p.Height = b.Height / 2;
            p.Show();

            Form[] fs = this.MdiChildren;

            //LayoutMdi(MdiLayout.Cascade);
        }

        private void onMDIChildActivate(object sender, EventArgs e)
        {
        }

        private void onShown(object sender, EventArgs e)
        {
            // zaktivuje hlavni plochu na kresleni
            this.MdiChildren[0].Activate();
            //nebo:
            //this.ActivateMdiChild(this.MdiChildren[0]);
        }
    }
}
