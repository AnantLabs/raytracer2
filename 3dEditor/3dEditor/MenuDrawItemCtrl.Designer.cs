namespace _3dEditor
{
    partial class MenuDrawItemCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelCaptionObj = new System.Windows.Forms.Label();
            this.pictureBoxObj = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxObj)).BeginInit();
            this.SuspendLayout();
            // 
            // labelCaptionObj
            // 
            this.labelCaptionObj.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelCaptionObj.Location = new System.Drawing.Point(1, 17);
            this.labelCaptionObj.Margin = new System.Windows.Forms.Padding(0);
            this.labelCaptionObj.Name = "labelCaptionObj";
            this.labelCaptionObj.Size = new System.Drawing.Size(73, 13);
            this.labelCaptionObj.TabIndex = 1;
            this.labelCaptionObj.Text = "WWWWWWWWW";
            this.labelCaptionObj.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelCaptionObj.MouseDown += new System.Windows.Forms.MouseEventHandler(this.onMouseDown);
            this.labelCaptionObj.MouseEnter += new System.EventHandler(this.onEnter);
            this.labelCaptionObj.MouseLeave += new System.EventHandler(this.onLeave);
            // 
            // pictureBoxObj
            // 
            this.pictureBoxObj.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxObj.Image = global::_3dEditor.Properties.Resources.add_1_icon16;
            this.pictureBoxObj.Location = new System.Drawing.Point(29, 1);
            this.pictureBoxObj.Name = "pictureBoxObj";
            this.pictureBoxObj.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxObj.TabIndex = 0;
            this.pictureBoxObj.TabStop = false;
            this.pictureBoxObj.MouseDown += new System.Windows.Forms.MouseEventHandler(this.onMouseDown);
            this.pictureBoxObj.MouseEnter += new System.EventHandler(this.onEnter);
            this.pictureBoxObj.MouseLeave += new System.EventHandler(this.onLeave);
            // 
            // MenuDrawItemCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelCaptionObj);
            this.Controls.Add(this.pictureBoxObj);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MenuDrawItemCtrl";
            this.Size = new System.Drawing.Size(74, 32);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.onMouseDown);
            this.MouseEnter += new System.EventHandler(this.onEnter);
            this.MouseLeave += new System.EventHandler(this.onLeave);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxObj)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxObj;
        private System.Windows.Forms.Label labelCaptionObj;

    }
}
