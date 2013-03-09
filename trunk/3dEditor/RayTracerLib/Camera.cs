using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;
using System.Runtime.Serialization;

namespace RayTracerLib
{
    /// <summary>
    /// Kamera, neboli gnerator paprsku, generuje paprsky z projekcni roviny, ktere jsou na ni kolme
    /// Obsahuje tedy souradnice stredu roviny a hodnoty potrebne pro sestaveni smeroveho vektoru:
    /// hranicni body projekcni roviny
    /// </summary>
    [DataContract]
    public class Camera
    {

        /// <summary>
        /// uhel, o ktery vektor UP rotuje okolo smeru kamery Norm
        /// </summary>
        [DataMember]
        public double AngleUp { get; private set; }
        /// <summary>
        /// Stred projekcni roviny (v karteskych souradnicich)
        /// </summary>
        [DataMember]
        public Vektor Source { get; set; }

        private Vektor _norm;
        private Vektor _normNormalized;
        /// <summary>
        /// Normala projekcni roviny
        /// </summary>
        [DataMember(Name="Normal")]
        public Vektor Norm { get { return _norm; } 
            private set 
            { 
                _norm = value;
                _normNormalized = new Vektor(_norm);
                _normNormalized.Normalize();
            } 
        }

        /// <summary>
        /// Vektor smeru nahoru (rovnobezny s y) v projekcni rovine (normalizovany)
        /// </summary>
        [DataMember]
        public Vektor Up { get; private set; }

        /// <summary>
        /// Vektor smeru doprava (rovnobezny s x) v projekcni rovine (normalizovany) 
        /// </summary>
        public Vektor Dx { get; private set; }

        /// <summary>
        /// Smer Y pro posun po rovine (= -up)
        /// </summary>
        public Vektor Dy { get; private set; }

        /// <summary>
        /// Minimalni hodnota X v rovine, kterou snima kamera
        /// </summary>
        public double XMin { get; set; }

        /// <summary>
        /// Maximalni hodnota X v rovine, kterou snima kamera
        /// </summary>
        public double XMax { get; set; }

        /// <summary>
        /// Minimalni hodnota Y v rovine, kterou snima kamera
        /// </summary>
        public double YMin { get; set; }

        /// <summary>
        /// Maximalni hodnota Y v rovine, kterou snima kamera
        /// </summary>
        public double YMax { get; set; }

        public Camera()
        {
            Source = new Vektor(0, 1, 0);
            Norm = new Vektor(0, 0, -1);
            Up = new Vektor(0, 1, 0);
            Dx = Vektor.CrossProduct(Up, Norm);
            Dx.MultiplyBy(-1);
            Dy = new Vektor(Vektor.ZeroVektor - Up);

            // zorny uhel:
            double angle = 62.0;
            double anglepul = angle / 2;
            XMax = Math.Tan(Math.PI / 180 * anglepul);
            XMin = -XMax;
            SetAspectParams(4.0 / 3.0);
        }

        public Camera(Camera old)
        {
            AngleUp = old.AngleUp;
            Source = new Vektor(old.Source);
            Norm = new Vektor(old.Norm);
            Up = new Vektor(old.Up);
            Dx = new Vektor(old.Dx);
            Dy = new Vektor(old.Dy);
            XMin = old.XMin;
            XMax = old.XMax;
            YMin = old.YMin;
            YMax = old.YMax;
        }

        /// <summary>
        /// nastavi oba hlavni vektory. Nejsou-li k sobe kolme, tak bude vektor up zmenen na kolmy k vektoru norm.
        /// Vynuluje pritom vzdy uhel rotace vektoru UP okolo vektoru Norm, jelikoz se oba nastavuji znovu.
        /// </summary>
        /// <param name="norm">vektor smeru vpred</param>
        /// <param name="up">vektor pohledu nahoru</param>
        public void SetNormAndUp(Vektor norm, Vektor up)
        {
            double temp = up * norm;
            if (Math.Abs(up * norm) > MyMath.EPSILON)
            {
                SetUpByNorm(norm);
                return;
            }
            
            Norm = new Vektor(norm);
            Up = new Vektor(up);
            // pri kazdem presnem nastaveni vektoru Norm a UP se vynuluje uhel rotace UP okolo Norm
            AngleUp = 0;

            setNormAndUp();
        }

        /// <summary>
        /// nastavi kameru jen podle smeru kamery vpred
        /// Ostatni vektory nastavi podle nej. Vektor UP bude vybran jako kolmy vektor k Norm
        /// </summary>
        /// <param name="norm">vektor pred - smer pohledu kamery</param>
        public void SetUpByNorm(Vektor norm)
        {
            Vektor up = norm.GetOrthogonal();
            up.Normalize();
            //if (up * Up < 0) up.MultiplyBy(-1);
            SetNormAndUp(norm, up);
        }

        /// <summary>
        /// nastavi kameru podle vektoru smerem nahoru, ktery je kolmy na vektor smeru vpred
        /// Ostatni vektory se nastavi podle nej. Vektor Norm smeru dopredu bude vybran jako jeden z kolmych k up
        /// </summary>
        /// <param name="up">vektor smeru nahoru</param>
        public void SetNormByUp(Vektor up)
        {
            Vektor norm = up.GetOrthogonal();
            norm.Normalize();
            SetNormAndUp(norm, up);
        }

        /// <summary>
        /// rotuje vektor smeru nahoru (vektor UP)
        /// nejdrive je vektor UP nastaven na inicializovany - tedy rotovan zpet o opacny stary uhel.
        /// Pak je z pocatecniho stavu opet rotovan o zadany novy uhel
        /// </summary>
        /// <param name="angleDeg">uhel, ktery bude svirat novy vektor UP s vektorem UP z inicializace</param>
        public void RotateUp(double angleDeg)
        {
            // predpoklad, ze Norm * Up = 0
            Quaternion q = new Quaternion(Norm, -AngleUp);
            double[] degs = q.ToEulerDegs();
            Matrix3D rotMat = Matrix3D.NewRotateByDegrees(-degs[0], -degs[1], -degs[2]);
            Up = rotMat.Transform2NewPoint(Up);
            
            AngleUp = angleDeg;

            q = new Quaternion(Norm, angleDeg);
            degs = q.ToEulerDegs();
            rotMat = Matrix3D.NewRotateByDegrees(-degs[0], -degs[1], -degs[2]);
            Up = rotMat.Transform2NewPoint(Up);

            // oba hlavni vektory nastaveny
            setNormAndUp();
        }

        /// <summary>
        /// Predpoklada jiz spravne nastaveny hlavni vektory Norm a UP
        /// Nastavi zbyvajici vektory.
        /// </summary>
        private void setNormAndUp()
        {
            _normNormalized = new Vektor(Norm);
            _normNormalized.Normalize();
            Vektor upNormalized = new Vektor(Up);
            upNormalized.Normalize();
            Dx = Vektor.CrossProduct(upNormalized, _normNormalized);
            Dx.MultiplyBy(-1);
            Dy = new Vektor(Vektor.ZeroVektor - upNormalized);
        }

        /// <summary>
        /// Nastavi pomer stran a podle nej je zvolen uhel
        /// cim ni
        /// typicke uhly pro pomer stran jsou:
        /// sirokouhly (16:9) ... 84° and 64°
        /// normalni (4:3) ... 62° and 40°
        /// </summary>
        /// <param name="ratio">pomer (napr. 4/3 nebo 16/9)</param>
        public void SetAspectParams(double ratio)
        {
            // preddefinovany zakladni uhel
            double angle = 60;
            if (Math.Round(ratio,2) == 1.78)
            {
                angle = 80;
            }

            SetAspectParams(ratio, angle);
        }


        /// <summary>
        /// Nastavi pomer stran a umozni nastavit i vlastni uhel
        /// typicke uhly pro pomer stran jsou:
        /// sirokouhly (16:9) ... 84° and 64°
        /// normalni (4:3) ... 62° and 40°
        /// </summary>
        /// <param name="ratio">pomer (napr. 4/3 nebo 16/9)</param>
        /// <param name="angle">zorny uhel kamery</param>
        public void SetAspectParams(double ratio, double angle)
        {
            // zorny uhel:
            if (angle <= 0 || angle >= 180)
                angle = 60.0;
            double anglepul = angle / 2;
            XMax = Math.Tan(Math.PI / 180 * anglepul);
            XMin = -XMax;
            YMax = XMax / ratio;
            YMin = XMin / ratio;
        }

        /// <summary>
        /// Vygeneruje pro zadane souradnice na projekcni rovine paprsek vychazejici z roviny, ktery je na ni kolmy
        /// </summary>
        /// <param name="x">x-ova souradnice projekcni roviny</param>
        /// <param name="y">y-ova souradnice projekcni roviny</param>
        /// <param name="P0">Pocatek parsku</param>
        /// <param name="P1">Smer paprsku</param>
        /// <returns>true, kdyz paprsek byl spravne vygenerovan. Vysledny paprsek bude tedy ulozen v P0 a P1</returns>
        public bool TryGetCameraRay(double x, double y, ref Vektor P0, ref Vektor P1)
        {
            if (x < XMin || x > XMax || y < YMin || y > YMax)
                return false;

            P0 = new Vektor(Source);

            P1.X = _normNormalized.X + x * Dx.X + y * Dy.X;
            P1.Y = _normNormalized.Y + x * Dx.Y + y * Dy.Y;
            P1.Z = _normNormalized.Z + x * Dx.Z + y * Dy.Z;

            //P1.Normalize();

            return true;
        }

        public void MoveToPoint(double dx, double dy, double dz)
        {
            this.Source.X = dx;
            this.Source.Y = dy;
            this.Source.Z = dz;
        }
        public override string ToString()
        {
            return "Camera: Center=" + Source + "; Norm=" + Norm;
        }

        public static Camera FromDeserial(Camera camera)
        {
            camera.SetNormAndUp(camera.Norm, camera.Up);
            camera.RotateUp(camera.AngleUp);
            return camera;
        }
    }
}