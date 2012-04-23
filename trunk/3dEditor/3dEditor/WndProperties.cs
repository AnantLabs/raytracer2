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
    public partial class WndProperties : Form
    {

        RayImage _currentImage;

        public WndProperties()
        {
            InitializeComponent();

            SetAllInvisible();

            this.panelSphere.Location = new Point(0, 0);
            this.panelCylindr.Location = new Point(0, 0);
            this.panelRovina.Location = new Point(0, 0);
            this.panelBox.Location = new Point(0, 0);
            this.panelLight.Location = new Point(0, 0);
            this.panelCamera.Location = new Point(0, 0);
            this.panelImage.Location = new Point(0, 0);
            this.panelAnimace.Location = new Point(0, 0);

            

            
        }

        public void ShowObject(object obj)
        {
            if (obj.GetType() == typeof(Sphere))
                ShowSphere((Sphere)obj);

            else if (obj.GetType() == typeof(Plane))
                ShowPlane((Plane)obj);

            else if (obj.GetType() == typeof(Cube))
                ShowCube((Cube)obj);

            else if (obj.GetType() == typeof(Cylinder))
                ShowCylinder((Cylinder)obj);

            else if (obj.GetType() == typeof(RayImage))
                ShowImage((RayImage)obj);

            else if (obj.GetType() == typeof(Light))
                ShowLight((Light)obj);

            else if (obj.GetType() == typeof(Camera))
                ShowCamera((Camera)obj);

            this.Update();
        }

        private void SetAllInvisible()
        {
            this.panelSphere.Visible = false;
            this.panelCylindr.Visible = false;
            this.panelRovina.Visible = false;
            this.panelBox.Visible = false;
            this.panelLight.Visible = false;
            this.panelCamera.Visible = false;
            this.panelImage.Visible = false;
            this.panelAnimace.Visible = false;
        }

        private void ShowSphere(Sphere sph)
        {
            SetAllInvisible();
            this.panelSphere.Visible = true;
            this.Text = "Properties: Sphere";

            this.numericKouleX.Value = (decimal)MyMath.Clamp(sph.Origin.X, -100, 100);
            this.numericKouleY.Value = (decimal)MyMath.Clamp(sph.Origin.Y, -100, 100);
            this.numericKouleZ.Value = (decimal)MyMath.Clamp(sph.Origin.Z, -100, 100);

            this.numericKouleRadius.Value = (decimal)sph.R;

            this.numSphKa.Value = (decimal)sph.Material.Ka;
            this.numSphKs.Value = (decimal)sph.Material.Ks;
            this.numSphKd.Value = (decimal)sph.Material.Kd;
            this.numSphKt.Value = (decimal)sph.Material.KT;
            this.numSphH.Value = (decimal)sph.Material.SpecularExponent;
            this.numSphN.Value = (decimal)sph.Material.N;

            this.numSphColR.Value = (decimal)sph.Material.Color.R;
            this.numSphColG.Value = (decimal)sph.Material.Color.G;
            this.numSphColB.Value = (decimal)sph.Material.Color.B;

        }
        private void ShowPlane(Plane pl)
        {
            SetAllInvisible();
            this.panelRovina.Visible = true;
            this.Text = "Properties: Plane";

            this.numericRovinaA.Value = (decimal)MyMath.Clamp(pl.Normal.X, -100, 100);
            this.numericRovinaB.Value = (decimal)MyMath.Clamp(pl.Normal.Y, -100, 100);
            this.numericRovinaC.Value = (decimal)MyMath.Clamp(pl.Normal.Z, -100, 100);
            this.numericRovinaD.Value = (decimal)MyMath.Clamp(pl.D, -100, 100);

            this.numPlaneKa.Value = (decimal)pl.Material.Ka;
            this.numPlaneKs.Value = (decimal)pl.Material.Ks;
            this.numPlaneKd.Value = (decimal)pl.Material.Kd;
            this.numPlaneKt.Value = (decimal)pl.Material.KT;
            this.numPlaneH.Value = (decimal)pl.Material.SpecularExponent;
            this.numPlaneN.Value = (decimal)pl.Material.N;

            this.numPlaneColR.Value = (decimal)pl.Material.Color.R;
            this.numPlaneColG.Value = (decimal)pl.Material.Color.G;
            this.numPlaneColB.Value = (decimal)pl.Material.Color.B;

            checkBoxMinX.Checked = (pl.MinX != Double.NegativeInfinity);
            if (checkBoxMinX.Checked)
                this.numMinX.Value = (decimal)pl.MinX;

            checkBoxMaxX.Checked = (pl.MaxX != Double.PositiveInfinity);
            if (checkBoxMaxX.Checked)
                this.numMaxX.Value = (decimal)pl.MaxX;


            checkBoxMinY.Checked = (pl.MinY != Double.NegativeInfinity);
            if (checkBoxMinY.Checked)
                this.numMinY.Value = (decimal)pl.MinY;

            checkBoxMaxY.Checked = (pl.MaxY != Double.PositiveInfinity);
            if (checkBoxMaxY.Checked)
                this.numMaxY.Value = (decimal)pl.MaxY;

            checkBoxMinZ.Checked = (pl.MinZ != Double.NegativeInfinity);
            if (checkBoxMinZ.Checked)
                this.numMinZ.Value = (decimal)pl.MinZ;

            checkBoxMaxZ.Checked = (pl.MaxZ != Double.PositiveInfinity);
            if (checkBoxMaxZ.Checked)
                this.numMaxZ.Value = (decimal)pl.MaxZ;
        }

        private void ShowCube(Cube c)
        {
            SetAllInvisible();
            this.panelBox.Visible = true;
            this.Text = "Properties: Cube";

            this.numericBoxX.Value = (decimal)MyMath.Clamp(c.Center.X, -100, 100);
            this.numericBoxY.Value = (decimal)MyMath.Clamp(c.Center.Y, -100, 100);
            this.numericBoxZ.Value = (decimal)MyMath.Clamp(c.Center.Z, -100, 100);

            this.numericBoxOsaX.Value = (decimal)MyMath.Clamp(c.Dir.X, -100, 100);
            this.numericBoxOsaY.Value = (decimal)MyMath.Clamp(c.Dir.Y, -100, 100);
            this.numericBoxOsaZ.Value = (decimal)MyMath.Clamp(c.Dir.Z, -100, 100);

            this.numericBoxSize.Value = (decimal)c.Size;

            this.numBoxKa.Value = (decimal)c.Material.Ka;
            this.numBoxKs.Value = (decimal)c.Material.Ks;
            this.numBoxKd.Value = (decimal)c.Material.Kd;
            this.numBoxKt.Value = (decimal)c.Material.KT;
            this.numBoxH.Value = (decimal)c.Material.SpecularExponent;
            this.numBoxN.Value = (decimal)c.Material.N;

            this.numBoxColR.Value = (decimal)c.Material.Color.R;
            this.numBoxColG.Value = (decimal)c.Material.Color.G;
            this.numBoxColB.Value = (decimal)c.Material.Color.B;
        }

        private void ShowCylinder(Cylinder c)
        {
            SetAllInvisible();
            this.panelCylindr.Visible = true;
            this.Text = "Properties: Cylinder";

            this.numericCylCentX.Value = (decimal)MyMath.Clamp(c.Center.X, -100, 100);
            this.numericCylCentY.Value = (decimal)MyMath.Clamp(c.Center.Y, -100, 100);
            this.numericCylCentZ.Value = (decimal)MyMath.Clamp(c.Center.Z, -100, 100);

            this.numericCylDirX.Value = (decimal)MyMath.Clamp(c.Dir.X, -100, 100);
            this.numericCylDirY.Value = (decimal)MyMath.Clamp(c.Dir.Y, -100, 100);
            this.numericCylDirZ.Value = (decimal)MyMath.Clamp(c.Dir.Z, -100, 100);

            this.numericCylH.Value = (decimal)c.H;
            this.numericCylR.Value = (decimal)c.R;

            this.numCylKa.Value = (decimal)c.Material.Ka;
            this.numCylKs.Value = (decimal)c.Material.Ks;
            this.numCylKd.Value = (decimal)c.Material.Kd;
            this.numCylKt.Value = (decimal)c.Material.KT;
            this.numCylH.Value = (decimal)c.Material.SpecularExponent;
            this.numCylN.Value = (decimal)c.Material.N;

            this.numCylColR.Value = (decimal)c.Material.Color.R;
            this.numCylColG.Value = (decimal)c.Material.Color.G;
            this.numCylColB.Value = (decimal)c.Material.Color.B;
        }

        private void ShowCamera(Camera cam)
        {
            SetAllInvisible();
            this.panelCamera.Visible = true;
            this.Text = "Properties: Camera";

            this.numericKameraDirX.Value = (decimal)MyMath.Clamp(cam.Norm.X, -100, 100);
            this.numericKameraDirY.Value = (decimal)MyMath.Clamp(cam.Norm.Y, -100, 100);
            this.numericKameraDirZ.Value = (decimal)MyMath.Clamp(cam.Norm.Z, -100, 100);

            this.numericKameraStredX.Value = (decimal)MyMath.Clamp(cam.Source.X, -100, 100);
            this.numericKameraStredY.Value = (decimal)MyMath.Clamp(cam.Source.Y, -100, 100);
            this.numericKameraStredZ.Value = (decimal)MyMath.Clamp(cam.Source.Z, -100, 100);

            this.numericKameraUpX.Value = (decimal)MyMath.Clamp(cam.Up.X, -100, 100);
            this.numericKameraUpY.Value = (decimal)MyMath.Clamp(cam.Up.Y, -100, 100);
            this.numericKameraUpZ.Value = (decimal)MyMath.Clamp(cam.Up.Z, -100, 100);
        }

        private void ShowLight(Light l)
        {
            SetAllInvisible();
            this.panelLight.Visible = true;
            this.Text = "Properties: Light";

            this.numericSvetloX.Value = (decimal)MyMath.Clamp(l.Coord.X, -100, 100);
            this.numericSvetloY.Value = (decimal)MyMath.Clamp(l.Coord.Y, -100, 100);
            this.numericSvetloZ.Value = (decimal)MyMath.Clamp(l.Coord.Z, -100, 100);

            this.numericSvetloR.Value = (decimal)l.Color.R;
            this.numericSvetloG.Value = (decimal)l.Color.G;
            this.numericSvetloB.Value = (decimal)l.Color.B;

            this.checkBoxLightIsSoft.Checked = l.IsSoftLight;


            this.numericLightNum.Value = l.SoftNumSize;
            this.numericLightEps.Value = (decimal)l.SoftEpsilon;

            if (l.IsSinglePass)
                this.radioSinglePass.Checked = true;
            else
                this.radioMultiPass.Checked = true;
        }

        private void ShowImage(RayImage img)
        {
            SetAllInvisible();
            _currentImage = img;

            this.panelImage.Visible = true;
            this.Text = "Properties: Image";

            this.comboResolution.DataSource = img.PictureSize;
            this.comboResolution.SelectedIndex = img.IndexPictureSize;         // nastaveni prvni polozky
            if (img.IndexPictureSize == img.PictureSize.Length - 1)
            {
                this.txbResX.Text = img.CurrentSize.Width.ToString();
                this.txbResY.Text = img.CurrentSize.Height.ToString();
            }

            this.numericRecurs.Value = (decimal)MyMath.Clamp(img.MaxRecurse, -1, 100);
            this.btnBgCol.BackColor = img.BackgroundColor.SystemColor();
            this.checkAntialias.Checked = img.IsAntialiasing;
        }

        private void ShowAnimation()
        {
            SetAllInvisible();
            this.panelImage.Visible = true;
            this.Text = "Properties: Animation";
        }


        #region comboResolution


        private void ComboResolIndexChange(object sender, EventArgs e)
        {
            // je-li vybrana posledni polozka - zobrazi se vyber pro vlastni rozliseni
            if (this.comboResolution.SelectedIndex == this.comboResolution.Items.Count - 1)
            {
                this.txbResX.Visible = true;
                this.txbResY.Visible = true;
                this.labelResCross.Visible = true;
                this.labelResPixels.Visible = true;
            }
            else
            {
                this.txbResX.Visible = false;
                this.txbResY.Visible = false;
                this.labelResCross.Visible = false;
                this.labelResPixels.Visible = false;
            }
        }

        /// <summary>
        /// Vlastni formatovani comboboxu pro vyber rozliseni
        /// </summary>
        private void OnFormatComboResol(object sender, ListControlConvertEventArgs e)
        {
            Size size;
            try
            {
                size = (Size)e.ListItem;
            }
            catch (Exception)
            {
                return;
            }
            if (size.Width == 0)
                e.Value = String.Format("Custom");
            else
            {
                string text = String.Format("{0} x {1} pixels ", size.Width, size.Height);
                double vysl = Math.Round((double)size.Width / (double)size.Height, 2);
                if (vysl == 1.78)
                    text += "(16:9)";
                else
                    text += "(4:3)";
                e.Value = String.Format(text, size.Width, size.Height);
            }
        }

        private void btnBgCol_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                btnBgCol.BackColor = colorDialog.Color;
                Colour col = Colour.ColourCreate(colorDialog.Color);
                this._currentImage.BackgroundColor = col;
            }
        }

        #endregion

        private void bntImageSave(object sender, EventArgs e)
        {
            btnBgCol.BackColor = colorDialog.Color;
            Colour col = Colour.ColourCreate(colorDialog.Color);
            this._currentImage.BackgroundColor = col;

            this._currentImage.MaxRecurse = (int)this.numericRecurs.Value;
            this._currentImage.IsAntialiasing = this.checkAntialias.Checked;

            this._currentImage.IndexPictureSize = this.comboResolution.SelectedIndex;
            int w = 100;
            int h= 100;
            // vybrano vlastni rozliseni
            if (this.comboResolution.SelectedIndex == this._currentImage.PictureSize.Length - 1)
            {
                Int32.TryParse(txbResX.Text, out w);
                Int32.TryParse(txbResY.Text, out h);
                
            } // prednastavene rozliseni
            else
            {
                w = this._currentImage.PictureSize[this._currentImage.IndexPictureSize].Width;
                h = this._currentImage.PictureSize[this._currentImage.IndexPictureSize].Height;
            }
            this._currentImage.CurrentSize = new Size(w, h);

        }
    }
}
