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
    public partial class WndScene : Form
    {
        enum TreeNodeTypes { 
            Objects, Lights, Cameras, Images, Animations, // top level nodes
            Spheres, Planes, Cubes, Cylinders}

        public WndScene()
        {
            InitializeComponent();

            this.Focus();
            this.Update();

            FillTree();
        }
        private void FillTree()
        {
            TreeNode nodeObjects = new TreeNode(TreeNodeTypes.Objects.ToString());
            nodeObjects.Tag = TreeNodeTypes.Objects;
            
            TreeNode nodeSpheres = new TreeNode(TreeNodeTypes.Spheres.ToString());
            nodeSpheres.Tag = TreeNodeTypes.Spheres;
            TreeNode nodePlanes = new TreeNode(TreeNodeTypes.Planes.ToString());
            nodePlanes.Tag = TreeNodeTypes.Planes;
            TreeNode nodeCubes = new TreeNode(TreeNodeTypes.Cubes.ToString());
            nodeCubes.Tag = TreeNodeTypes.Cubes;
            TreeNode nodeCyls = new TreeNode(TreeNodeTypes.Cylinders.ToString());
            nodeCyls.Tag = TreeNodeTypes.Cylinders;

            nodeObjects.Nodes.Add(nodeSpheres);
            nodeObjects.Nodes.Add(nodePlanes);
            nodeObjects.Nodes.Add(nodeCubes);
            nodeObjects.Nodes.Add(nodeCyls);

            TreeNode nodeLights = new TreeNode(TreeNodeTypes.Lights.ToString());
            nodeLights.Tag = TreeNodeTypes.Lights;

            TreeNode nodeCameras = new TreeNode(TreeNodeTypes.Cameras.ToString());
            nodeCameras.Tag = TreeNodeTypes.Cameras;

            TreeNode nodeImages = new TreeNode(TreeNodeTypes.Images.ToString());
            nodeImages.Tag = TreeNodeTypes.Images;

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
                typ == TreeNodeTypes.Planes || typ == TreeNodeTypes.Cylinders)
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
                            DefaultShape ds = (DefaultShape)obj;
                            TreeNode novyNode = new TreeNode(obj.ToString());
                            novyNode.Tag = obj;
                            if (ds.IsActive)
                                novyNode.Checked = true;

                            node2.Nodes.Add(novyNode);
                        }
                    }
                }

                else if((TreeNodeTypes)node.Tag == rootTyp && rootTyp == TreeNodeTypes.Lights)
                {
                    Light light = (Light)obj;
                    TreeNode novyNode = new TreeNode(obj.ToString());
                    novyNode.Tag = obj;
                    if (light.IsActive)
                        novyNode.Checked = true;
                    node.Nodes.Add(novyNode);
                }
                else if ((TreeNodeTypes)node.Tag == rootTyp)
                {
                    TreeNode novyNode = new TreeNode(obj.ToString());
                    novyNode.Tag = obj;
                    node.Nodes.Add(novyNode);
                }
            }
        }

        /// <summary>
        /// Prida do seznamu objekt ze sveta Raytraceru: 
        /// koule, rovina, valec, krychle, svetlo, kamera, image, animation
        /// </summary>
        /// <param name="obj"></param>
        public void AddItem(DefaultShape obj)
        {
            if (obj.GetType() == typeof(RayTracerLib.Sphere))
            {
                this.AddItem(obj, TreeNodeTypes.Spheres);
            }
            else if (obj.GetType() == typeof(RayTracerLib.Plane))
            {
                this.AddItem(obj, TreeNodeTypes.Planes);
            }
            else if (obj.GetType() == typeof(RayTracerLib.Cube))
            {
                this.AddItem(obj, TreeNodeTypes.Cubes);
            }
            else if (obj.GetType() == typeof(RayTracerLib.Cylinder))
            {
                this.AddItem(obj, TreeNodeTypes.Cylinders);
            }
        }
        public void AddItem(Light obj)
        {
            this.AddItem(obj, TreeNodeTypes.Lights);
        }
        public void AddItem(RayImage obj)
        {
            this.AddItem(obj, TreeNodeTypes.Images);
        }
        public void AddItem(Animation obj)
        {
        }
        private void OnAfterSelect(object sender, TreeViewEventArgs e)
        {

            //this.AddItem(new RayTracerLib.Sphere(new Vektor(), 1));
            TreeNode node = e.Node;
            if (node.Tag == null)
                return;

            if (node.Tag.GetType() != typeof(TreeNodeTypes))
            {
                ParentEditor form = (ParentEditor)this.ParentForm;
                form._wndProperties.ShowObject(node.Tag);
                if (node.Tag is DefaultShape)
                {
                    DefaultShape ds = (DefaultShape)node.Tag;
                    form._wndBoard.SetObjectSelected((DefaultShape)node.Tag);
                    node.Checked = ds.IsActive;
                }
            }
            else
            {
                TreeNodeTypes typ = (TreeNodeTypes)node.Tag;
                ParentEditor form = (ParentEditor)this.ParentForm;
                form._wndProperties.ShowObject(node.Tag);
            }

        }

        private void ShowNode(object shape, TreeNode rootNode)
        {
            if (rootNode.Nodes == null)
                return;

            foreach (TreeNode node in rootNode.Nodes)
            {
                // zjisteni dedicneho typu: zda-li je node.Tag zdedeny typ od DefaultShape
                if (node.Tag is DefaultShape)       
                {
                    if (node.Tag == shape)
                    {
                        treeView1.SelectedNode = node;
                        this.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                        this.OnClicked(this, new EventArgs());
                        this.onMouseDown(this, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                        this.treeView1.Focus();
                        this.treeView1.HideSelection = false;
                        this.OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                        //this.OnGotFocus(new EventArgs());
                        //this.Activate();
                        //this.Focus();
                        this.Update();
                        //this.Validate();
                        //this.Refresh();
                    }
                }
                else if (node.Tag is Light)
                {
                    if (node.Tag == shape)
                    {
                        treeView1.SelectedNode = node;
                        this.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                        this.OnClicked(this, new EventArgs());
                        this.onMouseDown(this, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                        this.treeView1.Focus();
                        this.treeView1.HideSelection = false;
                        this.OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                        this.Update();
                    }
                }
                else
                {
                    ShowNode(shape, node);
                }
            }
        }
        /// <summary>
        /// najde a vybere dany objekt v seznamu - ve strome objektu
        /// </summary>
        /// <param name="shape">bud: DefaultShape, Light</param>
        public void ShowNode(object shape)
        {
            foreach (TreeNode node in treeView1.Nodes)
            {
                if (((TreeNodeTypes)node.Tag == TreeNodeTypes.Objects) && (shape is DefaultShape))
                {
                    ShowNode(shape, node);
                }
                else if (( (TreeNodeTypes)node.Tag == TreeNodeTypes.Lights ) && ( shape is Light ))
                {
                    ShowNode(shape, node);
                }
            }
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            SetChildNodes(treeView1.Nodes[0]);

        }

        private void AfterCheck(object sender, TreeViewEventArgs e)
        {

            if (e.Node.Tag is DefaultShape)
            {
                DefaultShape ds = (DefaultShape)e.Node.Tag;
                ds.IsActive = e.Node.Checked;
                ParentEditor pe = (ParentEditor)this.ParentForm;
                pe._wndBoard.Redraw();
                this.Invalidate();
                this.Update();
            }
            else if (e.Node.Tag is Light)
            {
                Light l = (Light)e.Node.Tag;
                l.IsActive = e.Node.Checked;
                ParentEditor pe = (ParentEditor)this.ParentForm;
                pe._wndBoard.Redraw();
                this.Invalidate();
                this.Update();
            }
            else
            {
                SetChildNodes(e.Node);
            }

        }

        private void SetChildNodes(TreeNode root)
        {
            foreach (TreeNode node in root.Nodes)
            {
                node.Checked = root.Checked;
                SetChildNodes(node);
            }
        }
        private void NodeMouseDblClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }

        private void BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Tag is DefaultShape)
            {
                DefaultShape ds = (DefaultShape)e.Node.Tag;
                if (e.Node.Checked != ds.IsActive)
                    e.Node.Checked = ds.IsActive;
                ParentEditor pe = (ParentEditor)this.ParentForm;
                pe._wndBoard.Redraw();
                this.Invalidate();
                this.Update();
            }
        }


    }
}
