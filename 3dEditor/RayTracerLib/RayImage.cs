using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mathematics;

namespace RayTracerLib
{
    public class RayImage
    {
        /// <summary>
        /// seznam prednastavenych velikosti vykreslovaneho obrazku
        /// posledni musi byt nulova velikost - pro volbu vlastniho nastaveni rozliseni
        /// </summary>
        public readonly Size[] PictureSize = {
                new Size(320,240),
                new Size(512,384),
                new Size(640,480),
                new Size(854,480), //856x480
                new Size(1024,768),
                new Size(1280,720),
                new Size()
            };

        public Size CurrentSize { get; set; }
        public const int SizeWidthExtent = 10;
        public const int SizeHeightExtent = 60;
        private RayImage _rayImg;

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

        /// <summary>
        /// Zda se ma pouzit pri renderovani optimalizace
        /// </summary>
        public bool IsOptimalizing { get; set; }

        public RayImage() : this(1, new Colour(0.1, 0.1, 0.1, 1), false) { }

        

        /// <summary>
        /// obrazek pro zobrazeni v editoru
        /// </summary>
        /// <param name="maxRecurse">Maximalni hloubka rekurze pri vykreslovani obrazku</param>
        /// <param name="bgCol">pozadi obrazku</param>
        /// <param name="antial">indikator zapnuti antialiasingu pri vykreslovani</param>
        public RayImage(int maxRecurse, Colour bgCol, bool antial)
        {
            MaxRecurse = maxRecurse;
            BackgroundColor = bgCol;
            IsAntialiasing = antial;
            IndexPictureSize = 0;
            CurrentSize = PictureSize[0];
        }


        public RayImage(Size size, int recurse, bool isAntialias)
        {
            CurrentSize = size;
            MaxRecurse = recurse;
            IsAntialiasing = isAntialias;
            BackgroundColor = Colour.Black;
            this.IndexPictureSize = PictureSize.Length - 1;
        }
        public RayImage(RayImage old)
        {
            this.BackgroundColor = new Colour(old.BackgroundColor);
            this.CurrentSize = new Size(old.CurrentSize.Width, old.CurrentSize.Height);
            this.IndexPictureSize = old.IndexPictureSize;
            this.MaxRecurse = old.MaxRecurse;
            this.IsAntialiasing = old.IsAntialiasing;
            this.IsOptimalizing = old.IsOptimalizing;
        }

        public override string ToString()
        {
            return "Res=" + CurrentSize + "; " + "Recurse=" + MaxRecurse;
        }
    }
}
