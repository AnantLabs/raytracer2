using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;
using System.Runtime.Serialization;

namespace RayTracerLib
{

    /// <summary>
    /// Objekt svetla ve scene.
    /// </summary>
    public class Light
    {

        private String _label;
        /// <summary>
        /// oznaceni svetla, jeho popisek. Maximalni delka = 9 znaku
        /// </summary>
        public String Label
        {
            get { return _label; }
            set
            {
                String str;
                if (value.Length > 9)
                    str = value.Substring(0, 9);
                else
                    str = value;
                if (!labels.Contains(str))
                {
                    _label = str;
                    labels.Add(str);
                }
            }
        }

        private static List<String> labels = new List<string>();

        /// <summary>
        /// posice svetla
        /// </summary>
        public Vektor Coord { get; set; }

        /// <summary>
        /// barva svetla
        /// </summary>
        public Colour Color { get; set; }

        public int SoftNumSize = 32;
        public double SoftEpsilon = 0.50;

        /// <summary>
        /// indikator, zda je svetlo mekke, tj zastupuje mnozinu svetel z jeho okoli
        /// mnozina svetel je v SoftLights
        /// </summary>
        public bool IsSoftLight { get; set; }

        /// <summary>
        /// typ mekkeho svetla
        /// </summary>
        public bool IsSinglePass { get; set; }

        List<Light> _softLights;
 

        /// <summary>
        /// indikuje, zda bude bran zretel na svetlo pri vykreslovani
        /// je-li true, svetlo jako by ve scene vubec nebylo
        /// </summary>
        public bool IsActive { get; set; }

        public Light() : this(new Vektor(0, 0, 0), new Colour(1, 1, 1, 1)) { }

        public Light(Vektor coords, Colour color)
        {
            IsActive = true;
            Coord = coords;
            Color = color;
            IsSoftLight = false;
            IsSinglePass = false;
            Label = GetUniqueName();
        }

        /// <summary>
        /// konstruktor svetla
        /// </summary>
        /// <param name="coords">stred svetla (poloha)</param>
        /// <param name="color">barva svetla</param>
        /// <param name="numSoftLights">pocet mekkych svetel</param>
        /// <param name="epsSoftLights">epsilon vzdalenost mekkych svetel</param>
        public Light(Vektor coords, Colour color, int numSoftLights, double epsSoftLights)
        {
            IsActive = true;
            Coord = coords;
            Color = color;
            SoftNumSize = numSoftLights;
            SoftEpsilon = epsSoftLights;
            IsSoftLight = true;
            IsSinglePass = false;
        }

        public Light(Light old)
        {
            IsActive = old.IsActive;
            Coord = new Vektor(old.Coord);
            Color = new Colour(old.Color);
            IsSoftLight = old.IsSoftLight;
            SoftEpsilon = old.SoftEpsilon;
            SoftNumSize = old.SoftNumSize;
            IsSinglePass = old.IsSinglePass;
            _label = old._label;
        }

        /// <summary>
        /// vytvori jednoznacne jmeno mezi vsemi svetly
        /// </summary>
        /// <returns>jednoznacny retezec popisku svetla</returns>
        private String GetUniqueName()
        {
            int count = labels.Count;
            String label;
            do
            {
                count++;
                label = "Light" + count;
            }
            while (labels.Contains(label));
            return label;
        }

        public void MoveToPoint(double dx, double dy, double dz)
        {
            //Vektor dVec = new Vektor(dx, dy, dz);
            //this.Origin += dVec;
            this.Coord.X = dx;
            this.Coord.Y = dy;
            this.Coord.Z = dz;
        }


        public bool SetSoftLights(int numSoftLights, double epsSoftLights, bool isSinglePass)
        {
            SoftNumSize = numSoftLights;
            SoftEpsilon = epsSoftLights;
            IsSoftLight = true;
            IsSinglePass = isSinglePass;
            return true;
        }
        /// <summary>
        /// zjisti, zda bude bod osvetlen
        /// </summary>
        /// <param name="point">souradnice bodu</param>
        /// <param name="normala">normalovy vektor v bode</param>
        /// <returns>true, kdyz bude osvetlen</returns>
        public bool Lightning(SolidPoint point, Scene s)
        {
            if (!IsActive)
                return false;

            // smer paprsku ze zkoumaneho bodu do stredu svetla
            Vektor dir = Vektor.ToDirectionVektor(point.Coord, Coord);

            dir.Normalize();
            Vektor normal = new Vektor(point.Normal);
            //normal.Normalize();

            // znamenko, zda neni bod "zady ke svetlu", neboli proste nebude vubec osvetlen
            // zda svetlo neni na druhe strane normaly
            double sign = dir * normal;

            if (sign <= MyMath.EPSILON)
                return false;

            // zjistime, zda lezi ve smeru paprsku dalsi bod sceny
            SolidPoint sp = s.GetIntersectPoint(point.Coord, dir);

            // kdyz ne, osvetlime bod
            if (sp == null)
                return true;

            Vektor keSvetlu = Vektor.ToDirectionVektor(point.Coord, Coord); // paprsek od daneho bodu ke svetlu
            double keSvetluSize = keSvetlu.Size();                          // vzdalenost ke svetlu
            Vektor kBoduPruniku = sp.Coord - point.Coord;                   // paprsek k bodu pruniku
            double kBoduPrunikuSize = kBoduPruniku.Size();                  // vzdalenost k bodu pruniku

            // je-li stinici bod za svetlem (ve vetsi vzdalenosti, nez svetlo), vratime true - bod bude osvetlen
            return (keSvetluSize < kBoduPrunikuSize);                       
        }

        /// <summary>
        /// Metoda na zjisteni, zda je bod osvetlen - verze pro mekke stiny - single-pass
        /// </summary>
        /// <param name="sp">bod dopadu stinu</param>
        /// <param name="scene">scena</param>
        /// <returns>true, kdyz bod osvetlen</returns>
        public bool LightningSoft(SolidPoint point, Scene scene, out Light vystupLight)
        {
            vystupLight = this;
            if (!IsActive)
                return false;

            if (!IsSoftLight)
                return false;

            // smer paprsku ze zkoumaneho bodu do stredu svetla
            Vektor dir = Vektor.ToDirectionVektor(point.Coord, Coord);

            dir.Normalize();
            Vektor normal = new Vektor(point.Normal);
            normal.Normalize();

            // znamenko, zda neni bod "zady ke svetlu", neboli proste nebude vubec osvetlen
            // zda svetlo neni na druhe strane normaly
            double sign = dir * normal;

            if (sign <= 0)
                return false;

            // zjistime, zda lezi ve smeru paprsku dalsi bod sceny
            SolidPoint sp = scene.GetIntersectPoint(point.Coord, dir);

            // kdyz ne, osvetlime bod
            if (sp == null)
                return true;

            if (sp.Shape == null)
            {
                if (Lightning(point, scene))
                {
                    vystupLight = this;
                    return true;
                }
                return false;
            }

            if (sp.Shape.GetType() == typeof(Sphere))
            {
                Sphere sphere = (Sphere)sp.Shape;
                // 1)
                double t0 = (sphere.Origin - point.Coord) * dir;
                if (t0 < 0)
                {
                    vystupLight = new Light(this);
                    return true;       
                }
               // 2)
                double D = this.SoftEpsilon;
                double b = Vektor.Size(Coord - sphere.Origin);
                b = D * t0 / b;
                // 3)
                double d = Vektor.Size(dir * t0 - sphere.Origin + point.Coord) + 0.5;

                if ((sphere.R < d) && (d < sphere.R + b))
                {
                    double tau = (d - sphere.R) / b;
                    double s = sshadowFunc(tau);
                    Colour color = new Colour(this.Color * s);
                    vystupLight = new Light(this.Coord, color);
                    return true;
                }
                else if (d < sphere.R)
                {
                    return false;
                }
                else if (d > sphere.R + b)
                {
                    vystupLight = new Light(this);
                    return true;
                }
            }

            else if (sp.Shape.GetType() == typeof(Plane))
            {
                Plane plane = (Plane)sp.Shape;
                double dist;
                Vektor C = plane.GetNearestPoint(sp.Coord, out dist);

                C = (C - plane.Pocatek) * this.SoftEpsilon;
                double citatel = plane.Normal * dir;
                double jmenovatel = plane.Normal.Size() * dir.Size();
                double cos = citatel / jmenovatel;
                double coss = Math.Acos(cos);
                // 1)
                double t0 = (C - point.Coord) * dir;
                //if (t0 < 0)
                //    return false;
                //if (t0 < 0)
                //    t0 = -t0;
                //if (t0 < 0)
                //{
                //    vystupLight = new Light(this);
                //    return true;
                //}
                // 2)
                double len1 = (point.Coord - C).Size();
                double len2 = (point.Coord - this.Coord).Size();
                double D = this.SoftEpsilon;
                double b = Vektor.Size(Coord - C);
                //t0 = coss;
                //t0 = 
                //double b = coss;
                b = D * t0 / b;
                //b = coss;
                //if (b < 0)
                //{
                //    return false;
                //}
                //b = coss;
                // 3)
                double d = Vektor.Size(dir * t0 - C + point.Coord) + 0.5;

                if ((d > dist) && (d < dist + b))
                {
                    double tau = (d - dist) / b;
                    double s = sshadowFunc(tau);
                    Colour color = new Colour(this.Color *  (s));
                    vystupLight = new Light(this.Coord, color);
                    return true;
                }
                else if (d < dist)
                {
                    return false;
                }
                else if (d > dist + b)
                {
                    return false;
                    //vystupLight = new Light(this);
                    //return true; 
                }
            }

            else if (sp.Shape.GetType() == typeof(Cylinder))
            {
                Cylinder cyl = (Cylinder)sp.Shape;
                // 1)
                double t0 = (cyl.Center - point.Coord) * dir;
                if (t0 < 0)
                {
                    vystupLight = new Light(this);
                    return true;
                }
                // 2)
                double D = this.SoftEpsilon;
                double b = Vektor.Size(Coord - cyl.Center);
                b = D * t0 / b;
                // 3)
                double d = Vektor.Size(dir * t0 - cyl.Center + point.Coord) + 0.5;

                if ((cyl.Rad < d) && (d < cyl.Rad + b))
                {
                    double tau = (d - cyl.Rad) / b;
                    double s = sshadowFunc(tau);
                    Colour color = new Colour(this.Color * s);
                    vystupLight = new Light(this.Coord, color);
                    return true;
                }
                else if (d < cyl.Rad)
                {
                    return false;
                }
                else if (d > cyl.Rad + b)
                {
                    vystupLight = new Light(this);
                    return true;
                }
            }
            return false;
        }



        /// <summary>
        /// Metoda urcujici osvetleni bodu kombinaci metod mekkych stinu
        /// pro stinici objekt kouli vypocte mekky stin single-pass metodou
        /// pro ostatni stinici objekty vypocte mekky stin standardni nahodnou distrubuci svetel 
        /// okolo hlavniho hlavniho svetla
        /// </summary>
        /// <param name="point">bod, pro ktery overujeme osvetleni danym svetlem</param>
        /// <param name="scene">scena, ve ktere to zjistujeme</param>
        /// <param name="vystupLight">seznam vystupnich svetel osvetlujicich dany bod</param>
        /// <returns>true kdyz bod osvetlen alespon jednim svetlem</returns>
        public bool LightningSoft2(SolidPoint point, Scene scene, out List<Light> vystupLight)
        {
            vystupLight = new List<Light>();
            if (!IsActive)
                return false;

            if (!IsSoftLight)
                return false;

            // smer paprsku ze zkoumaneho bodu do stredu svetla
            Vektor dir = Vektor.ToDirectionVektor(point.Coord, Coord);

            dir.Normalize();
            Vektor normal = new Vektor(point.Normal);
            normal.Normalize();

            // znamenko, zda neni bod "zady ke svetlu", neboli proste nebude vubec osvetlen
            // zda svetlo neni na druhe strane normaly
            double sign = dir * normal;

            if (sign <= 0)
                return false;

            // zjistime, zda lezi ve smeru paprsku dalsi bod sceny
            SolidPoint sp = scene.GetIntersectPoint(point.Coord, dir);

            // kdyz ne, osvetlime bod
            if (sp == null)
            {
                vystupLight.Add(this);
                return true;
            }

            if (sp.Shape == null)
            {
                if (Lightning(point, scene))
                {
                    vystupLight.Add(this);
                    return true;
                }
                return false;
            }

            if (IsSinglePass && sp.Shape.GetType() == typeof(Sphere))
            {
                Sphere sphere = (Sphere)sp.Shape;
                // 1)
                double t0 = (sphere.Origin - point.Coord) * dir;
                if (t0 < 0)
                {
                    vystupLight.Add(new Light(this));
                    return true;
                }
                // 2)
                double D = this.SoftEpsilon;
                double b = Vektor.Size(Coord - sphere.Origin);
                b = D * t0 / b;
                // 3)
                double d = Vektor.Size(dir * t0 - sphere.Origin + point.Coord) + 0.4;

                if ((sphere.R < d) && (d < sphere.R + b))
                {
                    double tau = (d - sphere.R) / b;
                    double s = sshadowFunc(tau);
                    Colour color = new Colour(this.Color * s);
                    vystupLight.Add(new Light(this.Coord, color));
                    return true;
                }
                else if (d < sphere.R)
                {
                    return false;
                }
                else if (d > sphere.R + b)
                {
                    vystupLight.Add(new Light(this));
                    return true;
                }
            }
            else
            {
                List<Light> softLights = this.GetSoftLightsRandom();
                foreach (Light sl in softLights)
                {
                    if (sl.Lightning(point, scene))
                        vystupLight.Add(sl);
                }
                if (vystupLight.Count > 0)
                    return true;
                return false;
            }
            return false;
        }

        private double sshadowFunc(double tau)
        {
            double tau2 = tau * tau;
            return (3 * tau2 - 2 * tau2 * tau);
        }

        /// <summary>
        /// Vrati osvetlenou barvu od primeho svetla pro dany bod.
        /// </summary>
        /// <param name="point">zadany bod</param>
        /// <returns>barva bodu</returns>
        public Colour GetIntensity(Vektor point)
        {
            Colour colorPoint = new Colour();
            colorPoint.R = Color.R;
            colorPoint.G = Color.G;
            colorPoint.B = Color.B;
            return colorPoint;
        }

        /// <summary>
        /// vrati normalizovany vektor mezi bodem a svetlem smerem k svetlu
        /// </summary>
        /// <param name="point">pocatecni bod</param>
        /// <returns>smerovy normalizovany vektor</returns>
        public Vektor GetDirection(Vektor point)
        {
            Vektor dir = Vektor.ToDirectionVektor(point, Coord);
            dir.Normalize();
            return dir;
        }

        public List<Light> GetSoftLightsRandom()
        {
            if (_softLights != null)
                return _softLights;
            List<Light> resultLights = new List<Light>();
            if (!IsActive)
                return resultLights;

            double rad = this.SoftEpsilon;
            double alpha = 1.0 / this.SoftNumSize;
            double u, v;
            double q, f;
            double x, y, z;
            Random rand = new Random();
            Light softLight;
            for (int i = 0; i < this.SoftNumSize; i++)
            {
                u = rand.NextDouble();
                v = rand.NextDouble();
                q = 2 * Math.PI * u;
                f = Math.Acos(2 * v - 1);
                x = rad * Math.Cos(q) * Math.Sin(f);
                y = rad * Math.Sin(q) * Math.Sin(f);
                z = rad * Math.Cos(f);
                softLight = new Light(new Vektor(Coord.X + x, Coord.Y + y, Coord.Z + z), Color * alpha);
                resultLights.Add(softLight);
            }

            _softLights = resultLights;
            return resultLights;
        }


        public override string ToString()
        {
            return "Light: Center=" + Coord;
        }

        
    }
}
