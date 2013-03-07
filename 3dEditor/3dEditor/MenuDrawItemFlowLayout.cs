using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditorLib;

namespace _3dEditor
{
    /// <summary>
    /// posilame kliknutelny objekt
    /// </summary>
    public class MenuDrawingItemArg : EventArgs
    {
        public DrawingObject ObjectToDraw;
        public MenuDrawingItemArg(DrawingObject drob) { ObjectToDraw = drob; }
    }
    public class MenuDrawItemFlowLayout: System.Windows.Forms.FlowLayoutPanel
    {
        public delegate void MenuDrawingItemEventHandler(object sender, MenuDrawingItemArg e);

        public event MenuDrawingItemEventHandler OnMyEnter;
        public event EventHandler OnMyLeave;
        public event MenuDrawingItemEventHandler OnMyClick;
        

        public MenuDrawItemFlowLayout()
        {
            this.Visible = false;
        }

        public void AddItems(DrawingObject[] objectList)
        {
            this.Controls.Clear();

            foreach (DrawingObject obj in objectList)
            {
                if (obj is DrawingVertex) continue;
                MenuDrawItemCtrl item = new MenuDrawItemCtrl();
                item.AddDrawingObject(obj);
                this.Controls.Add(item);
                item.OnMyEnter += new EventHandler(onEnterSendingItem);
                item.OnMyClick += new EventHandler(onClickedSendingItem);

                item.OnMyLeave += new EventHandler(onLeaveLayout);
            }
        }

        protected override void Dispose(bool disposing)
        {
            foreach (System.Windows.Forms.Control ctrl in this.Controls)
            {
                ctrl.Dispose();
            }
            base.Dispose(disposing);
        }

        private void onEnterSendingItem(object sender, EventArgs e)
        {
            MenuDrawItemCtrl ctrl = sender as MenuDrawItemCtrl;
            if (OnMyEnter != null)
                OnMyEnter(this, new MenuDrawingItemArg(ctrl.DrObject));
        }

        private void onClickedSendingItem(object sender, EventArgs e)
        {
            MenuDrawItemCtrl ctrl = sender as MenuDrawItemCtrl;
            if (OnMyClick != null)
                OnMyClick(this, new MenuDrawingItemArg(ctrl.DrObject));
        }

        private void onLeaveLayout(object sender, EventArgs e)
        {
            if (OnMyLeave != null)
                OnMyLeave(this, EventArgs.Empty);
        }

    }
}
