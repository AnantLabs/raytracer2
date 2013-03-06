using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace RayTracerLib
{

    /// <summary>
    /// Materialove vlastnosti pro osvetlovani objektu
    /// </summary>
    [DataContract]
    [KnownType(typeof(Colour))]
    public class Material
    {

        /// <summary>
        /// Sklo
        /// </summary>
        public static Material Glass
        {
            get
            {
                Material glass = new Material();
                glass.Color = new Colour(1, 1, 1, 1);
                glass.Ka = 0.6;
                glass.Ks = 0.3;
                glass.Kd = 0.1;
                glass.KT = 1;
                glass.N = 1;
                glass.SpecularExponent = 25;
                return glass;
            }
        }

        /// <summary>
        /// Zrcadlo
        /// </summary>
        public static Material Mirror
        {
            get
            {
                Material mirror = new Material();
                mirror.Color = new Colour(0.5, 0.5, 0.5, 1);
                mirror.Ka = 0.1;
                mirror.Ks = 1.0;
                mirror.Kd = 0.1;
                mirror.KT = 0;
                mirror.N = 1;
                mirror.SpecularExponent = 50;
                return mirror;
            }
        }

        /// <summary>
        ///  Guma
        /// </summary>
        public static Material Rubber
        {
            get
            {
                Material rubber = new Material();
                rubber.Color = new Colour(0.9, 0.8, 0.2, 1);
                rubber.Ka = 0.2;
                rubber.Ks = 0.0;
                rubber.Kd = 1.0;
                rubber.KT = 0;
                rubber.N = 1;
                rubber.SpecularExponent = 20;
                return rubber;
            }
        }
        /// <summary>
        /// Aktualni barva
        /// </summary>
        [DataMember]
        public Colour Color { get; set; }

        /// <summary>
        /// Ambientni koeficient - prenesena barva z okoli
        /// </summary>
        [DataMember]
        public double Ka { get { return _ka; } set { if (value < 0) _ka = 0; else _ka = value; } }
        private double _ka;

        /// <summary>
        /// Difusni koeficient - kolik se pohlti - absorbuje
        /// </summary>
        [DataMember]
        public double Kd { get { return _kd; } set { if (value < 0) _kd = 0; else _kd = value; } }
        private double _kd;

        /// <summary>
        /// Zrcadlovy odlesk koeficient - kolik se odrazi
        /// </summary>
        [DataMember]
        public double Ks { get { return _ks; } set { if (value < 0) _ks = 0; else _ks = value; } }
        private double _ks;

        /// <summary>
        /// [h] Zrcadlovy odlesk - exponent urcujici siri hlavniho kolecka svetla
        /// cim vyssi exponent - tim mensi a zarivejsi kolecko
        /// </summary>
        [DataMember]
        public int SpecularExponent { get { return _spec; } set { if (value < 0) _spec = 0; else _spec = value; } }
        private int _spec;

        /// <summary>
        /// [KT] Koeficient lomu [0,1] - kolik projde skrz
        /// </summary>
        [DataMember]
        public double KT { get { return _kt; } set { if (value < 0) _kt = 0; else _kt = value; } }
        private double _kt;

        /// <summary>
        /// [N] Index lomu
        /// </summary>
        [DataMember]
        public double N { get { return _n; } set { if (value < 0) _n = 0; else _n = value; } }
        private double _n;

        public Material()
        {
            this.Color = new Colour(0.5, 0.1, 0.6, 1.0);
            this.Ka = 0.1;
            this.Ks = 0.3;
            this.Kd = 0.6;
            this.KT = 0.0;
            this.N = 1;
            this.SpecularExponent = 30;
        }

        public Material(Material old)
        {
            this.Color = new Colour(old.Color);
            this.Ka = old.Ka;
            this.Ks = old.Ks;
            this.Kd = old.Kd;
            this.KT = old.KT;
            this.N = old.N;
            this.SpecularExponent = old.SpecularExponent;
        }

    }
}
