using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RayTracerLib;
using Mathematics;

namespace RayTracerForm
{
    public partial class ListViewObject : Form
    {
        RayTracing _rayTracer;
        Sphere _sphere;
        Light _light;
        Box _box;
        Cylinder _cylinder;

        Material _material;

        Point _defaultPoint;

        public ListViewObject()
        {
            InitializeComponent();

            _material = new Material();

            this.Size = new Size(450, 620);
            _defaultPoint = new Point(30, 40);

            this.DialogResult = DialogResult.Cancel;
            SetPanelsVisible(false);

            this.panelLight.Location = _defaultPoint;
            this.panelSphere.Location = _defaultPoint;
            this.panelBox.Location = _defaultPoint;
            this.panelCylindr.Location = _defaultPoint;
            this.panelRovina.Location = _defaultPoint;

            this.comboSelect.SelectedIndex = 1;

            SetSphMaterialComponents();
            SetBoxMaterialComponents();
            SetCylMaterialComponents();
            SetPlaneMaterialComponents();
            
        }



        private void SetPanelsVisible(bool visib)
        {
            this.panelSphere.Visible = visib;
            this.panelLight.Visible = visib;
            this.panelBox.Visible = visib;
            this.panelCylindr.Visible = visib;
            this.panelRovina.Visible = visib;
        }

        public void Set(RayTracing rt, Sphere sph, Light l, Box b, Cylinder c)
        {
            _rayTracer = rt;
            _sphere = sph;
            _light = l;
            _box = b;
            _cylinder = c;
        }

        #region Sphere
        /// <summary>
        /// nic nedela zatim - metoda pro pripad, ze se bude modifikovat objekt v novem formulari
        /// modifikuje kouli z formulare do sceny
        /// </summary>
        private void buttonSphereSave_Click(object sender, EventArgs e)
        {
            
            if (_sphere == null)
                return;

            Vektor origin = new Vektor(
                (double)this.numericKouleX.Value,
                (double)this.numericKouleY.Value,
                (double)this.numericKouleZ.Value);

            double r = (double)this.numericKouleRadius.Value;

            _sphere.Origin = origin;
            _sphere.R = r;

        }

        /// <summary>
        /// pridani nove koule z formulare do sceny
        /// </summary>
        private void buttonSphereAdd_Click(object sender, EventArgs e)
        {
            Vektor origin = new Vektor(
            (double)this.numericKouleX.Value,
            (double)this.numericKouleY.Value,
            (double)this.numericKouleZ.Value);


            double r = (double)this.numericKouleRadius.Value;
            Sphere sph = new Sphere(origin, r);

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

            _rayTracer.RScene.SceneObjects.Add(sph);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonSphereCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion Sphere


        #region Light
        /// <summary>
        /// nic nedela zatim - metoda pro pripad, ze se bude modifikovat objekt v novem formulari
        /// modifikovane existujiciho svetla z formulare do sceny
        /// </summary>
        private void buttonLightSave_Click(object sender, EventArgs e)
        {

            if (_light == null)
                return;

            Vektor lightCoord = new Vektor(
                (double)this.numericSvetloX.Value,
                (double)this.numericSvetloY.Value,
                (double)this.numericSvetloZ.Value);

            RayTracerLib.Colour lightColor = new RayTracerLib.Colour(
                (double)this.numericSvetloR.Value,
                (double)this.numericSvetloG.Value,
                (double)this.numericSvetloB.Value,
                1);

            _light.Color = lightColor;
            _light.Coord = lightCoord;

        }

        /// <summary>
        /// pridani noveho svetla z formulare do sceny
        /// </summary>
        private void buttonLightAdd_Click(object sender, EventArgs e)
        {
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

            Light newLight;
            newLight = new Light(lightCoord, lightColor);
            newLight.IsSoftLight = this.checkBoxLightIsSoft.Checked;
            bool isSinglePass = this.radioSinglePass.Checked;
            if (newLight.IsSoftLight)
                newLight.SetSoftLights(numSoftLights, epsSoftLights, isSinglePass);

            _rayTracer.RScene.AddLight(newLight);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion Light

        private void btnLighColor_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                double r = colorDialog.Color.R / (double)255;
                double g = colorDialog.Color.G / (double)255;
                double b = colorDialog.Color.B / (double)255;
                double a = colorDialog.Color.A / (double)255;

                RayTracerLib.Colour col = new RayTracerLib.Colour(r, g, b, a);

                //this.btnLighColor.BackColor = colorDialog.Color;

                this.numericSvetloR.Value = (decimal)col.R;
                this.numericSvetloG.Value = (decimal)col.G;
                this.numericSvetloB.Value = (decimal)col.B;
            }
        }

        private void btnSphereColor_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                double r = colorDialog.Color.R / (double)255;
                double g = colorDialog.Color.G / (double)255;
                double b = colorDialog.Color.B / (double)255;
                double a = colorDialog.Color.A / (double)255;

                RayTracerLib.Colour col = new RayTracerLib.Colour(r, g, b, a);

            }
        }

        #region Cylindr
        private void btnCylSave_Click(object sender, EventArgs e)
        {
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

            Cylinder cyl = new Cylinder(center, dir, r, h);

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

            _rayTracer.RScene.SceneObjects.Add(cyl);

            this.DialogResult = DialogResult.OK;
            this.Close();

        }
        #endregion

        #region Krychle
        private void btnBoxSave_Click(object sender, EventArgs e)
        {
            Vektor center = new Vektor(
                (double)this.numericBoxX.Value,
                (double)this.numericBoxY.Value,
                (double)this.numericBoxZ.Value);


            Vektor dir = new Vektor(
                (double)this.numericBoxOsaX.Value,
                (double)this.numericBoxOsaY.Value,
                (double)this.numericBoxOsaZ.Value);

            double size = (double)this.numericBoxSize.Value;

            //Box b = new Box(center, size);

            Cube c = new Cube(center, dir, size);

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

            //b.Material = mat;

            c.Material = mat;

            _rayTracer.RScene.SceneObjects.Add(c);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        private void OnComboChange(object sender, EventArgs e)
        {
            SetPanelsVisible(false);
            if (this.comboSelect.SelectedIndex == 0)
            {
                this.panelLight.Visible = true;
            }
            else if (this.comboSelect.SelectedIndex == 1)
            {
                this.panelSphere.Visible = true;
            }
            else if (this.comboSelect.SelectedIndex == 2)
            {
                this.panelBox.Visible = true;
            }
            else if (this.comboSelect.SelectedIndex == 3)
            {
                this.panelCylindr.Visible = true;
            }
            else if (this.comboSelect.SelectedIndex == 4)
            {
                this.panelRovina.Visible = true;
            }
        }

        private void btnPlaneSave_Click(object sender, EventArgs e)
        {
            Vektor norm = new Vektor(
                (double)this.numericRovinaA.Value,
                (double)this.numericRovinaB.Value,
                (double)this.numericRovinaC.Value);


            double d = (double)numericRovinaD.Value;

            double minx, maxx, miny, maxy, minz, maxz;
            minx = miny = minz = Double.NegativeInfinity;
            maxx = maxy = maxz = Double.PositiveInfinity;

            if (checkBoxMinX.Checked)
                minx = (double)numMinX.Value;

            if (checkBoxMaxX.Checked)
                maxx = (double)numMaxX.Value;

            if (checkBoxMinY.Checked)
                miny = (double)numMinY.Value;

            if (checkBoxMaxY.Checked)
                maxy = (double)numMaxY.Value;

            if (checkBoxMinZ.Checked)
                minz = (double)numMinZ.Value;

            if (checkBoxMinZ.Checked)
                maxz = (double)numMaxZ.Value;

            Material mat = new Material();
            mat.Ka = (double)this.numPlaneKa.Value;
            mat.Ks = (double)this.numPlaneKs.Value;
            mat.Kd = (double)this.numPlaneKd.Value;
            mat.KT = (double)this.numPlaneKt.Value;
            mat.SpecularExponent = (int)this.numPlaneH.Value;
            mat.N = (double)this.numPlaneN.Value;

            mat.Color.R = (double)this.numPlaneColR.Value;
            mat.Color.G = (double)this.numPlaneColG.Value;
            mat.Color.B = (double)this.numPlaneColB.Value;

            Plane p = new Plane(norm, d, mat);
            p.MinX = minx;
            p.MaxX = maxx;
            p.MinY = miny;
            p.MaxY = maxy;
            p.MinZ = minz;
            p.MaxZ = maxz;

            //p.Material = mat;

            _rayTracer.RScene.SceneObjects.Add(p);

            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void btnMaterial_Click(object sender, EventArgs e)
        {
            MaterialForm form = new MaterialForm();
            form.Set(_material);
            DialogResult res = form.ShowDialog();
            if (res == DialogResult.OK)
            {
                _material = form._material;
            }
        }

        private void SetSphMaterialComponents()
        {
            this.numSphKa.Value = (decimal)_material.Ka;
            this.numSphKs.Value = (decimal)_material.Ks;
            this.numSphKd.Value = (decimal)_material.Kd;
            this.numSphKt.Value = (decimal)_material.KT;
            this.numSphH.Value = (decimal)_material.SpecularExponent;
            this.numSphN.Value = (decimal)_material.N;

            this.numSphColR.Value = (decimal)_material.Color.R;
            this.numSphColG.Value = (decimal)_material.Color.G;
            this.numSphColB.Value = (decimal)_material.Color.B;
        }

        private void SetCylMaterialComponents()
        {
            this.numCylKa.Value = (decimal)_material.Ka;
            this.numCylKs.Value = (decimal)_material.Ks;
            this.numCylKd.Value = (decimal)_material.Kd;
            this.numCylKt.Value = (decimal)_material.KT;
            this.numCylH.Value = (decimal)_material.SpecularExponent;
            this.numCylN.Value = (decimal)_material.N;

            this.numCylColR.Value = (decimal)_material.Color.R;
            this.numCylColG.Value = (decimal)_material.Color.G;
            this.numCylColB.Value = (decimal)_material.Color.B;
        }

        private void SetPlaneMaterialComponents()
        {
            this.numPlaneKa.Value = (decimal)_material.Ka;
            this.numPlaneKs.Value = (decimal)_material.Ks;
            this.numPlaneKd.Value = (decimal)_material.Kd;
            this.numPlaneKt.Value = (decimal)_material.KT;
            this.numPlaneH.Value = (decimal)_material.SpecularExponent;
            this.numPlaneN.Value = (decimal)_material.N;

            this.numPlaneColR.Value = (decimal)_material.Color.R;
            this.numPlaneColG.Value = (decimal)_material.Color.G;
            this.numPlaneColB.Value = (decimal)_material.Color.B;
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

                this.numSphColR.Value = (decimal)col.R;
                this.numSphColG.Value = (decimal)col.G;
                this.numSphColB.Value = (decimal)col.B;
            }
        }

        private void SetBoxMaterialComponents()
        {
            this.numBoxKa.Value = (decimal)_material.Ka;
            this.numBoxKs.Value = (decimal)_material.Ks;
            this.numBoxKd.Value = (decimal)_material.Kd;
            this.numBoxKt.Value = (decimal)_material.KT;
            this.numBoxH.Value = (decimal)_material.SpecularExponent;
            this.numBoxN.Value = (decimal)_material.N;

            this.numBoxColR.Value = (decimal)_material.Color.R;
            this.numBoxColG.Value = (decimal)_material.Color.G;
            this.numBoxColB.Value = (decimal)_material.Color.B;
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

                this.numCylColR.Value = (decimal)col.R;
                this.numCylColG.Value = (decimal)col.G;
                this.numCylColB.Value = (decimal)col.B;
            }
        }

        private void btnPlaneMaterialColor_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                double r = colorDialog.Color.R / (double)255;
                double g = colorDialog.Color.G / (double)255;
                double b = colorDialog.Color.B / (double)255;
                double a = colorDialog.Color.A / (double)255;

                RayTracerLib.Colour col = new RayTracerLib.Colour(r, g, b, a);

                this.numPlaneColR.Value = (decimal)col.R;
                this.numPlaneColG.Value = (decimal)col.G;
                this.numPlaneColB.Value = (decimal)col.B;
            }
        }

        private void btnBoxMaterialColor_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                double r = colorDialog.Color.R / (double)255;
                double g = colorDialog.Color.G / (double)255;
                double b = colorDialog.Color.B / (double)255;
                double a = colorDialog.Color.A / (double)255;

                RayTracerLib.Colour col = new RayTracerLib.Colour(r, g, b, a);

                this.numBoxColR.Value = (decimal)col.R;
                this.numBoxColG.Value = (decimal)col.G;
                this.numBoxColB.Value = (decimal)col.B;
            }
        }

        private void OnMinXChange(object sender, EventArgs e)
        {
            CheckBox ch = (CheckBox)sender;
            if (ch.Checked)
                numMinX.Enabled = true;
            else
                numMinX.Enabled = false;

        }

        private void OnMaxXChange(object sender, EventArgs e)
        {
            CheckBox ch = (CheckBox)sender;
            if (ch.Checked)
                numMaxX.Enabled = true;
            else
                numMaxX.Enabled = false;

        }

        private void OnMinYChange(object sender, EventArgs e)
        {
            CheckBox ch = (CheckBox)sender;
            if (ch.Checked)
                numMinY.Enabled = true;
            else
                numMinY.Enabled = false;

        }

        private void OnMaxYChange(object sender, EventArgs e)
        {
            CheckBox ch = (CheckBox)sender;
            if (ch.Checked)
                numMaxY.Enabled = true;
            else
                numMaxY.Enabled = false;

        }

        private void OnMinZChange(object sender, EventArgs e)
        {
            CheckBox ch = (CheckBox)sender;
            if (ch.Checked)
                numMinZ.Enabled = true;
            else
                numMinZ.Enabled = false;

        }

        private void OnMaxZChange(object sender, EventArgs e)
        {
            CheckBox ch = (CheckBox)sender;
            if (ch.Checked)
                numMaxZ.Enabled = true;
            else
                numMaxZ.Enabled = false;

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
    }
}
