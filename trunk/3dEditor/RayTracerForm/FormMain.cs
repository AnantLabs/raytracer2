using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RayTracerLib;
using System.Reflection;
using System.Resources;
using System.IO;
using System.Threading;
using System.Drawing.Imaging;
using Mathematics;

namespace RayTracerForm
{
    public partial class FormMain : Form
    {
        RayTracing _rayTracer;

        Graphics _graphics;


        Point _panelEditorLocation = new Point(770, 28);

        Size[] _pictureSize;        // seznam velikosti vykreslovaneho obrazku pro combobox "comboResolution"

        Scene[] _sceneList;         // seznam scen, ktere budou na vyber

        /// <summary>
        /// pomocnik pri kresleni editoru
        /// </summary>
        EditorHelper _editorHelper; 

        Bitmap _editorBmp;

        public FormMain()
        {
            InitializeComponent();
            InitRayTracer();
            InitForm();
            
        }

        private void InitRayTracer()
        {
            _rayTracer = new RayTracing();

            // nastaveni preddefinovanych scen:
            _sceneList = new Scene[9];
            _sceneList[0] = new Scene();
            _sceneList[0].SetDefaultScene4();

            _sceneList[1] = new Scene();
            _sceneList[1].SetDefaultScene5();

            _sceneList[2] = new Scene();
            _sceneList[2].SetDefaultScene6();

            _sceneList[3] = new Scene();
            _sceneList[3].SetDefaultScene1();

            _sceneList[4] = new Scene();
            _sceneList[4].SetDefaultScene3();

            _sceneList[5] = new Scene();
            _sceneList[5].SetDefaultScene2();

            _sceneList[6] = new Scene();
            _sceneList[6].SetDefaultScene7();

            _sceneList[7] = new Scene();
            _sceneList[7].SetDefaultScene8();

            _sceneList[8] = new Scene();
            _sceneList[8].SetDefaultScene9();

        }

        private void InitForm()
        {
            this.editorPic.MouseWheel += new System.Windows.Forms.MouseEventHandler(OnMouseWheel);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(OnMouseWheel);
            this.listViewObjects.MouseWheel += new System.Windows.Forms.MouseEventHandler(OnMouseWheel);

            this.Size = new Size(1160, 620);

            _graphics = this.editorPic.CreateGraphics();
            _editorHelper = new EditorHelper();

            // nastaveni rozliseni velikosti pro obrazek
            // posledni musi byt nulova velikost - pro volbu vlastniho nastaveni rozliseni
            _pictureSize = new Size[]{
                new Size(320,240),
                new Size(512,384),
                new Size(640,480),
                new Size(854,480), //856x480
                new Size(1024,768),
                new Size(1280,720),
                new Size()
            };

            this.numericRatio.Value = (decimal)_editorHelper.Meritko;

            this.radioXY.Checked = true;

            // panely pro editor
            this.panelLight.Location = _panelEditorLocation;        // nastaveni pozic
            this.panelSphere.Location = _panelEditorLocation;       // nastaveni pozic
            this.panelCamera.Location = _panelEditorLocation;       // nastaveni pozic
            this.panelRovina.Location = _panelEditorLocation;       // nastaveni pozic
            this.panelBox.Location = _panelEditorLocation;       // nastaveni pozic
            this.panelCylindr.Location = _panelEditorLocation;       // nastaveni pozic

            SetPanelsVisibility(false);                             // nastaveni neviditelnosti


            // groups pro ListView
            this.listViewObjects.Groups.Add(new ListViewGroup("Camera", HorizontalAlignment.Left));
            this.listViewObjects.Groups.Add(new ListViewGroup("Lights", HorizontalAlignment.Left));
            this.listViewObjects.Groups.Add(new ListViewGroup("Spheres", HorizontalAlignment.Left));
            this.listViewObjects.Groups.Add(new ListViewGroup("Planes", HorizontalAlignment.Left));
            this.listViewObjects.Groups.Add(new ListViewGroup("Cubes", HorizontalAlignment.Left));
            this.listViewObjects.Groups.Add(new ListViewGroup("Cylinders", HorizontalAlignment.Left));


            this.comboResolution.DataSource = _pictureSize;
            this.comboResolution.SelectedIndex = 0;         // nastaveni prvni polozky

            // nastaveni dat pro comboScene - vlozime sceny:
            this.comboScenes.DataSource = _sceneList;
            this.comboScenes.SelectedIndex = 0;

            FillListView();

            DrawBasicEditor(_graphics);
        }

        /// <summary>
        /// vrati rozliseni z formulare. 
        /// pripadne nastavi i spravne rozliseni: sirka delena 4, vyska delena 2
        /// </summary>
        /// <param name="size">vysledne rozliseni</param>
        /// <returns>true, kdyz rozliseni je korektni pro obrazek</returns>
        private bool GetImageSize(out Size size)
        {
            size = new Size();
            try
            {
                // vlastni rozliseni
                if (this.comboResolution.SelectedIndex == this.comboResolution.Items.Count - 1)
                {
                    size = new Size(Int32.Parse(this.txbResX.Text), Int32.Parse(this.txbResY.Text));
                }
                else
                {
                    size = (Size)this.comboResolution.SelectedItem;
                }

            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Enter image resolution", "Could not start rendering", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }
            catch (FormatException)
            {
                MessageBox.Show("Incorrect image resolution", "Could not start rendering", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }
            catch (OverflowException)
            {
                MessageBox.Show("Image resolution is too high", "Could not start rendering", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }
            catch (Exception)
            {
                MessageBox.Show("Incorrect image resolution", "Could not start rendering", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (size.Width < Renderer.MinSize.Width || size.Height < Renderer.MinSize.Height)
            {
                MessageBox.Show("Image resolution is too low", "Could not start rendering", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }

            if (size.Width > Renderer.MaxSize.Width || size.Height > Renderer.MaxSize.Height)
            {
                MessageBox.Show("Image resolution is too high", "Could not start rendering", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }

            return true;
        }
        /// <summary>
        /// Tlacitko pro vykresleni sceny
        /// nejprve zjisti pozadovane rozliseni pro obrazek a pak spusti novy formular,
        /// ve kterem se zacne provadet vykreslovani sceny
        /// </summary>
        private void btnDraw_Click(object sender, EventArgs e)
        {
            Size size;      // rozliseni vykreslovaneho obrazku
            if (!GetImageSize(out size))
                return;

            DrawingBoard form = new DrawingBoard();
            form.Size = new Size(size.Width + 10, size.Height + 60);

            int recursion = (int)numericRecurs.Value;
            form.Set(new RayTracerLib.RayTracing(_rayTracer), size.Width, size.Height, recursion, checkAntialias.Checked);
            form.Show();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {        }



        private void FillPlaceables()
        {
            _editorHelper.Placeables.Clear();
            foreach (DefaultShape obj in _rayTracer.RScene.SceneObjects)
            {
                //_placeables.Add(new Placeable(obj,obj.
            }

        }
        #region ListView "listViewObjects"

        /// <summary>
        /// Vyplni ListView objekty ze sceny
        /// </summary>
        private void FillListView()
        {
            SetPanelsVisibility(false);
            FillListView(-1);
        }
        /// <summary>
        /// Vyplni ListView objekty ze sceny a vybere prvek o zadanem indexu
        /// <param name="selectedIndex">index vybraneho prvku. -1 pro zadny vybrany</param>
        /// </summary>
        private void FillListView(int selectedIndex)
        {
            this.listViewObjects.Items.Clear();

            AddObject2ListViewItem(_rayTracer.RCamera);

            foreach (Light l in _rayTracer.RScene.Lights)
            {
                AddObject2ListViewItem(l);
            }

            foreach (DefaultShape obj in _rayTracer.RScene.SceneObjects)
            {
                if (obj.GetType() == typeof(Sphere))
                {
                    AddObject2ListViewItem((Sphere)obj);
                }
                if (obj.GetType() == typeof(Plane))
                {
                    AddObject2ListViewItem((Plane)obj);
                }
                if (obj.GetType() == typeof(Box))
                {
                    AddObject2ListViewItem((Box)obj);
                }
                if (obj.GetType() == typeof(Cube))
                {
                    AddObject2ListViewItem((Cube)obj);
                }
                if (obj.GetType() == typeof(Cylinder))
                {
                    AddObject2ListViewItem((Cylinder)obj);
                }
            }
            if (selectedIndex < 0 || this.listViewObjects.Items.Count < selectedIndex)
                return;

            this.listViewObjects.Items[selectedIndex].Selected = true;
        }

        /// <summary>
        /// Udalost - Vybrani polozky v ListViewu "listViewObjects"
        /// </summary>
        private void OnSelectedListObj(object sender, EventArgs e)
        {
            ListView listView = sender as ListView;
            //int i = listView.SelectedIndices[0];

            if (listView.SelectedItems.Count < 1)
            {
                SetPanelsVisibility(false);
                _editorHelper.PlaceObjSelected = null;
                Redraw();
                return;
            }

            ListViewItem item = listView.SelectedItems[0];

            if (item.Tag.GetType() == typeof(Light))
            {
                SetPanelVisible(panelLight);
                Light l = (Light)item.Tag;
                this.ShowLight(l);
                _editorHelper.PlaceObjSelected = new Placeable(l, Color.Firebrick, 0, 0, _editorHelper);
            }

            else if (item.Tag.GetType() == typeof(Sphere))
            {
                SetPanelVisible(panelSphere);
                Sphere sph = (Sphere)item.Tag;
                this.ShowSphere(sph);
                _editorHelper.PlaceObjSelected = new Placeable(sph, Color.Firebrick, 0, 0, _editorHelper);
            }

            else if (item.Tag.GetType() == typeof(Camera))
            {
                SetPanelVisible(panelCamera);
                Camera cam = (Camera)item.Tag;
                this.ShowCamera(cam);
                _editorHelper.PlaceObjSelected = new Placeable(cam, Color.Firebrick, 0, 0, _editorHelper);
            }

            else if (item.Tag.GetType() == typeof(Plane))
            {
                SetPanelVisible(panelRovina);
                Plane p = (Plane)item.Tag;
                ShowPlane(p);
                _editorHelper.PlaceObjSelected = new Placeable(p, Color.Firebrick, 0, 0, _editorHelper);
            }

            else if (item.Tag.GetType() == typeof(Box))
            {
                SetPanelVisible(panelBox);
                Box b = (Box)item.Tag;
                ShowBox(b);
                _editorHelper.PlaceObjSelected = new Placeable(b, Color.Firebrick, 0, 0, _editorHelper);
            }
            else if (item.Tag.GetType() == typeof(Cube))
            {
                SetPanelVisible(panelBox);
                Cube c = (Cube)item.Tag;
                ShowCube(c);
                _editorHelper.PlaceObjSelected = new Placeable(c, Color.Firebrick, 0, 0, _editorHelper);
            }
            else if (item.Tag.GetType() == typeof(Cylinder))
            {
                SetPanelVisible(panelCylindr);
                Cylinder c = (Cylinder)item.Tag;
                ShowCylinder(c);
                _editorHelper.PlaceObjSelected = new Placeable(c, Color.Firebrick, 0, 0, _editorHelper);
            }
            else
                return;

            Redraw();

        }

        /// <summary>
        /// prida novy objekt od ListView "listViewObjects"
        /// </summary>
        /// <param name="obj"></param>
        private void AddObject2ListViewItem(object obj)
        {
            ListViewItem item = new ListViewItem();
            ListViewItem.ListViewSubItem coord = new ListViewItem.ListViewSubItem();
            ListViewItem.ListViewSubItem color = new ListViewItem.ListViewSubItem();

            if (obj.GetType() == typeof(Light))
            {
                Light l = (Light)obj;
                item.Text = l.ToString();
                coord.Text = l.Coord.ToString();
                color.Text = l.Color.ToString();
                item.Group = listViewObjects.Groups[1];
                item.Checked = l.IsActive;
            }

            else if (obj.GetType() == typeof(Sphere))
            {
                Sphere sph = (Sphere)obj;
                item.Text = sph.ToString();
                coord.Text = sph.Origin.ToString();
                color.Text = sph.Material.Color.ToString();
                item.Group = listViewObjects.Groups[2];
                item.Checked = sph.IsActive;
            }

            else if (obj.GetType() == typeof(Camera))
            {
                Camera cam = (Camera)obj;
                item.Text = cam.ToString();
                coord.Text = cam.Source.ToString();
                color.Text = "";
                item.Group = listViewObjects.Groups[0];
                item.Checked = true;
            }
            else if (obj.GetType() == typeof(Plane))
            {
                Plane plane = (Plane)obj;
                item.Text = plane.ToString();
                coord.Text = plane.Normal.ToString();
                color.Text = plane.Material.Color.ToString();
                item.Group = listViewObjects.Groups[3];
                item.Checked = plane.IsActive;
            }
            else if (obj.GetType() == typeof(Box))
            {
                Box b = (Box)obj;
                item.Text = b.ToString();
                coord.Text = b.Center.ToString();
                color.Text = b.Material.Color.ToString();
                item.Group = listViewObjects.Groups[4];
                item.Checked = b.IsActive;
            }
            else if (obj.GetType() == typeof(Cube))
            {
                Cube c = (Cube)obj;
                item.Text = c.ToString();
                coord.Text = c.Center.ToString();
                color.Text = c.Material.Color.ToString();
                item.Group = listViewObjects.Groups[4];
                item.Checked = c.IsActive;
            }
            else if (obj.GetType() == typeof(Cylinder))
            {
                Cylinder c = (Cylinder)obj;
                item.Text = c.ToString();
                coord.Text = c.Center.ToString();
                color.Text = c.Material.Color.ToString();
                item.Group = listViewObjects.Groups[5];
                item.Checked = c.IsActive;
            }

            item.SubItems.Add(coord);
            item.SubItems.Add(color);
            item.Tag = obj;
            this.listViewObjects.Items.Add(item);
        }

        private void btnListViewDeleteFrom_Click(object sender, EventArgs e)
        {
            if (this.listViewObjects.SelectedItems.Count < 1)
                return;

            ListViewItem item = this.listViewObjects.SelectedItems[0];

            // vymazeme kouli ze sceny
            if (item.Tag.GetType() == typeof(Sphere))
            {
                Sphere sph = (Sphere)item.Tag;
                _rayTracer.RScene.SceneObjects.Remove(sph);

            }

            // vymazeme svetlo ze sceny
            else if (item.Tag.GetType() == typeof(Light))
            {
                Light l = (Light)item.Tag;
                _rayTracer.RScene.Lights.Remove(l);
            }

            // vymazani roviny ze sceny
            else if (item.Tag.GetType() == typeof(Plane))
            {
                Plane p = (Plane)item.Tag;
                _rayTracer.RScene.SceneObjects.Remove(p);
            }

            // vymazani krychle ze sceny
            else if (item.Tag.GetType() == typeof(Box))
            {
                Box b = (Box)item.Tag;
                _rayTracer.RScene.SceneObjects.Remove(b);
            }

                // vymazani krychle ze sceny
            else if (item.Tag.GetType() == typeof(Cube))
            {
                Cube c = (Cube)item.Tag;
                _rayTracer.RScene.SceneObjects.Remove(c);
            }
            else if (item.Tag.GetType() == typeof(Cylinder))
            {
                Cylinder cyl = (Cylinder)item.Tag;
                _rayTracer.RScene.SceneObjects.Remove(cyl);
            }

            FillListView();
            Redraw();
        }

        private void btnListViewAddTo_Click(object sender, EventArgs e)
        {
            ListViewObject form = new ListViewObject();
            form.Set(_rayTracer, null, null, null, null);
            DialogResult res = form.ShowDialog();
            if (res == DialogResult.OK)
            {
                FillListView();
                Redraw();
            }
        }

        #region Light

        /// <summary>
        /// Zobrazi svetlo v editoru formulare
        /// </summary>
        /// <param name="l">dane svetlo</param>
        private void ShowLight(Light l)
        {
            this.panelLight.Visible = true;
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

        /// <summary>
        /// modifikovane existujiciho svetla z formulare do sceny
        /// </summary>
        private void buttonLightSave_Click(object sender, EventArgs e)
        {

            if (this.listViewObjects.SelectedItems.Count < 1)
                return;

            int selectedIndex = this.listViewObjects.SelectedIndices[0];

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

            ListViewItem item = this.listViewObjects.SelectedItems[0];

            if (item.Tag.GetType() == typeof(Light))
            {
                Light l = (Light)item.Tag;
                l.Coord = lightCoord;
                l.Color = lightColor;

                l.IsSoftLight = this.checkBoxLightIsSoft.Checked;
                bool isSinglePass = this.radioSinglePass.Checked;
                if (l.IsSoftLight)
                    l.SetSoftLights(numSoftLights, epsSoftLights, isSinglePass);
            }

            FillListView(selectedIndex);
            this.Redraw();
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


            Light newLight = new Light(lightCoord, lightColor);

            _rayTracer.RScene.AddLight(newLight);

            AddObject2ListViewItem(newLight);
            this.Redraw();
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

                //this.btnLighColor.BackColor = colorDialog.Color;

                this.numericSvetloR.Value = (decimal)col.R;
                this.numericSvetloG.Value = (decimal)col.G;
                this.numericSvetloB.Value = (decimal)col.B;
            }
        }

        #endregion Light

        #region Sphere

        /// <summary>
        /// zobrazi kouli v editoru formulare
        /// </summary>
        /// <param name="sph"></param>
        private void ShowSphere(Sphere sph)
        {
            this.panelSphere.Visible = true;
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

        /// <summary>
        /// modifikuje kouli z formulare do sceny
        /// </summary>
        private void buttonSpereSave_Click(object sender, EventArgs e)
        {
            if (this.listViewObjects.SelectedItems.Count < 1)
                return;

            int selectedIndex = this.listViewObjects.SelectedIndices[0];

            Vektor origin = new Vektor(
                (double)this.numericKouleX.Value,
                (double)this.numericKouleY.Value,
                (double)this.numericKouleZ.Value);

            double r = (double)this.numericKouleRadius.Value;

            ListViewItem item = this.listViewObjects.SelectedItems[0];

            if (item.Tag.GetType() == typeof(Sphere))
            {
                Sphere sph = (Sphere)item.Tag;
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
            }

            FillListView(selectedIndex);
            this.Redraw();
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

            _rayTracer.RScene.SceneObjects.Add(sph);

            AddObject2ListViewItem(sph);
            this.Redraw();
        }

        /// <summary>
        /// vybrani barvy pro kouli
        /// </summary>
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

        #endregion Sphere

        #region Camera


        /// <summary>
        /// ulozeni kamery z formulare
        /// </summary>
        private void buttonKameraSave_Click(object sender, EventArgs e)
        {
            if (this.listViewObjects.SelectedItems.Count < 1)
                return;

            int selectedIndex = this.listViewObjects.SelectedIndices[0];

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

            ListViewItem item = this.listViewObjects.SelectedItems[0];

            if (item.Tag.GetType() == typeof(Camera))
            {
                Camera cam = (Camera)item.Tag;
                cam.Source = stred;
                cam.SetNormAndUp(dir, up);
            }

            FillListView(selectedIndex);
        }

        private void ShowCamera(Camera cam)
        {
            this.panelCamera.Visible = true;
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

        #endregion


        #endregion ListView


        #region ShowPanel - viditelnost editoru ve formulari

        /// <summary>
        /// zneviditelni vsechny panely k editoru
        /// panely jsou pro: kouli, svetlo
        /// </summary>
        /// <param name="visible">na jakou hodnotu je nastavime - true -> budou vsechny videt</param>
        private void SetPanelsVisibility(bool visible)
        {
            this.panelLight.Visible = visible;
            this.panelSphere.Visible = visible;
            this.panelCamera.Visible = visible;
            this.panelRovina.Visible = visible;
            this.panelBox.Visible = visible;
            this.panelCylindr.Visible = visible;
        }

        private void SetPanelVisible(Panel panel)
        {
            SetPanelsVisibility(false);
            panel.Visible = true;
        }

        #endregion ShowPanel


        #region EditorPicture

        private void Redraw()
        {
            //Graphics g = this.editorPic.CreateGraphics();
            this.DrawBasicEditor(_graphics);
        }

        /// <summary>
        /// Zakladni vykresleni editoru
        /// </summary>
        /// <param name="g"></param>
        private void DrawBasicEditor(Graphics gr)
        {
            _editorHelper.Placeables.Clear();

            _editorBmp = new Bitmap(editorPic.Width, editorPic.Height);
            gr = Graphics.FromImage(_editorBmp);
            //gr.Clear(Color.White);
            

            // zakladni zobrazeni editoru
            _editorHelper.Center = new Point(this.editorPic.Width / 2, this.editorPic.Height / 2);
            Font fontAxis = new Font(Font.FontFamily, 10, FontStyle.Italic);

            Point horizBegin;
            Point horizEnd;

            Point vertBegin;
            Point vertEnd;
            if (_editorHelper.Axes == EditorAxesType.XY)
            {
                // OSA X
                horizBegin = new Point(0, _editorHelper.Center.Y);
                horizEnd = new Point(this.editorPic.Width, _editorHelper.Center.Y);
                gr.DrawString("x", fontAxis, Brushes.Black, new Point((int)(this.editorPic.Width - this.editorPic.Width / 15), _editorHelper.Center.Y));

                // OSA Y
                vertBegin = new Point(_editorHelper.Center.X, 0);
                vertEnd = new Point(_editorHelper.Center.X, this.editorPic.Height);
                gr.DrawString("y", fontAxis, Brushes.Black, new Point(_editorHelper.Center.X + 5, (int)(this.editorPic.Height / 35)));
            }
            else if (_editorHelper.Axes == EditorAxesType.ZY)
            {
                // OSA Z
                horizBegin = new Point(0, _editorHelper.Center.Y);
                horizEnd = new Point(this.editorPic.Width, _editorHelper.Center.Y);
                gr.DrawString("z", fontAxis, Brushes.Black, new Point((int)(this.editorPic.Width - this.editorPic.Width / 15), _editorHelper.Center.Y));

                // OSA Y
                vertBegin = new Point(_editorHelper.Center.X, 0);
                vertEnd = new Point(_editorHelper.Center.X, this.editorPic.Height);
                gr.DrawString("y", fontAxis, Brushes.Black, new Point(_editorHelper.Center.X + 5, (int)(this.editorPic.Height / 35)));
            }
                // osa XZ
            else
            {

                // OSA X
                horizBegin = new Point(0, _editorHelper.Center.Y);
                horizEnd = new Point(this.editorPic.Width, _editorHelper.Center.Y);
                gr.DrawString("x", fontAxis, Brushes.Black, new Point((int)(this.editorPic.Width - this.editorPic.Width / 15), _editorHelper.Center.Y));

                // OSA Z
                vertBegin = new Point(_editorHelper.Center.X, 0);
                vertEnd = new Point(_editorHelper.Center.X, this.editorPic.Height);
                gr.DrawString("-z", fontAxis, Brushes.Black, new Point(_editorHelper.Center.X + 5, (int)(this.editorPic.Height / 35)));
            }


            Pen pen = new Pen(System.Drawing.Color.Black);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;

            //Graphics gr = Graphics.FromImage(_editorBmp);

            // osy:
            gr.DrawLine(pen, horizBegin, horizEnd);
            gr.DrawLine(pen, vertBegin, vertEnd);

            
            // vykreslime objekty ze sceny:

            foreach (RayTracerLib.DefaultShape obj in this._rayTracer.RScene.SceneObjects)
            {
                // vykresli kruh
                if (obj.GetType() == typeof(Sphere))
                {
                    Sphere sph = obj as Sphere;
                    if (!sph.IsActive)
                        continue;
                    System.Drawing.Color col = sph.Material.Color.SystemColor();
                    double r = sph.R * _editorHelper.Meritko;
                    Font font = new Font(Font.FontFamily, 7, FontStyle.Bold);
                    Pen p = new Pen(Color.Black);
                    Brush b = new SolidBrush(col);

                    Point loc;

                    // OSY x-y:
                    if (_editorHelper.Axes == EditorAxesType.XY)
                    {
                        loc = new Point((int)sph.Origin.X * _editorHelper.Meritko, (int)sph.Origin.Y * _editorHelper.Meritko);
                    }
                    // OSY z-y:
                    else if (_editorHelper.Axes == EditorAxesType.ZY)
                    {
                        loc = new Point((int)sph.Origin.Z * _editorHelper.Meritko, (int)sph.Origin.Y * _editorHelper.Meritko);
                    }
                    else
                    {
                        loc = new Point((int)sph.Origin.X * _editorHelper.Meritko, (int)sph.Origin.Z * _editorHelper.Meritko);
                    }

                    loc.Offset(_editorHelper.Center);
                    _editorHelper.Placeables.Add(new Placeable(sph, col, loc.X, loc.Y, _editorHelper));

                    

                    gr.FillEllipse(b, (float)(loc.X - r), (float)(loc.Y - r), (float)(r + r), (float)(r + r));
                    gr.DrawEllipse(p, (float)(loc.X - r), (float)(loc.Y - r), (float)(r + r), (float)(r + r));
                    gr.DrawString("Sphere", font, Brushes.Black, new Point((int)(loc.X - r / 2), loc.Y));


                    // vykresleni ruky, je-li koule vybrana
                    if (_editorHelper.PlaceObjSelected != null && _editorHelper.PlaceObjSelected.Object.GetType() == typeof(Sphere))
                    {
                        Sphere selected = (Sphere)_editorHelper.PlaceObjSelected.Object;
                        if (selected == sph)
                        {
                            gr.DrawImage(Properties.Resources.hand1,
                                                new Point(loc.X - (int)(Properties.Resources.hand1.Width / 2), loc.Y - (int)(Properties.Resources.hand1.Height / 2)));
                        }
                    }

                }

                    // vykresleni krychle
                else if (obj.GetType() == typeof(Box))
                {
                    Box b = obj as Box;
                    if (!b.IsActive)
                        continue;
                    System.Drawing.Color col = b.Material.Color.SystemColor();
                    double size = b.Size * _editorHelper.Meritko;
                    double sizePul = size / 2;
                    Pen p = new Pen(Color.Black);
                    Brush br = new SolidBrush(col);
                    Font font = new Font(Font.FontFamily, 7, FontStyle.Bold);

                    Point loc;

                    if (_editorHelper.Axes == EditorAxesType.XY)
                    {
                        loc = new Point((int)b.Center.X * _editorHelper.Meritko, (int)b.Center.Y * _editorHelper.Meritko);
                    }
                    // OSY z-y:
                    else if (_editorHelper.Axes == EditorAxesType.ZY)
                    {
                        loc = new Point((int)b.Center.Z * _editorHelper.Meritko, (int)b.Center.Y * _editorHelper.Meritko);
                    }
                    else
                    {
                        loc = new Point((int)b.Center.X * _editorHelper.Meritko, (int)b.Center.Z * _editorHelper.Meritko);
                    }

                    loc.Offset(_editorHelper.Center);
                    _editorHelper.Placeables.Add(new Placeable(b, col, loc.X, loc.Y, _editorHelper));

                    gr.FillRectangle(br, (float)(loc.X - sizePul), (float)(loc.Y - sizePul), (float)size, (float)size);
                    gr.DrawRectangle(p, (float)(loc.X - sizePul), (float)(loc.Y - sizePul), (float)size, (float)size);
                    gr.DrawString("Box", font, Brushes.Black, new Point((int)(loc.X - sizePul / 2), loc.Y));

                    // vykresleni ruky, je-li krychle vybrana
                    if (_editorHelper.PlaceObjSelected != null && _editorHelper.PlaceObjSelected.Object.GetType() == typeof(Box))
                    {
                        Box selected = (Box)_editorHelper.PlaceObjSelected.Object;
                        if (selected == b)
                        {
                            gr.DrawImage(Properties.Resources.hand1,
                                                new Point(loc.X - (int)(Properties.Resources.hand1.Width / 2), loc.Y - (int)(Properties.Resources.hand1.Height / 2)));
                        }
                    }
                }
                // vykresleni krychle
                else if (obj.GetType() == typeof(Cube))
                {
                    Cube c = obj as Cube;
                    if (!c.IsActive)
                        continue;
                    System.Drawing.Color col = c.Material.Color.SystemColor();
                    double size = c.Size * _editorHelper.Meritko;
                    double sizePul = size / 2;
                    Pen p = new Pen(Color.Black);
                    Brush br = new SolidBrush(col);
                    Font font = new Font(Font.FontFamily, 7, FontStyle.Bold);

                    Point loc;

                    if (_editorHelper.Axes == EditorAxesType.XY)
                    {
                        loc = new Point((int)c.Center.X * _editorHelper.Meritko, (int)c.Center.Y * _editorHelper.Meritko);
                    }
                    // OSY z-y:
                    else if (_editorHelper.Axes == EditorAxesType.ZY)
                    {
                        loc = new Point((int)c.Center.Z * _editorHelper.Meritko, (int)c.Center.Y * _editorHelper.Meritko);
                    }
                    else
                    {
                        loc = new Point((int)c.Center.X * _editorHelper.Meritko, (int)c.Center.Z * _editorHelper.Meritko);
                    }

                    loc.Offset(_editorHelper.Center);
                    _editorHelper.Placeables.Add(new Placeable(c, col, loc.X, loc.Y, _editorHelper));

                    
                    gr.FillRectangle(br, (float)(loc.X - sizePul), (float)(loc.Y - sizePul), (float)size, (float)size);
                    gr.DrawRectangle(p, (float)(loc.X - sizePul), (float)(loc.Y - sizePul), (float)size, (float)size);
                    gr.DrawString("Cube", font, Brushes.Black, new Point((int)(loc.X - sizePul / 2), loc.Y));

                    // vykresleni ruky, je-li krychle vybrana
                    if (_editorHelper.PlaceObjSelected != null && _editorHelper.PlaceObjSelected.Object.GetType() == typeof(Cube))
                    {
                        Cube selected = (Cube)_editorHelper.PlaceObjSelected.Object;
                        if (selected == c)
                        {
                            gr.DrawImage(Properties.Resources.hand1,
                                                new Point(loc.X - (int)(Properties.Resources.hand1.Width / 2), loc.Y - (int)(Properties.Resources.hand1.Height / 2)));
                        }
                    }
                }

                    // vykresleni valce
                else if (obj.GetType() == typeof(Cylinder))
                {
                    Cylinder cyl = (Cylinder)obj;
                    if (!cyl.IsActive)
                        continue;
                    Color col = cyl.Material.Color.SystemColor();
                    Pen p = new Pen(col);
                    Brush br = new SolidBrush(col);
                    double heigh = cyl.Height * _editorHelper.Meritko;
                    double rad = cyl.Rad * _editorHelper.Meritko;
                    
                    Point[] polyPoints;

                    Font font = new Font(Font.FontFamily, 7, FontStyle.Bold);

                    // hlavni stred cylindru
                    Point centerC;
                    Point lowerCenter;
                    Point upperCenter;

                    if (_editorHelper.Axes == EditorAxesType.XY)
                    {
                        centerC = new Point((int)(cyl.Center.X * _editorHelper.Meritko), (int)(cyl.Center.Y * _editorHelper.Meritko));
                        centerC.Offset(_editorHelper.Center);

                        Vektor DirNom = new Vektor(cyl.Dir);
                        DirNom.Normalize();
                        Vektor lowerC = new Vektor(cyl.Center - DirNom * (cyl.Height / 2));
                        Vektor upperC = new Vektor(cyl.Center + DirNom * (cyl.Height / 2));


                        lowerCenter = new Point((int)(lowerC.X * _editorHelper.Meritko), (int)(lowerC.Y * _editorHelper.Meritko));
                        lowerCenter.Offset(_editorHelper.Center);

                        upperCenter = new Point((int)(upperC.X * _editorHelper.Meritko), (int)(upperC.Y * _editorHelper.Meritko));
                        upperCenter.Offset(_editorHelper.Center);

                        Vektor kolmyNaDir = new Vektor(-DirNom.Y, DirNom.X, DirNom.Z);


                        Vektor p1C = lowerC + kolmyNaDir * cyl.Rad;
                        Vektor p2C = lowerC - kolmyNaDir * cyl.Rad;
                        Vektor p3C = upperC + kolmyNaDir * cyl.Rad;
                        Vektor p4C = upperC - kolmyNaDir * cyl.Rad;

                        polyPoints = new Point[4];
                        polyPoints[0] = new Point((int)(p1C.X * _editorHelper.Meritko), (int)(p1C.Y * _editorHelper.Meritko));
                        polyPoints[1] = new Point((int)(p2C.X * _editorHelper.Meritko), (int)(p2C.Y * _editorHelper.Meritko));
                        polyPoints[2] = new Point((int)(p4C.X * _editorHelper.Meritko), (int)(p4C.Y * _editorHelper.Meritko));
                        polyPoints[3] = new Point((int)(p3C.X * _editorHelper.Meritko), (int)(p3C.Y * _editorHelper.Meritko));
                        polyPoints[0].Offset(_editorHelper.Center);
                        polyPoints[1].Offset(_editorHelper.Center);
                        polyPoints[2].Offset(_editorHelper.Center);
                        polyPoints[3].Offset(_editorHelper.Center);
                    }
                    // OSY z-y:
                    else if (_editorHelper.Axes == EditorAxesType.ZY)
                    {
                        centerC = new Point((int)(cyl.Center.Z * _editorHelper.Meritko), (int)(cyl.Center.Y * _editorHelper.Meritko));
                        centerC.Offset(_editorHelper.Center);

                        Vektor DirNom = new Vektor(cyl.Dir);
                        DirNom.Normalize();
                        Vektor lowerC = new Vektor(cyl.Center - DirNom * (cyl.Height / 2));
                        Vektor upperC = new Vektor(cyl.Center + DirNom * (cyl.Height / 2));


                        lowerCenter = new Point((int)(lowerC.Z * _editorHelper.Meritko), (int)(lowerC.Y * _editorHelper.Meritko));
                        lowerCenter.Offset(_editorHelper.Center);

                        upperCenter = new Point((int)(upperC.Z * _editorHelper.Meritko), (int)(upperC.Y * _editorHelper.Meritko));
                        upperCenter.Offset(_editorHelper.Center);

                        // primka kolma na smer valce
                        Vektor kolmyNaDir = new Vektor(DirNom.X, -DirNom.Z, DirNom.Y);


                        Vektor p1C = lowerC + kolmyNaDir * cyl.Rad;
                        Vektor p2C = lowerC - kolmyNaDir * cyl.Rad;
                        Vektor p3C = upperC + kolmyNaDir * cyl.Rad;
                        Vektor p4C = upperC - kolmyNaDir * cyl.Rad;

                        polyPoints = new Point[4];
                        polyPoints[0] = new Point((int)(p1C.Z * _editorHelper.Meritko), (int)(p1C.Y * _editorHelper.Meritko));
                        polyPoints[1] = new Point((int)(p2C.Z * _editorHelper.Meritko), (int)(p2C.Y * _editorHelper.Meritko));
                        polyPoints[2] = new Point((int)(p4C.Z * _editorHelper.Meritko), (int)(p4C.Y * _editorHelper.Meritko));
                        polyPoints[3] = new Point((int)(p3C.Z * _editorHelper.Meritko), (int)(p3C.Y * _editorHelper.Meritko));
                        polyPoints[0].Offset(_editorHelper.Center);
                        polyPoints[1].Offset(_editorHelper.Center);
                        polyPoints[2].Offset(_editorHelper.Center);
                        polyPoints[3].Offset(_editorHelper.Center);
                    }
                        // osy X--Z:
                    else
                    {
                        centerC = new Point((int)(cyl.Center.X * _editorHelper.Meritko), (int)(cyl.Center.Z * _editorHelper.Meritko));
                        centerC.Offset(_editorHelper.Center);

                        Vektor DirNom = new Vektor(cyl.Dir);
                        DirNom.Normalize();
                        Vektor lowerC = new Vektor(cyl.Center - DirNom * (cyl.Height / 2));
                        Vektor upperC = new Vektor(cyl.Center + DirNom * (cyl.Height / 2));


                        lowerCenter = new Point((int)(lowerC.X * _editorHelper.Meritko), (int)(lowerC.Z * _editorHelper.Meritko));
                        lowerCenter.Offset(_editorHelper.Center);

                        upperCenter = new Point((int)(upperC.X * _editorHelper.Meritko), (int)(upperC.Z * _editorHelper.Meritko));
                        upperCenter.Offset(_editorHelper.Center);

                        // primka kolma na smer valce
                        //Vektor kolmyNaDir = new Vektor(DirNom.X, -DirNom.Z, DirNom.Y);
                        Vektor kolmyNaDir = new Vektor(-DirNom.Z, DirNom.Y, DirNom.X);


                        Vektor p1C = lowerC + kolmyNaDir * cyl.Rad;
                        Vektor p2C = lowerC - kolmyNaDir * cyl.Rad;
                        Vektor p3C = upperC + kolmyNaDir * cyl.Rad;
                        Vektor p4C = upperC - kolmyNaDir * cyl.Rad;

                        polyPoints = new Point[4];
                        polyPoints[0] = new Point((int)(p1C.X * _editorHelper.Meritko), (int)(p1C.Z * _editorHelper.Meritko));
                        polyPoints[1] = new Point((int)(p2C.X * _editorHelper.Meritko), (int)(p2C.Z * _editorHelper.Meritko));
                        polyPoints[2] = new Point((int)(p4C.X * _editorHelper.Meritko), (int)(p4C.Z * _editorHelper.Meritko));
                        polyPoints[3] = new Point((int)(p3C.X * _editorHelper.Meritko), (int)(p3C.Z * _editorHelper.Meritko));
                        polyPoints[0].Offset(_editorHelper.Center);
                        polyPoints[1].Offset(_editorHelper.Center);
                        polyPoints[2].Offset(_editorHelper.Center);
                        polyPoints[3].Offset(_editorHelper.Center);
                    }
                    p = new Pen(Color.Black);
                    
                    gr.FillPolygon(br, polyPoints);
                    gr.DrawPolygon(p, polyPoints);

                    //gr.DrawEllipse(p, (float)(upperCenter.X - rad), (float)(upperCenter.Y - rad), (float)(rad + rad), (float)(rad + rad));
                    gr.FillEllipse(br, (float)(upperCenter.X - rad), (float)(upperCenter.Y - rad), (float)(rad + rad), (float)(rad + rad));

                    //gr.DrawEllipse(p, (float)(lowerCenter.X - rad), (float)(lowerCenter.Y - rad), (float)(rad + rad), (float)(rad + rad));
                    gr.FillEllipse(br, (float)(lowerCenter.X - rad), (float)(lowerCenter.Y - rad), (float)(rad + rad), (float)(rad + rad));

                    gr.DrawString("Cylinder", font, Brushes.Black, new Point((int)(centerC.X - rad / 2), centerC.Y));
                    _editorHelper.Placeables.Add(new Placeable(cyl, col,centerC.X,centerC.Y, _editorHelper));

                    // vykresleni ruky, je-li valec vybran
                    if (_editorHelper.PlaceObjSelected != null && _editorHelper.PlaceObjSelected.Object.GetType() == typeof(Cylinder))
                    {
                        Cylinder selected = (Cylinder)_editorHelper.PlaceObjSelected.Object;
                        if (selected == cyl)
                        {
                            gr.DrawImage(Properties.Resources.hand1,
                                                new Point(centerC.X - (int)(Properties.Resources.hand1.Width / 2), centerC.Y - (int)(Properties.Resources.hand1.Height / 2)));
                        }
                    }
                }
            }

            // svetla - zaskrtnuti checkboxu
            // zobrazime svetla ve scene:
            if (this.checkShowLights.Checked)
            {
                foreach (Light l in _rayTracer.RScene.Lights)
                {
                    if (!l.IsActive)
                        continue;

                    Point loc;

                    if (_editorHelper.Axes == EditorAxesType.XY)
                    {
                        loc = new Point((int)l.Coord.X * _editorHelper.Meritko, (int)l.Coord.Y * _editorHelper.Meritko);
                    }
                    else if (_editorHelper.Axes == EditorAxesType.ZY)
                    {
                        loc = new Point((int)l.Coord.Z * _editorHelper.Meritko, (int)l.Coord.Y * _editorHelper.Meritko);
                    }
                    else
                    {
                        loc = new Point((int)l.Coord.X * _editorHelper.Meritko, (int)l.Coord.Z * _editorHelper.Meritko);
                    }

                    loc.Offset(_editorHelper.Center);
                    gr.DrawImage(Properties.Resources.bulb_transp,
                        new Point(loc.X - (int)(Properties.Resources.bulb_transp.Width / 2), loc.Y - (int)(Properties.Resources.bulb_transp.Height / 2)));

                    _editorHelper.Placeables.Add(new Placeable(l, new Color(), loc.X, loc.Y, _editorHelper));

                    // vykresleni ruky, je-li svetlo vybrano
                    if (_editorHelper.PlaceObjSelected != null && _editorHelper.PlaceObjSelected.Object.GetType() == typeof(Light))
                    {
                        Light selected = (Light)_editorHelper.PlaceObjSelected.Object;
                        if (selected == l)
                        {
                            gr.DrawImage(Properties.Resources.hand1,
                                                new Point(loc.X - (int)(Properties.Resources.hand1.Width / 2), loc.Y - (int)(Properties.Resources.hand1.Height / 2)));
                        }
                    }
                }
            }

            // kamera - zaskrtnuti checkboxu
            if (this.checkShowCamera.Checked)
            {
                Point loc;
                Camera cam = _rayTracer.RCamera;

                Vektor arrowEnd = new Vektor(cam.Norm);
                arrowEnd.Normalize();
                Point arrowEndLoc = new Point();
                if (_editorHelper.Axes == EditorAxesType.XY)
                {
                    loc = new Point((int)(cam.Source.X * _editorHelper.Meritko), (int)(cam.Source.Y * _editorHelper.Meritko));
                    arrowEndLoc = new Point((int)(arrowEnd.X * 3 * _editorHelper.Meritko), (int)(arrowEnd.Y * 3 * _editorHelper.Meritko));
                }
                else if (_editorHelper.Axes == EditorAxesType.ZY)
                {
                    loc = new Point((int)(cam.Source.Z * _editorHelper.Meritko), (int)(cam.Source.Y * _editorHelper.Meritko));
                    arrowEndLoc = new Point((int)(arrowEnd.Z * 3 * _editorHelper.Meritko), (int)(arrowEnd.Y * 3 * _editorHelper.Meritko));
                }
                else
                {
                    loc = new Point((int)(cam.Source.X * _editorHelper.Meritko), (int)(cam.Source.Z * _editorHelper.Meritko));
                    arrowEndLoc = new Point((int)(arrowEnd.X * 3 * _editorHelper.Meritko), (int)(arrowEnd.Z * 3 * _editorHelper.Meritko));
                }

                loc.Offset(_editorHelper.Center);
                arrowEndLoc.Offset(loc);

                _editorHelper.Placeables.Add(new Placeable(cam, new Color(), loc.X, loc.Y, _editorHelper));

                Pen arrowPen = new Pen(Brushes.Firebrick, 5);
                arrowPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                gr.DrawLine(arrowPen, loc, arrowEndLoc);
                gr.DrawImage(Properties.Resources.camera,
                    new Point(loc.X - (int)(Properties.Resources.camera.Width / 2), loc.Y - (int)(Properties.Resources.camera.Height / 2)));

                arrowPen.Dispose();

                // vykresleni ruky, je-li kamera vybrana
                if (_editorHelper.PlaceObjSelected != null && _editorHelper.PlaceObjSelected.Object.GetType() == typeof(Camera))
                {
                    Camera selected = (Camera)_editorHelper.PlaceObjSelected.Object;
                    if (selected == cam)
                    {
                        gr.DrawImage(Properties.Resources.hand1,
                                            new Point(loc.X - (int)(Properties.Resources.hand1.Width / 2), loc.Y - (int)(Properties.Resources.hand1.Height / 2)));
                    }
                }
            }
            pen.Dispose();
            editorPic.Image = _editorBmp;
        }

        private void OnPaintEditorPic(object sender, PaintEventArgs e)
        {
            //this.Invalidate();
            //DrawBasicEditor(e.Graphics);
            //_graphics = e.Graphics;
           //this.editorPic.Image = _editorBmp;
            //e.Graphics.DrawImage(_editorBmp, 0, 0);
        }

        private void OnPaintForm(object sender, PaintEventArgs e)
        {
            //DrawBasicEditor(_graphics);
            //e.Graphics.DrawImage(_editorBmp, 0, 0);
            //this.editorPic.Invalidate();
            //this.editorPic.Image = _editorBmp;
        }

        private void OnClickEditorPic(object sender, EventArgs e)
        {
            
        }

        private void OnCheckLightsChange(object sender, EventArgs e)
        {
            this.Redraw();
        }

        #endregion EditorPicture


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
        #endregion

        private void OnMainFormClick(object sender, EventArgs e)
        {
            this.checkBack.Select();
        }

        #region Plane

        private void btnPlaneSave_Click(object sender, EventArgs e)
        {
            if (this.listViewObjects.SelectedItems.Count < 1)
                return;

            int selectedIndex = this.listViewObjects.SelectedIndices[0];

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

            if (checkBoxMaxZ.Checked)
                maxz = (double)numMaxZ.Value;


            ListViewItem item = this.listViewObjects.SelectedItems[0];

            if (item.Tag.GetType() == typeof(Plane))
            {
                Plane plane = (Plane)item.Tag;
                plane.Normal = norm;
                plane.D = d;

                plane.MinX = minx;
                plane.MaxX = maxx;
                plane.MinY = miny;
                plane.MaxY = maxy;
                plane.MinZ = minz;
                plane.MaxZ = maxz;


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

                plane.Material = mat;

                plane.CreateBoundVektors();
            }

            FillListView(selectedIndex);
        }

        private void btnPlaneColor_Click(object sender, EventArgs e)
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

        private void ShowPlane(Plane pl)
        {
            this.panelRovina.Visible = true;
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

        #endregion

        #region Cylinder

        private void btnCylSave_Click(object sender, EventArgs e)
        {
            if (this.listViewObjects.SelectedItems.Count < 1)
                return;

            int selectedIndex = this.listViewObjects.SelectedIndices[0];

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

            ListViewItem item = this.listViewObjects.SelectedItems[0];

            if (item.Tag.GetType() == typeof(Cylinder))
            {
                Cylinder cyl = (Cylinder)item.Tag;
                cyl.SetValues(center, dir, r, h);

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
            }

            FillListView(selectedIndex);
            this.Redraw();
        }

        private void ShowCylinder(Cylinder c)
        {
            this.numericCylCentX.Value = (decimal)MyMath.Clamp(c.Center.X, -100, 100);
            this.numericCylCentY.Value = (decimal)MyMath.Clamp(c.Center.Y, -100, 100);
            this.numericCylCentZ.Value = (decimal)MyMath.Clamp(c.Center.Z, -100, 100);

            this.numericCylDirX.Value = (decimal)MyMath.Clamp(c.Dir.X, -100, 100);
            this.numericCylDirY.Value = (decimal)MyMath.Clamp(c.Dir.Y, -100, 100);
            this.numericCylDirZ.Value = (decimal)MyMath.Clamp(c.Dir.Z, -100, 100);

            this.numericCylH.Value = (decimal)c.Height;
            this.numericCylR.Value = (decimal)c.Rad;

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
        #endregion Cylinder

        #region Box
        private void btnBoxSave_Click(object sender, EventArgs e)
        {
            if (this.listViewObjects.SelectedItems.Count < 1)
                return;

            int selectedIndex = this.listViewObjects.SelectedIndices[0];

            Vektor center = new Vektor(
                (double)this.numericBoxX.Value,
                (double)this.numericBoxY.Value,
                (double)this.numericBoxZ.Value);

            Vektor dir = new Vektor(
                (double)this.numericBoxOsaX.Value,
                (double)this.numericBoxOsaY.Value,
                (double)this.numericBoxOsaZ.Value);

            double size = (double)this.numericBoxSize.Value;

            ListViewItem item = this.listViewObjects.SelectedItems[0];

            if (item.Tag.GetType() == typeof(Box))
            {
                Box b = (Box)item.Tag;
                b.SetCenterSize(center, size);

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

                b.Material = mat;
            }

            if (item.Tag.GetType() == typeof(Cube))
            {
                Cube c = (Cube)item.Tag;

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

                c.Material = mat;

                c.SetValues(center, dir, size);
            }

            FillListView(selectedIndex);
            Redraw();
        }

        private void btnBoxColor_Click(object sender, EventArgs e)
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

        private void ShowBox(Box b)
        {
            this.panelBox.Visible = true;

            this.numericBoxX.Value = (decimal)b.Center.X;
            this.numericBoxY.Value = (decimal)b.Center.Y;
            this.numericBoxZ.Value = (decimal)b.Center.Z;

            this.numericBoxSize.Value = (decimal)b.Size;

            this.numBoxKa.Value = (decimal)b.Material.Ka;
            this.numBoxKs.Value = (decimal)b.Material.Ks;
            this.numBoxKd.Value = (decimal)b.Material.Kd;
            this.numBoxKt.Value = (decimal)b.Material.KT;
            this.numBoxH.Value = (decimal)b.Material.SpecularExponent;
            this.numBoxN.Value = (decimal)b.Material.N;

            this.numBoxColR.Value = (decimal)b.Material.Color.R;
            this.numBoxColG.Value = (decimal)b.Material.Color.G;
            this.numBoxColB.Value = (decimal)b.Material.Color.B;
        }

        private void ShowCube(Cube c)
        {
            this.panelBox.Visible = true;

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

        #endregion


        private void ComboScenesIndexChange(object sender, EventArgs e)
        {
            Scene[] scenes = (Scene[])this.comboScenes.DataSource;
            _rayTracer.RScene = scenes[this.comboScenes.SelectedIndex];
            btnBgCol.BackColor = _rayTracer.RScene.BgColor.SystemColor();
            FillListView();
            this.Redraw();
        }

        /// <summary>
        /// Formatovac comboboxu pro vyber z prednastavenych scen
        /// </summary>
        private void OnFormatComboScene(object sender, ListControlConvertEventArgs e)
        {
            Scene s;
            try
            {
                s = (Scene)e.ListItem;
            }
            catch(Exception)
            {
                return;
            }
            e.Value = String.Format("{0}", s.Caption);
        }

        /// <summary>
        /// vrati index objektu DefaultShape v listView
        /// </summary>
        /// <param name="shape">DefaultShape</param>
        /// <returns>index nebo -1</returns>
        private int FindIndexOfObjectInListView(object shape)
        {
            foreach (ListViewItem item in listViewObjects.Items)
            {
                if (item.Tag.Equals(shape))
                    return item.Index;
            }

            return -1;
        }

        /// <summary>
        /// MouseDown udalost na editor. Testujeme, zda nejaky objekt obsahuje kliknuti mysi.
        /// Testujeme v opacnem poradi, nez jak objekty vykreslujeme
        /// </summary>
        private void OnPicMouseDown(object sender, MouseEventArgs e)
        {
            Point loc = new Point(e.X, e.Y);

            // prochazeni shora dolu:
            Placeable pl;
            for (int i = _editorHelper.Placeables.Count - 1; i >= 0; i--)
            {
                pl = _editorHelper.Placeables[i];
                if (pl.Contains(e.X, e.Y))
                {
                    _editorHelper.IsEditorObjectClicked = true;
                    _editorHelper.PlaceObjSelected = pl;
                    _editorHelper.SelectedIndex = FindIndexOfObjectInListView(pl.Object);
                    return;
                }
            }

            // prochazeni zdola nahoru:
            /*
            foreach (Placeable p in _editorHelper.Placeables)
            {
                if (p.Contains(e.X, e.Y))
                {
                    //MessageBox.Show("Clicked");
                    _editorHelper.IsEditorObjectClicked = true;
                    _editorHelper.PlaceObjSelected = p;
                    _editorHelper.SelectedIndex = FindIndexOfObjectInListView(p.Object);
                    //_editorHelper.ClickedPoint = loc;
                    return;
                }
            }
             * */
            _editorHelper.IsEditorObjectClicked = false;
            _editorHelper.PlaceObjSelected = null;
        }

        private void OnPicMouseUp(object sender, MouseEventArgs e)
        {
            if (_editorHelper.IsEditorObjectClicked)
            {
                _editorHelper.IsEditorObjectClicked = false;

                //_editorHelper.PlaceObjSelected = null;

                FillListView(_editorHelper.SelectedIndex);
                
            }
            Redraw();
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            Point loc = e.Location;
            if (sender.GetType() == typeof(ListView))
            {
                loc.Offset(listViewObjects.Location);
            }
            if (editorPic.Bounds.Contains(loc))
            {
                if (e.Delta > 0 && (numericRatio.Value < numericRatio.Maximum))
                {
                    numericRatio.Value += 1;
                }
                else if (e.Delta < 0 && (numericRatio.Value > numericRatio.Minimum))
                {
                    numericRatio.Value -= 1;
                }
            }
        }
        private void OnPicMouseHover(object sender, EventArgs e)
        {
        }

        private void OnPicMouseMove(object sender, MouseEventArgs e)
        {
            if (_editorHelper.IsEditorObjectClicked)
            {
                Point loc = new Point(e.X, e.Y);
                _editorHelper.PlaceObjSelected.Set(loc.X - _editorHelper.Center.X, loc.Y - _editorHelper.Center.Y);
                Redraw();
            }
        }

        /// <summary>
        /// nastaveni materialu pro vsechny panely k objektum DefaultShape
        /// zobrazi se nove okno pro nastaveni
        /// </summary>
        private void btnMaterial_Click(object sender, EventArgs e)
        {
            if (this.listViewObjects.SelectedItems.Count < 1)
                return;

            
            int index = listViewObjects.SelectedIndices[0];
            ListViewItem item = this.listViewObjects.SelectedItems[0];
            DefaultShape obj = (DefaultShape)item.Tag;

            MaterialForm form = new MaterialForm();
            form.Set(obj.Material);
            DialogResult res = form.ShowDialog();
            if (res == DialogResult.OK)
            {
                obj.Material = form._material;
                Redraw();
                this.FillListView();
                this.listViewObjects.Items[index].Selected = true;
            }
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

        private void btnBoxMaterialColor_Click_1(object sender, EventArgs e)
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

        ///// <summary>
        ///// zmena comboboxu pro vyber os v editoru 
        ///// </summary>
        //private void OnAxisChange(object sender, EventArgs e)
        //{
        //    if (comboAxes.SelectedIndex == 0)
        //        _editorHelper.Axes = EditorAxesType.XY;
        //    else
        //        _editorHelper.Axes = EditorAxesType.YZ;
        //    Redraw();
        //}

        private void OnRatioChange(object sender, EventArgs e)
        {
            _editorHelper.Meritko = (int)this.numericRatio.Value;
            Redraw();
        }

        private void OnCheckChange(object sender, EventArgs e)
        {
            if (radioXY.Checked == true)
                _editorHelper.Axes = EditorAxesType.XY;
            else if (radioYZ.Checked)
                _editorHelper.Axes = EditorAxesType.ZY;
            else
                _editorHelper.Axes = EditorAxesType.XZ;
            Redraw();
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

        private void OnCheckCameraChange(object sender, EventArgs e)
        {
            this.Redraw();
        }

        private void OnResize(object sender, EventArgs e)
        {
//            this.Text = this.Width.ToString() + "x" + this.Height.ToString();
        }

        private void btnAnimate_Click(object sender, EventArgs e)
        {
            Size size;      // rozliseni vykreslovaneho obrazku
            if (!GetImageSize(out size))
                return;

            
            List<Size> sizes = new List<Size>(_pictureSize);
            sizes.Add(new Size(100, 100));

            if (!(sizes.Contains(size)))
            {
                if (MessageBox.Show("You have chosen custom resolution. It is not guaranteed, that animation will be propetly created. It is recommended to you to select predefined resolution or to set custom width to value of multiple 4 and even number of height. Do you want to proceed anyway?",
                    "Custom resolution", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                return;
            }

            if (size.Height % 2 != 0)
                size.Height++;

            while (size.Width % 4 != 0)
                size.Width++;

            int recursion = (int)numericRecurs.Value;
            bool antialias = checkAntialias.Checked;

            AnimationForm form = new AnimationForm(new RayTracing(_rayTracer), size, antialias, recursion);
            try
            {
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.InnerException.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Animation anim = new Animation(_animation);

            //_animation.AddProgressChangeEventHandler(new ProgressChangedEventHandler(animation_ProgressChanged));
            //_animation.AddAnimationCompletedEventHandler(new RunWorkerCompletedEventHandler(animation_Completed));
            //_animation.StartAnimation(size, recursion, checkAntialias.Checked, "anim");
            
            //anim.DoAnimate(size, recursion, checkAntialias.Checked);

        }
        //private void animation_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    //Bitmap bmp = (Bitmap)e.UserState;
        //    this.progressBar1.Value = e.ProgressPercentage;
        //    this.labelProgress.Text = e.ProgressPercentage.ToString() + "%";
        //}

        //private void animation_Completed(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    this.progressBar1.Value = 100;
        //    this.labelProgress.Text = "Hotovo!";
        //}

        private void panelAnimace_Paint(object sender, PaintEventArgs e)
        {

        }

        private void drawDirectlyToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Size size;      // rozliseni vykreslovaneho obrazku
            if (!GetImageSize(out size))
                return;





            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                int recursion = (int)numericRecurs.Value;
                bool antialias = checkAntialias.Checked;
                string ext = Path.GetExtension(saveFileDialog.FileName);
                ext = ext.ToLower();
                Renderer renderer = new Renderer(new RayTracing(_rayTracer), new RayImage(size, recursion, antialias));
                //    switch (ext)
                //    {
                //        case ".png":
                //            renderer.SaveScene(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                //            break;

                //        case ".jpg":
                //            renderer.SaveScene(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                //            break;

                //        case ".bmp":
                //            renderer.SaveScene(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                //            break;

                //        default:
                //            renderer.SaveScene(saveFileDialog.FileName);
                //            break;
                //    }

                //}

                //form.Set(new RayTracerLib.RayTracing(_rayTracer), size.Width, size.Height, recursion, checkAntialias.Checked);
                //form.Show();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnItemCheckedChange(object sender, ItemCheckedEventArgs e)
        {
            ListViewItem it = e.Item;
            Type t = it.Tag.GetType();
            if (t == typeof(Sphere))
            {
                Sphere sph = (Sphere)it.Tag;
                sph.IsActive = it.Checked;
                Redraw();
            }
            else if (t == typeof(Plane))
            {
                Plane p = (Plane)it.Tag;
                p.IsActive = it.Checked;
                Redraw();
            }
            else if (t == typeof(Cube))
            {
                Cube c = (Cube)it.Tag;
                c.IsActive = it.Checked;
                Redraw();
            }
            else if (t == typeof(Cylinder))
            {
                Cylinder c = (Cylinder)it.Tag;
                c.IsActive = it.Checked;
                Redraw();
            }
            else if (t == typeof(Light))
            {
                Light l = (Light)it.Tag;
                l.IsActive = it.Checked;
                Redraw();
            }
            else if (t == typeof(Box))
            {
                Box b = (Box)it.Tag;
                b.IsActive = it.Checked;
                Redraw();
            }
            else if (t == typeof(Camera))
            {
                it.Checked = true;
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
            numericLightEps.Value = (decimal)(tr.Value*5/100.0);
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

        private void btn_A(object sender, EventArgs e)
        {

        }

        private void oAplikaciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox abox = new AboutBox();
            abox.ShowDialog();
        }

        private void btnBgCol_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                btnBgCol.BackColor = colorDialog.Color;
                Colour col = Colour.ColourCreate(colorDialog.Color);
                this._rayTracer.RScene.BgColor = col;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = @"D:\MyWorks\RayTracer - Vystupy\anim-large4-compress\";
            string[] names = Directory.GetFiles(path);
            Image img;

            ImageCodecInfo png = null;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo encoder in encoders)
            {
                png = encoder.FormatDescription.Equals("JPEG") ? encoder : png;
            }

            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality,1,1);
            //encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression,100,100);
            foreach (string imgname in names)
            {
                img = Image.FromFile(imgname);
                string pngname = Path.Combine(Path.GetDirectoryName(imgname), "resampled1");
                pngname = Path.Combine(pngname, Path.GetFileName(imgname));
                pngname = Path.ChangeExtension(pngname, "jpg");

                
                //System.Drawing.Imaging.EncoderParameters parm = new System.Drawing.Imaging.EncoderParameters();
                //System.Drawing.Imaging.EncoderParameter p = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.

                img.Save(pngname, png, encoderParameters);
            }
        }

        private void napovedaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help helpForm = new Help();
            helpForm.ShowDialog();
        }

    }
}
