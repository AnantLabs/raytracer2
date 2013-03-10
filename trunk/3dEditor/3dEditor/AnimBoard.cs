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
using Mathematics;
using RayTracerLib;

namespace _3dEditor
{
    public partial class AnimBoard : Form
    {
        RayTracing _rayTracer;
        RayImage _rayImg;
        Animation _animation;

        DateTime _startAnimTime;

        int _imgCounter;
        ParentEditor _ParentForm;


        bool _isCanceling;

        public AnimBoard(RayTracing raytracer, RayImage rayimage, Animation anim, ParentEditor parentForm)
        {
            InitializeComponent();

            _rayTracer = raytracer;
            _rayImg = rayimage;
            _animation = anim;
            _ParentForm = parentForm;

            _animation.InitAll();
            _animation.AddProgressChangeEventHandler(new ProgressChangedEventHandler(animation_ProgressChanged));
            _animation.AddAnimationCompletedEventHandler(new RunWorkerCompletedEventHandler(animation_Completed));

            string path = Environment.CurrentDirectory;
            DateTime dt = DateTime.Now;
            string now = String.Format("anim{0:MM}{1:dd}_{2:hh}{3:mm}", dt, dt, dt, dt);

            path = Path.Combine(path, now);
            path = Path.ChangeExtension(path, ".avi");

            textBoxOutput.Text = anim.FileFullPath;

        }

        private void InitializeAndStart()
        {
            _imgCounter = 0;
            string initPercents = "0%";
            this.Text = initPercents;
            this.labelProgress.Text = initPercents;
            this.progressBar.Value = 0;
            _startAnimTime = DateTime.Now;
            _isCanceling = false;
            _animation.StartAnimation(_rayTracer, _rayImg);
        }

        /// <summary>
        /// pocet zobrazeni daneho procenta
        /// </summary>
        int _countPerProcent;
        /// <summary>
        /// predchozi procento
        /// </summary>
        int _prevPercent;

        /// Spocita zbyvajici cas do ukonceni animace
        /// <param name="procenta"></param>
        /// <returns></returns>
        private TimeSpan GetEstimatedTime(int procenta)
        {
            if (_prevPercent == procenta) _countPerProcent++;
            else
            {
                _prevPercent = procenta;
                _countPerProcent = 1;
            }

            TimeSpan duration = DateTime.Now - _startAnimTime;
            long ticks = duration.Ticks;
            if (procenta < 1)
                ticks = ticks * 100;
            else
            {
                // 2 + 1 - 1/3 ... dokoncene procento + dokoncena cast: (1 - 1/3) je MENE procent, nez (1 - 1/4).. 
                // proto procenta v promenne "procs" rostou s kazdym pokrokem pri stejnem celociselnem procentu 
                double procs = procenta + 1.0 - (1.0 / _countPerProcent); // k soucasnym procentum pridame zlomek dokoncenych procent
                ticks = (long)(ticks / procs * (100.0 - procs));
            }
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
                    this.progressBar.Value = e.ProgressPercentage;
                    this.labelProgress.Text = e.ProgressPercentage.ToString() + "%";
                    this.Text = String.Format("Animation {0}%", e.ProgressPercentage.ToString());
                    TimeSpan estimated = GetEstimatedTime(e.ProgressPercentage);
                    this.labelProgress.Text += " estimated time: " + GetStringTime(estimated);
                }
                else
                {
                    this.labelProgress.Text = "99%" + " estimated time: few seconds...";
                    this.Text = "Animation 99%";
                }
            }
            catch (Exception ex)
            {
                FinishAnimationWorker();
                _ParentForm.MessageBoxShow("Error in processing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void animation_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                this.labelProgress.Text = "Cancelled!";
                this.Text = "Animation cancelled!";
            }
            else
            {
                TimeSpan duration = DateTime.Now - _startAnimTime;
                string time = GetStringTime(duration);
                this.progressBar.Value = 100;
                this.labelProgress.Text = String.Format("Done! Total time: {0}", time);
                this.Text = "Animation 100%";
                _ParentForm.MessageBoxShow("Animation was succesfully created", "Finished", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            _isCanceling = false;
            btnCancel.Enabled = true;
            btnCancel.Focus();
            this.btnCancel.Text = "Close";
        }

        
        private void FinishAnimationWorker()
        {
            if (_animation.IsBusy())
            {
                _animation.StopAnimation();
            }
        
        }

        /// <summary>
        /// pred zavrenim zaviranim formulare.
        /// Je-li animace aktivni, zakaze zavreni formulare a zepta se, jestli se ma skutecne animace ukoncit.
        /// Je-li potvrzeno ukonceni, zrusi animaci;
        /// Neni-li animace aktivni, zavre formular.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (_isCanceling)
            {
                e.Cancel = true;
                return;
            }
            if (_animation.IsBusy())
            {
                e.Cancel = true;
                DialogResult result = _ParentForm.MessageBoxShow("Are you sure you want to cancel animation?", "Confirm cancel",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    Cancel();
                }
            }
        }


        private void Cancel()
        {
            if (_animation.IsBusy())
            {
                _isCanceling = true;
                btnCancel.Enabled = false;
                Control ctr = this.GetNextControl(btnCancel, true);
                ctr.Focus();
                this.labelProgress.Text = "Cancelling! Please wait...";
                FinishAnimationWorker();
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnShown(object sender, EventArgs e)
        {
            InitializeAndStart();
        }

        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            FinishAnimationWorker();
        }
    }
}
