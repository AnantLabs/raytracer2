using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace EditorLib
{
    public class EditorObject
    {
        private List<GraphicsPath> PathList { get; set; }
        public DrawingObject AssociatedObject { get; private set; }

        public EditorObject(DrawingObject assocObj)
        {
            PathList = new List<GraphicsPath>();
            AssociatedObject = assocObj;
        }

        /// <summary>
        /// Prida "klikatelnou" cestu
        /// </summary>
        /// <param name="path"></param>
        public void AddPath(GraphicsPath path)
        {
            PathList.Add(path);
        }

        /// <summary>
        /// zjisti, zda je bod uvnitr nektere z cest
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsContained(Point point)
        {
            foreach (GraphicsPath gp in PathList)
            {
                if (gp.IsVisible(point))
                    return true;
            }
            return false;
        }


    }
}
