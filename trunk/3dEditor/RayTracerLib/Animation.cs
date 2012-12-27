using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Drawing.Imaging;
using Splicer.Timeline;
using Splicer.Renderer;
using System.Threading;
using Mathematics;

namespace RayTracerLib
{

    /// <summary>
    /// vyber typu animace:
    /// </summary>
    public enum AnimationType
    {
        /// <summary>
        /// vytvori obrazky i video z animace
        /// </summary>
        BothImagesAndVideo,
        /// <summary>
        /// vytvori pouze obrazky
        /// </summary>
        ImagesOnly,

        /// <summary>
        /// vytvori pouze video
        /// </summary>
        VideoOnly,
    };

    /// <summary>
    /// Trida slouzici pro vytvoreni animace
    /// </summary>
    public class Animation
    {

        /// <summary>
        /// OBECNA Rovnice elipsy:
        /// (x - x0)^2/A^2 + (y - y0)^2/B^2 = 1
        /// y bude ale rovno z
        /// 
        /// 
        /// Parametricky:
        /// X = X0 + a*cos(t)*cos(phi) - b*sin(t)*sin(phi)
        /// Y = Y0 + b*cos(t)*sin(phi) + b*sin(t)*cos(phi)
        /// </summary>
        public class Elipse
        {
            public Vektor Center { get; private set; }
            public double A { get; private set; }
            public double B { get; private set; }
            public double C { get; private set; }

            public Elipse() : this(new Vektor(0, -2, -5), 11, 2, 9) { }

            public Elipse(Vektor center, double a, double b, double c)
            {
                Center = center;
                A = a;
                B = b;
                C = c;
            }

            public Elipse(Elipse old)
            {
                Center = new Vektor(old.Center);
                A = old.A;
                B = old.B;
                C = old.C;
            }


            /// <summary>
            /// Spocita pozadovane BODY na elipse
            /// 
            /// X = X0 + a*cos(t)*cos(phi) - b*sin(t)*sin(phi)
            /// Y = Y0 + b*cos(t)*sin(phi) + b*sin(t)*cos(phi)
            /// </summary>
            /// <param name="count"></param>
            /// <returns></returns>
            public List<Vektor> GetEllipsePoints(int count)
            {
                if (count < 0)
                    count = 32;

                List<Vektor> points = new List<Vektor>(count);

                double beta = 180.0 * (Math.PI / 180.0); //(Math.PI/180) converts Degree Value into Radians
                double sinbeta = Math.Sin(beta);
                double cosbeta = Math.Cos(beta);

                double alpha, sinalpha, cosalpha;

                double x, y, z;
                double incr = 360.0 / count;
                for (double i = 0.0; i < 360; i += incr)
                {
                    alpha = i * (Math.PI / 180);
                    sinalpha = Math.Sin(alpha);
                    cosalpha = Math.Cos(alpha);

                    x = Center.X + (A * cosalpha * cosbeta - B * sinalpha * sinbeta);
                    y = Center.Y + (B * cosalpha * sinbeta + B * sinalpha * cosbeta);
                    z = Center.Z + (C * sinalpha);

                    Vektor p = new Vektor(x, y, z);
                    points.Add(p);
                }

                return points;
            }

            /// <summary>
            /// spocita obvod elipsy
            /// </summary>
            /// <returns>obvod elipsy</returns>
            public double Circumference()
            {
                double cit1 = (A - B - C) / (A + B + C);
                cit1 = 3 * cit1 * cit1;

                double jmenov = Math.Sqrt(4 - cit1);
                jmenov = 10 + jmenov;

                double zlom = 1 + cit1 / jmenov;

                double obvod = Math.PI * (A + B + C) * zlom;

                return obvod;
            }

            /// <summary>
            /// spocita POCET bodu elipsy potrebnych k vykresleni drahy trvajici zadany cas
            /// </summary>
            /// <param name="fps">rychlost (pocet jednotek za vterinu - fps) std=25/s</param>
            /// <param name="time">doba trvani jednoho obehu po elipse (ve vterinach)
            /// je-li 0, pak se vypocita optimalni pocet bodu</param>
            /// <returns>pocet bodu</returns>
            public int ComputeNumberOfPoints(double fps, double time)
            {
                double obvod = Circumference();
                double num = obvod;
                if (time > 0.0)
                {
                    double time1 = obvod / fps;
                    double timeRatio = time / time1;
                    num = obvod * timeRatio;
                }
                else
                {
                    double t = obvod / fps;
                    //double timeRatio = 1.0 / t;
                    num = obvod * t;
                }

                int pocet = (int)Math.Ceiling(num);
                return pocet;
            }

            public override string ToString()
            {
                return String.Format("Elipsa {0}; A={1}; B={2}", this.Center, this.A, this.B);
            }

        }

        private BackgroundWorker _bw;

        private RayTracing _rayTracer;

        /// <summary>
        /// Elipsa popisujici drahu kamery
        /// Smer kamery bude od bodu elipsy do stredu elipsy
        /// </summary>
        public Elipse Ellipse { get; private set; }

        /// <summary>
        /// souradnice vsech bodu na elipse, z nichz se bude vykreslovat animace
        /// </summary>
        private List<Vektor> _pathPoints;
        private int _counter;

        /// <summary>
        /// pocet bodu na elipse, neboli pocet obrazku v animaci
        /// </summary>
        private int _numPoints;
        private string _fileName;
        private string _imgName;

        private AnimationType _animType;

        RayImage _rayImg;

        double _fps;
        bool _generateImages;

        bool _isBusy;

        //public Animation(RayTracing raytracer) :this(raytracer,

        public Animation(RayTracing raytracer) :this(raytracer,new Elipse(), 25, 0.0){}

        public Animation(RayTracing raytracer, Elipse elipsa, double fps, double time)
        {
            _animType = AnimationType.VideoOnly;
            _rayTracer = raytracer;
            Ellipse = elipsa;
            SetPath(elipsa, fps, time);
            _counter = 0;
            _fileName = "anim";
            InitAll();
        }

        public Animation(Animation old)
        {
            _animType = old._animType;
            _rayTracer = new RayTracing(old._rayTracer);
            Ellipse = new Elipse(old.Ellipse);
            _numPoints = old._numPoints;
            _pathPoints = new List<Vektor>(old._pathPoints);
            _counter = old._counter;
            _fileName = old._fileName;
            _rayImg = new RayImage(_rayImg);
            _fps = old._fps;
            _bw = old._bw;
        }

        private void InitAll()
        {
            _bw = new BackgroundWorker();
            _bw.WorkerSupportsCancellation = true;
            _bw.WorkerReportsProgress = true;
            _bw.ProgressChanged += new ProgressChangedEventHandler(_bw_ProgressChanged);
            //_bw.DoWork += new DoWorkEventHandler(_bw_DoWork_Static);
            _bw.DoWork += new DoWorkEventHandler(_bw_DoWork_Paralel);
            _bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bw_WorkerCompleted);
        }

        /// <summary>
        /// nastavi celou cestu k animaci
        /// </summary>
        /// <param name="elipsa">elipsa popisujici cestu</param>
        /// <param name="fps">pocet snimku za sekundu</param>
        /// <param name="time">delka animace (ve vterinach). 
        /// je-li 0, pak bude delka optimalni vzhledem k fps.</param>
        public void SetPath(Elipse elipsa, double fps, double time)
        {
            Ellipse = elipsa;
            _fps = fps;
            _numPoints = elipsa.ComputeNumberOfPoints(fps, time);
            _pathPoints = Ellipse.GetEllipsePoints(_numPoints);
            
        }

        /// <summary>
        /// Progress Change vrati jak procenta, tak naposledny vykresleny obrazek
        /// </summary>
        /// <param name="eventHandler">delegat zachytavaci zmenu progresu</param>
        public void AddProgressChangeEventHandler(ProgressChangedEventHandler eventHandler)
        {
            _bw.ProgressChanged += eventHandler;
        }

        public void AddAnimationCompletedEventHandler(RunWorkerCompletedEventHandler eventHandler)
        {
            _bw.RunWorkerCompleted += eventHandler;
        }

        /// <summary>
        /// Hlavni metoda animace, ktera se posune do dalsiho policka animace
        /// </summary>
        /// <returns>true, kdyz byl uspesny posun</returns>
        public bool MoveToNextImage()
        {
            if (!(_counter < _numPoints))
                return false;

            Vektor camSource = _pathPoints[_counter];
            _rayTracer.RCamera.Source = camSource;
            Vektor camNorm = Vektor.ToDirectionVektor(camSource, Ellipse.Center);
            camNorm.Normalize();

            _rayTracer.RCamera.SetNormAndUp(camNorm, _rayTracer.RCamera.Up);
            _counter++;
            return true;
        }


        /// <summary>
        /// Zacne provadet celou animaci. Nejdrive nastavi potrebne pocatecni parametry
        /// </summary>
        /// <param name="size">rozliseni animace</param>
        /// <param name="recursion">rekurze kazdeho obrazku</param>
        /// <param name="antialias">antialiasing</param>
        /// <param name="file">soubor animace</param>
        /// <param name="generateImages">maji-li se s animaci generovat i obrazky</param>
        public void StartAnimation(Size size, string file, AnimationType animType)
        {
            if ((_bw.IsBusy == true))
                return;

            _fileName = file;
            _animType = animType;

            if (_animType != AnimationType.VideoOnly)
            {
                _imgName = Path.GetFileNameWithoutExtension(file);
                _imgName = Path.Combine(Path.GetDirectoryName(file), _imgName);
            }

            _counter = 0;
            _isBusy = true;
            if (_bw.IsBusy != true)
                _bw.RunWorkerAsync();
        }

        /// <summary>
        /// zjisti, zda jsme jiz vyrenderovali obrazky ze vsech bodu na draze kamery
        /// </summary>
        /// <returns>true, kdyz jsou vyrenderovany vsechny obrazky animace</returns>
        private bool IsEndOfAnimation()
        {
            return (!(_counter < _numPoints));
        }
        /// <summary>
        /// Zastavi provadeni cele animace
        /// </summary>
        public void StopAnimation()
        {
            lock (_bw)
            {
                if (_bw.IsBusy)
                    _bw.CancelAsync();
            }
        }

        /// <summary>
        /// provadeni animace v novem vlakne, ale seriove vytvari obrazky 
        /// (obrazky vytvari ve stejnem vlakne, takze pro ukonceni animace se musi cekat 
        /// na ukonceni vykresleni celeho obrazku - nevhodne pro velke a slozite obrazky)
        /// </summary>
        void _bw_DoWork_Static(object sender, DoWorkEventArgs e)
        {
            Renderer renderer = new Renderer(_rayTracer, _rayImg);
            int i = 0;
            List<Bitmap> images = new List<Bitmap>(this._numPoints);
            Bitmap bmp;
            StreamWriter str = new StreamWriter(Path.Combine(Path.GetDirectoryName(_fileName), "out.txt"));

            ITimeline timeline = new DefaultTimeline(_fps);
            IGroup group = timeline.AddVideoGroup(24, _rayImg.CurrentSize.Width, _rayImg.CurrentSize.Height);
            ITrack videoTrack = group.AddTrack();

            double secs = (double)(1.0 / _fps);     // doba obrazku v sekundach
            int progres;

            while (MoveToNextImage() && !_bw.CancellationPending)
            {
                //str.Write(String.Format("{0:0000}) Source: {1}, Norm: {2}\r\n", i, _rayTracer.RCamera.Source.ToString(),
                //    _rayTracer.RCamera.Norm.ToString()));

                
                bmp = renderer.Render2Bitmap();
                if (_generateImages)
                    bmp.Save(String.Format("{0}{1:0000}.png", _imgName, i), System.Drawing.Imaging.ImageFormat.Png);

                images.Add(bmp);
                videoTrack.AddImage(bmp, 0, secs);  // pridani obrazku do animace na konec predchozich obrazku
                                                    // s dobou zobrazeni obrazku: secs 
                progres = (int)(i*100 / _numPoints);
                _bw.ReportProgress(progres, new Bitmap(bmp));
                i++;
            };
            str.Close();
            str.Dispose();
            try
            {
                if (videoTrack.Duration > 0.0)
                    using (AviFileRenderer animRenderer = new AviFileRenderer(timeline, _fileName))
                    {
                        animRenderer.Render();
                    }
            }
            catch (Exception ex)
            {
                int jj = 0;
            }
            finally
            {
                timeline.Dispose();
                _isBusy = false;
            }

            if (_bw.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// iterator pres vsechny obrazky
        /// ukazuje na poradi aktualne vykreslovaneho obrazku
        /// </summary>
        int _iter;
        /// <summary>
        /// animacni seznam pro ukladani obrazku animace
        /// </summary>
        ITrack _videoTrack;
        /// <summary>
        /// doba jednoho obrazku v sekundach
        /// </summary>
        double _secs;
        /// <summary>
        /// indikator ukonceni vykreslovani posledniho obrazku
        /// </summary>
        bool _finished;

        void _bw_DoWork_Paralel(object sender, DoWorkEventArgs e)
        {
            ITimeline timeline = null;
            try
            {
                Renderer renderer = new Renderer(_rayTracer, _rayImg);
                renderer.AddRenderCompletedEventHandler(new RunWorkerCompletedEventHandler(this.OnRenderedImage));

                timeline = new DefaultTimeline(_fps);
                IGroup group = timeline.AddVideoGroup(24, _rayImg.CurrentSize.Width, _rayImg.CurrentSize.Height);
                _videoTrack = group.AddTrack();

                _iter = 0;
                _finished = true;
                _secs = (double)(1.0 / _fps);     // doba jednoho obrazku v sekundach

                while (!IsEndOfAnimation())
                {
                    // pozadavek na ukonceni vykreslovani animace
                    if (_bw.CancellationPending)
                    {
                        e.Cancel = true;
                        renderer.StopRendering();
                        break;
                    }

                    // jestli se jeste renderuje obrazek, uspime na chvili vlakno
                    if (!_finished)             // ceka na vsechny obrazky
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    renderer = new Renderer(_rayTracer,_rayImg);
                    renderer.AddRenderCompletedEventHandler(new RunWorkerCompletedEventHandler(this.OnRenderedImage));

                    MoveToNextImage();              //posuneme se do dalsiho bodu cesty
                    renderer.RenderAsyncImage();
                    _iter++;
                    _finished = false;

                };

                // cekani na ukonceni procesu vykreslujici posledni obrazek
                while (!_finished)
                {
                    Thread.Sleep(50);
                }
                if (_videoTrack.Duration > 0.0)
                    using (AviFileRenderer animRenderer = new AviFileRenderer(timeline, _fileName))
                    {
                        animRenderer.Render();
                    }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
            }
            finally
            {
                _videoTrack.Dispose();
                _isBusy = false;
            }
        }

        private void OnRenderedImage(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled)
                {
                    Bitmap bmp = (Bitmap)e.Result;

                    if (_animType != AnimationType.VideoOnly)
                        bmp.Save(String.Format("{0}{1:0000}.png", _imgName, _iter), System.Drawing.Imaging.ImageFormat.Png);

                    if (_animType != AnimationType.ImagesOnly)
                    {
                        // pridani obrazku do animace na konec predchozich obrazku
                        // s dobou zobrazeni obrazku: secs 
                        _videoTrack.AddImage(bmp, 0, _secs);  
                    }
                    
                    
                    int progres = (int)(_iter * 100 / _numPoints);
                    lock (_bw)
                    {
                        _bw.ReportProgress(progres, new Bitmap(bmp));
                    }

                    if (_animType != AnimationType.VideoOnly)
                        bmp.Dispose();
                }
            }
            catch (Exception ex)
            {
                if (_bw.IsBusy)
                    _bw.CancelAsync();
            }
            finally
            {
                _finished = true;
            }
        }


        void _bw_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
        }

        void _bw_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        public bool IsBusy()
        {
            return _isBusy;
        }

        public override string ToString()
        {
            return String.Format("Dráha: {0}", this.Ellipse);
        }

    }

    
}

