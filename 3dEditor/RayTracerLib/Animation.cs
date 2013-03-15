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
using System.Runtime.Serialization;
using Splicer.WindowsMedia;

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
    [DataContract]
    public class Animation : LabeledShape
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
        [DataContract]
        public class Elipse
        {
            [DataMember]
            public Vektor Center { get; private set; }
            [DataMember]
            public double A { get; private set; }
            [DataMember]
            public double B { get; private set; }

            [DataMember]
            private double[] Degs { get; set; }


            public Elipse() : this(new Vektor(1, -1, 1), 8, 6) { }

            public Elipse(Vektor center, double a, double b)
            {
                Center = center;
                A = a;
                B = b;
                //_shiftMatrix = Matrix3D.PosunutiNewMatrix(center);
                //_rotatMatrix = Matrix3D.Identity;
                Degs = new double[3] { 0, 0, 0 };
            }

            public Elipse(Elipse old)
            {
                Center = new Vektor(old.Center);
                A = old.A;
                B = old.B;
                Degs = new double[3] { old.Degs[0], old.Degs[1], old.Degs[2] };
                //_shiftMatrix = new Matrix3D(old._shiftMatrix);
                //_rotatMatrix = new Matrix3D(old._rotatMatrix);
            }


            public List<Vektor> GetEllipsePoints(double fps, double time)
            {
                int numberOfPoints = ComputeNumberOfPoints(fps, time);
                List<Vektor> points = GetEllipsePoints(numberOfPoints);
                return points;
            }

            /// <summary>
            /// vypocita vsechny body na elipse
            /// </summary>
            /// <param name="count">pocet bodu, kolik se ma vratit</param>
            /// <returns>body na elipse</returns>
            public List<Vektor> GetEllipsePoints(int count)
            {
                //int sides = _SIDE_NUM;  // The amount of segment to create the circle
                int sides = count;

                double thetaRads = 0;
                List<Vektor> points = new List<Vektor>();
                double loops = 360.0 / sides;
                for (double a = 0; a < 360; a += loops)
                {
                    double phi = a * Math.PI / 180;
                    float x = (float)(Math.Cos(thetaRads) * Math.Sin(phi) * A);
                    float y = (float)(Math.Sin(phi) * Math.Sin(thetaRads));
                    float z = (float)(Math.Cos(phi) * B);
                    Vektor p = new Vektor(x, z, y);
                    points.Add(p);
                }
                //points.Add(new Vektor(points[0].X, points[0].Y, points[0].Z)); // posledni bod stejny jako prvni?

                Matrix3D rotatMat = Matrix3D.NewRotateByDegrees(Degs[0], Degs[1], Degs[2]);
                Matrix3D shiftMat = Matrix3D.PosunutiNewMatrix(Center);
                Matrix3D localMatrix = rotatMat * shiftMat;
                localMatrix.TransformPoints(points);

                return points;
            }

            public Matrix3D GetRotationMatrix()
            {
                return Matrix3D.NewRotateByDegrees(Degs[0], Degs[1], Degs[2]);
            }

            public double[] GetRotationAngles()
            {
                double[] degs = new double[3] { Degs[0], Degs[1], Degs[2] };
                return degs;
            }
            public void Rotate(Matrix3D rotMatrix)
            {
                //_rotatMatrix = rotMatrix;
                Degs = rotMatrix.GetAnglesFromMatrix();
            }

            public void Move(Matrix3D shiftMatrix)
            {
                Center = new Vektor(shiftMatrix.Matrix[0, 3], shiftMatrix.Matrix[1, 3], shiftMatrix.Matrix[2, 3], shiftMatrix.Matrix[3, 3]);
                //_shiftMatrix = shiftMatrix;
            }

            /// <summary>
            /// spocita obvod elipsy
            /// </summary>
            /// <returns>obvod elipsy</returns>
            private double Circumference()
            {
                if (Math.Abs(A) < MyMath.EPSILON && Math.Abs(B) < MyMath.EPSILON) return 0;

                double cit1 = (A - B) / (A + B);
                cit1 = 3 * cit1 * cit1;

                double jmenov = Math.Sqrt(4 - cit1);
                jmenov = 10 + jmenov;

                double zlom = 1 + cit1 / jmenov;

                double obvod = Math.PI * (A + B) * zlom;

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
                if (obvod == 0) return 0;

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
                return String.Format("Ellipse {0}; A={1}; B={2}", this.Center, this.A, this.B);
            }

            public Elipse FromDeserial()
            {
                Elipse elips = new Elipse(this);
                return elips;
            }

        }

        private BackgroundWorker _bw;

        private RayTracing _rayTracer;

        /// <summary>
        /// Elipsa popisujici drahu kamery
        /// Smer kamery bude od bodu elipsy do stredu elipsy
        /// </summary>
        [DataMember]
        public Elipse ElipsePath { get; private set; }

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

        [DataMember]
        public String FileFullPath { get; set; }
        /// <summary>
        /// fullPath Without Extension
        /// </summary>
        private string _BaseName;

        [DataMember]
        public AnimationType AnimType { get; set; }

        RayImage _rayImg;

        private double _fps = 25;
        /// <summary>
        /// frames per second. Minimalne 20, Maximalne 50
        /// </summary>
        [DataMember]
        public double Fps { get { return _fps; } set { if (value < 20 && value > 50) _fps = 25; else _fps = value; } }

        private double _time = 1;
        /// <summary>
        /// cas v sekundach. minimalne 1s, maximalne 10s
        /// </summary>
        [DataMember]
        public double Time { get { return _time; } set { if (value < 1) _time = 1; else if (value > 50)  _time = 50; else _time = value; } }
        bool _generateImages;

        bool _isBusy;

        [DataMember]
        public bool IsVisible { get; set; }
        //public Animation(RayTracing raytracer) :this(raytracer,

        /// <summary>
        /// konstruktor s prednastavenymi hodnotami: fps=20, time =1
        /// </summary>
        /// <param name="raytracer"></param>
        public Animation() :this(25, 1.0){}

        public Animation(double fps, double time) : this(new Vektor(1, -1, 1), 10, 8, fps, time) { }
        public Animation(Vektor center, double a, double b, double fps, double time)
        {
            SetLabelPrefix("anim");
            AnimType = AnimationType.VideoOnly;
            SetPath(center, a, b, fps, time);
            _fileName = "anim.avi";
            FileFullPath = Path.Combine(Environment.CurrentDirectory, _fileName);
            InitAll();
        }

        public Animation(Animation old)
        {
            this.SetElipse(new Elipse(old.ElipsePath), old.Fps, old.Time);
            AnimType = old.AnimType;
            this.Label = old.Label;
            FileFullPath = old.FileFullPath;
            IsVisible = old.IsVisible;
        }

        Vektor _up;
        /// <summary>
        /// inicializace potrebna pred kazdym spustenim cele animace
        /// </summary>
        public void InitAll()
        {
            _numPoints = ElipsePath.ComputeNumberOfPoints(Fps, Time);
            _pathPoints = ElipsePath.GetEllipsePoints(_numPoints);
            _counter = 0;
            _up = CreateUpVektor();

            _bw = new BackgroundWorker();
            _bw.WorkerSupportsCancellation = true;
            _bw.WorkerReportsProgress = true;
            _bw.ProgressChanged += new ProgressChangedEventHandler(_bw_ProgressChanged);
            //_bw.DoWork += new DoWorkEventHandler(_bw_DoWork_Static);
            _bw.DoWork += new DoWorkEventHandler(_bw_DoWork_Paralel);
            _bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bw_WorkerCompleted);
        }

        public void SetElipse(Elipse elipse, double fps, double time)
        {
            ElipsePath = elipse;
            Fps = fps;
            Time = time;
        }

        /// <summary>
        /// nastavi celou cestu k animaci
        /// </summary>
        /// <param name="elipsa">elipsa popisujici cestu</param>
        /// <param name="fps">pocet snimku za sekundu</param>
        /// <param name="time">delka animace (ve vterinach). 
        /// je-li 0, pak bude delka optimalni vzhledem k fps.</param>
        public void SetPath(Vektor center, double a, double b, double fps, double time)
        {
            ElipsePath = new Elipse(center, a, b);
            Fps = fps;
            Time = time;
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

        private Vektor CreateUpVektor()
        {
            int count = _pathPoints.Count;
            Random rnd = new Random();
            int i11 = 0; //rnd.Next(0, (int)Math.Round(count / 2.0) - 1);
            int i12 = count / 4;
            int i21 = count / 2;//rnd.Next((int)Math.Round(count / 2.0), count);
            int i22 = count * 3 / 4;
            Vektor v1 = Vektor.ToDirectionVektor(_pathPoints[i11], _pathPoints[i21]);
            v1.Normalize();
            Vektor v2 = Vektor.ToDirectionVektor(_pathPoints[i12], _pathPoints[i22]);
            v2.Normalize();
            Vektor up = v1.CrossProduct(v2);
            up.Normalize();
            return up;
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
            Vektor camNorm = Vektor.ToDirectionVektor(camSource, ElipsePath.Center);
            camNorm.Normalize();

            _rayTracer.RCamera.SetNormAndUp(camNorm, _up);
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
        public void StartAnimation(RayTracing rayTracer, RayImage rayImage)
        {
            this._rayTracer = rayTracer;
            // animace pracuje nepracuje spravne pro vsechna rozliseni - vyvarujeme se toho zvolenim nejblilzsich odzkousenych rozlisenich
            if (rayImage.IsCustomResolution()) rayImage.SelectClosestResolution(); 
            this._rayImg = rayImage;

            if ((_bw.IsBusy == true))
                return;

            // oddelime priponu pro ukladani stejne jmneno animace i obrazku
            _BaseName = Path.GetFileNameWithoutExtension(FileFullPath);
            _BaseName = Path.Combine(Path.GetDirectoryName(FileFullPath), _BaseName); 

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

            ITimeline timeline = new DefaultTimeline(Fps);
            IGroup group = timeline.AddVideoGroup(24, _rayImg.CurrentSize.Width, _rayImg.CurrentSize.Height);
            ITrack videoTrack = group.AddTrack();

            double secs = (double)(1.0 / Fps);     // doba obrazku v sekundach
            int progres;

            while (MoveToNextImage() && !_bw.CancellationPending)
            {
                //str.Write(String.Format("{0:0000}) Source: {1}, Norm: {2}\r\n", i, _rayTracer.RCamera.Source.ToString(),
                //    _rayTracer.RCamera.Norm.ToString()));

                
                bmp = renderer.Render2Bitmap();
                if (_generateImages)
                    bmp.Save(String.Format("{0}{1:0000}.png", _BaseName, i), System.Drawing.Imaging.ImageFormat.Png);

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

                timeline = new DefaultTimeline(Fps);
                IGroup group = timeline.AddVideoGroup(24, _rayImg.CurrentSize.Width, _rayImg.CurrentSize.Height);
                _videoTrack = group.AddTrack();

                _iter = 0;
                _finished = true;
                _secs = (double)(1.0 / Fps);     // doba jednoho obrazku v sekundach

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
                    using (AviFileRenderer animRenderer = new AviFileRenderer(timeline, String.Format("{0}.avi", _BaseName)))
                    {
                        //AsyncCallback callback = new AsyncCallback(RenderCallback);
                        //IAsyncResult result = animRenderer.BeginRender(callback, null);

                        //try
                        //{
                        //    animRenderer.EndRender(result);
                        //}
                        //catch (Exception ex)
                        //{
                        //    int das = 0;
                        //}

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

        static void RenderCallback(IAsyncResult result)
        {
            int i = 0;
        }
        void animRenderer_RenderCompleted(object sender, EventArgs e)
        {
            int i = 0;
        }

        private void OnRenderedImage(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled)
                {
                    Bitmap bmp = (Bitmap)e.Result;

                    if (AnimType != AnimationType.VideoOnly)
                        bmp.Save(String.Format("{0}{1:0000}.png", _BaseName, _iter), System.Drawing.Imaging.ImageFormat.Png);

                    if (AnimType != AnimationType.ImagesOnly)
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

                    if (AnimType != AnimationType.VideoOnly)
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
            return String.Format("{0} Path: {1}", Label, this.ElipsePath);
        }


        public void Rotate(double degAroundX, double degAroundY, double degAroundZ)
        {
            return;
        }

        public Animation FromDeserial()
        {
            Animation anim = new Animation(this);
            return anim;
        }

        public static Animation[] MergeAnimations(Animation[] anim1, Animation[] anim2)
        {
            if (anim1 == null) return anim2;
            if (anim2 == null) return anim1;

            List<Animation> animList = new List<Animation>(anim1);
            foreach (Animation anim in anim2)
            {
                if (!LabeledShape.IsAvailable(anim.Label))
                    anim.Label = anim.GetUniqueName();
                animList.Add(anim);
            }

            return animList.ToArray();
        }
    }

    
}

