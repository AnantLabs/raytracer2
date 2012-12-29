using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RayTracerLib;
using Mathematics;

namespace EditorLib
{
    public abstract class DrawingObject
    {

        public object ModelObject { get; protected set; }

        /// <summary>
        /// All currently transformed points of the object.
        /// Souradnice objektu v editoru
        /// </summary>
        public Vektor[] Points { get; protected set; }

        /// <summary>
        /// All currently transformed lines of the object, that will be drawn in editor
        /// Contains points from Points
        /// </summary>
        public List<Line3D> Lines { get; protected set; }

        protected Matrix3D _RotatMatrix;
        protected Matrix3D _ShiftMatrix;
        protected Matrix3D _localMatrix;

        /// <summary>
        /// Nastaveni modeloveho objektu k objektu v editoru
        /// </summary>
        /// <param name="modelObject">objekt ze sveta Raytracingu</param>
        /// <param name="rotationMatrix">rotacni matice sveta Editoru; muze byt null, pak bude matice jednotokva</param>
        public virtual void SetModelObject(object modelObject) { }

        public abstract Vektor GetCenter();

        public void Rotate(double degAroundX, double degAroundY, double degAroundZ)
        {
            DefaultShape ds = (DefaultShape)ModelObject;
            ds.Rotate(degAroundX, degAroundY, degAroundZ);

            Matrix3D newRot = Matrix3D.NewRotateByDegrees(degAroundX, degAroundY, degAroundZ);
            Matrix3D transpLoc = _localMatrix.Transpose();

            Matrix3D transpRot = _RotatMatrix.Transpose();
            Matrix3D transpShift = _ShiftMatrix.GetOppositeShiftMatrix();

            transpShift.TransformPoints(Points);
            transpRot.TransformPoints(Points);
            
            this._RotatMatrix = newRot;
            _localMatrix = _RotatMatrix * _ShiftMatrix;
            _localMatrix.TransformPoints(Points);
            //this.SetModelObject(this.ModelObject);
        }

        /// <summary>
        /// Aplikuje rotacni matici na vsechny body sveta editoru.
        /// Nemeni modelovy objekt.
        /// </summary>
        /// <param name="rotationMatrix"></param>
        public void ApplyRotationMatrix(Matrix3D rotationMatrix)
        {
            rotationMatrix.TransformPoints(Points);
        }

        public void Move(double moveX, double moveY, double moveZ) { }

        public void Scale(double scale) { }


        public double[] GetRotationAngles()
        {
            //return _RotatMatrix.GetAnglesFromMatrix();
            return ((DefaultShape)ModelObject)._RotatMatrix.GetAnglesFromMatrix();
        }

        public override string ToString()
        {
            return ModelObject.ToString();
        }

        
    }
}
