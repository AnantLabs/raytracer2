using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using Mathematics;

namespace RayTracerLib
{
    public class Renderer
    {

        BackgroundWorker _bw;
        RayTracing _rayTracer;
        RayImage _rayImg;

        /// <summary>
        /// minimalni povolena velikost obrazku
        /// </summary>
        public static Size MinSize = new Size(20, 20);

        /// <summary>
        /// minimalni povolena velikost obrazku
        /// </summary>
        public static Size MaxSize = new Size(2000, 2000);
        
        /// <summary>
        /// nazev souboru, do ktereho se ulozi obrazek
        /// </summary>
        string _fileName;


        /// <summary>
        /// bitmapa pro uchovani vysledneho nebo docasneho obrazku
        /// </summary>
        public Bitmap Bitmap { get; private set; }

        /// <summary>
        /// pole vyrenderovaneho obrazku pro antialiasing
        /// </summary>
        Colour[,] _rawImgColours;


        private Renderer()
        {
            
        }

        /// <summary>
        /// <param name="raytracer">odkaz na raytracer, z ktereho budeme vykreslovat</param>
        /// <param name="size">velikost vysledneho obrazku</param>
        /// <param name="recurse">hloubka rekurze</param>
        /// <param name="antialias">zapnuty antialiasing</param>
        /// </summary>
        public Renderer(RayTracing raytracer, RayImage rayImg)
        {
            _rayTracer = raytracer;
            _rayTracer.RScene.IsOptimizing = rayImg.IsOptimalizing;
            _rayImg = rayImg;
            InitAll();
        }

        private void InitAll()
        {
            _bw = new BackgroundWorker();
            _bw.WorkerSupportsCancellation = true;
            _bw.WorkerReportsProgress = true;
            _bw.DoWork += new DoWorkEventHandler(_bw_DoWork);
            _bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bw_RunWorkerCompleted);
        }

        public void AddProgressChangeEventHandler(ProgressChangedEventHandler eventHandler)
        {
            _bw.ProgressChanged += eventHandler;
        }

        public void AddRenderCompletedEventHandler(RunWorkerCompletedEventHandler eventHandler)
        {
            _bw.RunWorkerCompleted += eventHandler;
        }

        bool _isBusy;
        
        /// <summary>
        /// vlakno dokoncilo svou cinnost
        /// </summary>
        void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        /// <summary>
        /// vlakno dokoncilo svou cinnost
        /// </summary>
        void _bw_SaveRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //this.Text = String.Format("Hotovo (100%) {0} x {1} {2}", this.pictureBoard.Width, this.pictureBoard.Height,
            //    _antialias ? "Antialias" : "");
            //this.saveAsToolStripMenuItem.Enabled = true;
            //if (_antialias && !_bw.CancellationPending)
            //    AntializeArrayCols();

            Bitmap.Save(_fileName, System.Drawing.Imaging.ImageFormat.Png);
        }

        private void RenderImage()
        {

            if (_rayImg.CurrentSize.Width < MinSize.Width || _rayImg.CurrentSize.Height < MinSize.Height)
                throw new Exception("size is smaller than minimum size");

            int rawWidth = _rayImg.CurrentSize.Width;
            int rawHeigh = _rayImg.CurrentSize.Height;

            RayTracerLib.Colour resCol;
            System.Drawing.Color color;

            if (_rayImg.IsAntialiasing)
            {
                rawWidth = _rayImg.CurrentSize.Width * 3;
                rawHeigh = _rayImg.CurrentSize.Height * 3;
            }

            _rayTracer.SetBoundValues(0, rawWidth, 0, rawHeigh);

            _rawImgColours = new Colour[rawHeigh, rawWidth];

            // hodnota progresu na hlavnim titulku formulare: height/celkovy_pocet_zobrazeni
            // napr. 20 zobrazeni - kazdych 5 procent
            int progres = rawHeigh / 20;

            int yPosuv;
            int coordX, coordY;

            for (int y = 0; y < rawHeigh; y++)
            {

                if (_bw.CancellationPending)
                    return;

                yPosuv = y % progres;

                if (yPosuv == 0)
                    _bw.ReportProgress(y * 100 / rawHeigh, new Bitmap(Bitmap));

                for (int x = 0; x < rawWidth; x++)
                {
                    if (_rayImg.MaxRecurse == -1)
                    {
                        resCol = _rayTracer.RayCast(x + 0.5, y + 0.5);
                    }
                    else
                    {
                        resCol = _rayTracer.RayTrace(_rayImg.MaxRecurse, x + 0.5, y + 0.5);
                    }

                    _rawImgColours[y, x] = resCol;

                    if (_rayImg.IsAntialiasing)
                    {
                        if (y % 3 == 0 && x % 3 == 0)
                        {
                            coordX = x / 3;
                            coordY = y / 3;
                            color = resCol.SystemColor();
                            Bitmap.SetPixel(coordX, coordY, color);
                        }
                    }
                    else
                    {
                        color = resCol.SystemColor();
                        Bitmap.SetPixel(x, y, color);
                    }
                }
            }

            if (_rayImg.IsAntialiasing)
                AntializeArrayCols();
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
            try
            {
                RenderImage();
                e.Result = Bitmap;
                if (_bw.CancellationPending)
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                int l = 0;
            }
            finally
            {
                _isBusy = false;
            }
        }

        /// <summary>
        /// Zastavi renderovani aktualniho obrazku
        /// </summary>
        public void StopRendering()
        {
            if (_bw.IsBusy)
                _bw.CancelAsync();
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

            RayTracerLib.Colour resCol;
            RayTracerLib.Colour[,] samples = new Colour[3, 3];
            System.Drawing.Color color;

            if (Bitmap != null)
                Bitmap.Dispose();

            Bitmap = new Bitmap(_rayImg.CurrentSize.Width, _rayImg.CurrentSize.Height);
            for (int y = 0; y < _rayImg.CurrentSize.Height; y++)
            {
                for (int x = 0; x < _rayImg.CurrentSize.Width; x++)
                {
                    resCol = new Colour(0, 0, 0, 0);
                    for (int i = -1; i < 2; i++)
                        for (int j = -1; j < 2; j++)
                        {
                            samples[i + 1, j + 1] = _rawImgColours[y * 3 + i + 1, x * 3 + j + 1];
                            resCol += samples[i + 1, j + 1];
                        }

                    resCol = resCol * ((double)1 / 9);
                    color = resCol.SystemColor();
                    Bitmap.SetPixel(x, y, color);
                }
            }
        }

        /// <summary>
        /// vyvola na pozadi nove vlakno pro vypocet obrazku
        /// </summary>
        public void RenderAsyncImage()
        {
            _isBusy = true;
            if (Bitmap != null)
                Bitmap.Dispose();
            Bitmap = new Bitmap(_rayImg.CurrentSize.Width, _rayImg.CurrentSize.Height);
            _bw.RunWorkerAsync();
        }

        /// <summary>
        /// vyrenderuje bitmapu a ulozi ji do zadaneho nazvu obrazku a fomatu
        /// </summary>
        /// <param name="fileName">nazev obrazku</param>
        /// <param name="imageFormat">format obrazku</param>
        public void Render2Image(string fileName, ImageFormat imageFormat)
        {
            //_fileName = fileName;
            //_bw.DoWork += new DoWorkEventHandler(_bw_DoWork);
            //_bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bw_SaveRunWorkerCompleted);
            if (Bitmap != null)
                Bitmap.Dispose();
            Bitmap = new Bitmap(_rayImg.CurrentSize.Width, _rayImg.CurrentSize.Height);
            RenderImage();
            Bitmap.Save(fileName, imageFormat);
            //_bw.RunWorkerAsync();
        }

        /// <summary>
        /// Vyrenderuje bitmapu a vrati ji
        /// </summary>
        /// <returns>bitmapa vyrenderovaneho obrazku</returns>
        public Bitmap Render2Bitmap()
        {
            if (Bitmap != null)
                Bitmap.Dispose();
            Bitmap = new Bitmap(_rayImg.CurrentSize.Width, _rayImg.CurrentSize.Height);
            RenderImage();
            return Bitmap;
        }

        public bool IsBusy()
        {
            return _isBusy;
            //if (_bw != null)
            //    return _bw.IsBusy;
            //return false;
        }
    }
}
