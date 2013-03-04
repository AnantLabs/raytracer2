namespace _3dEditor
{
    partial class WndBoard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WndBoard));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolsComboViewAngle = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBtnAxes = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBtnGrid = new System.Windows.Forms.ToolStripButton();
            this.toolsComboGridSize = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripBtnLights = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnCamera = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabelZoomConst = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelZoom = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelX = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelY = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelXConst = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelZ = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer2 = new System.Windows.Forms.ToolStripContainer();
            this.drawItemFlowLayout1 = new _3dEditor.MenuDrawItemFlowLayout();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.pictureBoard = new System.Windows.Forms.PictureBox();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStripContainer2.ContentPanel.SuspendLayout();
            this.toolStripContainer2.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoard)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolsComboViewAngle,
            this.toolStripSeparator4,
            this.toolBtnAxes,
            this.toolStripSeparator1,
            this.toolBtnGrid,
            this.toolsComboGridSize,
            this.toolStripSeparator2,
            this.toolStripBtnLights,
            this.toolStripBtnCamera});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(471, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(35, 22);
            this.toolStripLabel1.Text = "View:";
            // 
            // toolsComboViewAngle
            // 
            this.toolsComboViewAngle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolsComboViewAngle.DropDownWidth = 80;
            this.toolsComboViewAngle.Items.AddRange(new object[] {
            "Front",
            "Side +X",
            "Side -X",
            "Top",
            "Reset X",
            "Reset Y",
            "Reset Z"});
            this.toolsComboViewAngle.Name = "toolsComboViewAngle";
            this.toolsComboViewAngle.Size = new System.Drawing.Size(100, 25);
            this.toolsComboViewAngle.ToolTipText = "Select predefined view angle";
            this.toolsComboViewAngle.SelectedIndexChanged += new System.EventHandler(this.OnChangedComboAngleView);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolBtnAxes
            // 
            this.toolBtnAxes.Checked = true;
            this.toolBtnAxes.CheckOnClick = true;
            this.toolBtnAxes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolBtnAxes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBtnAxes.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnAxes.Image")));
            this.toolBtnAxes.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnAxes.Name = "toolBtnAxes";
            this.toolBtnAxes.Size = new System.Drawing.Size(35, 22);
            this.toolBtnAxes.Text = "Axes";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolBtnGrid
            // 
            this.toolBtnGrid.Checked = true;
            this.toolBtnGrid.CheckOnClick = true;
            this.toolBtnGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolBtnGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBtnGrid.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnGrid.Image")));
            this.toolBtnGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnGrid.Name = "toolBtnGrid";
            this.toolBtnGrid.Size = new System.Drawing.Size(33, 22);
            this.toolBtnGrid.Text = "Grid";
            // 
            // toolsComboGridSize
            // 
            this.toolsComboGridSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolsComboGridSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toolsComboGridSize.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.toolsComboGridSize.Name = "toolsComboGridSize";
            this.toolsComboGridSize.Size = new System.Drawing.Size(75, 25);
            this.toolsComboGridSize.SelectedIndexChanged += new System.EventHandler(this.OnChangedComboGrid);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnLights
            // 
            this.toolStripBtnLights.CheckOnClick = true;
            this.toolStripBtnLights.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripBtnLights.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnLights.Image")));
            this.toolStripBtnLights.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnLights.Name = "toolStripBtnLights";
            this.toolStripBtnLights.Size = new System.Drawing.Size(75, 22);
            this.toolStripBtnLights.Text = "Show Lights";
            this.toolStripBtnLights.Click += new System.EventHandler(this.OnShowLights);
            // 
            // toolStripBtnCamera
            // 
            this.toolStripBtnCamera.Checked = true;
            this.toolStripBtnCamera.CheckOnClick = true;
            this.toolStripBtnCamera.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripBtnCamera.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripBtnCamera.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnCamera.Image")));
            this.toolStripBtnCamera.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnCamera.Name = "toolStripBtnCamera";
            this.toolStripBtnCamera.Size = new System.Drawing.Size(84, 22);
            this.toolStripBtnCamera.Text = "Show Camera";
            this.toolStripBtnCamera.Click += new System.EventHandler(this.onShowCamera);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabelZoomConst,
            this.statusLabelZoom,
            this.toolStripStatusLabel4,
            this.statusLabelX,
            this.toolStripStatusLabel2,
            this.statusLabelY,
            this.statusLabelXConst,
            this.statusLabelZ});
            this.statusStrip1.Location = new System.Drawing.Point(0, 590);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(924, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabelZoomConst
            // 
            this.statusLabelZoomConst.Name = "statusLabelZoomConst";
            this.statusLabelZoomConst.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.statusLabelZoomConst.Size = new System.Drawing.Size(42, 17);
            this.statusLabelZoomConst.Text = "Zoom:";
            this.statusLabelZoomConst.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusLabelZoom
            // 
            this.statusLabelZoom.AutoSize = false;
            this.statusLabelZoom.Name = "statusLabelZoom";
            this.statusLabelZoom.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.statusLabelZoom.Size = new System.Drawing.Size(30, 17);
            this.statusLabelZoom.Text = "50";
            this.statusLabelZoom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(17, 17);
            this.toolStripStatusLabel4.Text = "X:";
            this.toolStripStatusLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusLabelX
            // 
            this.statusLabelX.AutoSize = false;
            this.statusLabelX.Name = "statusLabelX";
            this.statusLabelX.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.statusLabelX.Size = new System.Drawing.Size(40, 17);
            this.statusLabelX.Text = "stupne";
            this.statusLabelX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(17, 17);
            this.toolStripStatusLabel2.Text = "Y:";
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusLabelY
            // 
            this.statusLabelY.AutoSize = false;
            this.statusLabelY.Name = "statusLabelY";
            this.statusLabelY.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.statusLabelY.Size = new System.Drawing.Size(40, 17);
            this.statusLabelY.Text = "stupne";
            this.statusLabelY.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusLabelXConst
            // 
            this.statusLabelXConst.Name = "statusLabelXConst";
            this.statusLabelXConst.Size = new System.Drawing.Size(17, 17);
            this.statusLabelXConst.Text = "Z:";
            this.statusLabelXConst.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusLabelZ
            // 
            this.statusLabelZ.AutoSize = false;
            this.statusLabelZ.Name = "statusLabelZ";
            this.statusLabelZ.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.statusLabelZ.Size = new System.Drawing.Size(40, 17);
            this.statusLabelZ.Text = "stupne";
            this.statusLabelZ.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripContainer2
            // 
            // 
            // toolStripContainer2.ContentPanel
            // 
            this.toolStripContainer2.ContentPanel.Controls.Add(this.drawItemFlowLayout1);
            this.toolStripContainer2.ContentPanel.Controls.Add(this.flowLayoutPanel1);
            this.toolStripContainer2.ContentPanel.Controls.Add(this.panel1);
            this.toolStripContainer2.ContentPanel.Controls.Add(this.pictureBoard);
            this.toolStripContainer2.ContentPanel.Size = new System.Drawing.Size(924, 587);
            this.toolStripContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer2.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.toolStripContainer2.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer2.Name = "toolStripContainer2";
            this.toolStripContainer2.Size = new System.Drawing.Size(924, 612);
            this.toolStripContainer2.TabIndex = 0;
            this.toolStripContainer2.Text = "toolStripContainer2";
            // 
            // toolStripContainer2.TopToolStripPanel
            // 
            this.toolStripContainer2.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // drawItemFlowLayout1
            // 
            this.drawItemFlowLayout1.AutoSize = true;
            this.drawItemFlowLayout1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.drawItemFlowLayout1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.drawItemFlowLayout1.Location = new System.Drawing.Point(333, 80);
            this.drawItemFlowLayout1.MaximumSize = new System.Drawing.Size(400, 500);
            this.drawItemFlowLayout1.MinimumSize = new System.Drawing.Size(100, 30);
            this.drawItemFlowLayout1.Name = "drawItemFlowLayout1";
            this.drawItemFlowLayout1.Size = new System.Drawing.Size(100, 30);
            this.drawItemFlowLayout1.TabIndex = 3;
            this.drawItemFlowLayout1.Visible = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(198, 214);
            this.flowLayoutPanel1.MaximumSize = new System.Drawing.Size(300, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(239, 0);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.numericUpDown1);
            this.panel1.Controls.Add(this.numericUpDown2);
            this.panel1.Controls.Add(this.numericUpDown3);
            this.panel1.Location = new System.Drawing.Point(834, 404);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(90, 76);
            this.panel1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Z";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Y";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "X";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(28, 6);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(52, 20);
            this.numericUpDown1.TabIndex = 4;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.onValNumChange);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown2.Location = new System.Drawing.Point(28, 27);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(52, 20);
            this.numericUpDown2.TabIndex = 5;
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.onValNumChange);
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown3.Location = new System.Drawing.Point(28, 48);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown3.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(52, 20);
            this.numericUpDown3.TabIndex = 6;
            this.numericUpDown3.ValueChanged += new System.EventHandler(this.onValNumChange);
            // 
            // pictureBoard
            // 
            this.pictureBoard.BackColor = System.Drawing.Color.White;
            this.pictureBoard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoard.Location = new System.Drawing.Point(0, 0);
            this.pictureBoard.Name = "pictureBoard";
            this.pictureBoard.Size = new System.Drawing.Size(924, 587);
            this.pictureBoard.TabIndex = 0;
            this.pictureBoard.TabStop = false;
            this.pictureBoard.Paint += new System.Windows.Forms.PaintEventHandler(this.onPaintBoard);
            this.pictureBoard.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseDoubleClick);
            this.pictureBoard.MouseDown += new System.Windows.Forms.MouseEventHandler(this.onPicMouseDown);
            this.pictureBoard.MouseMove += new System.Windows.Forms.MouseEventHandler(this.onPicMouseMove);
            this.pictureBoard.MouseUp += new System.Windows.Forms.MouseEventHandler(this.onPicMouseUp);
            // 
            // WndBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 612);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStripContainer2);
            this.DoubleBuffered = true;
            this.Name = "WndBoard";
            this.ShowIcon = false;
            this.Text = "Board";
            this.Deactivate += new System.EventHandler(this.OnDeactivate);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripContainer2.ContentPanel.ResumeLayout(false);
            this.toolStripContainer2.ContentPanel.PerformLayout();
            this.toolStripContainer2.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer2.TopToolStripPanel.PerformLayout();
            this.toolStripContainer2.ResumeLayout(false);
            this.toolStripContainer2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoard)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoard;
        public System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripButton toolBtnGrid;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelZoomConst;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelXConst;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelZ;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelZoom;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelX;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelY;
        private System.Windows.Forms.ToolStripButton toolBtnAxes;
        private System.Windows.Forms.ToolStripComboBox toolsComboGridSize;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripContainer toolStripContainer2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripButton toolStripBtnLights;
        private System.Windows.Forms.ToolStripButton toolStripBtnCamera;
        private System.Windows.Forms.ToolStripComboBox toolsComboViewAngle;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private MenuDrawItemFlowLayout drawItemFlowLayout1;
    }
}