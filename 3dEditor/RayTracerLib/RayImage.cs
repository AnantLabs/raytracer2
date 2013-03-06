using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mathematics;
using System.Runtime.Serialization;

namespace RayTracerLib
{
    [DataContract]
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

        [DataMember]
        public Size CurrentSize { get; set; }
        public const int SizeWidthExtent = 10;
        public const int SizeHeightExtent = 60;

        [DataMember]
        public Optimalizer.OptimizeType OptimizType { get; set; }
        /// <summary>
        /// Aktualne vybrany index rozliseni
        /// </summary>
        [DataMember]
        public int IndexPictureSize { get; set; }

        /// <summary>
        /// Maximalni hloubka rekurze pri vykreslovani obrazku
        /// </summary>
        [DataMember]
        public int MaxRecurse { get; set; }

        /// <summary>
        /// pozadi obrazku
        /// </summary>
        [DataMember]
        public Colour BackgroundColor { get; set; }

        /// <summary>
        /// indikator zapnuti antialiasingu pri vykreslovani
        /// </summary>
        [DataMember]
        public bool IsAntialiasing { get; set; }

        /// <summary>
        /// Zda se ma pouzit pri renderovani optimalizace
        /// </summary>
        [DataMember]
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
            this.OptimizType = old.OptimizType;
        }

        public override string ToString()
        {
            return "Res=" + CurrentSize + "; " + "Recurse=" + MaxRecurse;
        }

        public static RayImage FromDeserial(RayImage serImg)
        {
            RayImage rayImg = new RayImage();

            rayImg.BackgroundColor = serImg.BackgroundColor;
            
            rayImg.IndexPictureSize = serImg.IndexPictureSize;
            rayImg.CurrentSize = serImg.CurrentSize;
            if (serImg.IndexPictureSize < rayImg.PictureSize.Length - 1 && serImg.IndexPictureSize >= 0)
            {
                rayImg.IndexPictureSize = serImg.IndexPictureSize;
                rayImg.CurrentSize = new Size(rayImg.PictureSize[serImg.IndexPictureSize].Width, rayImg.PictureSize[serImg.IndexPictureSize].Height);
            }
            if (rayImg.CurrentSize.Width < 100 || rayImg.CurrentSize.Height < 100)
            {
                rayImg.IndexPictureSize = 0;
                rayImg.CurrentSize = new Size(rayImg.PictureSize[0].Width, rayImg.PictureSize[1].Height);
            }

            rayImg.IsAntialiasing = serImg.IsAntialiasing;
            
            rayImg.IsOptimalizing = serImg.IsOptimalizing;
            
            rayImg.MaxRecurse = serImg.MaxRecurse;
            if (rayImg.MaxRecurse < -1) rayImg.MaxRecurse = 0;
            if (rayImg.MaxRecurse > 10) rayImg.MaxRecurse = 10;

            rayImg.OptimizType = serImg.OptimizType;

            return rayImg;
        }
    }
}
