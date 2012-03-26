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
        /// All currently transformed points of the object
        /// </summary>
        public Point3D[] Points { get; protected set; }

        /// <summary>
        /// All currently transformed lines of the object, that will be drawn in editor
        /// Contains points from Points
        /// </summary>
        public List<Line3D> Lines { get; protected set; }

        public void Rotate(double degAroundX, double degAroundY, double degAroundZ)
        {
            Matrix3D matr = Matrix3D.NewRotateByDegrees(degAroundX, degAroundY, degAroundZ);
            matr.TransformLines(Lines);
        }

        public void Rotate(Matrix3D rotationMatrix)
        {
            rotationMatrix.TransformPoints(Points);
        }

        public void Move(double moveX, double moveY, double moveZ) { }

        public void Scale(double scale) { }

        
    }
}
