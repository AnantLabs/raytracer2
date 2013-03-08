namespace _3dEditor
{
    partial class AnimBoard
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
            this.panelProgress = new System.Windows.Forms.Panel();
            this.pictureBoxProgress = new System.Windows.Forms.PictureBox();
            this.labelProgress = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panelProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProgress)).BeginInit();
            this.SuspendLayout();
            // 
            // panelProgress
            // 
            this.panelProgress.Controls.Add(this.pictureBoxProgress);
            this.panelProgress.Controls.Add(this.labelProgress);
            this.panelProgress.Controls.Add(this.progressBar);
            this.panelProgress.Location = new System.Drawing.Point(12, 12);
            this.panelProgress.Name = "panelProgress";
            this.panelProgress.Size = new System.Drawing.Size(416, 296);
            this.panelProgress.TabIndex = 59;
            // 
            // pictureBoxProgress
            // 
            this.pictureBoxProgress.BackColor = System.Drawing.Color.White;
            this.pictureBoxProgress.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBoxProgress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxProgress.Location = new System.Drawing.Point(40, 3);
            this.pictureBoxProgress.Name = "pictureBoxProgress";
            this.pictureBoxProgress.Size = new System.Drawing.Size(320, 240);
            this.pictureBoxProgress.TabIndex = 54;
            this.pictureBoxProgress.TabStop = false;
            // 
            // labelProgress
            // 
            this.labelProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelProgress.Location = new System.Drawing.Point(63, 246);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(260, 15);
            this.labelProgress.TabIndex = 53;
            this.labelProgress.Text = "Progress:";
            this.labelProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(18, 264);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(370, 23);
            this.progressBar.TabIndex = 43;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(367, 314);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(61, 23);
            this.btnCancel.TabIndex = 58;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // AnimBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 346);
            this.Controls.Add(this.panelProgress);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AnimBoard";
            this.ShowIcon = false;
            this.Text = "Animation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosing);
            this.Shown += new System.EventHandler(this.OnShown);
            this.panelProgress.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProgress)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelProgress;
        private System.Windows.Forms.PictureBox pictureBoxProgress;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnCancel;
    }
}