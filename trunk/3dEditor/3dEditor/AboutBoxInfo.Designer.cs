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
            this.labelRecurse = new System.Windows.Forms.Label();
            this.labelAntialias = new System.Windows.Forms.Label();
            this.labelOptimize = new System.Windows.Forms.Label();
            this.labelSize = new System.Windows.Forms.Label();
            this.labelTotalInters = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Controls.Add(this.labelTotalInters, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.labelRecurse, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelAntialias, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.labelOptimize, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.labelSize, 0, 3);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(9, 9);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 5;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(158, 113);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // labelRecurse
            // 
            this.labelRecurse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelRecurse.Location = new System.Drawing.Point(6, 0);
            this.labelRecurse.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelRecurse.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelRecurse.Name = "labelRecurse";
            this.labelRecurse.Size = new System.Drawing.Size(149, 17);
            this.labelRecurse.TabIndex = 19;
            this.labelRecurse.Text = "Recursion: ";
            this.labelRecurse.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelAntialias
            // 
            this.labelAntialias.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelAntialias.Location = new System.Drawing.Point(6, 23);
            this.labelAntialias.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelAntialias.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelAntialias.Name = "labelAntialias";
            this.labelAntialias.Size = new System.Drawing.Size(149, 17);
            this.labelAntialias.TabIndex = 0;
            this.labelAntialias.Text = "Is Antialiasing: ";
            this.labelAntialias.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelOptimize
            // 
            this.labelOptimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelOptimize.Location = new System.Drawing.Point(6, 46);
            this.labelOptimize.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelOptimize.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelOptimize.Name = "labelOptimize";
            this.labelOptimize.Size = new System.Drawing.Size(149, 17);
            this.labelOptimize.TabIndex = 21;
            this.labelOptimize.Text = "Optimization: ";
            this.labelOptimize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelSize
            // 
            this.labelSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSize.Location = new System.Drawing.Point(6, 69);
            this.labelSize.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelSize.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelSize.Name = "labelSize";
            this.labelSize.Size = new System.Drawing.Size(149, 17);
            this.labelSize.TabIndex = 22;
            this.labelSize.Text = "Size: ";
            this.labelSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelTotalInters
            // 
            this.labelTotalInters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTotalInters.Location = new System.Drawing.Point(6, 92);
            this.labelTotalInters.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelTotalInters.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelTotalInters.Name = "labelTotalInters";
            this.labelTotalInters.Size = new System.Drawing.Size(149, 17);
            this.labelTotalInters.TabIndex = 23;
            this.labelTotalInters.Text = "Total Intersections: ";
            // 
            // AboutBoxInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(176, 131);
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBoxInfo";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Info";
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelRecurse;
        private System.Windows.Forms.Label labelAntialias;
        private System.Windows.Forms.Label labelOptimize;
        private System.Windows.Forms.Label labelSize;
        private System.Windows.Forms.Label labelTotalInters;
    }
}
