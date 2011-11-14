namespace _3dEditor
{
    partial class WndScene
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Sphere (Center: 0;0;0, Color:0.4;0.5;0.6)");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("Sphere (Center: -7.0;2.0;0.0, Color:1.0;0.5;0.1)");
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("Spheres", new System.Windows.Forms.TreeNode[] {
            treeNode21,
            treeNode22});
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("Plane (Norm: 1;0;0, Color:0.4;0.0;1.0)");
            System.Windows.Forms.TreeNode treeNode25 = new System.Windows.Forms.TreeNode("Planes", new System.Windows.Forms.TreeNode[] {
            treeNode24});
            System.Windows.Forms.TreeNode treeNode26 = new System.Windows.Forms.TreeNode("Cube (Center: 1;0.3;0, Color:1.0;0.0;0.5)");
            System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("Cubes", new System.Windows.Forms.TreeNode[] {
            treeNode26});
            System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("Cylinder (Center: -5.0;0;0, Color:0.4;0.5;0.6)");
            System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("Cylinders", new System.Windows.Forms.TreeNode[] {
            treeNode28});
            System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("Objects", new System.Windows.Forms.TreeNode[] {
            treeNode23,
            treeNode25,
            treeNode27,
            treeNode29});
            System.Windows.Forms.TreeNode treeNode31 = new System.Windows.Forms.TreeNode("Light (Center: 2.0;2.0;1.0 Color:1.0;1.0;1.0)");
            System.Windows.Forms.TreeNode treeNode32 = new System.Windows.Forms.TreeNode("Light (Center: 0.0;0.0;-5.0 Color:1.0;1.0;1.0)");
            System.Windows.Forms.TreeNode treeNode33 = new System.Windows.Forms.TreeNode("Lights", new System.Windows.Forms.TreeNode[] {
            treeNode31,
            treeNode32});
            System.Windows.Forms.TreeNode treeNode34 = new System.Windows.Forms.TreeNode("Camera");
            System.Windows.Forms.TreeNode treeNode35 = new System.Windows.Forms.TreeNode("Img001 (Res:800x600;Rec:3)");
            System.Windows.Forms.TreeNode treeNode36 = new System.Windows.Forms.TreeNode("ImageName (Res:320x240;Rec:6)");
            System.Windows.Forms.TreeNode treeNode37 = new System.Windows.Forms.TreeNode("Images", new System.Windows.Forms.TreeNode[] {
            treeNode35,
            treeNode36});
            System.Windows.Forms.TreeNode treeNode38 = new System.Windows.Forms.TreeNode("Animation1 (Duration:5)");
            System.Windows.Forms.TreeNode treeNode39 = new System.Windows.Forms.TreeNode("AnimationXYZ (Duration:9)");
            System.Windows.Forms.TreeNode treeNode40 = new System.Windows.Forms.TreeNode("Animations", new System.Windows.Forms.TreeNode[] {
            treeNode38,
            treeNode39});
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode21.Name = "Node1";
            treeNode21.Text = "Sphere (Center: 0;0;0, Color:0.4;0.5;0.6)";
            treeNode22.Checked = true;
            treeNode22.Name = "Node16";
            treeNode22.Text = "Sphere (Center: -7.0;2.0;0.0, Color:1.0;0.5;0.1)";
            treeNode23.Checked = true;
            treeNode23.Name = "Node12";
            treeNode23.Text = "Spheres";
            treeNode24.Checked = true;
            treeNode24.Name = "Node4";
            treeNode24.Text = "Plane (Norm: 1;0;0, Color:0.4;0.0;1.0)";
            treeNode25.Checked = true;
            treeNode25.Name = "Node13";
            treeNode25.Text = "Planes";
            treeNode26.Checked = true;
            treeNode26.Name = "Node2";
            treeNode26.Text = "Cube (Center: 1;0.3;0, Color:1.0;0.0;0.5)";
            treeNode27.Checked = true;
            treeNode27.Name = "Node14";
            treeNode27.Text = "Cubes";
            treeNode28.Checked = true;
            treeNode28.Name = "Node3";
            treeNode28.Text = "Cylinder (Center: -5.0;0;0, Color:0.4;0.5;0.6)";
            treeNode29.Checked = true;
            treeNode29.Name = "Node15";
            treeNode29.Text = "Cylinders";
            treeNode30.Checked = true;
            treeNode30.Name = "Node5";
            treeNode30.Text = "Objects";
            treeNode31.Checked = true;
            treeNode31.Name = "Node8";
            treeNode31.Text = "Light (Center: 2.0;2.0;1.0 Color:1.0;1.0;1.0)";
            treeNode32.Name = "Node9";
            treeNode32.Text = "Light (Center: 0.0;0.0;-5.0 Color:1.0;1.0;1.0)";
            treeNode33.Checked = true;
            treeNode33.Name = "Node6";
            treeNode33.Text = "Lights";
            treeNode34.Checked = true;
            treeNode34.Name = "Node7";
            treeNode34.Text = "Camera";
            treeNode35.Name = "Node19";
            treeNode35.Text = "Img001 (Res:800x600;Rec:3)";
            treeNode36.Name = "Node20";
            treeNode36.Text = "ImageName (Res:320x240;Rec:6)";
            treeNode37.Name = "Node17";
            treeNode37.Text = "Images";
            treeNode38.Name = "Node21";
            treeNode38.Text = "Animation1 (Duration:5)";
            treeNode39.Name = "Node22";
            treeNode39.Text = "AnimationXYZ (Duration:9)";
            treeNode40.Name = "Node18";
            treeNode40.Text = "Animations";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode30,
            treeNode33,
            treeNode34,
            treeNode37,
            treeNode40});
            this.treeView1.Size = new System.Drawing.Size(432, 307);
            this.treeView1.TabIndex = 0;
            // 
            // WndScene
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(432, 307);
            this.Controls.Add(this.treeView1);
            this.Name = "WndScene";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SceneWnd";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;

    }
}