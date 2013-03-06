using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mathematics;
using System.Runtime.Serialization;

namespace RayTracerLib
{

    /// <summary>
    /// Trida pro barvu. - v RGB slozkach + Alfa slozka pro pruhlednost.
    /// Barvy se ukladaji jako realne cislo v intervalu [0, 1] pro presnejsi definici barvy.
    /// </summary>
    [DataContract]
    public class Colour
    {

        public static Colour DefaultColor { get { return new Colour(0.3, 0.4, 0.5, 1.0); } }
        public static Colour Black { get { return new Colour(0.0, 0.0, 0.0, 1.0); } }

        public static Colour White { get { return new Colour(1.0, 1.0, 1.0, 1.0); } }


        [DataMember]
        public double R { get { return _r; } set { if (value < 0) _r = 0; else _r = value; } }
        private double _r;
        
        [DataMember]
        public double G { get { return _g; } set { if (value < 0) _g = 0; else _g = value; } }
        private double _g;
        
        [DataMember]
        public double B { get { return _b; } set { if (value < 0) _b = 0; else _b = value; } }
        private double _b;

        /// <summary>
        /// pruhlednost
        /// </summary>
        public double Alfa { get; set; }


        public Colour() : this(0.0, 0.0, 0.0, 1.0) { }

        public Colour(double red, double green, double blue, double alfa)
        {
            R = red;
            G = green;
            B = blue;
            Alfa = alfa;
            //R = MyMath.Clamp(red, 0, 1);
            //G = MyMath.Clamp(green, 0, 1);
            //B = MyMath.Clamp(blue, 0, 1);
            //Alfa = MyMath.Clamp(alfa, 0, 1);
        }

        public Colour (Colour old)
        {
            R = old.R;
            G = old.G;
            B = old.B;
            Alfa = old.Alfa;
        }

        public static Colour operator +(Colour c1, Colour c2)
        {
            Colour vysl = new Colour(c1);
            vysl.R += c2.R;
            vysl.G += c2.G;
            vysl.B += c2.B;

            return vysl;
        }

        public static Colour operator *(Colour c1, Colour c2)
        {
            Colour vysl = new Colour(c1);
            vysl.R *= c2.R;
            vysl.G *= c2.G;
            vysl.B *= c2.B;

            return vysl;
        }

        public static Colour operator*(Colour col, double k)
        {
            Colour vysl = new Colour(col);
            vysl.R *= k;
            vysl.G *= k;
            vysl.B *= k;

            return vysl;
        }
        /// <summary>
        /// Prevede barvu do integer pole s hodnotami 0-255 pro kazdou slozku
        /// </summary>
        /// <returns></returns>
        public int[] ToInt255()
        {
            int[] vysl = new int[4];
            vysl[0] = (int)(MyMath.Clamp(R * 255, 0, 255));
            vysl[1] = (int)(MyMath.Clamp(G * 255, 0, 255));
            vysl[2] = (int)(MyMath.Clamp(B * 255, 0, 255));
            vysl[3] = (int)(MyMath.Clamp(Alfa * 255, 0, 255));
            return vysl;
        }

        public System.Drawing.Color SystemColor()
        {
            System.Drawing.Color vyslCol = new System.Drawing.Color();
            int[] col255 = ToInt255();
            vyslCol = System.Drawing.Color.FromArgb(col255[0], col255[1], col255[2]);
            return vyslCol;
        }

        public override string ToString()
        {
            return "[ " + R + " ; " + G + " ; " + B + " ]";
        }

        /// <summary>
        /// Vytvori se systemove barvy novou instanci tridy Colour
        /// </summary>
        /// <param name="color">systemova barva</param>
        /// <returns>objekt tridy Colour</returns>
        public static Colour ColourCreate(System.Drawing.Color color)
        {
            Colour col = new Colour();
            col.R = color.R / (double)255;
            col.G = color.G / (double)255;
            col.B = color.B / (double)255;
            col.Alfa = color.A / (double)255;

            return col;
        }
    }
}
