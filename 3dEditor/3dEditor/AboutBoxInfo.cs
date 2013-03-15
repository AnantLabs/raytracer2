using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
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

        DrawingBoard _parent;
        public void Set(DrawingBoard parent, int recurse, bool isAntialias, String optimaliz, Size size,TimeSpan time, long totalTestedObjs, long totalTestedCubes)
        {
            _parent = parent;
            this.labelTime.Text += GetStringTime(time);
            this.labelAntialias.Text += isAntialias ? "YES" : "NO";
            this.labelOptimize.Text += optimaliz;
            this.labelRecurse.Text += recurse.ToString();
            this.labelSize.Text += size.Width.ToString() + " x " + size.Height.ToString();


            this.labelTotalObjInters.Text += GetFormatString(totalTestedObjs);
            if (totalTestedCubes > 0)
            {
                this.labelTotalBVInters.Text += GetFormatString(totalTestedCubes);
                this.labelTotalBVInters.Visible = true;
            }
            else
                this.labelTotalBVInters.Visible = false;

        }

        private String GetFormatString(long number)
        {
            // formatovani dlouheho cisla po trech cifrach s mezerou
            StringBuilder str = new StringBuilder(number.ToString());
            StringBuilder sb = new StringBuilder();
            int rem;
            int pre = Math.DivRem(str.Length, 3, out rem);
            if (rem > 0)
            {
                sb.Append(str.ToString().Substring(0, rem) + " ");
                str.Remove(0, rem);
            }
            while (str.Length >= 3)
            {
                String part = str.ToString().Substring(0, 3);
                str.Remove(0, 3);
                sb.Append(part + " ");
            }
            return sb.ToString();
        }

        private string GetStringTime(TimeSpan ts)
        {
            string time;
            if (ts.TotalHours > 1)
            {
                time = String.Format("{0}h, {1}m, {2}s, {3}ms ", ts.Hours.ToString(), ts.Minutes.ToString(), ts.Seconds.ToString(), ts.Milliseconds.ToString());
            }
            else if (ts.TotalMinutes > 1)
            {
                time = String.Format("{0}m, {1}s, {2}ms", ts.Minutes.ToString(), ts.Seconds.ToString(), ts.Milliseconds.ToString());
            }
            else
            {
                time = String.Format("{0}s, {1}ms", ts.Seconds.ToString(), ts.Milliseconds.ToString());
            }
            return time;
        }

        private void AfterClosed(object sender, FormClosedEventArgs e)
        {
            _parent.AboutBoxClosed();
        }

    }
}
