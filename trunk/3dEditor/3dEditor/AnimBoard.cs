﻿using System;
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
            _animation.StartAnimation(_rayTracer, _rayImg);
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
                    this.progressBar.Value = e.ProgressPercentage;
                    this.labelProgress.Text = e.ProgressPercentage.ToString() + "%";
                    this.Text = e.ProgressPercentage.ToString() + "%";
                    TimeSpan estimated = GetEstimatedTime(e.ProgressPercentage);
                    this.labelProgress.Text += " estimated time: " + GetStringTime(estimated);
                }
                else
                {
                    this.labelProgress.Text = "99%" + " estimated time: few seconds...";
                    this.Text = "99%";
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
                this.Text = "Cancelled!";
            }
            else
            {
                TimeSpan duration = DateTime.Now - _startAnimTime;
                string time = GetStringTime(duration);
                this.progressBar.Value = 100;
                this.labelProgress.Text = String.Format("Done! Time: {0}", time);
                this.Text = "100%";
                _ParentForm.MessageBoxShow("Animation was succesfully created", "Finished", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.btnCancel.Text = "Close";
        }

        private void FinishAnimationWorker()
        {
            if (_animation.IsBusy())
            {
                _animation.StopAnimation();
            }
        
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_animation.IsBusy())
            {
                this.labelProgress.Text = "Cancelling! Please wait...";
                FinishAnimationWorker();
            }
            else
                this.Close();
        }

        private void OnShown(object sender, EventArgs e)
        {
            InitializeAndStart();
        }

        private void OnClosing(object sender, FormClosedEventArgs e)
        {
            FinishAnimationWorker();
            //this.Close();
        }
    }
}
