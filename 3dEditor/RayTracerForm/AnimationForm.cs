using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RayTracerLib;
using System.IO;
using System.Threading;

namespace RayTracerForm
{
    public partial class AnimationForm : Form
    {
        RayTracing _rayTracer;
        OneThreadAnimation _animation;
        Size _size;
        bool _antialias;
        int _recursion;

        DateTime _startAnimTime;

        public AnimationForm(RayTracing raytr, Size size, bool antialias, int recursion)
        {
            InitializeComponent();
            panelProgress.Visible = false;

            _rayTracer = raytr;
            _size = new Size(size.Width, size.Height);
            _antialias = antialias;
            _recursion = recursion;

            _animation = new OneThreadAnimation(new RayTracing(_rayTracer));
            _animation.AddProgressChangeEventHandler(new ProgressChangedEventHandler(animation_ProgressChanged));
            _animation.AddAnimationCompletedEventHandler(new RunWorkerCompletedEventHandler(animation_Completed));

            ShowAnimation(_animation);

            string path = Environment.CurrentDirectory;
            DateTime dt = DateTime.Now;
            string now = String.Format("anim{0:MM}{1:dd}_{2:hh}{3:mm}", dt, dt, dt, dt);

            path = Path.Combine(path,now);
            path = Path.ChangeExtension(path,".avi");
            textBoxAnimat.Text = path;
        }

        private void ShowAnimation(OneThreadAnimation anim)
        {
            this.panelAnimace.Visible = true;

            this.numericElipseCenterX.Value = (decimal)anim.Ellipse.Center.X;
            this.numericElipseCenterY.Value = (decimal)anim.Ellipse.Center.Y;
            this.numericElipseCenterZ.Value = (decimal)anim.Ellipse.Center.Z;

            this.numericElipseA.Value = (decimal)anim.Ellipse.A;
            this.numericElipseB.Value = (decimal)anim.Ellipse.B;
            this.numericElipseC.Value = (decimal)anim.Ellipse.C;
        }

        private void btnSaveAsFile_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                this.textBoxAnimat.Text = filename;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (Path.GetExtension(this.textBoxAnimat.Text).ToLower() != ".avi")
            {
                MessageBox.Show("Output file must be in AVI format");
                return;
            }

            if (!Directory.Exists(Path.GetDirectoryName(this.textBoxAnimat.Text)))
            {
                MessageBox.Show("Nonexisting file path");
                return;
            }

            Vektor center = new Vektor((double)this.numericElipseCenterX.Value,
                (double)this.numericElipseCenterY.Value,
            (double)this.numericElipseCenterZ.Value);

            double a = (double)this.numericElipseA.Value;
            double b = (double)this.numericElipseB.Value;
            double c = (double)this.numericElipseC.Value;
            OneThreadAnimation.Elipse elipsa = new OneThreadAnimation.Elipse(center, a, b, c);

            double time = (double)this.numericAnimationSeconds.Value;
            double fps = (double)this.numericAnimationFps.Value;

            AnimationType animType = GeTAnimationType();
            string name = textBoxAnimat.Text;

            string initPercents = "0%";
            this.Text = initPercents;
            this.labelProgress.Text = initPercents;
            this.progressBar1.Value = 0;
            _startAnimTime = DateTime.Now;
            _animation.SetPath(elipsa, fps, time);
            _animation.StartAnimation(new RayImage(_recursion, Colour.Black,_antialias), name, animType);

            this.btnStart.Enabled = false;
            panelProgress.Visible = true;
        }

        private AnimationType GeTAnimationType()
        {
            if (radioImgsOnly.Checked)
                return AnimationType.ImagesOnly;
            else if (radioVideoOnly.Checked)
                return AnimationType.VideoOnly;
            else if (radioBothImgVideo.Checked)
                return AnimationType.BothImagesAndVideo;

            return AnimationType.VideoOnly;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_animation.IsBusy())
                FinishAnimationWorker();
            else
                this.Close();
        }

        /// <summary>
        /// vrati typ z radiobuttonu
        /// </summary>
        /// <param name="procenta"></param>
        /// <returns></returns>
        private TimeSpan GetEstimatedTime(int procenta)
        {
            TimeSpan duration = DateTime.Now - _startAnimTime;
            long ticks = duration.Ticks;
            if (procenta < 1)
                ticks = ticks * 100;
            else
                ticks = ticks / procenta * (100 - procenta);
            TimeSpan estim = new TimeSpan(ticks);
            return estim;
        }

        /// <summary>
        /// vrati naformatovany retezec obsahujici cas
        /// je-li pres hodinu: hodiny a minuty
        /// je-li do hodiny: minuty a sekundy
        /// je-li do minuty: jen sekundy
        /// </summary>
        /// <param name="ts">casovy interval (delka casu)</param>
        /// <returns>retezec s casem</returns>
        private string GetStringTime(TimeSpan ts)
        {
            string time;
            if (ts.TotalHours > 1)
            {
                time = String.Format("{0}h, {1}m", Math.Floor(ts.TotalHours).ToString(), ts.Minutes.ToString());
            }
            else if (ts.TotalMinutes > 1)
            {
                time = String.Format("{0}m, {1}s", Math.Floor(ts.TotalMinutes).ToString(), ts.Seconds.ToString());
            }
            else
            {
                time = String.Format("{0}s", ts.Seconds.ToString());
            }
            return time;
        }

        private void animation_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                Bitmap bmp = (Bitmap)e.UserState;
                // zmena rozliseni do nahledu:
                Bitmap bmp2 = new Bitmap(this.pictureBoxProgress.Width, this.pictureBoxProgress.Height);
                using (Graphics g = Graphics.FromImage((Image)bmp2))
                    g.DrawImage(bmp, 0, 0, this.pictureBoxProgress.Width, this.pictureBoxProgress.Height);
                this.pictureBoxProgress.Image = bmp2;

                bmp.Dispose();
                if (e.ProgressPercentage < 100)
                {
                    this.progressBar1.Value = e.ProgressPercentage;
                    this.labelProgress.Text = e.ProgressPercentage.ToString() + "%";
                    this.Text = e.ProgressPercentage.ToString() + "%";
                    TimeSpan estimated = GetEstimatedTime(e.ProgressPercentage);
                    this.labelProgress.Text += " estimated time: " + GetStringTime(estimated);
                }
                else
                {
                    this.labelProgress.Text = "99%" + " estimated time: 1s";
                    this.Text = "99%";
                }
            }
            catch (Exception ex)
            {
                FinishAnimationWorker();
                MessageBox.Show("Error in processing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void animation_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                this.labelProgress.Text = "Cancelled!";
                this.Text = "Cancelled!";
            }
            else
            {
                TimeSpan duration = DateTime.Now - _startAnimTime;
                string time = GetStringTime(duration);
                this.progressBar1.Value = 100;
                this.labelProgress.Text = String.Format("Done! Time: {0}", time);
                this.Text = "100%";
                MessageBox.Show("Animation was succesfully created", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.btnStart.Enabled = true;
        }

        private void FinishAnimationWorker()
        {
            if (_animation.IsBusy())
            {
                _animation.StopAnimation();
                //while (_animation.IsBusy())
                //{
                //    Thread.Sleep(200);
                //}
            }
        
        }
        private void OnClose(object sender, FormClosedEventArgs e)
        {
            FinishAnimationWorker();
            this.Close();
        }

        
    }
}
