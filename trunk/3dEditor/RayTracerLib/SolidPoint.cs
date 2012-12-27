using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{
    /// <summary>
    /// Trida bodu, jenz vznikne pri pruniku paprsku s objektem ve scene
    /// </summary>
    public class SolidPoint: IComparable<SolidPoint>
    {
        /// <summary>
        /// parametr t z vypoctu pruniku
        /// </summary>
        public double T { get; set; }

        /// <summary>
        /// souradnice bodu pruniku
        /// </summary>
        public Vektor Coord { get; set; }

        /// <summary>
        /// vektor kolmy v danem bode
        /// </summary>
        public Vektor Normal { get; set; }

        /// <summary>
        /// barva bodu pruniku
        /// </summary>
        public Colour Color { get; set; }

        /// <summary>
        /// Materialove vlastnosti bodu pruniku
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// objekt, ke kteremu je solidpoint vytvoren
        /// </summary>
        public DefaultShape Shape { get; set; }

        public SolidPoint()
        {
            T = 0.0;
            Coord = Normal = null;
            Color = RayTracerLib.Colour.DefaultColor;
            Shape = null;
        }


        #region IComparable<SolidPoint> Members

        /// <summary>
        /// porovna dva body nejdrive podre parametru T, pak podle jejich hashe
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(SolidPoint other)
        {
            if (other == null)
                throw new NotImplementedException();

            if (this == other)
                return 0;

            if (T != other.T)
            {
                if (T < other.T)
                    return -1;
                else return 1;
            }

            int hash1 = this.GetHashCode();
            int hash2 = other.GetHashCode();

            if (hash1 < hash2)
                return -1;
            else
                return 1;
        }

        #endregion


    }
}
