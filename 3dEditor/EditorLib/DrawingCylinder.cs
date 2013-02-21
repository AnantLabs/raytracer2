using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RayTracerLib;
using Mathematics;

namespace EditorLib
{
    public class DrawingCylinder : DrawingDefaultShape
    {

        public Vektor Center
        {
            get
            {
                return Points[0];
            }
            private set
            {
                Points[0] = value;
            }
        }
        public Vektor Norm { get; private set; }
        public double Lenght { get; private set; }
        public double Radius { get; private set; }


        public DrawingCylinder(Cylinder cylinder)
        {
            _RotatMatrix = Matrix3D.Identity;
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(cylinder.Center.X, cylinder.Center.Y, cylinder.Center.Z);
            SetLabelPrefix("cylind");
            this.SetModelObject(cylinder);
        }

        //public DrawingCylinder(Vektor origin, double radius, double lenght)
        //{
        //    _RotatMatrix = Matrix3D.Identity;
        //    Lable = GetUniqueName();
        //    this.Set(origin, radius, lenght);
        //}



        private void Set(Vektor origin, double radius, double lenght)
        {
            //double pulLen = lenght / 2.0;
            //Vektor c1 = center + norm * pulLen;
            //Vektor c2 = center - norm * pulLen;

            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(origin.X, origin.Y, origin.Z);

            int NSplit = 10;
            List<Vektor> listPoint = new List<Vektor>();
            Vektor center = new Vektor(0, 0, 0);
            listPoint.Add(center);
            Vektor cUpper = new Vektor(center);
            double lenPul = lenght / 2;
            cUpper.Posunuti(0, lenPul, 0);

            Vektor cLower = new Vektor(center);
            cLower.Posunuti(0, -lenPul, 0);

            Points = new Vektor[1 + NSplit];
            Points[0] = center;

            double iter = lenght / (NSplit - 1); // aby byly vykresleny obe podstavy, potrebujeme -1
            // prvni faze: kruhy - vyplnujeme zespoda - smer od cLower
            for (int i = 0; i < NSplit; i++)
            {
                Vektor p1 = new Vektor(cLower);
                p1.Posunuti(-radius, i * iter, 0);
                listPoint.Add(p1);
                Vektor p2 = new Vektor(cLower);
                p2.Posunuti(0, i * iter, radius);
                listPoint.Add(p2);
                Vektor p3 = new Vektor(cLower);
                p3.Posunuti(radius, i * iter, 0);
                listPoint.Add(p3);
                Vektor p4 = new Vektor(cLower);
                p4.Posunuti(0, i * iter, -radius);
                listPoint.Add(p4);
            }


            Points = listPoint.ToArray();
            this.Radius = radius;
            this.Lenght = lenght;
            Lines = new List<Line3D>();

            // druha faze - vytvoreni spojnic mezi podstavami
            List<Vektor[]> quarts = GetQuartets();
            Vektor[] lowers = quarts[0];
            Vektor[] uppers = quarts[quarts.Count - 1];
            for (int i = 0; i < 4; i++)
            {
                Line3D line = new Line3D(lowers[i], uppers[i]);
                Lines.Add(line);
            }

            //Matrix3D m = Matrix3D.NewRotateByDegrees(30, 0, 0);
            //_LocalMatrix = m;
            //m.TransformPoints(Points);
            //this.ApplyRotationMatrix(m);
            
            //_RotatMatrix.TransformPoints(Points);
            //_localMatrix.TransformPoints(Points);

            //Matrix3D shiftMat = Matrix3D.PosunutiNewMatrix(origin.X, origin.Y, origin.Z);
            //shiftMat.TransformPoints(Points);
            // nakonec posuneme
            //foreach (Vektor p in Points)
            //{
            //    p.Posunuti(origin.X, origin.Y, origin.Z);
            //}
        }
        //public void RotateCyl(double x, double y, double z)
        //{
        //    Matrix3D newRot = Matrix3D.NewRotateByDegrees(x, y, z);

        //    Matrix3D transpRot = _RotatMatrix.Transpose();
        //    Matrix3D transpShift = _ShiftMatrix.Transpose();
        //    Matrix3D transpLoc = _localMatrix.Transpose();

        //    transpLoc.TransformPoints(Points);

        //    //transpShift.TransformPoints(Points);
        //    //transpRot.TransformPoints(Points);

        //    this._RotatMatrix =newRot;
        //    this.SetModelObject(this.ModelObject);
            
        //}

        public void ShiftCyl(double dx, double dy, double dz)
        {
            Matrix3D mPos = Matrix3D.PosunutiNewMatrix(dx, dy, dz);
            Matrix3D m2Pos = _ShiftMatrix * mPos;
            Vektor p1 = new Vektor(2, 3, 4);
            m2Pos.TransformPoint(p1);

            mPos.TransformPoints(Points);
            _ShiftMatrix = m2Pos;
        }
        public override void SetModelObject(object modelObject)
        {
            if (modelObject.GetType() == typeof(Cylinder))
                this.SetModelObject((Cylinder)modelObject);
        }
        public void SetModelObject(Cylinder cylinder)
        {
            this.ModelObject = cylinder;
            double radius = cylinder.Rad;
            double lenght = cylinder.Height;
            Vektor origin = new Vektor(cylinder.Center);
            this.Set(origin, radius, lenght);
            _RotatMatrix = cylinder._RotatMatrix;
            _localMatrix = _RotatMatrix * _ShiftMatrix;
            _localMatrix.TransformPoints(Points);
        } 

        /// <summary>
        /// Vrati seznam ctveric bodu, ktere jsou potrebne k vykresleni rovnobezek pomoci splineu. 
        /// Vrati tedy ctverec v prostoru, do ktereho se vykresli jedna kruznice.
        /// Tyto body jsou vraceny v poli delky 4
        /// </summary>
        /// <returns></returns>
        public List<Vektor[]> GetQuartets()
        {
            List<Vektor[]> list = new List<Vektor[]>();

            for (int i = 1; i < Points.Length; i += 4)
            {
                Vektor[] arr = new Vektor[4];
                arr[0] = Points[i];
                arr[1] = Points[i + 1];
                arr[2] = Points[i + 2];
                arr[3] = Points[i + 3];
                list.Add(arr);
            }
            return list;
        }

        public override Vektor GetCenter()
        {

            return new Vektor(Center.X, Center.Y, Center.Z);
        }
    }
}
