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
using Mathematics;

namespace _3dEditor
{
    public partial class WndProperties : Form
    {

        /// aktualne zobrazovany objekt
        object _currentlyDisplayed;

        /// <summary>
        /// Indikuje, zda se maji uhly rotace objektu zobrazit jako vypoctene z matice rotace,
        /// nebo zda maji byt ponechany - potreba pri zmene uhlu rotace okolo osy Y, ktera se nemuze behem nastavovani 
        /// v editoru vypocitavat z matice rotace, jelikoz by nebyl plynuly prechod pri presahu 90, nebo -90 stupnu.
        /// Proto se uhly prepocitavaji az po jine zmene vlastnosti objektu - to jsou vsechny zmeny krome one rotace.
        /// </summary>
        bool _showAngles = true;
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

            ShowDefault();

            this.panelSphere.Location = new Point(0, 0);
            this.panelCylindr.Location = new Point(0, 0);
            this.panelRovina.Location = new Point(0, 0);
            this.panelBox.Location = new Point(0, 0);
            this.panelLight.Location = new Point(0, 0);
            this.panelCamera.Location = new Point(0, 0);
            this.panelImage.Location = new Point(0, 0);
            this.panelAnimace.Location = new Point(0, 0);
            this.panelTriangle.Location = new Point(0, 0);
            this.panelCone.Location = new Point(0, 0);
            this.panelCustomObj.Location = new Point(0, 0);

            _permissionToModify = true;
            this.GetNextControl(this, true);
        }

        /// <summary>
        /// zakladni zobrazeni pro nezobrazovany zadny objekt
        /// Inicializace po otevreni okna, nebo po odstraneni objektu ze sceny
        /// </summary>
        public void ShowDefault()
        {
            imgRadioOptAABBTree.Text = Enum.GetName(typeof(Optimalizer.OptimizeType), Optimalizer.OptimizeType.RTREE);
            imgRadioOptAABBTree.Tag = Optimalizer.OptimizeType.RTREE;
            imgRadioOptOctTree.Text = Enum.GetName(typeof(Optimalizer.OptimizeType), Optimalizer.OptimizeType.OCTREE);
            imgRadioOptOctTree.Tag = Optimalizer.OptimizeType.OCTREE;
            imgRadioOptKdtree.Text = Enum.GetName(typeof(Optimalizer.OptimizeType), Optimalizer.OptimizeType.KDTREE);
            imgRadioOptKdtree.Tag = Optimalizer.OptimizeType.KDTREE;
            SetAllInvisible();
            this.Text = "Properties";
        }
        
        public void ShowObject(object obj)
        {
            if (obj == null) return;

            // pouze zobrazeni objektu - nutno zakazet jeho vnitrni zmenu pri udalostech k nastaveni
            // hodnot v prislusnych prvich - numericBoxech apod.
            _permissionToModify = false;
            _currentlyDisplayed = obj;

            if (obj.GetType() == typeof(DrawingSphere))
                ShowSphere((DrawingSphere)obj);

            else if (obj.GetType() == typeof(DrawingPlane))
                ShowPlane((DrawingPlane)obj);

            else if (obj.GetType() == typeof(DrawingCube))
                ShowCube((DrawingCube)obj);

            else if (obj.GetType() == typeof(DrawingCylinder))
                ShowCylinder((DrawingCylinder)obj);

            else if (obj.GetType() == typeof(DrawingCone))
                ShowCone((DrawingCone)obj);

            else if (obj is DrawingTriangle)
                ShowTriangle((DrawingTriangle)obj);

            else if (obj.GetType() == typeof(DrawingCustom))
                ShowCustom((DrawingCustom)obj);

            else if (obj.GetType() == typeof(RayImage))
                ShowImage((RayImage)obj);

            else if (obj.GetType() == typeof(DrawingLight))
                ShowLight((DrawingLight)obj);

            else if (obj.GetType() == typeof(DrawingCamera))
                ShowCamera((DrawingCamera)obj);
            else if (obj.GetType() == typeof(DrawingAnimation))
                ShowAnimation((DrawingAnimation)obj);

            else
            {
                _permissionToModify = true;
                return;
            }

            WndBoard wndBoard = GetWndBoard();
            if (_currentlyDisplayed is DrawingObject && wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            _permissionToModify = true;
            
            this.Update();
        }

        private void SetAllInvisible()
        {
            this.panelSphere.Visible = false;
            this.panelCylindr.Visible = false;
            this.panelCone.Visible = false;
            this.panelRovina.Visible = false;
            this.panelBox.Visible = false;
            this.panelLight.Visible = false;
            this.panelCamera.Visible = false;
            this.panelImage.Visible = false;
            this.panelAnimace.Visible = false;
            this.panelTriangle.Visible = false;
            this.panelCustomObj.Visible = false;
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

            this.txtbSphereLabel.Text = drSphere.Label;

            this.numericKouleX.Value = (decimal)MyMath.Clamp(sph.Origin.X, -100, 100);
            this.numericKouleY.Value = (decimal)MyMath.Clamp(sph.Origin.Y, -100, 100);
            this.numericKouleZ.Value = (decimal)MyMath.Clamp(sph.Origin.Z, -100, 100);

            this.numericKouleRadius.Value = (decimal)sph.R;

            if (_showAngles)
            {
                double[] angles = drSphere.GetRotationAngles();
                this.numSphRotX.Value = (decimal)MyMath.Clamp(angles[0], -360, 360);
                this.numSphRotY.Value = (decimal)MyMath.Clamp(angles[1], -360, 360);
                this.numSphRotZ.Value = (decimal)MyMath.Clamp(angles[2], -360, 360);
            }

            this.numSphKa.Value = (decimal)sph.Material.Ka;
            this.numSphKs.Value = (decimal)sph.Material.Ks;
            this.numSphKd.Value = (decimal)sph.Material.Kd;
            this.numSphKt.Value = (decimal)sph.Material.KT;
            this.numSphH.Value = (decimal)sph.Material.SpecularExponent;
            this.numSphN.Value = (decimal)sph.Material.N;

            this.numSphColR.Value = (decimal)sph.Material.Color.R;
            this.numSphColG.Value = (decimal)sph.Material.Color.G;
            this.numSphColB.Value = (decimal)sph.Material.Color.B;

            this.numSphSize.Value = (decimal)drSphere.Sides;
            this.numSphPhi.Value = (decimal)drSphere.DecremPhi;
            this.numSphTheta.Value = (decimal)drSphere.DecremTheta;

            btnSphMaterialColor.BackColor = sph.Material.Color.SystemColor();
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

            this.txtbPlaneLabel.Text = drPlane.Label;

            if (_showAngles)
            {
                double[] angles = drPlane.GetRotationAngles();
                this.numPlaneRotX.Value = (decimal)MyMath.Clamp(angles[0], -360, 360);
                this.numPlaneRotY.Value = (decimal)MyMath.Clamp(angles[1], -360, 360);
                this.numPlaneRotZ.Value = (decimal)MyMath.Clamp(angles[2], -360, 360);

            }
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

            btnPlaneMaterialColor.BackColor = pl.Material.Color.SystemColor();

            this.numPlaneSize.Value = (decimal)drPlane.Size;
            this.numPlaneDist.Value = (decimal)drPlane.Distance;
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

            this.txtbCubeLabel.Text = drCube.Label;

            this.numericBoxX.Value = (decimal)MyMath.Clamp(c.Center.X, -100, 100);
            this.numericBoxY.Value = (decimal)MyMath.Clamp(c.Center.Y, -100, 100);
            this.numericBoxZ.Value = (decimal)MyMath.Clamp(c.Center.Z, -100, 100);

            this.numericBoxOsaX.Value = (decimal)MyMath.Clamp(c.Dir.X, -100, 100);
            this.numericBoxOsaY.Value = (decimal)MyMath.Clamp(c.Dir.Y, -100, 100);
            this.numericBoxOsaZ.Value = (decimal)MyMath.Clamp(c.Dir.Z, -100, 100);

            this.numericBoxSize.Value = (decimal)c.Size;

            if (_showAngles)
            {
                double[] angles = drCube.GetRotationAngles();
                this.numBoxRotateX.Value = (decimal)MyMath.Clamp(angles[0], -360, 360);
                this.numBoxRotateY.Value = (decimal)MyMath.Clamp(angles[1], -360, 360);
                this.numBoxRotateZ.Value = (decimal)MyMath.Clamp(angles[2], -360, 360);
            }

            this.numBoxKa.Value = (decimal)c.Material.Ka;
            this.numBoxKs.Value = (decimal)c.Material.Ks;
            this.numBoxKd.Value = (decimal)c.Material.Kd;
            this.numBoxKt.Value = (decimal)c.Material.KT;
            this.numBoxH.Value = (decimal)c.Material.SpecularExponent;
            this.numBoxN.Value = (decimal)c.Material.N;

            this.numBoxColR.Value = (decimal)c.Material.Color.R;
            this.numBoxColG.Value = (decimal)c.Material.Color.G;
            this.numBoxColB.Value = (decimal)c.Material.Color.B;

            btnBoxMaterialColor.BackColor = c.Material.Color.SystemColor();
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

            this.txtbCylLabel.Text = drCyl.Label;

            this.numericCylCentX.Value = (decimal)MyMath.Clamp(c.Center.X, -100, 100);
            this.numericCylCentY.Value = (decimal)MyMath.Clamp(c.Center.Y, -100, 100);
            this.numericCylCentZ.Value = (decimal)MyMath.Clamp(c.Center.Z, -100, 100);

            this.numericCylDirX.Value = (decimal)MyMath.Clamp(c.Dir.X, -100, 100);
            this.numericCylDirY.Value = (decimal)MyMath.Clamp(c.Dir.Y, -100, 100);
            this.numericCylDirZ.Value = (decimal)MyMath.Clamp(c.Dir.Z, -100, 100);

            this.numericCylH.Value = (decimal)c.Height;
            this.numericCylR.Value = (decimal)c.Rad;

            if (_showAngles)
            {
                double[] angles = drCyl.GetRotationAngles();
                this.numCylRotateX.Value = (decimal)MyMath.Clamp(angles[0], -360, 360);
                this.numCylRotateY.Value = (decimal)MyMath.Clamp(angles[1], -360, 360);
                this.numCylRotateZ.Value = (decimal)MyMath.Clamp(angles[2], -360, 360);
            }

            this.numCylKa.Value = (decimal)c.Material.Ka;
            this.numCylKs.Value = (decimal)c.Material.Ks;
            this.numCylKd.Value = (decimal)c.Material.Kd;
            this.numCylKt.Value = (decimal)c.Material.KT;
            this.numCylH.Value = (decimal)c.Material.SpecularExponent;
            this.numCylN.Value = (decimal)c.Material.N;

            this.numCylColR.Value = (decimal)c.Material.Color.R;
            this.numCylColG.Value = (decimal)c.Material.Color.G;
            this.numCylColB.Value = (decimal)c.Material.Color.B;

            btnCylMaterialColor.BackColor = c.Material.Color.SystemColor();
        }

        private void ShowTriangle(DrawingTriangle drTriangl)
        {
            // zabraneni neustalemu blikani pri modifikaci stejne koule
            if (!this.panelTriangle.Visible)
            {
                SetAllInvisible();
                this.panelTriangle.Visible = true;
                this.Text = "Properties: Triangle";
            }

            if (_currentlyDisplayed is DrawingFacet)
            {
                this.btnTriangDivide.Visible = true;
                this.btnTriangDelete.Visible = true;
            }
            else
            {
                this.btnTriangDivide.Visible = false;
                this.btnTriangDelete.Visible = false;
            }
            Triangle triangl = (Triangle)drTriangl.ModelObject;

            this.txtbTriangLabel.Text = drTriangl.Label;

            WndBoard wndB = GetWndBoard();
            Matrix3D transp = wndB.RotationMatrix.Transpose();
            Vektor a = transp.Transform2NewPoint(drTriangl.A);
            Vektor b = transp.Transform2NewPoint(drTriangl.B);
            Vektor c = transp.Transform2NewPoint(drTriangl.C);

            this.numericTriangleAX.Value = (decimal)a.X;
            this.numericTriangleAY.Value = (decimal)a.Y;
            this.numericTriangleAZ.Value = (decimal)a.Z;

            this.numericTriangleBX.Value = (decimal)b.X;
            this.numericTriangleBY.Value = (decimal)b.Y;
            this.numericTriangleBZ.Value = (decimal)b.Z;

            this.numericTriangleCX.Value = (decimal)c.X;
            this.numericTriangleCY.Value = (decimal)c.Y;
            this.numericTriangleCZ.Value = (decimal)c.Z;

            Material mat = triangl.Material;
            this.numTriangKa.Value = (decimal)mat.Ka;
            this.numTriangKs.Value = (decimal)mat.Ks;
            this.numTriangKd.Value = (decimal)mat.Kd;
            this.numTriangKt.Value = (decimal)mat.KT;
            this.numTriangH.Value = (decimal)mat.SpecularExponent;
            this.numTriangN.Value = (decimal)mat.N;

            this.numTriangColR.Value = (decimal)mat.Color.R;
            this.numTriangColG.Value = (decimal)mat.Color.G;
            this.numTriangColB.Value = (decimal)mat.Color.B;

            btnTriangMaterialCol.BackColor = mat.Color.SystemColor();
        }

        private void ShowCustom(DrawingCustom drCust)
        {
            // zabraneni neustalemu blikani pri modifikaci stejne koule
            if (!this.panelCustomObj.Visible)
            {
                SetAllInvisible();
                this.panelCustomObj.Visible = true;
                this.Text = "Properties: Custom Object";
            }

            WndBoard wndB = GetWndBoard();
            Matrix3D transp = wndB.RotationMatrix.Transpose();
            Vektor center = transp.Transform2NewPoint(drCust.Center);

            CustomObject cust = (CustomObject)drCust.ModelObject;

            this.txtbCustomLabel.Text = drCust.Label;

            if (_showAngles)
            {
                double[] angles = drCust.GetRotationAngles();
                this.numCustomRotX.Value = (decimal)MyMath.Clamp(angles[0], -360, 360);
                this.numCustomRotY.Value = (decimal)MyMath.Clamp(angles[1], -360, 360);
                this.numCustomRotZ.Value = (decimal)MyMath.Clamp(angles[2], -360, 360);
            }

            this.numCustomCenterX.Value = (decimal)center.X;
            this.numCustomCenterY.Value = (decimal)center.Y;
            this.numCustomCenterZ.Value = (decimal)center.Z;

            Material mat = cust.Material;
            this.numCustomKa.Value = (decimal)mat.Ka;
            this.numCustomKs.Value = (decimal)mat.Ks;
            this.numCustomKd.Value = (decimal)mat.Kd;
            this.numCustomKt.Value = (decimal)mat.KT;
            this.numCustomH.Value = (decimal)mat.SpecularExponent;
            this.numCustomN.Value = (decimal)mat.N;

            this.numCustomColR.Value = (decimal)mat.Color.R;
            this.numCustomColG.Value = (decimal)mat.Color.G;
            this.numCustomColB.Value = (decimal)mat.Color.B;

            btnCustomColor.BackColor = mat.Color.SystemColor();
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

            this.numericKameraAngle.Value = (decimal)MyMath.Clamp(cam.AngleUp, -360, 360);

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

            this.txtbLightLabel.Text = drLight.Label;

            this.numericSvetloX.Value = (decimal)MyMath.Clamp(l.Coord.X, -100, 100);
            this.numericSvetloY.Value = (decimal)MyMath.Clamp(l.Coord.Y, -100, 100);
            this.numericSvetloZ.Value = (decimal)MyMath.Clamp(l.Coord.Z, -100, 100);

            this.numericSvetloR.Value = (decimal)l.Color.R;
            this.numericSvetloG.Value = (decimal)l.Color.G;
            this.numericSvetloB.Value = (decimal)l.Color.B;

            this.checkBoxLightIsSoft.Checked = l.IsSoftLight;

            btnLighColor.BackColor = l.Color.SystemColor();

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

            this.comboResolution.DataSource = img.PictureSize;
            this.comboResolution.SelectedIndex = img.IndexPictureSize;         // nastaveni prvni polozky
            if (img.IndexPictureSize == img.PictureSize.Length - 1)
            {
                this.txbResX.Text = img.CurrentSize.Width.ToString();
                this.txbResY.Text = img.CurrentSize.Height.ToString();
            }

            this.numericRecurs.Value = (decimal)MyMath.Clamp(img.MaxRecurse, -1, 100);
            this.checkAntialias.Checked = img.IsAntialiasing;
            this.checkOptimize.Checked = img.IsOptimalizing;
            this.btnImageBgr.BackColor = img.BackgroundColor.SystemColor();

            if (img.IsOptimalizing)
            {
                this.panelOptimGroup.Visible = true;
                if (img.OptimizType == Optimalizer.OptimizeType.OCTREE)
                    this.imgRadioOptOctTree.Checked = true;
                else if (img.OptimizType == Optimalizer.OptimizeType.RTREE)
                    this.imgRadioOptAABBTree.Checked = true;
            }
            else
            {
                this.panelOptimGroup.Visible = false;
            }
        }

        private GroupBox CreateOptimizeGroup()
        {
            string[] names = Enum.GetNames(typeof(Optimalizer.OptimizeType));
            GroupBox gbox = new GroupBox();
            gbox.Width = 100;
            gbox.Height = names.Length * 18;
            gbox.Name = "imgOptimizeGbox";
            foreach (string name in names)
            {
                RadioButton rb = new RadioButton();
                rb.Name = "imgRadioOpt" + name;
                gbox.Controls.Add(rb);
            }
            return gbox;

        }
        private void ShowAnimation(DrawingAnimation drAnim)
        {
            // zabraneni neustalemu blikani pri modifikaci stejne koule
            if (!this.panelAnimace.Visible)
            {
                SetAllInvisible();
                this.panelAnimace.Visible = true;
                this.Text = "Properties: Animation";
            }
            this.txtbAnimLabel.Text = drAnim.Label;

            this.numAnimCenterX.Value = (decimal)drAnim.CenterWorld.X;
            this.numAnimCenterY.Value = (decimal)drAnim.CenterWorld.Y;
            this.numAnimCenterZ.Value = (decimal)drAnim.CenterWorld.Z;

            this.numAnimElipseA.Value = (decimal)drAnim.A;
            this.numAnimElipseB.Value = (decimal)drAnim.B;

            this.numAnimFps.Value = (decimal)drAnim.FPS;
            this.numAnimSecs.Value = (decimal)drAnim.Time;
            this.textBAnimFile.Text = drAnim.FileFullPath;

            switch (drAnim.TypeAnim)
            {
                case AnimationType.VideoOnly:
                    this.radioAnimVideoOnly.Checked = true;
                    break;
                case AnimationType.ImagesOnly:
                    this.radioAnimImgsOnly.Checked = true;
                    break;
                case AnimationType.BothImagesAndVideo:
                    this.radioAnimBothImgVideo.Checked = true;
                    break;
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
            this.actionImageSet(sender, e);
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

        private void btnImageBgCol_Click(object sender, EventArgs e)
        {
            //this.colorDialog.ShowDialog();
            //this.ParentForm.Visible = false;
            //this.Parent.Parent.Hide();
            this.SendToBack();
            this.ParentForm.Hide();
            //this.ParentForm.Parent.SendToBack();
            
            
            if (this.colorDialog.ShowDialog(this.Parent.Parent) == DialogResult.OK)
            {
                if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(RayImage))
                    return;

                if (!_permissionToModify)
                    return;

                RayImage img = (RayImage)_currentlyDisplayed;

                btnImageBgr.BackColor = colorDialog.Color;
                Colour col = Colour.ColourCreate(colorDialog.Color);
                img.BackgroundColor = col;
            }
            this.ParentForm.Show();
        }

        #endregion

        

        ///////////////////////////////////////////////
        //////// S P H E R E
        //////////////////////////////////////////////

        private void actionSphereSet(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingSphere))
                return;

            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

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

            int size = (int)this.numSphSize.Value;
            int theta = (int)this.numSphTheta.Value;
            int phi = (int)this.numSphPhi.Value;

            btnSphMaterialColor.BackColor = mat.Color.SystemColor();

            drSph.SetModelObject(sph, size, theta, phi);
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


        #region plane
        ///////////////////////////////////////////////
        //////// P L A N E
        //////////////////////////////////////////////
        private void actionPlaneSet(object sender, EventArgs e)
        {

            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingPlane))
                return;

            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            DrawingPlane drPlane = (DrawingPlane)_currentlyDisplayed;
            Plane plane = (Plane)drPlane.ModelObject;

            Vektor norm = new Vektor(
                (double)this.numericRovinaA.Value,
                (double)this.numericRovinaB.Value,
                (double)this.numericRovinaC.Value);

            double d = (double)numericRovinaD.Value;

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

            int size = (int)this.numPlaneSize.Value;
            float dist = (float)this.numPlaneDist.Value;

            plane.SetValues(norm, d);

            plane.Material = mat;
            plane.CreateBoundVektors();

            //btnPlaneMaterialColor.BackColor = mat.Color.SystemColor();

            drPlane.SetModelObject(plane, size, dist);
            WndBoard wndB = GetWndBoard();
            drPlane.ApplyRotationMatrix(wndB.RotationMatrix);
            WndScene wndSc = GetWndScene();
            wndSc.UpdateRecords();
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

                _permissionToModify = false;
                this.numPlaneColR.Value = (decimal)col.R;
                this.numPlaneColG.Value = (decimal)col.G;
                _permissionToModify = true;
                this.numPlaneColB.Value = (decimal)col.B;
            }
        }

        #endregion
        ///////////////////////////////////////////////
        //////// C U B E
        //////////////////////////////////////////////

        private void actionCubeSet(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingCube))
                return;

            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

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

            btnBoxMaterialColor.BackColor = mat.Color.SystemColor();

            drCube.SetModelObject(cube);
            WndBoard wndB = GetWndBoard();
            drCube.ApplyRotationMatrix(wndB.RotationMatrix);
            WndScene wndSc = GetWndScene();
            wndSc.UpdateRecords();
            this.ShowCube(drCube);
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

                Button btn = sender as Button;
                btn.BackColor = colorDialog.Color;
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

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

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


            cyl.SetValues(center, dir, r, h);

            drCyl.SetModelObject(cyl);
            WndBoard wndB = GetWndBoard();
            drCyl.ApplyRotationMatrix(wndB.RotationMatrix);
            WndScene wndSc = GetWndScene();
            wndSc.UpdateRecords();
        }
        private void actionCylinderSetMaterial(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingCylinder))
                return;

            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            DrawingCylinder drCyl = (DrawingCylinder)_currentlyDisplayed;
            Cylinder cyl = (Cylinder)drCyl.ModelObject;

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

            btnCylMaterialColor.BackColor = mat.Color.SystemColor();

            drCyl.SetModelObject(cyl);
            WndBoard wndB = GetWndBoard();
            drCyl.ApplyRotationMatrix(wndB.RotationMatrix);
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
                Button btn = sender as Button;
                btn.BackColor = colorDialog.Color;
            }
        }
        #endregion


        ///////////////////////////////////////////////
        //////// C O N E
        //////////////////////////////////////////////
        
        private void ShowCone(DrawingCone drCone)
        {
            // zabraneni neustalemu blikani pri modifikaci stejne koule
            if (!this.panelCone.Visible)
            {
                SetAllInvisible();
                this.panelCone.Visible = true;
                this.Text = "Properties: Cone";
            }

            Cone c = (Cone)drCone.ModelObject;

            this.txtbConeLabel.Text = drCone.Label;

            this.numericConePeakX.Value = (decimal)MyMath.Clamp(c.Peak.X, -100, 100);
            this.numericConePeakY.Value = (decimal)MyMath.Clamp(c.Peak.Y, -100, 100);
            this.numericConePeakZ.Value = (decimal)MyMath.Clamp(c.Peak.Z, -100, 100);

            this.numericConeDirX.Value = (decimal)MyMath.Clamp(c.Dir.X, -100, 100);
            this.numericConeDirY.Value = (decimal)MyMath.Clamp(c.Dir.Y, -100, 100);
            this.numericConeDirZ.Value = (decimal)MyMath.Clamp(c.Dir.Z, -100, 100);

            this.numericConeHeight.Value = (decimal)c.Height;
            this.numericConeRadius.Value = (decimal)c.Rad;

            if (_showAngles)
            {
                double[] angles = drCone.GetRotationAngles();
                this.numericConeAngleX.Value = (decimal)MyMath.Clamp(angles[0], -360, 360);
                this.numericConeAngleY.Value = (decimal)MyMath.Clamp(angles[1], -360, 360);
                this.numericConeAngleZ.Value = (decimal)MyMath.Clamp(angles[2], -360, 360);
            }

            this.numericConeKa.Value = (decimal)c.Material.Ka;
            this.numericConeKs.Value = (decimal)c.Material.Ks;
            this.numericConeKd.Value = (decimal)c.Material.Kd;
            this.numericConeKt.Value = (decimal)c.Material.KT;
            this.numericConeH.Value = (decimal)c.Material.SpecularExponent;
            this.numericConeN.Value = (decimal)c.Material.N;

            this.numericConeColR.Value = (decimal)c.Material.Color.R;
            this.numericConeColG.Value = (decimal)c.Material.Color.G;
            this.numericConeColB.Value = (decimal)c.Material.Color.B;

            btnConeColor.BackColor = c.Material.Color.SystemColor();
        }

        private void actionConeSet(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingCone))
                return;

            if (!_permissionToModify)
                return;
            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            DrawingCone drCone = (DrawingCone)_currentlyDisplayed;
            Cone cone = (Cone)drCone.ModelObject;

            Vektor peak = new Vektor(
                (double)this.numericConePeakX.Value,
                (double)this.numericConePeakY.Value,
                (double)this.numericConePeakZ.Value);
            
            Vektor dir = new Vektor(
                (double)this.numericConeDirX.Value,
                (double)this.numericConeDirY.Value,
                (double)this.numericConeDirZ.Value);
            
            double rad = (double)this.numericConeRadius.Value;
            double height = (double)this.numericConeHeight.Value;

            cone.SetValues(peak, dir, rad, height);

            drCone.SetModelObject(cone);
            WndBoard wndB = GetWndBoard();
            drCone.ApplyRotationMatrix(wndB.RotationMatrix);
            WndScene wndSc = GetWndScene();
            wndSc.UpdateRecords();
            this.ShowCone(drCone);
        }

        private void actionConeSetMaterial(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingCone))
                return;

            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            DrawingCone drCone = (DrawingCone)_currentlyDisplayed;
            Cone cone = (Cone)drCone.ModelObject;

            Material mat = new Material();
            mat.Ka = (double)this.numericConeKa.Value;
            mat.Ks = (double)this.numericConeKs.Value;
            mat.Kd = (double)this.numericConeKd.Value;
            mat.KT = (double)this.numericConeKt.Value;
            mat.SpecularExponent = (int)this.numericConeH.Value;
            mat.N = (double)this.numericConeN.Value;

            mat.Color.R = (double)this.numericConeColR.Value;
            mat.Color.G = (double)this.numericConeColG.Value;
            mat.Color.B = (double)this.numericConeColB.Value;

            cone.Material = mat;

            btnConeColor.BackColor = mat.Color.SystemColor();
            
            drCone.SetModelObject(cone);
            WndBoard wndB = GetWndBoard();
            drCone.ApplyRotationMatrix(wndB.RotationMatrix);
        }

        private void btnConeMaterialColor_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                double r = colorDialog.Color.R / (double)255;
                double g = colorDialog.Color.G / (double)255;
                double b = colorDialog.Color.B / (double)255;
                double a = colorDialog.Color.A / (double)255;

                RayTracerLib.Colour col = new RayTracerLib.Colour(r, g, b, a);

                _permissionToModify = false;
                this.numericConeColR.Value = (decimal)col.R;
                this.numericConeColG.Value = (decimal)col.G;
                _permissionToModify = true;
                this.numericConeColB.Value = (decimal)col.B;
                Button btn = sender as Button;
                btn.BackColor = colorDialog.Color;
            }
        }

        private void actionConeRotate(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingCone))
                return;

            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            _permissionToModify = false;

            NumericUpDown num = sender as NumericUpDown;
            if (num.Value > 359)
                num.Value = num.Value % 360;
            else if (num.Value < 0)
                num.Value = 360 + num.Value;

            DrawingCone drCone = (DrawingCone)_currentlyDisplayed;

            double[] angles = new double[]{
                (double)this.numericConeAngleX.Value,
                (double)this.numericConeAngleY.Value,
                (double)this.numericConeAngleZ.Value};

            WndBoard wnd = GetWndBoard();
            Matrix3D transp = wnd.RotationMatrix.Transpose();
            transp.TransformPoints(drCone.Points);
            drCone.Rotate(angles[0], angles[1], angles[2]);
            wnd.RotationMatrix.TransformPoints(drCone.Points);

            _showAngles = false;
            this.ShowCone(drCone);
            _permissionToModify = true;
            _showAngles = true;
        }

        ///////////////////////////////////////////////
        //////// C A M E R A
        //////////////////////////////////////////////
        #region CAMERA
        private void actionCameraSet(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingCamera))
                return;

            if (!_permissionToModify)
                return;
            
            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

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
                //_permissionToModify = false;
                //ShowCamera(drCam);
                //_permissionToModify = true;
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

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

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

            btnLighColor.BackColor = light.Color.SystemColor();

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


        private void actionImageSet(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(RayImage))
                return;

            if (!_permissionToModify)
                return;

            RayImage img = (RayImage)_currentlyDisplayed;

            //btnImageBgr.BackColor = colorDialog.Color;
            //Colour col = Colour.ColourCreate(colorDialog.Color);
            //img.BackgroundColor = col;

            img.MaxRecurse = (int)this.numericRecurs.Value;
            img.IsAntialiasing = this.checkAntialias.Checked;

            img.IndexPictureSize = this.comboResolution.SelectedIndex;
            int w = 100;
            int h = 100;
            // vybrano vlastni rozliseni
            if (this.comboResolution.SelectedIndex == img.PictureSize.Length - 1)
            {
                Int32.TryParse(txbResX.Text, out w);
                Int32.TryParse(txbResY.Text, out h);
                w = w > 0 ? w : 100;
                h = h > 0 ? h : 100;

            } // prednastavene rozliseni
            else
            {
                w = img.PictureSize[img.IndexPictureSize].Width;
                h = img.PictureSize[img.IndexPictureSize].Height;
            }
            img.CurrentSize = new Size(w, h);

            WndScene wndSc = GetWndScene();
            wndSc.UpdateRecords();
        }

        

        private void actionAnimationSet(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingAnimation))
                return;

            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            DrawingAnimation drAnim = (DrawingAnimation)_currentlyDisplayed;

            Vektor center = new Vektor();
            center.X = (double)this.numAnimCenterX.Value;
            center.Y = (double)this.numAnimCenterY.Value;
            center.Z = (double)this.numAnimCenterZ.Value;

            double a = (double)this.numAnimElipseA.Value;
            double b = (double)this.numAnimElipseB.Value;
            double c = (double)this.numAnimElipseB.Value;

            drAnim.Set(center, a, b);

            drAnim.FPS=(double)this.numAnimFps.Value;
            drAnim.Time = (double)this.numAnimSecs.Value;
            drAnim.FileFullPath = this.textBAnimFile.Text;

            if (this.radioAnimVideoOnly.Checked)
                drAnim.TypeAnim = AnimationType.VideoOnly;
            else if (this.radioAnimImgsOnly.Checked)
                drAnim.TypeAnim = AnimationType.ImagesOnly;
            else if (this.radioAnimBothImgVideo.Checked)
                drAnim.TypeAnim = AnimationType.BothImagesAndVideo;

            WndBoard wnd = GetWndBoard();
            wnd.RotationMatrix.TransformPoints(drAnim.Points);
            WndScene wndSc = GetWndScene();
            wndSc.UpdateRecords();
        }
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

        private void actionSphereRotate(object sender, EventArgs e)
        {
            if (_currentlyDisplayed.GetType() != typeof(DrawingSphere))
                return;

            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            _permissionToModify = false;

            NumericUpDown num = sender as NumericUpDown;
            if (num.Value > 359)
                num.Value = num.Value % 360;
            else if (num.Value < 0)
                num.Value = 360 + num.Value;

            double x = (double)this.numSphRotX.Value;
            double y = (double)this.numSphRotY.Value;
            double z = (double)this.numSphRotZ.Value;

            DrawingSphere drSph = _currentlyDisplayed as DrawingSphere;

            WndBoard wnd = GetWndBoard();
            Matrix3D transp = wnd.RotationMatrix.Transpose();
            transp.TransformPoints(drSph.Points);
            drSph.Rotate(x, y, z);
            wnd.RotationMatrix.TransformPoints(drSph.Points);

            _showAngles = false;
            this.ShowSphere(drSph);
            _permissionToModify = true;
            _showAngles = true;
        }

        private void actionCylinderRotate(object sender, EventArgs e)
        {
            if (_currentlyDisplayed.GetType() != typeof(DrawingCylinder))
                return;

            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            _permissionToModify = false;

            NumericUpDown num = sender as NumericUpDown;
            if (num.Value > 359)
                num.Value = num.Value % 360;
            else if (num.Value < 0)
                num.Value = 360 + num.Value;

            double[] angles = new double[]{
            (double)this.numCylRotateX.Value,
            (double)this.numCylRotateY.Value,
            (double)this.numCylRotateZ.Value};

            DrawingCylinder drCyl = _currentlyDisplayed as DrawingCylinder;

            WndBoard wnd = GetWndBoard();
            Matrix3D transp = wnd.RotationMatrix.Transpose();
            transp.TransformPoints(drCyl.Points);
            drCyl.Rotate(angles[0], angles[1], angles[2]);
            wnd.RotationMatrix.TransformPoints(drCyl.Points);

            _showAngles = false;
            this.ShowCylinder(drCyl);
            _permissionToModify = true;
            _showAngles = true;
        }

        private void actionCubeRotate(object sender, EventArgs e)
        {
            if (_currentlyDisplayed.GetType() != typeof(DrawingCube))
                return;
            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            _permissionToModify = false;

            NumericUpDown num = sender as NumericUpDown;
            if (num.Value > 359)
                num.Value = num.Value % 360;
            else if (num.Value < 0)
                num.Value = 360 + num.Value;

            double[] angles = new double[]{
                (double)this.numBoxRotateX.Value,
                (double)this.numBoxRotateY.Value,
                (double)this.numBoxRotateZ.Value};

            DrawingCube drCube = _currentlyDisplayed as DrawingCube;

            WndBoard wnd = GetWndBoard();
            Matrix3D transp = wnd.RotationMatrix.Transpose();
            transp.TransformPoints(drCube.Points);
            drCube.Rotate(angles[0], angles[1], angles[2]);
            wnd.RotationMatrix.TransformPoints(drCube.Points);

            _showAngles = false;
            this.ShowCube(drCube);
            _permissionToModify = true;
            _showAngles = true;
        }

        private void actionPlaneRotate(object sender, EventArgs e)
        {
            if (_currentlyDisplayed.GetType() != typeof(DrawingPlane))
                return;
            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            _permissionToModify = false;

            NumericUpDown num = sender as NumericUpDown;
            if (num.Value > 359)
                num.Value = num.Value % 360;
            else if (num.Value < 0)
                num.Value = 360 + num.Value;

            double[] angles = new double[]{
            (double)this.numPlaneRotX.Value,
            (double)this.numPlaneRotY.Value,
            (double)this.numPlaneRotZ.Value};

            DrawingPlane drPlane = _currentlyDisplayed as DrawingPlane;

            WndBoard wnd = GetWndBoard();
            Matrix3D transp = wnd.RotationMatrix.Transpose();
            transp.TransformPoints(drPlane.Points);
            drPlane.Rotate(angles[0], angles[1], angles[2]);
            wnd.RotationMatrix.TransformPoints(drPlane.Points);
            _showAngles = false;
            this.ShowPlane(drPlane);
            _permissionToModify = true;
            _showAngles = true;
        }

        private void actionAnimRotate(object sender, EventArgs e)
        {
            if (_currentlyDisplayed.GetType() != typeof(DrawingAnimation))
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            NumericUpDown num = sender as NumericUpDown;
            if (num.Value > 359)
                num.Value = num.Value % 360;
            else if (num.Value < 0)
                num.Value = 360 + num.Value;

            double x = (double)this.numAnimRotX.Value;
            double y = (double)this.numAnimRotY.Value;
            double z = (double)this.numAnimRotZ.Value;
            Matrix3D m = Matrix3D.NewRotateByDegrees(x, y, z);

            DrawingAnimation drAnim = _currentlyDisplayed as DrawingAnimation;

            WndBoard wnd = GetWndBoard();
            Matrix3D transp = wnd.RotationMatrix.Transpose();
            transp.TransformPoints(drAnim.Points);
            drAnim.Rotate(x, y, z);
            wnd.RotationMatrix.TransformPoints(drAnim.Points);
        }

        private void OnFileAnimSelect(object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                this.textBAnimFile.Text = filename;
            }
        }

        ///////////////////////////////////////////////
        //////// T R I A N G L E
        //////////////////////////////////////////////

        private void actionTriangleSet(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || !(_currentlyDisplayed is DrawingTriangle))
                return;

            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            DrawingTriangle drTriang = (DrawingTriangle)_currentlyDisplayed;
            Triangle triangl = (Triangle)drTriang.ModelObject;

            Vektor A = new Vektor(
                (double)this.numericTriangleAX.Value,
                (double)this.numericTriangleAY.Value,
                (double)this.numericTriangleAZ.Value);

            Vektor B = new Vektor(
                (double)this.numericTriangleBX.Value,
                (double)this.numericTriangleBY.Value,
                (double)this.numericTriangleBZ.Value);

            Vektor C = new Vektor(
                (double)this.numericTriangleCX.Value,
                (double)this.numericTriangleCY.Value,
                (double)this.numericTriangleCZ.Value);


            Material mat = triangl.Material;
            mat.Ka = (double)this.numTriangKa.Value;
            mat.Ks = (double)this.numTriangKs.Value;
            mat.Kd = (double)this.numTriangKd.Value;
            mat.KT = (double)this.numTriangKt.Value;
            mat.SpecularExponent = (int)this.numTriangH.Value;
            mat.N = (double)this.numTriangN.Value;

            mat.Color.R = (double)this.numTriangColR.Value;
            mat.Color.G = (double)this.numTriangColG.Value;
            mat.Color.B = (double)this.numTriangColB.Value;

            triangl.Material = mat;
            triangl.Set(A, B, C);

            btnTriangMaterialCol.BackColor = mat.Color.SystemColor();

            drTriang.SetModelObject(triangl);

            WndBoard wndB = GetWndBoard();
            if (_currentlyDisplayed is DrawingFacet)
            {
                DrawingFacet drFace = _currentlyDisplayed as DrawingFacet;
                drTriang.ApplyRotationMatrix(wndB.RotationMatrix);
                //drFace.DrCustObject.SetModelObject(drFace.DrCustObject.ModelObject);
                //drFace.DrCustObject.ApplyRotationMatrix(wndB.RotationMatrix);
            }
            else
            {
                drTriang.ApplyRotationMatrix(wndB.RotationMatrix);
                WndScene wndSc = GetWndScene();
                wndSc.UpdateRecords();
            }
        }

        private void btnTriangMaterialColor_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                double r = colorDialog.Color.R / (double)255;
                double g = colorDialog.Color.G / (double)255;
                double b = colorDialog.Color.B / (double)255;
                double a = colorDialog.Color.A / (double)255;

                RayTracerLib.Colour col = new RayTracerLib.Colour(r, g, b, a);

                _permissionToModify = false;
                this.numTriangColR.Value = (decimal)col.R;
                this.numTriangColG.Value = (decimal)col.G;
                this.numTriangColB.Value = (decimal)col.B;
                _permissionToModify = true;

                actionTriangleSet(sender, e);
                Button btn = sender as Button;
                btn.BackColor = colorDialog.Color;
            }
        }

        private void actionCameraSetUp(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingCamera))
                return;

            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            DrawingCamera drCam = (DrawingCamera)_currentlyDisplayed;
            Camera cam = (Camera)drCam.ModelObject;

            try
            {
                Vektor up = new Vektor(
                    (double)this.numericKameraUpX.Value,
                    (double)this.numericKameraUpY.Value,
                    (double)this.numericKameraUpZ.Value);

                cam.SetNormByUp(up);


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

        private void actionCameraSet3Rotate(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingCamera))
                return;

            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            DrawingCamera drCam = (DrawingCamera)_currentlyDisplayed;
            Camera cam = (Camera)drCam.ModelObject;

            try
            {
                if (this.numericKameraAngle.Value >= 360)
                    this.numericKameraAngle.Value = this.numericKameraAngle.Value - 360;
                else if (this.numericKameraAngle.Value < 0)
                    this.numericKameraAngle.Value = this.numericKameraAngle.Value + 360;

                double angleCamDegs = (double)this.numericKameraAngle.Value;
                cam.RotateUp(angleCamDegs);

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



        /// ////////////////////////////////////////////////////////// C U S T O M    O B J E C T 
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 
        private void actionCustomSet(object sender, EventArgs e)
        {
            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingCustom))
                return;

            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            DrawingCustom drCust = (DrawingCustom)_currentlyDisplayed;
            CustomObject cust = (CustomObject)drCust.ModelObject;

            Vektor center = new Vektor(
                (double)this.numCustomCenterX.Value,
                (double)this.numCustomCenterY.Value,
                (double)this.numCustomCenterZ.Value);

            Material mat = cust.Material;
            mat.Ka = (double)this.numCustomKa.Value;
            mat.Ks = (double)this.numCustomKs.Value;
            mat.Kd = (double)this.numCustomKd.Value;
            mat.KT = (double)this.numCustomKt.Value;
            mat.SpecularExponent = (int)this.numCustomH.Value;
            mat.N = (double)this.numCustomN.Value;

            mat.Color.R = (double)this.numCustomColR.Value;
            mat.Color.G = (double)this.numCustomColG.Value;
            mat.Color.B = (double)this.numCustomColB.Value;

            drCust.SetMaterial2All(mat);
            //cust.SetCenter(center);

            btnCustomColor.BackColor = mat.Color.SystemColor();

            WndBoard wndB = GetWndBoard();
            //drCust.SetModelObject(cust);
            center = wndB.RotationMatrix.Transform2NewPoint(center);
            drCust.Center = center;
            
            //drCust.ApplyRotationMatrix(wndB.RotationMatrix);
            WndScene wndSc = GetWndScene();
            wndSc.UpdateRecords();
        }

        private void btnCustomObjColor_Click(object sender, EventArgs e)
        {
            ParentEditor parrent = (ParentEditor)this.ParentForm;
            //parrent.colorDialog1.ShowDialog(this);

            if (colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                double r = colorDialog.Color.R / (double)255;
                double g = colorDialog.Color.G / (double)255;
                double b = colorDialog.Color.B / (double)255;
                double a = colorDialog.Color.A / (double)255;

                RayTracerLib.Colour col = new RayTracerLib.Colour(r, g, b, a);

                _permissionToModify = false;
                this.numCustomColR.Value = (decimal)col.R;
                this.numCustomColG.Value = (decimal)col.G;
                _permissionToModify = true;
                this.numCustomColB.Value = (decimal)col.B;

                Button btn = sender as Button;
                btn.BackColor = colorDialog.Color;
            }
        }


        private void OnLabelLeave(object sender, EventArgs e)
        {
            TextBox txtb = sender as TextBox;
            txtb.BackColor = SystemColors.Window;
        }

        private void OnLabelTextChange(object sender, EventArgs e)
        {
            TextBox txtb = sender as TextBox;
            if (!(_currentlyDisplayed is DrawingObject)) return;
            DrawingObject drob = _currentlyDisplayed as DrawingObject;
            if (!DrawingObject.IsAvailable(txtb.Text) && drob.Label != txtb.Text)
            {
                txtb.BackColor = Color.Red;
            }
            else
            {
                txtb.BackColor = SystemColors.Window;
            }

        }

        private void OnLabelKeyDown(object sender, KeyEventArgs e)
        {
            TextBox txtb = sender as TextBox;
            if (!(_currentlyDisplayed is DrawingObject)) return;
            DrawingObject drob = _currentlyDisplayed as DrawingObject;

            if (e.KeyCode == Keys.Enter)
            {
                if (DrawingObject.IsAvailable(txtb.Text))
                {
                    drob.Label = txtb.Text;
                    WndScene wndSc = GetWndScene();
                    wndSc.UpdateRecords();
                }
                else
                    txtb.Text = drob.Label;

                Control c = this.GetNextControl((Control)sender, true); // ztrata focusu - preneseni na label
                c.Focus();
            }
        }

        /// <summary>
        /// Aktivace formulare vlastnosti. 
        /// Je potreba zkontrolovat, aby zobrazeny objekt byl stejny jako vybrany objekt v platne (Boardu)
        /// </summary>
        private void OnFormActivated(object sender, EventArgs e)
        {
            WndBoard wndBoard = GetWndBoard();
            if (_currentlyDisplayed != null && _currentlyDisplayed is DrawingObject && wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }
        }

        private void OnChbCustFillChange(object sender, EventArgs e)
        {
            if (_currentlyDisplayed != null && _currentlyDisplayed is DrawingCustom)
            {
                CheckBox chb = sender as CheckBox;
                DrawingCustom drCust = _currentlyDisplayed as DrawingCustom;
                drCust.ShowFilled = chb.Checked;

            }
        }

        private void CustomReset(object sender, EventArgs e)
        {
            if (_currentlyDisplayed != null && _currentlyDisplayed is DrawingCustom)
            {
                DrawingCustom drCust = _currentlyDisplayed as DrawingCustom;
                drCust.Reset();
                WndBoard wnd = GetWndBoard();
                wnd.RotationMatrix.TransformPoints(drCust.Points);
            }
        }

        private void OptimizeRadioChanged(object sender, EventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            RayImage img = _currentlyDisplayed as RayImage;
            if (radio != null && img != null && radio.Tag != null)
                img.OptimizType = (Optimalizer.OptimizeType)radio.Tag;

        }

        private void actionImageOptimCheckChange(object sender, EventArgs e)
        {
            CheckBox chb = sender as CheckBox;
            panelOptimGroup.Visible = chb.Checked;
            RayImage img = _currentlyDisplayed as RayImage;
            img.IsOptimalizing = chb.Checked;
            if (!chb.Checked)
            {
                img.OptimizType = Optimalizer.OptimizeType.NONE;
            }
            else
            {
                img.OptimizType = Optimalizer.OptimizeType.RTREE;
            }
            ShowImage(img);
        }

        private void OnTriangDivide(object sender, EventArgs e)
        {
            if (_currentlyDisplayed != null && _currentlyDisplayed is DrawingFacet)
            {
                DrawingFacet drFacet = _currentlyDisplayed as DrawingFacet;
                WndBoard wnd = GetWndBoard();
                //Matrix3D transp = wnd.RotationMatrix.Transpose();
                //transp.TransformPoints(drFacet.DrCustObject.Points);
                drFacet.Split(wnd.RotationMatrix);
                wnd.RotationMatrix.TransformPoints(drFacet.DrCustObject.Points);
            }
        }

        private void OnTriangDelete(object sender, EventArgs e)
        {
            if (_currentlyDisplayed != null && _currentlyDisplayed is DrawingFacet)
            {
                DrawingFacet drFacet = _currentlyDisplayed as DrawingFacet;
                WndBoard wnd = GetWndBoard();
                //Matrix3D transp = wnd.RotationMatrix.Transpose();
                //transp.TransformPoints(drFacet.DrCustObject.Points);
                drFacet.Delete(wnd.RotationMatrix);
                wnd.RotationMatrix.TransformPoints(drFacet.DrCustObject.Points);
            }
        }

        private void actionCustRotate(object sender, EventArgs e)
        {

            if (_currentlyDisplayed == null || _currentlyDisplayed.GetType() != typeof(DrawingCustom))
                return;

            if (!_permissionToModify)
                return;

            // modifikovany objekt musi byt stejny, jako prave vybrany objekt na platne (Boardu)
            WndBoard wndBoard = GetWndBoard();
            if (wndBoard._Selected != (DrawingObject)_currentlyDisplayed)
            {
                wndBoard.SetObjectSelected((DrawingObject)_currentlyDisplayed);
            }

            _permissionToModify = false;

            NumericUpDown num = sender as NumericUpDown;
            if (num.Value > 359)
                num.Value = num.Value % 360;
            else if (num.Value < 0)
                num.Value = 360 + num.Value;

            DrawingCustom drCust = _currentlyDisplayed as DrawingCustom;

            double[] angles = new double[]{
                (double)this.numCustomRotX.Value,
                (double)this.numCustomRotY.Value,
                (double)this.numCustomRotZ.Value};

            WndBoard wnd = GetWndBoard();
            Matrix3D transp = wnd.RotationMatrix.Transpose();
            transp.TransformPoints(drCust.Points);
            drCust.Rotate(angles[0], angles[1], angles[2]);
            wnd.RotationMatrix.TransformPoints(drCust.Points);

            _showAngles = false;
            this.ShowCustom(drCust);
            _permissionToModify = true;
            _showAngles = true;
        }
        

    }
}
