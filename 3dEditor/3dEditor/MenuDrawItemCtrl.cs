using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EditorLib;

namespace _3dEditor
{
    public partial class MenuDrawItemCtrl : UserControl
    {
        public event EventHandler OnMyEnter;
        public event EventHandler OnMyLeave;
        public event EventHandler OnMyClick;

        private Font _boldFont;
        private Font _normalFont;

        private static Color backNomal = SystemColors.Control;
        private static Color backActive = SystemColors.ActiveCaption;

        public MenuDrawItemCtrl()
        {
            InitializeComponent();
            if (ImgList == null)
            {
                int sizeImg = 16;
                this.pictureBoxObj.Size = new System.Drawing.Size(sizeImg, sizeImg);

                ImgList = new ImageList();
                ImgList.ColorDepth = ColorDepth.Depth32Bit;
                ImgList.ImageSize = new System.Drawing.Size(sizeImg, sizeImg);
                
                ImgList.Images.Add(typeof(DrawingCone).ToString(), (Properties.Resources.cone_gray16));
                ImgList.Images.Add(typeof(DrawingCube).ToString(), (Properties.Resources.cube_icon24));
                ImgList.Images.Add(typeof(DrawingCylinder).ToString(), (Properties.Resources.cyl16));
                ImgList.Images.Add(typeof(DrawingSphere).ToString(), (Properties.Resources.sphere16));
                ImgList.Images.Add(typeof(DrawingCamera).ToString(), (Properties.Resources.camera));
                ImgList.Images.Add(typeof(DrawingPlane).ToString(), (Properties.Resources.rectangle16));
                ImgList.Images.Add(typeof(DrawingLight).ToString(), (Properties.Resources.sun16));
                ImgList.Images.Add(typeof(DrawingTriangle).ToString(), (Properties.Resources.TriangleMagenta16));
                ImgList.Images.Add(typeof(DrawingAnimation).ToString(), (Properties.Resources.icon_camera16));
            }
            _boldFont = new System.Drawing.Font(labelCaptionObj.Font, FontStyle.Bold);
            _normalFont = new System.Drawing.Font(labelCaptionObj.Font, FontStyle.Regular);

        }

        private static ImageList ImgList;

        /// <summary>
        /// drawing object, ktery je prirazen danemu prvku v nabidce
        /// </summary>
        public DrawingObject DrObject { get; private set; }

        public String Label
        {
            get { return labelCaptionObj.Text; }
            set { labelCaptionObj.Text = value; }
        }

        public Image Img
        {
            get { return pictureBoxObj.Image; }
            set { pictureBoxObj.Image = value; }
        }

        public void AddDrawingObject(DrawingObject drob)
        {
            DrObject = drob;
            String key = drob.GetType().ToString();
            Label = drob.Label;
            
            int index = ImgList.Images.IndexOfKey(key);
            Img = ImgList.Images[index];
        }

        private void onEnter(object sender, EventArgs e)
        {
            Font bold = labelCaptionObj.Font;
            //labelCaptionObj.Font = _boldFont;
            labelCaptionObj.BackColor = backActive;
            this.BackColor = backActive;
            if (OnMyEnter != null)
                OnMyEnter(this, EventArgs.Empty);
        }

        private void onMouseDown(object sender, MouseEventArgs e)
        {
            if (OnMyClick != null)
                OnMyClick(this, EventArgs.Empty);
        }

        private void onLeave(object sender, EventArgs e)
        {
            //labelCaptionObj.Font = _normalFont;
            labelCaptionObj.BackColor = backNomal;
            this.BackColor = backNomal;
            if (OnMyLeave != null)
                OnMyLeave(this, EventArgs.Empty);

        }
    }
}
