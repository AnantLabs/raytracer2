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
                            TreeNode novyNode = new TreeNode(obj.ToString());
                            novyNode.Tag = obj;
                            node2.Nodes.Add(novyNode);
                        }
                    }
                }

                else if((TreeNodeTypes)node.Tag == rootTyp)
                {
                    TreeNode novyNode = new TreeNode(obj.ToString());
                    novyNode.Tag = obj;
                    node.Nodes.Add(novyNode);
                }
            }
        }

        public void AddItem(object obj)
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
            else if (obj.GetType() == typeof(RayTracerLib.Light))
            {
                this.AddItem(obj, TreeNodeTypes.Lights);
            }
        }
        private void OnAfterSelect(object sender, TreeViewEventArgs e)
        {

            this.AddItem(new RayTracerLib.Sphere(new Vektor(), 1));
            TreeNode node = e.Node;
            if (node.Tag == null)
                return;

            if (node.Tag.GetType() != typeof(TreeNodeTypes))
            {
                ParentEditor form = (ParentEditor)this.ParentForm;
                form._properties.ShowObject(node.Tag);
            }
            else
            {
                TreeNodeTypes typ = (TreeNodeTypes)node.Tag;
            }

            //try
            //{
            //    if ((TreeNodeTypes)node.Tag == TreeNodeTypes.Spheres ||
            //        (TreeNodeTypes)node.Tag == TreeNodeTypes.Planes ||
            //        (TreeNodeTypes)node.Tag == TreeNodeTypes.Cubes ||
            //        (TreeNodeTypes)node.Tag == TreeNodeTypes.Cylinders)
            //    {

            //    }
            //}
            //catch (InvalidCastException ex)
            //{
            //}
        }
    }
}
