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
    public partial class WndScene : Form
    {
        public bool BlinkActivate = true;
        enum TreeNodeTypes
        {
            Objects, Lights, Camera, Images, Animations, // top level nodes
            Spheres, Planes, Cubes, Cylinders, Triangles,
            Cones,
            Custom
        }

        /// <summary>
        /// zda je prave meneho zaskrtnuti pro vsechny sousedy
        /// </summary>
        private bool isChecking = false;

        public WndScene()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.Selectable, true);

            this.Focus();
            this.Update();

            FillTree();
        }
        /// <summary>
        /// Vyprazdni cely seznam
        /// </summary>
        public void ClearAll()
        {
            this.treeView1.Nodes.Clear();
            FillTree();
        }

        private void FillTree()
        {
            TreeNode nodeObjects = new TreeNode(TreeNodeTypes.Objects.ToString());
            nodeObjects.Tag = TreeNodeTypes.Objects;
            nodeObjects.Checked = true;

            TreeNode nodeSpheres = new TreeNode(TreeNodeTypes.Spheres.ToString());
            nodeSpheres.Tag = TreeNodeTypes.Spheres;
            TreeNode nodePlanes = new TreeNode(TreeNodeTypes.Planes.ToString());
            nodePlanes.Tag = TreeNodeTypes.Planes;
            TreeNode nodeCubes = new TreeNode(TreeNodeTypes.Cubes.ToString());
            nodeCubes.Tag = TreeNodeTypes.Cubes;
            TreeNode nodeCyls = new TreeNode(TreeNodeTypes.Cylinders.ToString());
            nodeCyls.Tag = TreeNodeTypes.Cylinders;
            TreeNode nodeTriangs = new TreeNode(TreeNodeTypes.Triangles.ToString());
            nodeTriangs.Tag = TreeNodeTypes.Triangles;
            TreeNode nodeCones = new TreeNode(TreeNodeTypes.Cones.ToString());
            nodeCones.Tag = TreeNodeTypes.Cones;
            TreeNode nodeCustoms = new TreeNode(TreeNodeTypes.Custom.ToString());
            nodeCustoms.Tag = TreeNodeTypes.Custom;

            nodeObjects.Nodes.Add(nodeSpheres);
            nodeObjects.Nodes.Add(nodePlanes);
            nodeObjects.Nodes.Add(nodeCubes);
            nodeObjects.Nodes.Add(nodeCyls);
            nodeObjects.Nodes.Add(nodeCones);
            nodeObjects.Nodes.Add(nodeTriangs);
            nodeObjects.Nodes.Add(nodeCustoms);

            TreeNode nodeLights = new TreeNode(TreeNodeTypes.Lights.ToString());
            nodeLights.Tag = TreeNodeTypes.Lights;

            TreeNode nodeCameras = new TreeNode(TreeNodeTypes.Camera.ToString());
            nodeCameras.Tag = TreeNodeTypes.Camera;

            TreeNode nodeImages = new TreeNode(TreeNodeTypes.Images.ToString());
            nodeImages.Tag = TreeNodeTypes.Images;
            nodeImages.Checked = true;

            TreeNode nodeAnimations = new TreeNode(TreeNodeTypes.Animations.ToString());
            nodeAnimations.Tag = TreeNodeTypes.Animations;

            this.treeView1.Nodes.Add(nodeObjects);
            this.treeView1.Nodes.Add(nodeLights);
            this.treeView1.Nodes.Add(nodeCameras);
            this.treeView1.Nodes.Add(nodeImages);
            this.treeView1.Nodes.Add(nodeAnimations);
        }

        private void AddItem(object obj, TreeNodeTypes typ)
        {
            //TreeNode node = new TreeNode(obj.ToString());
            //node.Tag = obj;
            //this.treeView1.Nodes.Add(node);

            TreeNodeTypes rootTyp = typ;
            if (typ == TreeNodeTypes.Cubes || typ == TreeNodeTypes.Spheres ||
                typ == TreeNodeTypes.Planes || typ == TreeNodeTypes.Cylinders ||
                typ == TreeNodeTypes.Triangles || typ == TreeNodeTypes.Cones ||
                typ == TreeNodeTypes.Custom)
                rootTyp = TreeNodeTypes.Objects;

            foreach (TreeNode node in treeView1.Nodes)
            {
                if (node.Tag == null)
                    continue;

                if ((TreeNodeTypes)node.Tag == rootTyp && rootTyp == TreeNodeTypes.Objects)
                {
                    foreach (TreeNode node2 in node.Nodes)
                    {
                        if (node2.Tag == null)
                            continue;

                        if ((TreeNodeTypes)node2.Tag == typ)
                        {
                            DrawingObject drobj = (DrawingObject)obj;
                            //DefaultShape ds = (DefaultShape)obj;
                            DefaultShape ds = (DefaultShape)drobj.ModelObject;
                            TreeNode novyNode = new TreeNode(ds.ToString());
                            novyNode.Tag = obj;
                            if (ds.IsActive)
                            {
                                novyNode.Checked = true;
                                isChecking = true;
                                node2.Checked = true;
                                isChecking = false;
                            }
                            

                            node2.Nodes.Add(novyNode);
                        }
                    }
                }

                else if((TreeNodeTypes)node.Tag == rootTyp && rootTyp == TreeNodeTypes.Lights)
                {
                    DrawingLight drLight = (DrawingLight)obj;
                    //Light light = (Light)obj;
                    Light light = (Light)drLight.ModelObject;
                    TreeNode novyNode = new TreeNode(obj.ToString());
                    novyNode.Tag = obj;
                    if (light.IsActive)
                    {
                        novyNode.Checked = true;
                        isChecking = true;
                        node.Checked = true;
                        isChecking = false;
                    }
                    node.Nodes.Add(novyNode);
                }
                else if ((TreeNodeTypes)node.Tag == rootTyp && rootTyp == TreeNodeTypes.Camera)
                {
                    DrawingCamera drCam = (DrawingCamera)obj;
                    //Camera cam = (Camera)obj;
                    Camera cam = (Camera)drCam.ModelObject;
                    TreeNode novyNode = new TreeNode(cam.ToString());
                    novyNode.Tag = drCam;
                    novyNode.Checked = true;
                    node.Checked = true;
                    node.Nodes.Add(novyNode);
                }
                else if ((TreeNodeTypes)node.Tag == rootTyp && rootTyp == TreeNodeTypes.Images)
                {
                    RayImage img = (RayImage)obj;
                    TreeNode novyNode = new TreeNode(img.ToString());
                    novyNode.Tag = img;
                    if (node.Nodes.Count == 0)
                        novyNode.Checked = true;
                    //node.Checked = true;
                    node.Nodes.Add(novyNode);
                }
                else if ((TreeNodeTypes)node.Tag == rootTyp && rootTyp == TreeNodeTypes.Animations)
                {
                    DrawingAnimation drAnim = (DrawingAnimation)obj;
                    TreeNode novyNode = new TreeNode(drAnim.ToString());
                    novyNode.Tag = drAnim;
                    node.Nodes.Add(novyNode);
                    if (node.Nodes.Count == 1)
                    {
                        isChecking = true;
                        node.Checked = true;
                        isChecking = false;
                    }
                    novyNode.Checked = drAnim.ShowAnimation;
                    //node.Checked = true;
                    
                }
                else if ((TreeNodeTypes)node.Tag == rootTyp)
                {
                    TreeNode novyNode = new TreeNode(obj.ToString());
                    novyNode.Tag = obj;
                    novyNode.Checked = true;
                    node.Checked = true;
                    node.Nodes.Add(novyNode);
                }
            }
        }

        /// <summary>
        /// Vsechna data aktualizuje
        /// </summary>
        public void UpdateRecords()
        {
            foreach (TreeNode node in treeView1.Nodes)
            {
                UpdateRecords(node);
            }
            //this.treeView1.Focus();

            WndProperties prop = GetWndProperties();
            prop.ShowObject(treeView1.SelectedNode.Tag);

            this.Update();
        }

        private void UpdateRecords(TreeNode rootNode)
        {
            foreach (TreeNode childNode in rootNode.Nodes)
            {
                childNode.Text = childNode.Tag.ToString();
                UpdateRecords(childNode);
            }
        }

        /// <summary>
        /// Prida do seznamu objekt ze sveta Raytraceru: 
        /// koule, rovina, valec, krychle, svetlo, kamera, image, animation
        /// </summary>
        public void AddItem(DrawingObject drawObj)
        {
            if (drawObj.GetType() == typeof(DrawingSphere))
            {
                this.AddItem(drawObj, TreeNodeTypes.Spheres);
            }
            else if (drawObj.GetType() == typeof(DrawingPlane))
            {
                this.AddItem(drawObj, TreeNodeTypes.Planes);
            }
            else if (drawObj.GetType() == typeof(DrawingCube))
            {
                this.AddItem(drawObj, TreeNodeTypes.Cubes);
            }
            else if (drawObj.GetType() == typeof(DrawingCylinder))
            {
                this.AddItem(drawObj, TreeNodeTypes.Cylinders);
            }
            else if (drawObj.GetType() == typeof(DrawingTriangle))
            {
                this.AddItem(drawObj, TreeNodeTypes.Triangles);
            }
            else if (drawObj.GetType() == typeof(DrawingCustom))
            {
                this.AddItem(drawObj, TreeNodeTypes.Custom);
            }
            else if (drawObj.GetType() == typeof(DrawingCone))
            {
                this.AddItem(drawObj, TreeNodeTypes.Cones);
            }
            else if (drawObj.GetType() == typeof(DrawingAnimation))
            {
                this.AddItem(drawObj, TreeNodeTypes.Animations);
            }
        }

        public void AddItem(DrawingLight light)
        {
            this.AddItem(light, TreeNodeTypes.Lights);
        }
        public void AddItem(DrawingCamera cam)
        {
            this.AddItem(cam, TreeNodeTypes.Camera);
        }
        public void AddItem(RayImage img)
        {
            this.AddItem(img, TreeNodeTypes.Images);
        }

        private void AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (BlinkActivate == false) return;

            //this.AddItem(new RayTracerLib.Sphere(new Vektor(), 1));
            TreeNode node = e.Node;
            if (node.Tag == null)
                return;

            ParentEditor form = (ParentEditor)this.ParentForm;
            // jen kamera i pro jeji root uzel
            if (node.Tag.GetType() == typeof(TreeNodeTypes) && (TreeNodeTypes)node.Tag == TreeNodeTypes.Camera)
            {
                DrawingObject dro = (DrawingObject)node.Nodes[0].Tag;
                form._WndProperties.ShowObject(dro);
                form._WndBoard.SetObjectSelected(dro);
                return;
            }
            // kdyz se jedna o konkretni instanci objektu v seznamu - neni to obecna skupina
            if (node.Tag.GetType() != typeof(TreeNodeTypes))
            {
                form._WndProperties.ShowObject(node.Tag);
                // zviditelni vykreslovany objekt, ktery byl vybran ze seznamu objektu
                if (node.Tag is DrawingObject)
                {
                    DrawingObject dro = (DrawingObject)node.Tag;
                    form._WndBoard.SetObjectSelected(dro);
                }
            }
            else
            {
                TreeNodeTypes typ = (TreeNodeTypes)node.Tag;
                form._WndProperties.ShowObject(node.Tag);
            }
        }

        public void AddImages(RayImage[] rayImgs)
        {
            this.BlinkActivate = false;
            foreach (RayImage img in rayImgs)
            {
                this.AddItem(img);
            }
            this.BlinkActivate = true;

        }

        private void ShowNode(object shape, TreeNode rootNode)
        {
            
            if (rootNode.Nodes == null)
                return;

            //rootNode.ExpandAll();

            if (shape is DrawingFacet)
            {
                DrawingFacet drF = shape as DrawingFacet;
                BlinkActivate = false;
                this.ShowNode(drF.DrCustObject);
                BlinkActivate = true;
                ParentEditor form = (ParentEditor)this.ParentForm;
                form._WndProperties.ShowObject((DrawingTriangle)shape);
                return;
            }
            foreach (TreeNode node in rootNode.Nodes)
            {
                // zjisteni dedicneho typu: zda-li je node.Tag zdedeny typ od DefaultShape

                //if (node.Tag is DrawingLight)
                //{
                //    if (node.Tag == shape)
                //    {
                //        treeView1.SelectedNode = null;
                //        treeView1.SelectedNode = node;
                //        node.Text = node.Tag.ToString();
                //        //this.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                //        //this.OnClicked(this, new EventArgs());
                //        //this.onMouseDown(this, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                //        //this.treeView1.Focus();
                //        //this.treeView1.HideSelection = false;
                //        //this.OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                //        //this.Update();
                //    }
                //}
                //else if (node.Tag is DrawingCamera)
                //{
                //    if (node.Tag == shape)
                //    {
                //        treeView1.SelectedNode = null;
                //        treeView1.SelectedNode = node;
                //        node.Text = node.Tag.ToString();
                //        this.treeView1.Focus();
                //        this.treeView1.HideSelection = false;
                //    }
                //}
                //else if (node.Tag is DrawingDefaultShape)
                //{
                //    if (node.Tag == shape)
                //    {
                //        //treeView1.SelectedNode = null;
                //        treeView1.SelectedNode = node;
                //        //node.Text = node.Tag.ToString();
                //        //this.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                //        //this.OnClicked(this, new EventArgs());
                //        //this.onMouseDown(this, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                //        //this.treeView1.Focus();
                //        //this.treeView1.HideSelection = false;
                //        //this.OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                //        //this.Update();
                //    }
                //}
                //else if (node.Tag is DrawingAnimation)
                //{
                //    if (node.Tag == shape)
                //    {
                //        treeView1.SelectedNode = null;
                //        treeView1.SelectedNode = node;
                //        node.Text = node.Tag.ToString();
                //        this.treeView1.Focus();
                //        this.treeView1.HideSelection = false;
                //    }
                //}
                //else if (node.Tag is RayImage)
                //{
                //    if (node.Tag == shape)
                //    {
                //        treeView1.SelectedNode = node;
                //        node.Text = node.Tag.ToString();
                //        this.treeView1.Focus();
                //        this.treeView1.HideSelection = false;
                //    }
                //}
                if (node.Tag == shape)
                {
                    treeView1.SelectedNode = null;
                    treeView1.SelectedNode = node;
                    node.Text = node.Tag.ToString();
                    this.treeView1.Focus();
                    this.treeView1.HideSelection = false;
                }
                else
                {
                    ShowNode(shape, node);
                }
            }
        }
        /// <summary>
        /// najde a vybere dany objekt v seznamu - ve strome objektu
        /// prochazi uzly stromu sceny a porovnava, jestli se shoduji se zadanym, ktery chceme zobrazit
        /// </summary>
        /// <param name="shape">bud: DefaultShape, Light</param>
        public void ShowNode(object shape)
        {
            foreach (TreeNode node in treeView1.Nodes)
            {
                if (((TreeNodeTypes)node.Tag == TreeNodeTypes.Objects) && (shape is DrawingObject))
                {
                    ShowNode(shape, node);
                }
                else if (( (TreeNodeTypes)node.Tag == TreeNodeTypes.Lights ) && ( shape is DrawingLight ))
                {
                    ShowNode(shape, node);
                }
                else if (((TreeNodeTypes)node.Tag == TreeNodeTypes.Camera) && (shape is DrawingCamera))
                {
                    ShowNode(shape, node);
                }
                else if (((TreeNodeTypes)node.Tag == TreeNodeTypes.Images) && (shape is RayImage))
                {
                    ShowNode(shape, node);
                }
                else if (((TreeNodeTypes)node.Tag == TreeNodeTypes.Animations) && (shape is DrawingAnimation))
                {
                    ShowNode(shape, node);
                }
            }
            
        }

        public Size GetSize()
        {
            return treeView1.Size;
        }
        private void OnClicked(object sender, EventArgs e)
        {

        }

        private void onMouseDown(object sender, MouseEventArgs e)
        {
            int  a = 1;
        }

        
        public void CollapseAll()
        {
            this.treeView1.CollapseAll();
            this.treeView1.Nodes[0].Collapse();
            //this.treeView1.Nodes[0].Collapse(false);
        }

        public void ExpandAll()
        {
            this.treeView1.ExpandAll();
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            SetChildNodes(treeView1.Nodes[0]);

        }

        private void AfterCheck(object sender, TreeViewEventArgs e)
        {

            if (e.Node.Tag is DrawingDefaultShape)
            {
                DrawingDefaultShape dds = (DrawingDefaultShape)e.Node.Tag;
                DefaultShape ds = (DefaultShape)dds.ModelObject;
                ds.IsActive = e.Node.Checked;
                if (e.Node.Checked)
                {
                    isChecking = true;
                    e.Node.Parent.Checked = true;
                    isChecking = false;
                }
                ParentEditor pe = (ParentEditor)this.ParentForm;
                pe._WndBoard.Redraw();
                this.Invalidate();
                this.Update();
            }
            else if (e.Node.Tag is DrawingLight)
            {
                DrawingLight dl = (DrawingLight)e.Node.Tag;
                Light l = (Light)dl.ModelObject;
                l.IsActive = e.Node.Checked;
                if (e.Node.Checked)
                {
                    isChecking = true;
                    e.Node.Parent.Checked = true;
                    isChecking = false;
                }
                ParentEditor pe = (ParentEditor)this.ParentForm;
                pe._WndBoard.Redraw();
                this.Invalidate();
                this.Update();
            }
            else if (e.Node.Tag is DrawingAnimation)
            {
                //if (!isChecking && e.Node.Checked)
                //{
                //    isChecking = true;
                //    this.UncheckChildren(e.Node.Parent);
                //    e.Node.Checked = true;
                //    isChecking = false;
                DrawingAnimation drAnim;
                if (!_checkingFromParent)
                {
                    drAnim = GetSelectedAnimation();
                    if (drAnim == null)
                    {
                        isChecking = true;
                        e.Node.Parent.Checked = false;
                        isChecking = false;
                    }
                    else
                    {
                        isChecking = true;
                        e.Node.Parent.Checked = true;
                        isChecking = false;
                    }
                }
                drAnim = (DrawingAnimation)e.Node.Tag;
                drAnim.ShowAnimation = e.Node.Checked;
                //}
            }
            else if (e.Node.Tag is RayImage)
            {
                if (!isChecking)
                {
                    isChecking = true;
                    this.UncheckChildren(e.Node.Parent);
                    e.Node.Checked = true;
                    isChecking = false;
                }
            }
            
            else
            {
                if (((TreeNodeTypes)e.Node.Tag) == TreeNodeTypes.Animations && e.Node.Nodes.Count > 0)
                {
                    ParentEditor pe = (ParentEditor)this.ParentForm;
                    pe.SetAnimationEnabled(e.Node.Checked);
                }
                if (!isChecking) // zaskrtnut korenovy uzel, takze se musi podle nej nastavit vsichni jeho potomci
                {
                    //if (((TreeNodeTypes)e.Node.Tag) == TreeNodeTypes.Animations && e.Node.Nodes.Count > 0)
                    //{
                    //    ParentEditor pe = (ParentEditor)this.ParentForm;
                    //    pe.SetAnimationEnabled(e.Node.Checked);
                    //}
                     
                    SetChildNodes(e.Node);
                }
            }

        }

        /// <summary>
        /// nastavovani od otce. Je-li TRUE, tak syn nesmi menit check otci
        /// </summary>
        bool _checkingFromParent;
        private void SetChildNodes(TreeNode root)
        {
            _checkingFromParent = true;
            foreach (TreeNode node in root.Nodes)
            {
                node.Checked = root.Checked;
                SetChildNodes(node);
            }
            _checkingFromParent = false;
        }
        private void NodeMouseDblClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }

        private void BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            
            if (e.Node.Tag is TreeNodeTypes && (TreeNodeTypes)e.Node.Tag == TreeNodeTypes.Images)
            {
                e.Cancel = true;
            }

            else if (e.Node.Tag is TreeNodeTypes && (TreeNodeTypes)e.Node.Tag == TreeNodeTypes.Animations)
            {

                if (!isChecking && e.Node.Nodes.Count == 0)
                    e.Cancel = true;
            }
            else if (e.Node.Tag is DrawingCamera)
            {
                e.Cancel = true;
            }

            // osetreni, aby kamera byla porad zaskrknuta v seznamu
            if (e.Node.Tag is TreeNodeTypes && 
            (TreeNodeTypes)e.Node.Tag == TreeNodeTypes.Camera && 
            e.Node.Checked == true)
                e.Cancel = true;

            
            //if (e.Node.Tag is DrawingObject)
            //{
            //    DrawingObject dro = (DrawingObject)e.Node.Tag;
            //    if (dro.ModelObject is DefaultShape)
            //    {
            //        DefaultShape ds = (DefaultShape)dro.ModelObject;
            //        if (e.Node.Checked != ds.IsActive)
            //            e.Node.Checked = ds.IsActive;
            //        ParentEditor pe = (ParentEditor)this.ParentForm;
            //        pe._wndBoard.Redraw();
            //        this.Invalidate();
            //        this.Update();
            //    }
            //}
        }

        private WndBoard GetWndBoard()
        {
            ParentEditor pf = (ParentEditor)this.ParentForm;
            return pf._WndBoard;
        }

        private WndProperties GetWndProperties()
        {
            ParentEditor form = (ParentEditor)this.ParentForm;
            return form._WndProperties;
        }

        private void OnRemoveObjectFromScene(object sender, EventArgs e)
        {
            // kdyz je to obecny typ, nemazeme nic
            if (treeView1.SelectedNode.Tag is TreeNodeTypes || treeView1.SelectedNode.Tag is DrawingCamera)
                return;

            // kdyz chce odstranit posledni obrazek, nesmazeme ho
            if (treeView1.SelectedNode.Tag is RayImage)
            {
                if (treeView1.SelectedNode.Parent.Nodes.Count == 1)
                    return;
                else if (treeView1.SelectedNode.Checked)
                {
                    TreeNode neigh = treeView1.SelectedNode.PrevNode;
                    if (neigh == null) neigh = treeView1.SelectedNode.NextNode;
                    treeView1.Nodes.Remove(treeView1.SelectedNode);
                    neigh.Checked = true;
                    return;
                }
            }
            
            // kdyz byl z podstromu odstranen posledni uzel, odskrtneme uzel otce
            if (treeView1.SelectedNode.Parent.Nodes.Count == 1)
            {
                isChecking = true;
                treeView1.SelectedNode.Parent.Checked = false;
                isChecking = false;
            }

            WndBoard wndBoard = GetWndBoard();
            wndBoard.RemoveRaytrObject(treeView1.SelectedNode.Tag);// musi byt pred odstranenim z Editoru

            treeView1.Nodes.Remove(treeView1.SelectedNode); // musi byt zachovano poradi

            WndProperties wndProp = GetWndProperties();
            // v okne Properties zobrazime bud zakladni obrazovku, nebo vlastnosti dalsiho prvku
            // Muze se ve strome vybrat bud dalsi prvek, nebo koren stejneho podstromu
            if (treeView1.SelectedNode.Tag is TreeNodeTypes)
                wndProp.ShowDefault();      // vybran koren - bud prazdny podstrom, nebo nemusi
            else
                wndProp.ShowObject(treeView1.SelectedNode.Tag); // vybran dalsi prvek ze stejneho podstromu
        }

        private void OnAddSphere(object sender, EventArgs e)
        {
            WndBoard wndBoard = GetWndBoard();
            Sphere sph = new Sphere(new Vektor(), 1);
            wndBoard.AddRaytrObject(sph);
            ParentEditor pared = (ParentEditor)this.ParentForm;
            pared.AddRaytrObject(sph);
        }

        private void OnAddPlane(object sender, EventArgs e)
        {
            WndBoard wndBoard = GetWndBoard();
            Plane plane = new Plane(new Vektor(1, 0, 0), 2);
            wndBoard.AddRaytrObject(plane);
            ParentEditor pared = (ParentEditor)this.ParentForm;
            pared.AddRaytrObject(plane);
        }

        private void onAddCube(object sender, EventArgs e)
        {
            WndBoard wndBoard = GetWndBoard();
            Cube cube = new Cube(new Vektor(), new Vektor(1, 0, 0), 1);
            wndBoard.AddRaytrObject(cube);
            ParentEditor pared = (ParentEditor)this.ParentForm;
            pared.AddRaytrObject(cube);
        }

        private void onAddCylinder(object sender, EventArgs e)
        {
            WndBoard wndBoard = GetWndBoard();
            Cylinder cyl = new Cylinder(new Vektor(), new Vektor(1, 0, 0), 1, 5);
            wndBoard.AddRaytrObject(cyl);
            ParentEditor pared = (ParentEditor)this.ParentForm;
            pared.AddRaytrObject(cyl);
        }
        private void onAddTriangle(object sender, EventArgs e)
        {
            WndBoard wndBoard = GetWndBoard();
            Triangle triangl = new Triangle();
            wndBoard.AddRaytrObject(triangl);
            ParentEditor pared = (ParentEditor)this.ParentForm;
            pared.AddRaytrObject(triangl);
        }
        private void onAddCone(object sender, EventArgs e)
        {
            WndBoard wndBoard = GetWndBoard();
            Cone cone = new Cone(new Vektor(), new Vektor(1, 0, 0), 2, 5);
            wndBoard.AddRaytrObject(cone);
            ParentEditor pared = (ParentEditor)this.ParentForm;
            pared.AddRaytrObject(cone);
        }
        private void onAddLight(object sender, EventArgs e)
        {
            WndBoard wndBoard = GetWndBoard();
            Light l = new Light();
            wndBoard.AddRaytrObject(l);
            ParentEditor pared = (ParentEditor)this.ParentForm;
            pared.AddRaytrObject(l);
        }

        private void onAddImage(object sender, EventArgs e)
        {
            RayImage img = new RayImage(1, new Colour(1, 0, 0, 0), false);
            this.AddItem(img, TreeNodeTypes.Images);
            this.ShowNode(img);
        }

        private void onAddAnimation(object sender, EventArgs e)
        {
            Animation anim = new Animation();
            WndBoard wndBoard = GetWndBoard();
            wndBoard.AddAnimation(anim);
        }

        private void onAddCustomObject(object sender, EventArgs e)
        {
            WndBoard wndBoard = GetWndBoard();
            CustomObject cust = CustomObject.CreateCube();
            wndBoard.AddRaytrObject(cust);
            ParentEditor pared = (ParentEditor)this.ParentForm;
            pared.AddRaytrObject(cust);
        }
        private void onAddCustomPlane(object sender, EventArgs e)
        {
            WndBoard wndBoard = GetWndBoard();
            CustomObject cust = CustomObject.CreatePlane();
            wndBoard.AddRaytrObject(cust);
            ParentEditor pared = (ParentEditor)this.ParentForm;
            pared.AddRaytrObject(cust);
        }

        /// <summary>
        /// vrati Vsechny RayImg vytvorene ve scene
        /// </summary>
        public RayImage[] GetImages()
        {
            List<RayImage> imgList = new List<RayImage>();

            foreach (TreeNode node in treeView1.Nodes)
            {
                if ((TreeNodeTypes)node.Tag == TreeNodeTypes.Images)
                {
                    foreach (TreeNode n in node.Nodes)
                    {
                        imgList.Add((RayImage)n.Tag);
                    }
                }
            }
            return imgList.ToArray();
        }
        public RayImage GetSelectedImage()
        {
            RayImage sel = null;

            foreach (TreeNode node in treeView1.Nodes)
            {
                if ((TreeNodeTypes)node.Tag == TreeNodeTypes.Images)
                {
                    foreach (TreeNode n in node.Nodes)
                    {
                        if (n.Checked)
                            sel = (RayImage)n.Tag;
                    }
                }
            }
            return sel;
        }


        private void UncheckChildren(TreeNode root)
        {
            foreach (TreeNode node in root.Nodes)
                node.Checked = false;
        }

        private void CheckChildren(TreeNode root)
        {
            foreach (TreeNode node in root.Nodes)
                node.Checked = true;
        }


        /// <summary>
        /// udalost pred zavrenim formulare
        /// </summary>
        private void BeforeClosing(object sender, FormClosingEventArgs e)
        {
            // pouze pri zavreni od uzivatele se formular nezavre
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }

        internal DrawingAnimation GetSelectedAnimation()
        {
            DrawingAnimation sel = null;

            foreach (TreeNode node in treeView1.Nodes)
            {
                if ((TreeNodeTypes)node.Tag == TreeNodeTypes.Animations)
                {
                    foreach (TreeNode n in node.Nodes)
                    {
                        if (n.Checked)
                            sel = (DrawingAnimation)n.Tag;
                    }
                }
            }
            return sel;
        }

        internal Animation[] GetAnimations()
        {
            List<Animation> anims = new List<Animation>();
            foreach (TreeNode node in treeView1.Nodes)
            {
                if ((TreeNodeTypes)node.Tag == TreeNodeTypes.Animations)
                {
                    foreach (TreeNode n in node.Nodes)
                    {
                        DrawingAnimation dranim = n.Tag as DrawingAnimation;
                        anims.Add((Animation)dranim.ModelObject);
                    }
                }
            }
            return anims.ToArray();
        }

        internal void AddAnimations(Animation[] anims)
        {
            if (anims == null) return;
            this.BlinkActivate = false;
            foreach (Animation anim in anims)
            {
                DrawingAnimation drAnim = new DrawingAnimation(anim);
                this.AddItem(drAnim);
            }
            this.BlinkActivate = true;
        }
    }
}
