﻿using System;
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

        /// aktualne zobrazovany objekt
        object _currentlyDisplayed;

        RayImage _currentImage;

        /// <summary>
        /// IMPORTANT!
        /// Indikuje povoleni zmeneni zobrazovaneho objektu.
        /// Je-li nastaveno na false, tak se objekt pouze zobrazuje zavolanim z jineho okna.
        /// Na TRUE jenastaven vzdy - kliknutim na libolne tlactiko zmeny vlastnosti.
        /// TRUE: pouze zobrazeni objektu - nutno zakazet jeho vnitrni zmenu pri udalostech k nastaveni
        /// hodnot v prislusnych prvich - numericBoxech apod.
        /// </summary>
        private bool _permissionToModify;

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

            _permissionToModify = true;
        }
        
        public void ShowObject(object obj)
        {
            // pouze zobrazeni objektu - nutno zakazet jeho vnitrni zmenu pri udalostech k nastaveni
            // hodnot v prislusnych prvich - numericBoxech apod.
            _permissionToModify = false;        

            if (obj.GetType() == typeof(DrawingSphere))
                ShowSphere((DrawingSphere)obj);

            else if (obj.GetType() == typeof(DrawingPlane))
                ShowPlane((DrawingPlane)obj);

            else if (obj.GetType() == typeof(DrawingCube))
                ShowCube((DrawingCube)obj);

            else if (obj.GetType() == typeof(DrawingCylinder))
                ShowCylinder((DrawingCylinder)obj);

            else if (obj.GetType() == typeof(RayImage))
                ShowImage((RayImage)obj);

            else if (obj.GetType() == typeof(DrawingLight))
                ShowLight((DrawingLight)obj);

            else if (obj.GetType() == typeof(DrawingCamera))
                ShowCamera((DrawingCamera)obj);

            else
            {
                _permissionToModify = true;
                return;
            }
            _permissionToModify = true;
            _currentlyDisplayed = obj;
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

        private void ShowSphere(DrawingSphere drSphere)
        {
            // zabraneni neustalemu blikani pri modifikaci stejne koule
            if (!this.panelSphere.Visible)
            {
                SetAllInvisible();
                this.panelSphere.Visible = true;
                this.Text = "Properties: Sphere";
            }

            Sphere sph = (Sphere)drSphere.ModelObject;

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
        private void ShowPlane(DrawingPlane drPlane)
        {
            // zabraneni neustalemu blikani pri modifikaci stejne koule
            if (!this.panelRovina.Visible)
            {
                SetAllInvisible();
                this.panelRovina.Visible = true;
                this.Text = "Properties: Plane";
            }

            Plane pl = (Plane)drPlane.ModelObject;

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

        private void ShowCube(DrawingCube drCube)
        {
            // zabraneni neustalemu blikani pri modifikaci stejne koule
            if (!this.panelBox.Visible)
            {
                SetAllInvisible();
                this.panelBox.Visible = true;
                this.Text = "Properties: Cube";
            }

            Cube c = (Cube)drCube.ModelObject;

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

        private void ShowCylinder(DrawingCylinder drCyl)
        {
            // zabraneni neustalemu blikani pri modifikaci stejne koule
            if (!this.panelCylindr.Visible)
            {
                SetAllInvisible();
                this.panelCylindr.Visible = true;
                this.Text = "Properties: Cylinder";
            }

            Cylinder c = (Cylinder)drCyl.ModelObject;

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

        private void ShowCamera(DrawingCamera drCam)
        {
            // zabraneni neustalemu blikani pri modifikaci stejne koule
            if (!this.panelCamera.Visible)
            {
                SetAllInvisible();
                this.panelCamera.Visible = true;
                this.Text = "Properties: Camera";
            }

            Camera cam = (Camera)drCam.ModelObject;

            this.numericKameraDirX.Value = (decimal)MyMath.Clamp(cam.Norm.X, -100, 100);
            this.numericKameraDirY.Value = (decimal)MyMath.Clamp(cam.Norm.Y, -100, 100);
            this.numericKameraDirZ.Value = (decimal)MyMath.Clamp(cam.Norm.Z, -100, 100);

            this.numericKameraStredX.Value = (decimal)MyMath.Clamp(cam.Source.X, -100, 100);
            this.numericKameraStredY.Value = (decimal)MyMath.Clamp(cam.Source.Y, -100, 100);
            this.numericKameraStredZ.Value = (decimal)MyMath.Clamp(cam.Source.Z, -100, 100);

            this.numericKameraUpX.Value = (decimal)MyMath.Clamp(cam.Up.X, -100, 100);
            this.numericKameraUpY.Value = (decimal)MyMath.Clamp(cam.Up.Y, -100, 100);
            this.numericKameraUpZ.Value = (decimal)MyMath.Clamp(cam.Up.Z, -100, 100);

            this.checkCross.Checked = drCam.ShowCross;
            this.checkSide1.Checked = drCam.ShowSide1;
            this.checkSide2.Checked = drCam.ShowSide2;

            this.numericKamDist.Value = (decimal)drCam.Dist;
            this.numericKamHeight.Value = (decimal)drCam.Height;
            this.numericKamWidth.Value = (decimal)drCam.Width;
        }

        private void ShowLight(DrawingLight drLight)
        {
            // zabraneni neustalemu blikani pri modifikaci stejne koule
            if (!this.panelLight.Visible)
            {
                SetAllInvisible();
                this.panelLight.Visible = true;
                this.Text = "Properties: Light";
            }

            Light l = (Light)drLight.ModelObject;

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
            // zabraneni neustalemu blikani pri modifikaci stejne koule
            if (!this.panelImage.Visible)
            {
                SetAllInvisible();
                this.panelImage.Visible = true;
                this.Text = "Properties: Image";
            }

            _currentImage = img;

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
            // zabraneni neustalemu blikani pri modifikaci stejne koule
            if (!this.panelAnimace.Visible)
            {
                SetAllInvisible();
                this.panelAnimace.Visible = true;
                this.Text = "Properties: Animation";
            }

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

        ///////////////////////////////////////////////
        //////// S P H E R E
        //////////////////////////////////////////////

        private void actionSphereSet(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingSphere))
                return;

            if (!_permissionToModify)
                return;

            DrawingSphere drSph = (DrawingSphere)_currentlyDisplayed;
            Sphere sph = (Sphere)drSph.ModelObject;

            Vektor origin = new Vektor(
                (double)this.numericKouleX.Value,
                (double)this.numericKouleY.Value,
                (double)this.numericKouleZ.Value);

            double r = (double)this.numericKouleRadius.Value;

            sph.Origin = origin;
            sph.R = r;

            Material mat = new Material();
            mat.Ka = (double)this.numSphKa.Value;
            mat.Ks = (double)this.numSphKs.Value;
            mat.Kd = (double)this.numSphKd.Value;
            mat.KT = (double)this.numSphKt.Value;
            mat.SpecularExponent = (int)this.numSphH.Value;
            mat.N = (double)this.numSphN.Value;

            mat.Color.R = (double)this.numSphColR.Value;
            mat.Color.G = (double)this.numSphColG.Value;
            mat.Color.B = (double)this.numSphColB.Value;

            sph.Material = mat;

            drSph.SetModelObject(sph);
            WndBoard wndB = GetWndBoard();
            drSph.ApplyRotationMatrix(wndB.RotationMatrix);
            WndScene wndSc = GetWndScene();
            wndSc.UpdateRecords();
        }

        private void btnSphMaterialColor_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                double r = colorDialog.Color.R / (double)255;
                double g = colorDialog.Color.G / (double)255;
                double b = colorDialog.Color.B / (double)255;
                double a = colorDialog.Color.A / (double)255;

                RayTracerLib.Colour col = new RayTracerLib.Colour(r, g, b, a);

                _permissionToModify = false;
                this.numSphColR.Value = (decimal)col.R;
                this.numSphColG.Value = (decimal)col.G;
                _permissionToModify = true;
                this.numSphColB.Value = (decimal)col.B;
            }
        }

        ///////////////////////////////////////////////
        //////// C U B E
        //////////////////////////////////////////////

        private void actionCubeSet(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingCube))
                return;

            if (!_permissionToModify)
                return;

            DrawingCube drCube = (DrawingCube)_currentlyDisplayed;
            Cube cube = (Cube)drCube.ModelObject;

            Vektor center = new Vektor(
                (double)this.numericBoxX.Value,
                (double)this.numericBoxY.Value,
                (double)this.numericBoxZ.Value);

            Vektor dir = new Vektor(
                (double)this.numericBoxOsaX.Value,
                (double)this.numericBoxOsaY.Value,
                (double)this.numericBoxOsaZ.Value);

            double size = (double)this.numericBoxSize.Value;


            Material mat = new Material();
            mat.Ka = (double)this.numBoxKa.Value;
            mat.Ks = (double)this.numBoxKs.Value;
            mat.Kd = (double)this.numBoxKd.Value;
            mat.KT = (double)this.numBoxKt.Value;
            mat.SpecularExponent = (int)this.numBoxH.Value;
            mat.N = (double)this.numBoxN.Value;

            mat.Color.R = (double)this.numBoxColR.Value;
            mat.Color.G = (double)this.numBoxColG.Value;
            mat.Color.B = (double)this.numBoxColB.Value;

            cube.Material = mat;
            cube.SetValues(center, dir, size);

            drCube.SetModelObject(cube);
            WndBoard wndB = GetWndBoard();
            drCube.ApplyRotationMatrix(wndB.RotationMatrix);
            WndScene wndSc = GetWndScene();
            wndSc.UpdateRecords();
        }

        private void btnCubeMaterialColor_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                double r = colorDialog.Color.R / (double)255;
                double g = colorDialog.Color.G / (double)255;
                double b = colorDialog.Color.B / (double)255;
                double a = colorDialog.Color.A / (double)255;

                RayTracerLib.Colour col = new RayTracerLib.Colour(r, g, b, a);

                _permissionToModify = false;
                this.numBoxColR.Value = (decimal)col.R;
                this.numBoxColG.Value = (decimal)col.G;
                _permissionToModify = true;
                this.numBoxColB.Value = (decimal)col.B;
            }
        }

        ///////////////////////////////////////////////
        //////// C Y L I N D E R
        //////////////////////////////////////////////
        #region CYLINDER
        private void actionCylinderSet(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingCylinder))
                return;

            if (!_permissionToModify)
                return;

            DrawingCylinder drCyl = (DrawingCylinder)_currentlyDisplayed;
            Cylinder cyl = (Cylinder)drCyl.ModelObject;

            Vektor center = new Vektor(
                (double)this.numericCylCentX.Value,
                (double)this.numericCylCentY.Value,
                (double)this.numericCylCentZ.Value);

            Vektor dir = new Vektor(
                (double)this.numericCylDirX.Value,
                (double)this.numericCylDirY.Value,
                (double)this.numericCylDirZ.Value);

            double r = (double)this.numericCylR.Value;
            double h = (double)this.numericCylH.Value;

            Material mat = new Material();
            mat.Ka = (double)this.numCylKa.Value;
            mat.Ks = (double)this.numCylKs.Value;
            mat.Kd = (double)this.numCylKd.Value;
            mat.KT = (double)this.numCylKt.Value;
            mat.SpecularExponent = (int)this.numCylH.Value;
            mat.N = (double)this.numCylN.Value;

            mat.Color.R = (double)this.numCylColR.Value;
            mat.Color.G = (double)this.numCylColG.Value;
            mat.Color.B = (double)this.numCylColB.Value;

            cyl.Material = mat;
            cyl.SetValues(center, dir, r, h);

            drCyl.SetModelObject(cyl);
            WndBoard wndB = GetWndBoard();
            drCyl.ApplyRotationMatrix(wndB.RotationMatrix);
            WndScene wndSc = GetWndScene();
            wndSc.UpdateRecords();
        }
        

        private void btnCylMaterialColor_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                double r = colorDialog.Color.R / (double)255;
                double g = colorDialog.Color.G / (double)255;
                double b = colorDialog.Color.B / (double)255;
                double a = colorDialog.Color.A / (double)255;

                RayTracerLib.Colour col = new RayTracerLib.Colour(r, g, b, a);

                _permissionToModify = false;
                this.numCylColR.Value = (decimal)col.R;
                this.numCylColG.Value = (decimal)col.G;
                _permissionToModify = true;
                this.numCylColB.Value = (decimal)col.B;
            }
        }
        #endregion
        ///////////////////////////////////////////////
        //////// C A M E R A
        //////////////////////////////////////////////
        #region CAMERA
        private void actionKameraSet(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingCamera))
                return;

            if (!_permissionToModify)
                return;

            DrawingCamera drCam = (DrawingCamera)_currentlyDisplayed;
            Camera cam = (Camera)drCam.ModelObject;

            try
            {

                Vektor stred = new Vektor(
                    (double)this.numericKameraStredX.Value,
                    (double)this.numericKameraStredY.Value,
                    (double)this.numericKameraStredZ.Value);

                Vektor dir = new Vektor(
                    (double)this.numericKameraDirX.Value,
                    (double)this.numericKameraDirY.Value,
                    (double)this.numericKameraDirZ.Value);

                Vektor up = new Vektor(
                    (double)this.numericKameraUpX.Value,
                    (double)this.numericKameraUpY.Value,
                    (double)this.numericKameraUpZ.Value);

                cam.Source = stred;
                cam.SetNormAndUp(dir, up);
                
                
                bool showCross = this.checkCross.Checked;
                bool showSide1 = this.checkSide1.Checked;
                bool showSide2 = this.checkSide2.Checked;

                double dist = (double)this.numericKamDist.Value;
                double height = (double)this.numericKamHeight.Value;
                double width = (double)this.numericKamWidth.Value;

                drCam.Set(cam, dist, height, width, showCross, showSide1, showSide2);
                WndBoard wnd = GetWndBoard();
                drCam.ApplyRotationMatrix(wnd.RotationMatrix);
                WndScene wndSc = GetWndScene();
                wndSc.UpdateRecords();

            }
            catch (Exception ex)
            {
            }

        }

        #endregion
        ///////////////////////////////////////////////
        //////// L I G H T
        //////////////////////////////////////////////
        #region LIGHT
        private void actionLightSet(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingLight))
                return;

            if (!_permissionToModify)
                return;

            DrawingLight drLight = (DrawingLight)_currentlyDisplayed;
            Light light = (Light)drLight.ModelObject;

            Vektor lightCoord = new Vektor(
                (double)this.numericSvetloX.Value,
                (double)this.numericSvetloY.Value,
                (double)this.numericSvetloZ.Value);

            RayTracerLib.Colour lightColor = new RayTracerLib.Colour(
                (double)this.numericSvetloR.Value,
                (double)this.numericSvetloG.Value,
                (double)this.numericSvetloB.Value,
                1);

            int numSoftLights = (int)this.numericLightNum.Value;
            double epsSoftLights = (double)this.numericLightEps.Value;


            light.Coord = lightCoord;
            light.Color = lightColor;

            light.IsSoftLight = this.checkBoxLightIsSoft.Checked;
            bool isSinglePass = this.radioSinglePass.Checked;
            if (light.IsSoftLight)
                light.SetSoftLights(numSoftLights, epsSoftLights, isSinglePass);

            drLight.SetModelObject(light);
            WndBoard wndB = GetWndBoard();
            drLight.ApplyRotationMatrix(wndB.RotationMatrix);
            WndScene wndSc = GetWndScene();
            wndSc.UpdateRecords();

        }

        /// <summary>
        /// vybrani barvy prosvetlo
        /// </summary>
        private void btnLighColor_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                double r = colorDialog.Color.R / (double)255;
                double g = colorDialog.Color.G / (double)255;
                double b = colorDialog.Color.B / (double)255;
                double a = colorDialog.Color.A / (double)255;

                RayTracerLib.Colour col = new RayTracerLib.Colour(r, g, b, a);

                _permissionToModify = false;
                this.numericSvetloR.Value = (decimal)col.R;
                this.numericSvetloG.Value = (decimal)col.G;
                _permissionToModify = true;
                this.numericSvetloB.Value = (decimal)col.B;
            }
        }
        private void OnScrollNum(object sender, EventArgs e)
        {
            TrackBar tr = (TrackBar)sender;

            numericLightNum.Value = tr.Value;
        }

        private void OnScrollEps(object sender, EventArgs e)
        {
            TrackBar tr = (TrackBar)sender;
            numericLightEps.Value = (decimal)(tr.Value * 5 / 100.0);
        }
        private void OnNumericLightNumChanged(object sender, EventArgs e)
        {
            NumericUpDown num = (NumericUpDown)sender;
            this.trackBarLightNum.Value = (int)num.Value;
        }

        private void OnNumericLightEpsChanged(object sender, EventArgs e)
        {
            NumericUpDown num = (NumericUpDown)sender;
            this.trackBarLightEps.Value = (int)(num.Value * 100 / 5);
        }

        private void OnLightSoftCheckedChang(object sender, EventArgs e)
        {
            CheckBox ch = (CheckBox)sender;
            this.panelLightSoft.Visible = ch.Checked;
            this.panelSoftPasses.Visible = ch.Checked;
        }
        #endregion

        public void UpdateSelected()
        {

        }

        private WndBoard GetWndBoard()
        {
            ParentEditor pf = (ParentEditor)this.ParentForm;
            return pf._WndBoard;
        }
        private WndScene GetWndScene()
        {
            ParentEditor pf = (ParentEditor)this.ParentForm;
            return pf._WndScene;
        }

        private void WndProperties_Load(object sender, EventArgs e)
        {

        }

        private void onNumericRotateCylinder(object sender, EventArgs e)
        {
            if (_currentlyDisplayed.GetType() != typeof(DrawingCylinder))
                return;

            NumericUpDown num = sender as NumericUpDown;
            if (num.Value > 359)
                num.Value = num.Value % 360;
            else if (num.Value < 0)
                num.Value = 360 + num.Value;

            double x = (double)this.numCylRotateX.Value;
            double y = (double)this.numCylRotateY.Value;
            double z = (double)this.numCylRotateZ.Value;
            Matrix3D m = Matrix3D.NewRotateByDegrees(x, y, z);

            DrawingCylinder drCyl = _currentlyDisplayed as DrawingCylinder;

            WndBoard wnd = GetWndBoard();
            Matrix3D transp = wnd.RotationMatrix.Transpose();
            drCyl.ApplyRotationMatrix(transp);
            drCyl.RotateCyl(x, y, z);
            //drCyl.ApplyRotationMatrix(m);
        }

    }
}