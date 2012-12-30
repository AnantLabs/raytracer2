﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{
    
    /// <summary>
    /// Zakladni trida pro vykreslovany objekt ve scene.
    /// Kazdy objekt musi mit:
    ///     vlastni barvu
    ///     metodu, jenz vrati pro dany paprsek body, ktere protinaji objekt
    ///     transformacni metodu pro pohyb objektu
    /// </summary>
    public abstract class DefaultShape
    {

        public Matrix3D _localMatrix;
        public Matrix3D _RotatMatrix;
        public Matrix3D _ShiftMatrix;

        /// <summary>
        /// Materialove vlastnosti objektu
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// indikuje, zda bude bran zretel na objekt pri vykreslovani
        /// je-li true, objekt jako by ve scene vubec nebyl
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Zjisti, zda paprsek protina dany objekt
        /// </summary>
        /// <param name="P0">pocatek paprsku</param>
        /// <param name="Pd">smerovy vektor paprsku</param>
        /// <param name="InterPoint">pripadny vysledny bod pruniku</param>
        /// <returns>true, kdyz existuje bod pruniku s paprskem</returns>
        public abstract bool Intersects(Vektor P0, Vektor Pd, ref List<SolidPoint> InterPoint);

        /// <summary>
        /// Moves the object according to given differences
        /// </summary>
        /// <param name="dx">x-direction move</param>
        /// <param name="dy">y-direction move</param>
        /// <param name="dz">z-direction move</param>
        public abstract void Move(double dx, double dy, double dz);

        public abstract void MoveToPoint(double dx, double dy, double dz);

        public abstract void Rotate(double degAroundX, double degAroundY, double degAroundZ);
    }
}
