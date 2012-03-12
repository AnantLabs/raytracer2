namespace RayTracerForm
{
    partial class MaterialForm
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
            this.panelMaterial = new System.Windows.Forms.Panel();
            this.label69 = new System.Windows.Forms.Label();
            this.numN = new System.Windows.Forms.NumericUpDown();
            this.label68 = new System.Windows.Forms.Label();
            this.numKt = new System.Windows.Forms.NumericUpDown();
            this.label67 = new System.Windows.Forms.Label();
            this.btnMaterialColor = new System.Windows.Forms.Button();
            this.numH = new System.Windows.Forms.NumericUpDown();
            this.numColR = new System.Windows.Forms.NumericUpDown();
            this.numColB = new System.Windows.Forms.NumericUpDown();
            this.label58 = new System.Windows.Forms.Label();
            this.numColG = new System.Windows.Forms.NumericUpDown();
            this.label59 = new System.Windows.Forms.Label();
            this.label60 = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
            this.numKa = new System.Windows.Forms.NumericUpDown();
            this.label63 = new System.Windows.Forms.Label();
            this.numKd = new System.Windows.Forms.NumericUpDown();
            this.label64 = new System.Windows.Forms.Label();
            this.numKs = new System.Windows.Forms.NumericUpDown();
            this.label65 = new System.Windows.Forms.Label();
            this.label66 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.panelMaterial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numColR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numColB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numColG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKa)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKs)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMaterial
            // 
            this.panelMaterial.Controls.Add(this.label69);
            this.panelMaterial.Controls.Add(this.numN);
            this.panelMaterial.Controls.Add(this.label68);
            this.panelMaterial.Controls.Add(this.numKt);
            this.panelMaterial.Controls.Add(this.label67);
            this.panelMaterial.Controls.Add(this.btnMaterialColor);
            this.panelMaterial.Controls.Add(this.numH);
            this.panelMaterial.Controls.Add(this.numColR);
            this.panelMaterial.Controls.Add(this.numColB);
            this.panelMaterial.Controls.Add(this.label58);
            this.panelMaterial.Controls.Add(this.numColG);
            this.panelMaterial.Controls.Add(this.label59);
            this.panelMaterial.Controls.Add(this.label60);
            this.panelMaterial.Controls.Add(this.label61);
            this.panelMaterial.Controls.Add(this.numKa);
            this.panelMaterial.Controls.Add(this.label63);
            this.panelMaterial.Controls.Add(this.numKd);
            this.panelMaterial.Controls.Add(this.label64);
            this.panelMaterial.Controls.Add(this.numKs);
            this.panelMaterial.Controls.Add(this.label65);
            this.panelMaterial.Controls.Add(this.label66);
            this.panelMaterial.Location = new System.Drawing.Point(46, 21);
            this.panelMaterial.Name = "panelMaterial";
            this.panelMaterial.Size = new System.Drawing.Size(309, 350);
            this.panelMaterial.TabIndex = 58;
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label69.Location = new System.Drawing.Point(94, 175);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(78, 13);
            this.label69.TabIndex = 61;
            this.label69.Text = "Index lomu (N):";
            // 
            // numN
            // 
            this.numN.DecimalPlaces = 1;
            this.numN.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numN.Location = new System.Drawing.Point(172, 173);
            this.numN.Name = "numN";
            this.numN.Size = new System.Drawing.Size(61, 20);
            this.numN.TabIndex = 60;
            this.numN.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label68.Location = new System.Drawing.Point(67, 149);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(105, 13);
            this.label68.TabIndex = 59;
            this.label68.Text = "Koeficient lomu (KT):";
            // 
            // numKt
            // 
            this.numKt.DecimalPlaces = 1;
            this.numKt.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numKt.Location = new System.Drawing.Point(172, 147);
            this.numKt.Name = "numKt";
            this.numKt.Size = new System.Drawing.Size(61, 20);
            this.numKt.TabIndex = 58;
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label67.Location = new System.Drawing.Point(66, 123);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(108, 13);
            this.label67.TabIndex = 57;
            this.label67.Text = "Zrcadlový odlesk (H):";
            // 
            // btnMaterialColor
            // 
            this.btnMaterialColor.Location = new System.Drawing.Point(69, 291);
            this.btnMaterialColor.Name = "btnMaterialColor";
            this.btnMaterialColor.Size = new System.Drawing.Size(61, 23);
            this.btnMaterialColor.TabIndex = 42;
            this.btnMaterialColor.Text = "vybrat";
            this.btnMaterialColor.UseVisualStyleBackColor = true;
            this.btnMaterialColor.Click += new System.EventHandler(this.btnMaterialColor_Click);
            // 
            // numH
            // 
            this.numH.Location = new System.Drawing.Point(172, 121);
            this.numH.Name = "numH";
            this.numH.Size = new System.Drawing.Size(61, 20);
            this.numH.TabIndex = 54;
            this.numH.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // numColR
            // 
            this.numColR.DecimalPlaces = 5;
            this.numColR.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numColR.Location = new System.Drawing.Point(172, 253);
            this.numColR.Name = "numColR";
            this.numColR.Size = new System.Drawing.Size(61, 20);
            this.numColR.TabIndex = 43;
            // 
            // numColB
            // 
            this.numColB.DecimalPlaces = 5;
            this.numColB.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numColB.Location = new System.Drawing.Point(172, 305);
            this.numColB.Name = "numColB";
            this.numColB.Size = new System.Drawing.Size(61, 20);
            this.numColB.TabIndex = 41;
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label58.Location = new System.Drawing.Point(152, 256);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(18, 13);
            this.label58.TabIndex = 42;
            this.label58.Text = "R:";
            // 
            // numColG
            // 
            this.numColG.DecimalPlaces = 5;
            this.numColG.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numColG.Location = new System.Drawing.Point(172, 279);
            this.numColG.Name = "numColG";
            this.numColG.Size = new System.Drawing.Size(61, 20);
            this.numColG.TabIndex = 44;
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label59.Location = new System.Drawing.Point(152, 308);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(17, 13);
            this.label59.TabIndex = 45;
            this.label59.Text = "B:";
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label60.Location = new System.Drawing.Point(152, 282);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(18, 13);
            this.label60.TabIndex = 46;
            this.label60.Text = "G:";
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label61.Location = new System.Drawing.Point(67, 272);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(53, 16);
            this.label61.TabIndex = 40;
            this.label61.Text = "Barva:";
            // 
            // numKa
            // 
            this.numKa.DecimalPlaces = 1;
            this.numKa.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numKa.Location = new System.Drawing.Point(172, 43);
            this.numKa.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numKa.Name = "numKa";
            this.numKa.Size = new System.Drawing.Size(61, 20);
            this.numKa.TabIndex = 20;
            this.numKa.Value = new decimal(new int[] {
            3,
            0,
            0,
            65536});
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label63.Location = new System.Drawing.Point(115, 11);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(78, 20);
            this.label63.TabIndex = 15;
            this.label63.Text = "Materiál:";
            // 
            // numKd
            // 
            this.numKd.DecimalPlaces = 1;
            this.numKd.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numKd.Location = new System.Drawing.Point(172, 95);
            this.numKd.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numKd.Name = "numKd";
            this.numKd.Size = new System.Drawing.Size(61, 20);
            this.numKd.TabIndex = 16;
            this.numKd.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label64.Location = new System.Drawing.Point(42, 45);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(130, 13);
            this.label64.TabIndex = 17;
            this.label64.Text = "Ambientní koeficient (KA):";
            // 
            // numKs
            // 
            this.numKs.DecimalPlaces = 1;
            this.numKs.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numKs.Location = new System.Drawing.Point(172, 69);
            this.numKs.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numKs.Name = "numKs";
            this.numKs.Size = new System.Drawing.Size(61, 20);
            this.numKs.TabIndex = 21;
            this.numKs.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label65.Location = new System.Drawing.Point(55, 97);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(117, 13);
            this.label65.TabIndex = 23;
            this.label65.Text = "Difusní koeficient (KD):";
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label66.Location = new System.Drawing.Point(49, 71);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(123, 13);
            this.label66.TabIndex = 24;
            this.label66.Text = "Odrazivý koeficient (KS):";
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(91, 400);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 59;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(230, 400);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 60;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // colorDialog
            // 
            this.colorDialog.AnyColor = true;
            this.colorDialog.Color = System.Drawing.Color.Blue;
            this.colorDialog.FullOpen = true;
            // 
            // MaterialForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 458);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.panelMaterial);
            this.Name = "MaterialForm";
            this.Text = "MaterialForm";
            this.panelMaterial.ResumeLayout(false);
            this.panelMaterial.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numColR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numColB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numColG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKa)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMaterial;
        private System.Windows.Forms.Label label69;
        private System.Windows.Forms.NumericUpDown numN;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.NumericUpDown numKt;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.Button btnMaterialColor;
        private System.Windows.Forms.NumericUpDown numH;
        private System.Windows.Forms.NumericUpDown numColR;
        private System.Windows.Forms.NumericUpDown numColB;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.NumericUpDown numColG;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.NumericUpDown numKa;
        private System.Windows.Forms.Label label63;
        private System.Windows.Forms.NumericUpDown numKd;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.NumericUpDown numKs;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.Label label66;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ColorDialog colorDialog;
    }
}