using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RayTracerLib;
using System.Threading;
using System.IO;

namespace RayTracerForm
{
    public partial class DrawingBoard : Form
    {
        public struct BWArgs
        {
            public RayTracing RayTr;
            public Graphics Graph;
            public BWArgs(RayTracing raytracer, Graphics graphics)
            {
                RayTr = raytracer;
                Graph = graphics;
            }
        }
        BackgroundWorker _bw;
        RayTracing _rayTracer;
        Graphics _graphics;
        int _recurse;

        bool _antialias;

        DateTime _startTime;
        Renderer _renderer;
        /// <summary>
        /// bitmapa pro uchovani vysledneho nebo docasneho obrazku
        /// </summary>
        Bitmap _bitmap;

        /// <summary>
        /// pole vyrenderovaneho obrazku pro antialiasing
        /// </summary>
        Colour[,] _rawImgColours;

        public DrawingBoard()
        {
            InitializeComponent();
            InitAll();
        }


        private void InitAll()
        {
            _bw = new BackgroundWorker();
            _bw.WorkerSupportsCancellation = true;
            _bw.WorkerReportsProgress = true;
            _bw.ProgressChanged += new ProgressChangedEventHandler(_bw_ProgressChanged);
            _bw.DoWork += new DoWorkEventHandler(_bw_DoWork);
            _bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bw_RunWorkerCompleted);
        }

        void _bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Bitmap bmp = (Bitmap)e.UserState;
            _bitmap = new Bitmap(bmp);
            pictureBoard.Image = _bitmap;
            this.Text = e.ProgressPercentage.ToString() + "%";
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
                time = String.Format("{0}h, {1}m", Math.Floor(ts.TotalHours).ToString(), Math.Floor(ts.TotalMinutes).ToString());
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

        /// <summary>
        /// vlakno dokoncilo svou cinnost
        /// </summary>
        void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;
            _bitmap = (Bitmap)e.Result;
            pictureBoard.Image = _bitmap;
            TimeSpan duration = DateTime.Now -_startTime;
            string time = GetStringTime(duration);

            this.Text = String.Format("Done! {0}x{1} {2}Time: {3}", this.pictureBoard.Width, this.pictureBoard.Height,
                 _antialias ? "Antialias " : "", time);
            this.saveAsToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// zakladni verze pro nakresleni, pracuje se sdilenymi promennymi, 
        /// ty predavane ignoruje
        /// pri generovani obrazku jej ukladame do hlavni bitmapy a zamykame ji, 
        /// aby k ni nemel pristup nekdo jiny
        /// alternativa je ukladat obrazek do tempBitmapy a na konci vypoctu jej priradit hlavni bitmape
        /// </summary>
        void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            int width = this.pictureBoard.Width;
            int height = this.pictureBoard.Height;
            int rawWidth = width;
            int rawHeigh = height;
            
            RayTracerLib.Colour resCol;
            System.Drawing.Color color;

            if (_antialias)
            {
                rawWidth = this.pictureBoard.Width * 3;
                rawHeigh = this.pictureBoard.Height * 3;
            }

            _rayTracer.SetBoundValues(0, rawWidth, 0, rawHeigh);

            _rawImgColours = new Colour[rawHeigh, rawWidth];
            Bitmap tempBitmap = new Bitmap(width, height);

            // hodnota progresu na hlavnim titulku formulare: height/celkovy_pocet_zobrazeni
            // napr. 20 zobrazeni - kazdych 5 procent
            int progres = rawHeigh / 20;

            int yPosuv;
            int coordX, coordY;

            Pen pen = new Pen(Color.White);

            for (int y = 0; y < rawHeigh; y++)
            {

                if (_bw.CancellationPending)
                    return;

                yPosuv = y % progres;

                if (yPosuv == 0)
                    _bw.ReportProgress(y * 100 / rawHeigh);

                for (int x = 0; x < rawWidth; x++)
                {
                    if (_recurse == -1)
                    {
                        resCol = _rayTracer.RayCast(x + 0.5, y + 0.5);
                    }
                    else
                    {
                        resCol = _rayTracer.RayTrace(_recurse, x + 0.5, y + 0.5);
                    }

                    _rawImgColours[y, x] = resCol;

                    if (_antialias)
                    {
                        if (y % 3 == 0 && x % 3 == 0)
                        {
                            coordX = x / 3;
                            coordY = y / 3;
                            color = resCol.SystemColor();
                            pen.Color = color;
                            _graphics.DrawLine(pen, (int)coordX, (int)coordY, (int)(coordX + 1), (int)(coordY + 0.5));
                            tempBitmap.SetPixel(coordX, coordY, color);
                            lock (_bitmap)
                            {
                                _bitmap.SetPixel(coordX, coordY, color);
                            }
                        }
                    }
                    else
                    {
                        color = resCol.SystemColor();
                        pen.Color = color;
                        _graphics.DrawLine(pen, (int)x, (int)y, (int)(x + 1), (int)(y + 0.5));
                        tempBitmap.SetPixel(x, y, color);
                        lock (_bitmap)
                        {
                            _bitmap.SetPixel(x , y, color);
                        }
                    }
                }
            }

            if (_antialias)
                AntializeArrayCols();

            //_bitmap = tempBitmap;
        }

        /// <summary>
        /// antialiasingova procedura
        /// </summary>
        private void AntializeArrayCols()
        {
            //double[,] weights = new double[3, 3] { 
            //{ (double)1/18, (double)1/18, (double)2/18 }, 
            //{ (double)2/18, (double)3/18, (double)2/18 }, 
            //{ (double)1/18, (double)1/18, (double)2/18 } };

            bool v = _bw.IsBusy;

            _graphics.Clear(Color.White);
            RayTracerLib.Colour resCol;
            RayTracerLib.Colour[,] samples = new Colour[3, 3];
            System.Drawing.Color color;
            Pen pen = new Pen(Color.White);

            _bitmap = new Bitmap(this.pictureBoard.Width, this.pictureBoard.Height, _graphics);
            for (int y = 0; y < pictureBoard.Height; y++)
            {
                for (int x = 0; x < pictureBoard.Width; x++)
                {

                    resCol = new Colour(0, 0, 0, 0);
                    for (int i = -1; i < 2; i++)
                        for (int j = -1; j < 2; j++)
                        {
                            //samples[i + 1, j + 1] = _rayTracer.RayTrace(_recurse, x + i, y + i);
                            samples[i + 1, j + 1] = _rawImgColours[y * 3 + i + 1, x * 3 + j + 1];
                            //samples[i + 1, j + 1] *=weights[i + 1, j + 1];

                            resCol += samples[i + 1, j + 1];
                        }

                    resCol = resCol * ((double)1 / 9);

                    color = resCol.SystemColor();

                    pen.Color = color;
                    _graphics.DrawLine(pen, (int)x, (int)y, (int)(x + 1), (int)(y + 0.5));
                    lock (_bitmap)
                    {
                        _bitmap.SetPixel(x, y, color);
                    }
                }
            }
        }

        //private Bitmap AntializeBitmap(Bitmap bmp)
        //{

        //    System.Drawing.Color resCol;
        //    System.Drawing.Color[,] samples = new Colour[3, 3];

        //    System.Drawing.Color color;


        //    _bitmap = new Bitmap(this.pictureBoard.Width, this.pictureBoard.Height, _graphics);
        //    for (int y = 0; y < pictureBoard.Height; y++)
        //    {
        //        for (int x = 0; x < pictureBoard.Width; x++)
        //        {

        //            resCol = new Color();
        //            for (int i=-1; i<2; i++)
        //                for (int j = -1; j < 2; j++)
        //                {
        //                    //samples[i + 1, j + 1] = _rayTracer.RayTrace(_recurse, x + i, y + i);
        //                    samples[i + 1, j + 1] = bmp.GetPixel(x + i, y + j);
        //                    resCol += samples[i + 1, j + 1];
        //                }

        //            resCol = resCol * (1/9);

        //            color = resCol.SystemColor();
        //            Pen pen = new Pen(color);
        //            _graphics.DrawLine(pen, (int)x, (int)y, (int)(x + 1), (int)(y + 0.5));
        //            lock (_bitmap)
        //            {
        //                _bitmap.SetPixel(x, y, color);
        //            }
        //        }
        //    }
        //}



        /// <summary>
        /// nastaveni raytraceru do formulare
        /// </summary>
        /// <param name="raytracer">object raytracer</param>
        public void Set(RayTracing raytracer, int picWidth, int picHeight, int recurse, bool antialias)
        {
            _rayTracer = raytracer;
            _recurse = recurse;
            _antialias = antialias;
            this.pictureBoard.Width = picWidth;
            this.pictureBoard.Height = picHeight;

            // nastaveni rozliseni obrazku:
            //Size newSize = new Size(picWidth, picHeight);
            //this.pictureBoard.Size = newSize;
            //this.Size = new Size(newSize.Width + 10, newSize.Height + 10);
        }

        /// <summary>
        /// vyvola na pozadi nove vlakno pro vypocet obrazku
        /// </summary>
        public void DrawScene()
        {
            _graphics = this.pictureBoard.CreateGraphics();
            _bitmap = new Bitmap(this.pictureBoard.Width, this.pictureBoard.Height, _graphics);
            _startTime = DateTime.Now;
            _renderer = new Renderer(_rayTracer, new RayImage(new Size(_bitmap.Width, _bitmap.Height), _recurse, _antialias));
            _renderer.AddProgressChangeEventHandler(new ProgressChangedEventHandler(_bw_ProgressChanged));
            _renderer.AddRenderCompletedEventHandler(new RunWorkerCompletedEventHandler(_bw_RunWorkerCompleted));
            _renderer.RenderAsyncImage();
        }

        /// <summary>
        /// udalost pri zobrazeni formulare
        /// </summary>
        private void OnShow(object sender, EventArgs e)
        {
            DrawScene();
        }


        /// <summary>
        /// Hlavni metoda na prekresleni bitmapy do pictureBoxu
        /// bezi-li vypocet jeste na pozadi, musime vytvorit novou bitmapu a tu zobrazit,
        /// aby nevznikl soucasny pristup k hlavni bitmape
        /// </summary>
        private void OnPaint(object sender, PaintEventArgs e)
        {
            pictureBoard.Image = _bitmap;
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (_renderer.IsBusy())
                _renderer.StopRendering();
            while (_renderer.IsBusy())
            {
                Thread.Sleep(50);
            }
        }

        private void OnCLosed(object sender, FormClosedEventArgs e)
        {
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// ulozeni obrazku
        /// </summary>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(saveFileDialog.FileName);
                ext = ext.ToLower();
                switch (ext)
                {
                    case ".png":
                        _bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;

                    case ".jpg":
                        _bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case ".bmp":
                        _bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    default:
                        _bitmap.Save(saveFileDialog.FileName);
                        break;
                }
                
            }
        }
    }
}