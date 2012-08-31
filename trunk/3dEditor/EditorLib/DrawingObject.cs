using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RayTracerLib;

namespace EditorLib
{
    public abstract class DrawingObject
    {

        public object ModelObject { get; protected set; }

        /// <summary>
        /// All currently transformed points of the object.
        /// Souradnice objektu v editoru
        /// </summary>
        public Point3D[] Points { get; protected set; }

        /// <summary>
        /// All currently transformed lines of the object, that will be drawn in editor
        /// Contains points from Points
        /// </summary>
        public List<Line3D> Lines { get; protected set; }

        /// <summary>
        /// Nastaveni modeloveho objektu k objektu v editoru
        /// </summary>
        /// <param name="modelObject">objekt ze sveta Raytracingu</param>
        /// <param name="rotationMatrix">rotacni matice sveta Editoru; muze byt null, pak bude matice jednotokva</param>
        public virtual void SetModelObject(object modelObject) { }

        public void Rotate(double degAroundX, double degAroundY, double degAroundZ)
        {
            Matrix3D matr = Matrix3D.NewRotateByDegrees(degAroundX, degAroundY, degAroundZ);
            matr.TransformLines(Lines);
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


        public override string ToString()
        {
            return ModelObject.ToString();
        }

        
    }
}
