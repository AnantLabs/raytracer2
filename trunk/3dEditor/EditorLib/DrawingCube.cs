using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RayTracerLib;
using Mathematics;

namespace EditorLib
{
    /// <summary>
    /// Krychle
    /// -- stred v Points[0]
    /// </summary>
    public class DrawingCube : DrawingDefaultShape
    {

        public Vektor Center
        {
            get { return Points[0]; }
            set { Points[0] = value; }
        }

        public DrawingCube() : this(0, 0, 0) { }

        public DrawingCube(RayTracerLib.Cube cube)
        {
            _RotatMatrix = Matrix3D.Identity;
            this.SetModelObject(cube);
        }

        public DrawingCube(double centerX, double centerY, double centerZ)
        {
            Cube cube = new Cube(new Vektor(centerX, centerY, centerZ), new Vektor(1, 0, 0), 1);
            cube.Material = new Material();
            cube.Material.Color = new Colour(1, 0.5, 0.1, 1);
            this.SetModelObject(cube);
        }

        private void Set(Vektor center, double sideLen)
        {
            Points = new Vektor[9];

            //_ShiftMatrix = Matrix3D.PosunutiNewMatrix(center.X, center.Y, center.Z);
            center = new Vektor(0, 0, 0);
            Points[0] = center;

            double sideLenHalf = sideLen / 2.0;
            Vektor upper1 = new Vektor(center.X - sideLenHalf, center.Y + sideLenHalf, center.Z - sideLenHalf);
            Points[1] = upper1;
            Vektor upper2 = new Vektor(center.X + sideLenHalf, center.Y + sideLenHalf, center.Z - sideLenHalf);
            Points[2] = upper2;
            Vektor upper3 = new Vektor(center.X - sideLenHalf, center.Y + sideLenHalf, center.Z + sideLenHalf);
            Points[3] = upper3;
            Vektor upper4 = new Vektor(center.X + sideLenHalf, center.Y + sideLenHalf, center.Z + sideLenHalf);
            Points[4] = upper4;

            Vektor lower1 = new Vektor(center.X - sideLenHalf, center.Y - sideLenHalf, center.Z - sideLenHalf);
            Points[5] = lower1;
            Vektor lower2 = new Vektor(center.X + sideLenHalf, center.Y - sideLenHalf, center.Z - sideLenHalf);
            Points[6] = lower2;
            Vektor lower3 = new Vektor(center.X - sideLenHalf, center.Y - sideLenHalf, center.Z + sideLenHalf);
            Points[7] = lower3;
            Vektor lower4 = new Vektor(center.X + sideLenHalf, center.Y - sideLenHalf, center.Z + sideLenHalf);
            Points[8] = lower4;

            Lines = new List<Line3D>(12);
            Lines.Add(new Line3D(upper1, upper2));
            Lines.Add(new Line3D(upper1, upper3));
            Lines.Add(new Line3D(upper2, upper4));
            Lines.Add(new Line3D(upper3, upper4));

            Lines.Add(new Line3D(lower1, lower2));
            Lines.Add(new Line3D(lower1, lower3));
            Lines.Add(new Line3D(lower2, lower4));
            Lines.Add(new Line3D(lower3, lower4));

            Lines.Add(new Line3D(lower1, upper1));
            Lines.Add(new Line3D(lower2, upper2));
            Lines.Add(new Line3D(lower3, upper3));
            Lines.Add(new Line3D(lower4, upper4));


        }

        /// <summary>
        /// vrati 6 ctveric pro polygony
        /// </summary>
        /// <returns></returns>
        public List<Vektor[]> GetQuarts()
        {
            List<Vektor[]> list = new List<Vektor[]>();

            Vektor[] sez1 = new Vektor[4];
            sez1[0] = Points[1];
            sez1[1] = Points[2];
            sez1[2] = Points[4];
            sez1[3] = Points[3];
            list.Add(sez1);

            Vektor[] sez2 = new Vektor[4];
            sez2[0] = Points[5];
            sez2[1] = Points[6];
            sez2[2] = Points[8];
            sez2[3] = Points[7];
            list.Add(sez2);

            Vektor[] sez3 = new Vektor[4];
            sez3[0] = Points[1];
            sez3[1] = Points[2];
            sez3[2] = Points[6];
            sez3[3] = Points[5];
            list.Add(sez3);

            Vektor[] sez4 = new Vektor[4];
            sez4[0] = Points[3];
            sez4[1] = Points[4];
            sez4[2] = Points[8];
            sez4[3] = Points[7];
            list.Add(sez4);

            Vektor[] sez5 = new Vektor[4];
            sez5[0] = Points[1];
            sez5[1] = Points[3];
            sez5[2] = Points[7];
            sez5[3] = Points[5];
            list.Add(sez5);

            Vektor[] sez6 = new Vektor[4];
            sez6[0] = Points[2];
            sez6[1] = Points[4];
            sez6[2] = Points[8];
            sez6[3] = Points[6];
            list.Add(sez6);

            return list;
        }

        public override void SetModelObject(object modelObject)
        {
            if (modelObject.GetType() == typeof(Cube))
                this.SetModelObject((Cube)modelObject);
        }

        public void SetModelObject(RayTracerLib.Cube cube)
        {
            this.ModelObject = cube;
            double sideLen = cube.Size;
            Vektor center = new Vektor(cube.Center);
            this.Set(center, sideLen);

            _RotatMatrix = cube._RotatMatrix;
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(cube.Center);
            _localMatrix = _RotatMatrix * _ShiftMatrix;
            _localMatrix.TransformPoints(Points);
        }

        //public void RotateCube(double x, double y, double z)
        //{
        //    Matrix3D newRot = Matrix3D.NewRotateByDegrees(x, y, z);

        //    Matrix3D transpLoc = _localMatrix.Transpose();
        //    transpLoc.TransformPoints(Points);

        //    this._RotatMatrix = newRot;
        //    this.SetModelObject(this.ModelObject);
        //}

        public override Vektor GetCenter()
        {
            return new Vektor(this.Center);
        }
    }
}
