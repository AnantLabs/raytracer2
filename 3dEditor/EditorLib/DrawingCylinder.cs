using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RayTracerLib;

namespace EditorLib
{
    public class DrawingCylinder : DrawingObject
    {

        /// <summary>
        /// Associated object from Raytracer World to be represented in editor
        /// </summary>
        public Cylinder ModelObject { get; private set; }


        public Point3D Center { get; private set; }
        public Point3D Norm { get; private set; }
        public double Lenght { get; private set; }
        public double Radius { get; private set; }


        public DrawingCylinder()
        {

        }

        public DrawingCylinder(Point3D origin, double radius, double lenght)
        {
            //double pulLen = lenght / 2.0;
            //Point3D c1 = center + norm * pulLen;
            //Point3D c2 = center - norm * pulLen;

            int NSplit = 8;
            List<Point3D> listPoint = new List<Point3D>();
            Point3D center = new Point3D(0, 0, 0);
            listPoint.Add(center);
            Point3D cUpper = new Point3D(center);
            double lenPul = lenght / 2;
            cUpper.Posunuti(0, lenPul, 0);
            
            Point3D cLower = new Point3D(center);
            cLower.Posunuti(0, -lenPul, 0);

            Points = new Point3D[1 + NSplit];
            Points[0] = center;

            double iter = lenght / (NSplit - 1); // aby byly vykresleny obe podstavy, potrebujeme -1
            // prvni faze: kruhy - vyplnujeme zespoda - smer od cLower
            for (int i = 0; i < NSplit; i++)
            {
                Point3D p1 = new Point3D(cLower);
                p1.Posunuti(-radius, i * iter, 0);
                listPoint.Add(p1);
                Point3D p2 = new Point3D(cLower);
                p2.Posunuti(0, i * iter, radius);
                listPoint.Add(p2);
                Point3D p3 = new Point3D(cLower);
                p3.Posunuti(radius, i * iter, 0);
                listPoint.Add(p3);
                Point3D p4 = new Point3D(cLower);
                p4.Posunuti(0, i * iter, -radius);
                listPoint.Add(p4);
            }


            Points = listPoint.ToArray();
            this.Radius = radius;
            this.Lenght = lenght;
            Lines = new List<Line3D>();

            // druha faze - vytvoreni spojnic mezi podstavami
            List<Point3D[]> quarts = GetQuartets();
            Point3D[] lowers = quarts[0];
            Point3D[] uppers = quarts[quarts.Count - 1];
            for (int i = 0; i < 4; i++)
            {
                Line3D line = new Line3D(lowers[i], uppers[i]);
                Lines.Add(line);
            }

            // nakonec posuneme
            foreach (Point3D p in Points)
            {
                p.Posunuti(origin.X, origin.Y, origin.Z);
            }
        }

        public void SetModelObject(Cylinder cylinder)
        {
            double radius = cylinder.R;
            double lenght = cylinder.H;
            //Point3D origin = cylinder.Center.
            throw new NotImplementedException();
        } 

        /// <summary>
        /// Vrati seznam ctveric bodu, ktere jsou potrebne k vykresleni rovnobezek pomoci splineu. 
        /// Vrati tedy ctverec v prostoru, do ktereho se vykresli jedna kruznice.
        /// Tyto body jsou vraceny v poli delky 4
        /// </summary>
        /// <returns></returns>
        public List<Point3D[]> GetQuartets()
        {
            List<Point3D[]> list = new List<Point3D[]>();

            for (int i = 1; i < Points.Length; i += 4)
            {
                Point3D[] arr = new Point3D[4];
                arr[0] = Points[i];
                arr[1] = Points[i + 1];
                arr[2] = Points[i + 2];
                arr[3] = Points[i + 3];
                list.Add(arr);
            }
            return list;
        }
    }
}
