using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace _3dEditor
{
    /// <summary>
    /// About box pro kazdy renderovany obrazek
    /// </summary>
    partial class AboutBoxInfo : Form
    {
        public AboutBoxInfo()
        {
            InitializeComponent();
        }

        public void Set(int recurse, bool isAntialias, String optimaliz, Size size, ulong totalInters)
        {
            this.labelAntialias.Text += isAntialias ? "YES" : "NO";
            this.labelOptimize.Text += optimaliz;
            this.labelRecurse.Text += recurse.ToString();
            this.labelSize.Text += size.Width.ToString() + " x " + size.Height.ToString();
            this.labelTotalInters.Text += totalInters.ToString();
        }

    }
}
