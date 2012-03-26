using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RayTracerLib
{
    public class RayImage
    {
        /// <summary>
        /// seznam prednastavenych velikosti vykreslovaneho obrazku
        /// </summary>
        public Size[] PictureSize { get; private set; }

        public Size CurrentSize { get; set; }

        /// <summary>
        /// Aktualne vybrany index rozliseni
        /// </summary>
        public int IndexPictureSize { get; set; }

        /// <summary>
        /// Maximalni hloubka rekurze pri vykreslovani obrazku
        /// </summary>
        public int MaxRecurse { get; set; }

        /// <summary>
        /// pozadi obrazku
        /// </summary>
        public Colour BackgroundColor { get; set; }

        /// <summary>
        /// indikator zapnuti antialiasingu pri vykreslovani
        /// </summary>
        public bool IsAntialiasing { get; set; }

        public RayImage() : this(1, new Colour(0.1, 0.1, 0.1, 1), false) { }

        

        /// <summary>
        /// obrazek pro zobrazeni v editoru
        /// </summary>
        /// <param name="maxRecurse">Maximalni hloubka rekurze pri vykreslovani obrazku</param>
        /// <param name="bgCol">pozadi obrazku</param>
        /// <param name="antial">indikator zapnuti antialiasingu pri vykreslovani</param>
        public RayImage(int maxRecurse, Colour bgCol, bool antial)
        {
            // nastaveni rozliseni velikosti pro obrazek
            // posledni musi byt nulova velikost - pro volbu vlastniho nastaveni rozliseni
            PictureSize = new Size[]{
                new Size(320,240),
                new Size(512,384),
                new Size(640,480),
                new Size(854,480), //856x480
                new Size(1024,768),
                new Size(1280,720),
                new Size()
            };

            MaxRecurse = maxRecurse;
            BackgroundColor = bgCol;
            IsAntialiasing = antial;
            IndexPictureSize = 0;
            CurrentSize = PictureSize[0];
        }

        public override string ToString()
        {
            return "Image; Res=" + PictureSize[IndexPictureSize] + "; " + "Recurse=" + MaxRecurse;
        }
    }
}
