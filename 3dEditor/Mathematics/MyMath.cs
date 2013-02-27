using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mathematics
{

    /// <summary>
    /// Staticka trida pro pocetni operace
    /// </summary>
    public static class MyMath
    {
        /// <summary>
        /// Nejmensi cislo pouzivane pri vypoctech
        /// </summary>
        public static double EPSILON = 0.000001;

        /// <summary>
        /// Prevede radiany na stupne
        /// </summary>
        public static double Radians2Deg(double radians)
        {
            return radians / Math.PI * 180.0;
        }

        /// <summary>
        /// Prevede stupne na radiany
        /// </summary>
        public static double Degrees2Rad(double degrees)
        {
            return degrees / 180.0 * Math.PI;
        }

        /// <summary>
        /// Orizne cislo na nejblizsi hranici, pokud je mimo zadany interval
        /// </summary>
        /// <param name="num">testovane cislo</param>
        /// <param name="min">zacatek intervalu</param>
        /// <param name="max">konec intervalu</param>
        /// <returns>cislo v rozmezi [min;max]</returns>
        public static double Clamp(double num, double min, double max)
        {
            if (num < min)
                return min;
            if (num > max)
                return max;
            return num;
        }

        /// <summary>
        /// Spocita smerovy vektor lamaneho paprsku
        /// Refracted = 
        /// </summary>
        /// <param name="input">vstupni vektor (smer od bodu pruniku do zdroje svetla)</param>
        /// <param name="normala">normalovy vektor</param>
        /// <param name="indexLomu">index lomu materialu</param>
        /// <returns>smerovy vektor ve smeru od bodu pruniku</returns>
        public static Vektor Refraction(Vektor input, Vektor normala, double indexLomu)
        {
            Vektor norm = new Vektor(normala);
            Vektor ins = new Vektor(input);

            if (normala == null || input == null)
                return Vektor.ZeroVektor;

            // vstupni vektoru musi byt jednotkovy
            double skalar = ins * ins;
            if (skalar > MyMath.EPSILON && (skalar < 1.0 - MyMath.EPSILON || skalar > 1.0 + MyMath.EPSILON))
            {
                skalar = 1.0 / Math.Sqrt(skalar);
                ins.MultiplyBy(skalar);
            }

            // normala musi byt jednokova
            skalar = norm * norm;
            if (skalar > 0 && (skalar < 1.0 || skalar > 1.0 ))
            {
                skalar = 1.0 / Math.Sqrt(skalar);
                norm.MultiplyBy(skalar);
            }

            skalar = ins * norm;

            if (skalar < 0.0)        // ins*norm by melo byt > 0.0 (ve stejnem smeru)
            {
                skalar = -skalar;
                norm = Vektor.ZeroVektor - norm;
            }
            else
            {
                /* 
                 n = n1/n2;
                 n1 = index lomu vzduchu = 1.0
                 n2 = index lomu objektu na nejz dopada paprsek
                */
                indexLomu = 1.0 / indexLomu; 
            }

            double temp = 1.0 - indexLomu * indexLomu * (1.0 - skalar * skalar);

            // test, abychom mohli odmocnovat
            if (temp <= 0.0)
                return null;


            skalar = indexLomu * skalar - Math.Sqrt(temp);
            norm.MultiplyBy(skalar);
            ins.MultiplyBy(indexLomu);

            // TODO: zjistit, zda + nebo -:
            Vektor vysl = norm - ins;
            //Vektor vysl = norm + ins;
            return vysl;
        }


        /// <summary>
        /// vypocita odrazenej paprsek
        /// Reflected = ins - 2*norm*(ins*norm)
        /// </summary>
        /// <param name="ins">vstupni paprsek - normalizovany</param>
        /// <param name="norm">normalovy vektor v bode odrazu - normalizovany</param>
        /// <returns>odrazeny paprsek</returns>
        public static Vektor Reflection(Vektor ins, Vektor norm)
        {
            if (ins == Vektor.ZeroVektor || norm == Vektor.ZeroVektor)
                return Vektor.ZeroVektor;

            double temp = ins * norm;

            Vektor refl = norm * (temp * 2);
            Vektor vysl = refl - ins;

            return vysl;
        }

        /// <summary>
        /// vzdalenost svou bodu
        /// </summary>
        public static double Distance2Points2d(double x1, double y1, double x2, double y2)
        {
            double len = (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
            return Math.Sqrt(len);
        }

        public static double Distance2Points(Vektor p1, Vektor p2)
        {
            Vektor p = p1 - p2;
            return p.Size();
        }

        public static bool IsDistance2PointsMaxims(Vektor p1, Vektor p2, double max)
        {
            if (Math.Abs(p1.X - p2.X) < max &&
                Math.Abs(p1.Y - p2.Y) < max &&
                Math.Abs(p1.Z - p2.Z) < max)
                return true;

            return false;
        }

        /// <summary>
        /// vzdalenost bodu od primky
        /// </summary>
        /// <param name="A">BOD</param>
        /// <param name="C">pocatek primky</param>
        /// <param name="direction">smer primky</param>
        /// <returns></returns>
        public static double DistancePointAndLine(Vektor A, Vektor C, Vektor direction)
        {
            Vektor C2 = C + direction;
            Vektor cross = Vektor.CrossProduct(A - C, A - C2);
            double len1 = cross.Size();
            double vys = len1 / direction.Size();
            return vys;
        }


        /// <summary>
        /// Vrati prusecik primky (C,direction) a bodu, ktery promitneme na primku
        /// </summary>
        /// <param name="A"></param>
        /// <param name="C"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vektor NearestPoint(Vektor A, Vektor C, Vektor direction)
        {
            Vektor C2 = C + direction;
            double t = (C - A) * (C2 - C);
            double t2 = (C2 - C).SkalarniSoucin();
            t = - (t / t2);
            Vektor X = C + direction * t;
            return X;
        }

        public static double Minimum4(double m1, double m2, double m3, double m4)
        {
            double min = Math.Min(m1, m2);
            min = Math.Min(min, m3);
            min = Math.Min(min, m4);
            return min;
        }

        public static double Maximum4(double m1, double m2, double m3, double m4)
        {
            double max = Math.Max(m1, m2);
            max = Math.Max(max, m3);
            max = Math.Max(max, m4);
            return max;
        }

    }
}
