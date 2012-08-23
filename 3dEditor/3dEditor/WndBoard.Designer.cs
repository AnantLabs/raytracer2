﻿namespace _3dEditor
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
            this.pictureBoard = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolBtnFront = new System.Windows.Forms.ToolStripButton();
            this.toolBtnTop = new System.Windows.Forms.ToolStripButton();
            this.toolBtnSide = new System.Windows.Forms.ToolStripButton();
            this.toolBtnReset = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBtnGrid = new System.Windows.Forms.ToolStripButton();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBtnAxes = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.labelClick = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoard)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStripContainer2.ContentPanel.SuspendLayout();
            this.toolStripContainer2.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoard
            // 
            this.pictureBoard.BackColor = System.Drawing.Color.White;
            this.pictureBoard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoard.Location = new System.Drawing.Point(0, 0);
            this.pictureBoard.Name = "pictureBoard";
            this.pictureBoard.Size = new System.Drawing.Size(792, 571);
            this.pictureBoard.TabIndex = 0;
            this.pictureBoard.TabStop = false;
            this.pictureBoard.MouseMove += new System.Windows.Forms.MouseEventHandler(this.onPicMouseMove);
            this.pictureBoard.MouseDown += new System.Windows.Forms.MouseEventHandler(this.onPicMouseDown);
            this.pictureBoard.Paint += new System.Windows.Forms.PaintEventHandler(this.onPaintBoard);
            this.pictureBoard.MouseUp += new System.Windows.Forms.MouseEventHandler(this.onPicMouseUp);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBtnFront,
            this.toolBtnTop,
            this.toolBtnSide,
            this.toolBtnReset,
            this.toolStripSeparator1,
            this.toolBtnGrid,
            this.toolStripComboBox1,
            this.toolStripSeparator2,
            this.toolBtnAxes,
            this.toolStripSeparator3,
            this.toolStripBtnLights,
            this.toolStripBtnCamera});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(453, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolBtnFront
            // 
            this.toolBtnFront.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBtnFront.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnFront.Image")));
            this.toolBtnFront.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnFront.Name = "toolBtnFront";
            this.toolBtnFront.Size = new System.Drawing.Size(37, 22);
            this.toolBtnFront.Text = "Front";
            // 
            // toolBtnTop
            // 
            this.toolBtnTop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBtnTop.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnTop.Image")));
            this.toolBtnTop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnTop.Name = "toolBtnTop";
            this.toolBtnTop.Size = new System.Drawing.Size(29, 22);
            this.toolBtnTop.Text = "Top";
            this.toolBtnTop.Click += new System.EventHandler(this.toolBtnTop_Click);
            // 
            // toolBtnSide
            // 
            this.toolBtnSide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBtnSide.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnSide.Image")));
            this.toolBtnSide.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnSide.Name = "toolBtnSide";
            this.toolBtnSide.Size = new System.Drawing.Size(31, 22);
            this.toolBtnSide.Text = "Side";
            this.toolBtnSide.Click += new System.EventHandler(this.toolBtnSide_Click);
            // 
            // toolBtnReset
            // 
            this.toolBtnReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBtnReset.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnReset.Image")));
            this.toolBtnReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnReset.Name = "toolBtnReset";
            this.toolBtnReset.Size = new System.Drawing.Size(39, 22);
            this.toolBtnReset.Text = "Reset";
            this.toolBtnReset.Click += new System.EventHandler(this.toolBtnReset_Click);
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
            this.toolBtnGrid.Size = new System.Drawing.Size(30, 22);
            this.toolBtnGrid.Text = "Grid";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(75, 25);
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.OnChanged1);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolBtnAxes
            // 
            this.toolBtnAxes.CheckOnClick = true;
            this.toolBtnAxes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBtnAxes.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnAxes.Image")));
            this.toolBtnAxes.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnAxes.Name = "toolBtnAxes";
            this.toolBtnAxes.Size = new System.Drawing.Size(35, 22);
            this.toolBtnAxes.Text = "Axes";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnLights
            // 
            this.toolStripBtnLights.CheckOnClick = true;
            this.toolStripBtnLights.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripBtnLights.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnLights.Image")));
            this.toolStripBtnLights.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnLights.Name = "toolStripBtnLights";
            this.toolStripBtnLights.Size = new System.Drawing.Size(68, 22);
            this.toolStripBtnLights.Text = "Show Lights";
            this.toolStripBtnLights.Click += new System.EventHandler(this.OnShowLights);
            // 
            // toolStripBtnCamera
            // 
            this.toolStripBtnCamera.CheckOnClick = true;
            this.toolStripBtnCamera.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripBtnCamera.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnCamera.Image")));
            this.toolStripBtnCamera.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnCamera.Name = "toolStripBtnCamera";
            this.toolStripBtnCamera.Size = new System.Drawing.Size(77, 22);
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 574);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(792, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabelZoomConst
            // 
            this.statusLabelZoomConst.Name = "statusLabelZoomConst";
            this.statusLabelZoomConst.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.statusLabelZoomConst.Size = new System.Drawing.Size(37, 17);
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
            this.toolStripContainer2.ContentPanel.Controls.Add(this.panel1);
            this.toolStripContainer2.ContentPanel.Controls.Add(this.pictureBoard);
            this.toolStripContainer2.ContentPanel.Size = new System.Drawing.Size(792, 571);
            this.toolStripContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer2.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.toolStripContainer2.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer2.Name = "toolStripContainer2";
            this.toolStripContainer2.Size = new System.Drawing.Size(792, 596);
            this.toolStripContainer2.TabIndex = 5;
            this.toolStripContainer2.Text = "toolStripContainer2";
            // 
            // toolStripContainer2.TopToolStripPanel
            // 
            this.toolStripContainer2.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.labelClick);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.numericUpDown1);
            this.panel1.Controls.Add(this.numericUpDown2);
            this.panel1.Controls.Add(this.numericUpDown3);
            this.panel1.Location = new System.Drawing.Point(636, 300);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(156, 101);
            this.panel1.TabIndex = 5;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(93, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(25, 23);
            this.button3.TabIndex = 12;
            this.button3.Text = "-";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // labelClick
            // 
            this.labelClick.AutoSize = true;
            this.labelClick.Location = new System.Drawing.Point(105, 41);
            this.labelClick.Name = "labelClick";
            this.labelClick.Size = new System.Drawing.Size(29, 13);
            this.labelClick.TabIndex = 11;
            this.labelClick.Text = "click";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(119, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(25, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "+";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(93, 74);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(51, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "btn";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(48, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "+-50";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Z";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Y";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "X";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(35, 25);
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
            this.numericUpDown1.TabIndex = 1;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.onValNumChange);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown2.Location = new System.Drawing.Point(35, 51);
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
            this.numericUpDown2.TabIndex = 2;
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.onValNumChange);
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown3.Location = new System.Drawing.Point(35, 77);
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
            this.numericUpDown3.TabIndex = 3;
            this.numericUpDown3.ValueChanged += new System.EventHandler(this.onValNumChange);
            // 
            // WndBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 596);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStripContainer2);
            this.Name = "WndBoard";
            this.ShowIcon = false;
            this.Text = "Board";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoard)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripContainer2.ContentPanel.ResumeLayout(false);
            this.toolStripContainer2.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer2.TopToolStripPanel.PerformLayout();
            this.toolStripContainer2.ResumeLayout(false);
            this.toolStripContainer2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoard;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripButton toolBtnGrid;
        private System.Windows.Forms.ToolStripButton toolBtnReset;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolBtnFront;
        private System.Windows.Forms.ToolStripButton toolBtnTop;
        private System.Windows.Forms.ToolStripButton toolBtnSide;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelZoomConst;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelXConst;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelZ;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelZoom;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelX;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelY;
        private System.Windows.Forms.ToolStripButton toolBtnAxes;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripContainer toolStripContainer2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label labelClick;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripBtnLights;
        private System.Windows.Forms.ToolStripButton toolStripBtnCamera;
    }
}