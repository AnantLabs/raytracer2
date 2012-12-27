using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{

    /// <summary>
    /// Trida pro zobrazeni sceny.
    /// Obsahuje objekt pro popis sceny a kameru, kterou se scena snima.
    /// Obsahuje objekty pro scenu, kameru
    /// 
    /// </summary>
    public class RayTracing
    {

        Scene _rscene;
        /// <summary>
        /// objekt pro scenu: svetla, objekty v ni
        /// </summary>
        public Scene RScene
        {
            get
            {
                return _rscene;
            }
            set
            {
                _rscene = value;
                if (_rscene != null)
                    RCamera = _rscene.Camera;
            }
        }

        /// <summary>
        /// kamera snima scenu - vrha paprsky do sceny
        /// </summary>
        public Camera RCamera { get; set; }

        /// <summary>
        /// hranice kresliciho platna
        /// </summary>
        double _xMin, _xMax, _yMin, _yMax;

        /// <summary>
        /// scitaci faktor pro linearni transformaci projekcni roviny osy X
        /// </summary>
        double _xA;
        /// <summary>
        /// nasobici faktor pro linearni transformaci projekcni roviny osy X
        /// </summary>
        double _xK;
        /// <summary>
        /// scitaci faktor pro linearni transformaci projekcni roviny osy Y
        /// </summary>
        double _yA;
        /// <summary>
        /// nasobici faktor pro linearni transformaci projekcni roviny osy Y
        /// </summary>
        double _yK;

        int _MaxDepth;


        public RayTracing()
        {
            _xMin = 0;
            _xMax = 320;
            _yMin = 0;
            _yMax = 240;

            _xA = 1;
            _xK = 10;
            _yA = 1;
            _yK = 10;

            RScene = new Scene();
            if (RScene.Camera != null)
                RCamera = RScene.Camera;

            _MaxDepth = 3;
        }

        public RayTracing(RayTracing old)
        {
            RScene = new Scene(old.RScene);
            RCamera = new Camera(old.RCamera);
            _xA = old._xA;
            _xK = old._xK;
            _xMax = old._xMax;
            _xMin = old._xMin;
            _yA = old._yA;
            _yK = old._yK;
            _yMax = old._yMax;
            _yMin = old._yMin;
            _MaxDepth = old._MaxDepth;
        }

        /// <summary>
        /// Nastavi hranice a koeficienty pro projekcni rovinu.
        /// Dostane hranicni souradnice kresliciho platna a vypocita z nich hranicni souradnice 
        /// projekcni roviny a transformacni koeficienty
        /// </summary>
        /// <param name="xmin">minimalni souradnice bodu x v kreslicim platne</param>
        /// <param name="xmax">maximalni souradnice bodu x v kreslicim platne</param>
        /// <param name="ymin">minimalni souradnice bodu y v kreslicim platne</param>
        /// <param name="ymax">maximalni souradnice bodu y v kreslicim platne</param>
        public void SetBoundValues(double xmin, double xmax, double ymin, double ymax)
        {
            _xMin = xmin;
            _xMax = xmax;
            if (_xMax <= _xMin)
                _xMax = _xMin + 1;

            _yMin = ymin;
            _yMax = ymax;
            if (_yMax <= _yMin)
                _yMax = _yMin + 1;

            // nastavime pomer stran kamery
            RCamera.SetAspectParams((double)((_xMax - _xMin) / (_yMax - _yMin)));

            // nastavime transformacni koeficienty mezi kreslicim platnem a projekcni rovinou
            _xK = (RCamera.XMax - RCamera.XMin) / (_xMax - _xMin);
            _xA = RCamera.XMin - _xMin * _xK;

            _yK = (RCamera.YMax - RCamera.YMin) / (_yMax - _yMin);
            _yA = RCamera.YMin - _yMin * _yK;
        }

        /// <summary>
        /// zakladni metoda pro urceni barvy bodu. pretransformuje souradnice kresliciho platna neboli obrazku
        /// do souradnic projekcni roviny, zjisti paprsek vychazejici z kamery a ze sceny urci pripadny
        /// bod, jenz paprsek protina a podle toho vrati barvu, jez se ma zobrazit v kreslicim platne
        /// </summary>
        /// <param name="x">souradnice x na kreslicim platne (obrazku)</param>
        /// <param name="y">souradnice y na kreslicim platne (obrazku)</param>
        /// <returns>barva bodu o danych souradnicich</returns>
        public Colour RayCast(double x, double y)
        {
            // pocatecni barva
            Colour color = new Colour(0, 0, 0, 1);

            Vektor P0 = new Vektor();
            Vektor Pd = new Vektor();

            // zkusime vygenerovat paprsek z kamery do bodu [x,y] prevedeneho na projekcni rovinu
            if (!RCamera.TryGetCameraRay(_xA + _xK * x, _yA + _yK * y, ref P0, ref Pd))
                return color;

            // zjistime, zda paprsek protina nejaky objekt
            SolidPoint sp = RScene.GetIntersectPoint(P0, Pd);

            if (sp == null)     // paprsek nic neprotina -> bude pouze barva pozadi
            {
                color = new Colour(RScene.BgColor);
                return color;
            }

            // sp - prave mame bod povrchu objektu, ktery vykreslime


            Pd = Vektor.ZeroVektor - Pd;

            Light[] lights = RScene.GetAllLightningsToPoint(sp);

            color = this.lightSum(sp, Pd, lights);

            return color;
        }


        public Colour RayTrace(int maxDepth, double x, double y)
        {
            _MaxDepth = maxDepth;

            // pocatecni barva
            Colour color = new Colour(0, 0, 0, 1);

            Vektor P0 = new Vektor();
            Vektor Pd = new Vektor();

            // zkusime vygenerovat paprsek z kamery do bodu [x,y] prevedeneho na projekcni rovinu
            if (!RCamera.TryGetCameraRay(_xA + _xK * x, _yA + _yK * y, ref P0, ref Pd))
                return color;

            Colour rayCol = DoRayTracing(0, P0, Pd);

            return rayCol;
        }


        private Colour DoRayTracing(int depth, Vektor P0, Vektor P1)
        {
            // zjistime, zda paprsek protina nejaky objekt
            SolidPoint sp = RScene.GetIntersectPoint(P0, P1);

            Colour barvaVysled;

            if (sp == null)     // paprsek nic neprotina -> bude pouze barva pozadi
            {
                barvaVysled = Colour.Black;
                if (depth == 0)
                    barvaVysled = new Colour(this.RScene.BgColor);
                return barvaVysled;
            }

            Vektor Pd = Vektor.ZeroVektor - P1;
            //Pd.Normalize;

            Light[] lights = RScene.GetAllLightningsToPoint(sp);

            // SHADING:
            barvaVysled = this.lightSum(sp, Pd, lights);

            if (depth++ > _MaxDepth)
                return barvaVysled;

            Colour novaBarva = new Colour(sp.Color);

            // REFLECTION:
            Vektor ray = MyMath.Reflection(Pd, sp.Normal);
            Colour odrazBarva = this.ColourReflected(Pd, ray, sp.Normal, sp.Material);

            // pridani nasledne odrazene barvy:
            if (odrazBarva != null)
            {
                novaBarva = DoRayTracing(depth, sp.Coord, ray);

                barvaVysled = barvaVysled + odrazBarva * novaBarva;
            }

            // REFRACTION:
            // pridani lamaneho paprsku:
            double kt = sp.Material.KT;
            double n = sp.Material.N;

            ray = MyMath.Refraction(Pd, sp.Normal, n);
            if (ray == null)
            {
                return barvaVysled;
            }

            novaBarva = DoRayTracing(depth, sp.Coord, ray);

            novaBarva = novaBarva * kt;
            
            barvaVysled = barvaVysled + novaBarva;


            return barvaVysled;
        }

        /// <summary>
        /// Pricita prispevky svetla na dany pevny bod
        /// </summary>
        /// <param name="sp">osvetleny bod</param>
        /// <param name="Pd">smer pohledu</param>
        /// <param name="lights">svetla, ktera miri na dany bod</param>
        /// <returns>barva od svetel</returns>
        private Colour lightSum(SolidPoint sp, Vektor Pd, Light[] lights)
        {
            Vektor lightDir;    // smer svetla
            Colour lightIntens;
            Colour lightContr;

            Colour vysl = new Colour();     // uplne cerna

            // kdyz zadne svetlo neosvetluje, mel by vratit jen ambientni svetlo (nebo uplne cerne)
            if (lights == null || lights.Length == 0)
                return vysl;

            foreach (Light light in lights)
            {
                if (light == null)
                    continue;

                lightDir = light.GetDirection(sp.Coord);
                lightIntens = light.GetIntensity(sp.Coord);

                lightContr = this.ColourSpecular(lightDir, Pd, sp.Normal, sp.Material);
                if (lightContr == null)
                    continue;

                vysl.R += lightContr.R * lightIntens.R;
                vysl.G += lightContr.G * lightIntens.G;
                vysl.B += lightContr.B * lightIntens.B;
            }
            return vysl;
        }

        /// <summary>
        /// Bezne vypocitani barvy bodu
        /// </summary>
        /// <param name="ins">vstupni smer paprsku</param>
        /// <param name="outs">vystupni smer paprsku</param>
        /// <param name="normal">normala v bode</param>
        /// <param name="mat">material bodu</param>
        /// <returns>vysledna barva</returns>
        private Colour ColourSpecular(Vektor ins, Vektor outs, Vektor normal, Material mat)
        {
            double outXnormal = outs * normal;
            bool isOutsForward = outXnormal > 0.0;

            double insXnormal = ins * normal;
            bool isInsForward = insXnormal > 0.0;

            double ks = mat.Ks;
            double kd = mat.Kd;
            double kt = mat.KT;

            double colorCoef;

            Colour barvaVysl = Colour.Black;    // uplne cerna

            if (ins == null)
            {
                // jsme uvnitr, uvazime jen ambientni svetlo
                if (isOutsForward)
                    colorCoef = mat.Ka;
                else
                    colorCoef = mat.Ka * mat.KT;

                barvaVysl = mat.Color * colorCoef;

                return barvaVysl;                
            }

            colorCoef = 1.0;
            Vektor Ray = null;

            if (isOutsForward == isInsForward)        // pocitame ODRAZ
            {
                Ray = MyMath.Reflection(ins, normal);
                if (!isInsForward && -insXnormal <= 0.0)
                {
                    if (ks + kt + kd > 1.0)
                        ks = 1.0 - kd;
                    else
                        ks = ks + kt;
                }
            }
            else            // protejsi strany - pocitame LAMANI SVETLA
            {
                Ray = MyMath.Refraction(ins, normal, mat.N);
                colorCoef = kt;
            }

            double diffuse = colorCoef * kd * Math.Abs(insXnormal);
            double specular = 0.0;

            if (Ray != null)
            {
                double angle = Ray * outs;
                if (angle > 0.0)
                    specular = colorCoef * ks * Math.Pow(angle, mat.SpecularExponent);
            }

            barvaVysl.R = diffuse * mat.Color.R + specular;
            barvaVysl.G = diffuse * mat.Color.G + specular;
            barvaVysl.B = diffuse * mat.Color.B + specular;

            return barvaVysl;
        }


        /// <summary>
        /// Bezne vypocitani barvy bodu - jen reflected a refracted
        /// </summary>
        /// <param name="ins">vstupni smer paprsku</param>
        /// <param name="outs">vystupni smer paprsku</param>
        /// <param name="normal">normala v bode</param>
        /// <param name="mat">material bodu</param>
        /// <returns>vysledna barva</returns>
        private Colour ColourReflected(Vektor ins, Vektor outs, Vektor normal, Material mat)
        {
            double outXnormal = outs * normal;
            bool isOutsForward = outXnormal > 0.0;
            
            double insXnormal = ins * normal;
            bool isInsForward = insXnormal > 0.0;

            double ks = mat.Ks;
            double kd = mat.Kd;
            double kt = mat.KT;

            double colorCoef;

            Colour barvaVysl = Colour.Black;    // uplne cerna

            if (ins == null)
            {
                // jsme uvnitr, uvazime jen ambientni svetlo
                if (isOutsForward)
                    colorCoef = mat.Ka;
                else
                    colorCoef = mat.Ka * mat.KT;

                barvaVysl = mat.Color * colorCoef;

                return barvaVysl;
            }

            colorCoef = 1.0;
            Vektor Ray = null;

            if (isOutsForward == isInsForward)        // pocitame ODRAZ
            {
                Ray = MyMath.Reflection(ins, normal);
                if (!isInsForward && -insXnormal <= 0.0)
                {
                    if (ks + kt + kd > 1.0)
                        ks = 1.0 - kd;
                    else
                        ks = ks + kt;
                }
            }
            else            // protejsi strany - pocitame LAMANI SVETLA
            {
                Ray = MyMath.Refraction(ins, normal, mat.N);
                colorCoef = kt;
            }

            double diffuse = 0.0;
            double specular = 0.0;

            if (Ray != null)
            {
                double angle = Ray * outs;
                if (angle > 0.0)
                    specular = colorCoef * ks * Math.Pow(angle, mat.SpecularExponent);
            }

            barvaVysl.R = diffuse * mat.Color.R + specular;
            barvaVysl.G = diffuse * mat.Color.G + specular;
            barvaVysl.B = diffuse * mat.Color.B + specular;

            return barvaVysl;
        }
    }
}
