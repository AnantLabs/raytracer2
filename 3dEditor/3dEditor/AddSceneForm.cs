using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mathematics;
using RayTracerLib;
using EditorLib;

namespace _3dEditor
{
    public partial class AddSceneForm : Form
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

        public AddSceneForm(Scene scene, RayImage[] rayImgs, Animation[] anims)
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.Selectable, true);

            this.Focus();
            this.Update();

            FillTree();

            AddScene(scene);
            AddImages(rayImgs);
            AddAnimations(anims);

            treeView1.ExpandAll();
        }


        private void AddScene(Scene scene)
        {
            if (scene.Camera != null)
                AddItem(scene.Camera);
            if (scene.Lights != null)
                foreach (Light l in scene.Lights)
                    if (l != null)
                        AddItem(l);
            if (scene.SceneObjects != null)
                foreach (DefaultShape ds in scene.SceneObjects)
                    if (ds != null)
                        AddItem(ds);
        }
        private void AddRayTracing(RayTracing raytr)
        {
            AddScene(raytr.RScene);
        }
        private void AddImages(RayImage[] imgs)
        {
            if (imgs == null) return;
            foreach (RayImage img in imgs)
            {
                if (img != null)
                    AddItem(img);
            }
        }
        private void AddAnimations(Animation[] anims)
        {
            if (anims == null) return;
            foreach (Animation anim in anims)
            {
                if (anim != null)
                    AddItem(anim);
            }
        }

        public Animation[] GetSelectedAnimations()
        {
            List<Animation> animList = new List<Animation>();
            foreach (TreeNode node in treeView1.Nodes)
            {
                if ((TreeNodeTypes)node.Tag == TreeNodeTypes.Animations)
                {
                    if (node.Nodes != null && node.Nodes.Count > 0)
                        foreach (TreeNode child in node.Nodes)
                            if (child.Checked)
                                animList.Add((Animation)child.Tag);
                }
            }
            if (animList.Count > 0) return animList.ToArray();
            return null;
        }

        public RayImage[] GetSelectedImages()
        {
            List<RayImage> imgsList = new List<RayImage>();
            foreach (TreeNode node in treeView1.Nodes)
            {
                if ((TreeNodeTypes)node.Tag == TreeNodeTypes.Images)
                {
                    if (node.Nodes != null && node.Nodes.Count > 0)
                        foreach (TreeNode child in node.Nodes)
                            if (child.Checked)
                                imgsList.Add((RayImage)child.Tag);
                }
            }
            if (imgsList.Count > 0) return imgsList.ToArray();
            return null;
        }

        public Scene GetSelectedScene()
        {
            Scene scene = new Scene();
            Camera cam = GetSelectedCamera();
            List<Light> lights = GetSelectedLights();
            List<DefaultShape> defShapes = GetSelectedDefaultShapes();
            scene.Camera = cam;
            scene.Lights = lights;
            scene.SceneObjects = defShapes;
            return scene;
        }

        private List<DefaultShape> GetSelectedDefaultShapes()
        {
            List<DefaultShape> dsList = new List<DefaultShape>();
            foreach (TreeNode node in treeView1.Nodes)
            {
                if ((TreeNodeTypes)node.Tag == TreeNodeTypes.Objects)
                {
                    foreach (TreeNode childRoot in node.Nodes)
                    {
                        foreach (TreeNode dsNode in childRoot.Nodes)
                        {
                            if (dsNode.Checked) dsList.Add((DefaultShape)dsNode.Tag);
                        }
                    }

                }
            }
            //if (dsList.Count > 0) return dsList.ToArray();
            return dsList;
        }

        private List<Light> GetSelectedLights()
        {
            List<Light> lights = new List<Light>();
            foreach (TreeNode node in treeView1.Nodes)
            {
                if ((TreeNodeTypes)node.Tag == TreeNodeTypes.Lights)
                {
                    if (node.Nodes != null)
                    {
                        foreach (TreeNode child in node.Nodes)
                        {
                            if (child.Checked)
                                lights.Add((Light)child.Tag);
                        }
                    }

                }
            }
            //if (lights.Count > 0) return lights.ToArray();
            return lights;
        }

        private Camera GetSelectedCamera()
        {
            foreach (TreeNode node in treeView1.Nodes)
            {
                if ((TreeNodeTypes)node.Tag == TreeNodeTypes.Camera)
                {
                    if (node.Nodes != null && node.Nodes.Count > 0)
                        if (node.Nodes[0].Checked)
                            return (Camera)node.Nodes[0].Tag;
                    
                }
            }
            return null;
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
            //nodeObjects.Checked = true;

            TreeNode nodeSpheres = new TreeNode(TreeNodeTypes.Spheres.ToString());
            nodeSpheres.Tag = TreeNodeTypes.Spheres;
            //nodeSpheres.Checked = true;
            TreeNode nodePlanes = new TreeNode(TreeNodeTypes.Planes.ToString());
            nodePlanes.Tag = TreeNodeTypes.Planes;
            //nodePlanes.Checked = true;
            TreeNode nodeCubes = new TreeNode(TreeNodeTypes.Cubes.ToString());
            nodeCubes.Tag = TreeNodeTypes.Cubes;
            //nodeCubes.Checked = true;
            TreeNode nodeCyls = new TreeNode(TreeNodeTypes.Cylinders.ToString());
            nodeCyls.Tag = TreeNodeTypes.Cylinders;
            //nodeCyls.Checked = true;
            TreeNode nodeTriangs = new TreeNode(TreeNodeTypes.Triangles.ToString());
            nodeTriangs.Tag = TreeNodeTypes.Triangles;
            //nodeTriangs.Checked = true;
            TreeNode nodeCones = new TreeNode(TreeNodeTypes.Cones.ToString());
            nodeCones.Tag = TreeNodeTypes.Cones;
            //nodeCones.Checked = true;
            TreeNode nodeCustoms = new TreeNode(TreeNodeTypes.Custom.ToString());
            nodeCustoms.Tag = TreeNodeTypes.Custom;
            //nodeCustoms.Checked = true;

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
            //nodeImages.Checked = true;

            TreeNode nodeAnimations = new TreeNode(TreeNodeTypes.Animations.ToString());
            nodeAnimations.Tag = TreeNodeTypes.Animations;
            //nodeAnimations.Checked = true;

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
                            //DefaultShape ds = (DefaultShape)obj;
                            TreeNode novyNode = new TreeNode(obj.ToString());
                            novyNode.Tag = obj;
                            novyNode.Checked = true;
                            

                            node2.Nodes.Add(novyNode);
                            CheckParent(novyNode);
                        }
                    }
                }

                else if((TreeNodeTypes)node.Tag == rootTyp && rootTyp == TreeNodeTypes.Lights)
                {
                    //Light light = (Light)obj;
                    TreeNode novyNode = new TreeNode(obj.ToString());
                    novyNode.Tag = obj;

                        novyNode.Checked = true;
                        
                        node.Nodes.Add(novyNode);
                        CheckParent(novyNode);
                }
                else if ((TreeNodeTypes)node.Tag == rootTyp && rootTyp == TreeNodeTypes.Camera)
                {
                    //Camera cam = (Camera)obj;
                    TreeNode novyNode = new TreeNode(obj.ToString());
                    novyNode.Tag = obj;
                    novyNode.Checked = true;
                    node.Nodes.Add(novyNode);
                    CheckParent(novyNode);
                }
                else if ((TreeNodeTypes)node.Tag == rootTyp && rootTyp == TreeNodeTypes.Images)
                {
                    TreeNode novyNode = new TreeNode(obj.ToString());
                    novyNode.Tag = obj;
                        novyNode.Checked = true;
                    //node.Checked = true;
                    node.Nodes.Add(novyNode);
                    CheckParent(novyNode);

                }
                else if ((TreeNodeTypes)node.Tag == rootTyp && rootTyp == TreeNodeTypes.Animations)
                {
                    TreeNode novyNode = new TreeNode(obj.ToString());
                    novyNode.Tag = obj;
                    node.Nodes.Add(novyNode);
                    node.Checked = true;
                    novyNode.Checked = true;
                    CheckParent(novyNode);
                    //node.Checked = true;

                }
                else if ((TreeNodeTypes)node.Tag == rootTyp)
                {
                    TreeNode novyNode = new TreeNode(obj.ToString());
                    novyNode.Tag = obj;
                    novyNode.Checked = true;
                    node.Checked = true;
                    node.Nodes.Add(novyNode);
                    CheckParent(novyNode);

                }
            }
        }

        /// <summary>
        /// Prida do seznamu objekt ze sveta Raytraceru: 
        /// koule, rovina, valec, krychle, svetlo, kamera, image, animation
        /// </summary>
        public void AddItem(object obj)
        {
            if (obj is Sphere)
            {
                this.AddItem(obj, TreeNodeTypes.Spheres);
            }
            else if (obj is Plane)
            {
                this.AddItem(obj, TreeNodeTypes.Planes);
            }
            else if (obj is Cube)
            {
                this.AddItem(obj, TreeNodeTypes.Cubes);
            }
            else if (obj is Cylinder)
            {
                this.AddItem(obj, TreeNodeTypes.Cylinders);
            }
            else if (obj is Triangle)
            {
                this.AddItem(obj, TreeNodeTypes.Triangles);
            }
            else if (obj is CustomObject)
            {
                this.AddItem(obj, TreeNodeTypes.Custom);
            }
            else if (obj is Cone)
            {
                this.AddItem(obj, TreeNodeTypes.Cones);
            }
            else if (obj is Animation)
            {
                this.AddItem(obj, TreeNodeTypes.Animations);
            }
            else if (obj is RayImage)
            {
                this.AddItem(obj, TreeNodeTypes.Images);
            }
            else if (obj is Light)
            {
                this.AddItem(obj, TreeNodeTypes.Lights);
            }
            else if (obj is RayTracing)
            {
                AddRayTracing((RayTracing)obj);
            }
        }

        public void AddItem(Light light)
        {
            this.AddItem(light, TreeNodeTypes.Lights);
        }
        public void AddItem(Camera cam)
        {
            this.AddItem(cam, TreeNodeTypes.Camera);
        }
        public void AddItem(RayImage img)
        {
            this.AddItem(img, TreeNodeTypes.Images);
        }

        private void onAddFormItems(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void onCancel(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void SetChildNodes(TreeNode root)
        {
            foreach (TreeNode node in root.Nodes)
            {
                node.Checked = root.Checked;
                SetChildNodes(node);
            }
        }

        private void AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is TreeNodeTypes)
            {
                if (!isChecking)
                SetChildNodes(e.Node);
            }
            
            CheckParent(e.Node);
        }

        private void BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            //if (isChecking) e.Cancel = true;
        }

        private void CheckParent(TreeNode node)
        {
            TreeNode parent = node.Parent;
            if (parent == null) return;
            isChecking = true;
            if (node.Checked)
            {
                parent.Checked = true;
                CheckParent(parent);
            }
            else
            {
                bool checkedChild = false;
                foreach (TreeNode child in parent.Nodes)
                {
                    if (child.Checked) checkedChild = true;
                }
                if (!checkedChild)
                {
                    parent.Checked = false;
                    CheckParent(parent);
                }
            }

            isChecking = false;
        }
    }
}
