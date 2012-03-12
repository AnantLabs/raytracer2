using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RayTracerLib;

namespace RayTracerForm
{
    public partial class MaterialForm : Form
    {
        public MaterialForm()
        {
            InitializeComponent();
        }

        public Material _material;

        public void Set(Material mat)
        {
            _material = mat;
            SetMaterialComponents();
        }

        private void SetMaterialComponents()
        {
            this.numKa.Value = (decimal)_material.Ka;
            this.numKs.Value = (decimal)_material.Ks;
            this.numKd.Value = (decimal)_material.Kd;
            this.numKt.Value = (decimal)_material.KT;
            this.numH.Value = (decimal)_material.SpecularExponent;
            this.numN.Value = (decimal)_material.N;

            this.numColR.Value = (decimal)_material.Color.R;
            this.numColG.Value = (decimal)_material.Color.G;
            this.numColB.Value = (decimal)_material.Color.B;
        }

        private void btnMaterialColor_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                double r = colorDialog.Color.R / (double)255;
                double g = colorDialog.Color.G / (double)255;
                double b = colorDialog.Color.B / (double)255;
                double a = colorDialog.Color.A / (double)255;

                RayTracerLib.Colour col = new RayTracerLib.Colour(r, g, b, a);

                this.numColR.Value = (decimal)col.R;
                this.numColG.Value = (decimal)col.G;
                this.numColB.Value = (decimal)col.B;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Material mat = new Material();
            mat.Ka = (double)this.numKa.Value;
            mat.Ks = (double)this.numKs.Value;
            mat.Kd = (double)this.numKd.Value;
            mat.KT = (double)this.numKt.Value;
            mat.SpecularExponent = (int)this.numH.Value;
            mat.N = (double)this.numH.Value;

            mat.Color.R = (double)this.numColR.Value;
            mat.Color.G = (double)this.numColG.Value;
            mat.Color.B = (double)this.numColB.Value;

            _material = mat;
        }
    }
}
