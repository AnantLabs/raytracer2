namespace _3dEditor
{
    partial class AboutBoxInfo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelTotalBVInters = new System.Windows.Forms.Label();
            this.labelTotalObjInters = new System.Windows.Forms.Label();
            this.labelRecurse = new System.Windows.Forms.Label();
            this.labelAntialias = new System.Windows.Forms.Label();
            this.labelOptimize = new System.Windows.Forms.Label();
            this.labelSize = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.labelTime, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelTotalBVInters, 0, 6);
            this.tableLayoutPanel.Controls.Add(this.labelTotalObjInters, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.labelRecurse, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.labelAntialias, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.labelOptimize, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.labelSize, 0, 4);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(9, 9);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 7;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(187, 137);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // labelTotalBVInters
            // 
            this.labelTotalBVInters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTotalBVInters.Location = new System.Drawing.Point(6, 114);
            this.labelTotalBVInters.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelTotalBVInters.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelTotalBVInters.Name = "labelTotalBVInters";
            this.labelTotalBVInters.Size = new System.Drawing.Size(178, 17);
            this.labelTotalBVInters.TabIndex = 24;
            this.labelTotalBVInters.Text = "BV Tests: ";
            // 
            // labelTotalObjInters
            // 
            this.labelTotalObjInters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTotalObjInters.Location = new System.Drawing.Point(6, 95);
            this.labelTotalObjInters.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelTotalObjInters.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelTotalObjInters.Name = "labelTotalObjInters";
            this.labelTotalObjInters.Size = new System.Drawing.Size(178, 17);
            this.labelTotalObjInters.TabIndex = 23;
            this.labelTotalObjInters.Text = "Object Tests: ";
            // 
            // labelRecurse
            // 
            this.labelRecurse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelRecurse.Location = new System.Drawing.Point(6, 19);
            this.labelRecurse.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelRecurse.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelRecurse.Name = "labelRecurse";
            this.labelRecurse.Size = new System.Drawing.Size(178, 17);
            this.labelRecurse.TabIndex = 19;
            this.labelRecurse.Text = "Recursion:  ";
            this.labelRecurse.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelAntialias
            // 
            this.labelAntialias.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelAntialias.Location = new System.Drawing.Point(6, 38);
            this.labelAntialias.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelAntialias.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelAntialias.Name = "labelAntialias";
            this.labelAntialias.Size = new System.Drawing.Size(178, 17);
            this.labelAntialias.TabIndex = 0;
            this.labelAntialias.Text = "Is Antialiasing: ";
            this.labelAntialias.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelOptimize
            // 
            this.labelOptimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelOptimize.Location = new System.Drawing.Point(6, 57);
            this.labelOptimize.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelOptimize.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelOptimize.Name = "labelOptimize";
            this.labelOptimize.Size = new System.Drawing.Size(178, 17);
            this.labelOptimize.TabIndex = 21;
            this.labelOptimize.Text = "Optimization: ";
            this.labelOptimize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelSize
            // 
            this.labelSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSize.Location = new System.Drawing.Point(6, 76);
            this.labelSize.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelSize.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelSize.Name = "labelSize";
            this.labelSize.Size = new System.Drawing.Size(178, 17);
            this.labelSize.TabIndex = 22;
            this.labelSize.Text = "Size:  ";
            this.labelSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelTime
            // 
            this.labelTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTime.Location = new System.Drawing.Point(6, 0);
            this.labelTime.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelTime.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(178, 17);
            this.labelTime.TabIndex = 25;
            this.labelTime.Text = "Total Time:  ";
            this.labelTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AboutBoxInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(205, 155);
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBoxInfo";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Info";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AfterClosed);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelRecurse;
        private System.Windows.Forms.Label labelAntialias;
        private System.Windows.Forms.Label labelOptimize;
        private System.Windows.Forms.Label labelSize;
        private System.Windows.Forms.Label labelTotalObjInters;
        private System.Windows.Forms.Label labelTotalBVInters;
        private System.Windows.Forms.Label labelTime;
    }
}
