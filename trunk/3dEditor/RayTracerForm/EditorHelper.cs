using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracerLib;
using System.Drawing;

namespace RayTracerForm
{
    /// <summary>
    /// Osy vykreslovane v editoru:
    /// X---Y
    /// Y---Z
    /// </summary>
    public enum EditorAxesType
    {
        XY,
        ZY,
        XZ
    };
    public class EditorHelper
    {
        /// <summary>
        /// Osy vykreslovane v editoru
        /// XY nebo YZ
        /// </summary>
        public EditorAxesType Axes { get; set; }

        /// <summary>
        /// objekty vykreslovane v editoru, se kterymi lze hybat
        /// </summary>
        public List<Placeable> Placeables { get; set; }
        /// <summary>
        /// stred editoru v pixelech
        /// </summary>
        public Point Center { get; set; }
        /// <summary>
        /// zda je stale kliknuto nad objektem
        /// </summary>
        public bool IsEditorObjectClicked { get; set; }
        /// <summary>
        /// bod kliknuti nad objektem
        /// </summary>
        public Point ClickedPoint { get; set; }
        /// <summary>
        /// aktualne vybrany objekt kliknutim
        /// </summary>
        public Placeable PlaceObjSelected { get; set; }

        /// <summary>
        /// index vybraneho objektu v listView
        /// </summary>
        public int SelectedIndex { get; set; }

        /// <summary>
        /// Meritko editoru
        /// default: 1:30
        /// </summary>
        public int Meritko { get; set; }

        public EditorHelper()
        {
            Placeables = new List<Placeable>();
            IsEditorObjectClicked = false;
            Meritko = 30;
            Axes = EditorAxesType.XY;
        }
    }
}
