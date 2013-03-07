using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;
using RayTracerLib;

namespace EditorLib
{
    public class DrawingVertex : DrawingObject
    {

        public Vektor Center
        {
            get
            {
                return Points[0];
            }
            set
            {
                Points[0] = value;
            }
        }

        public DrawingVertex(DrawingObject drTriangle, Vektor drTrCenterPoint)
        {
            Points = new Vektor[1];
            this.Lines = new List<Line3D>(0);
            Center = drTrCenterPoint;
            this.SetModelObject(drTriangle);
        }
        public override void SetModelObject(object modelObject)
        {
            if (modelObject != null && modelObject.GetType() == typeof(DrawingObject))
                this.SetModelObject((DrawingObject)modelObject);
        }

        public void SetModelObject(DrawingObject drTriangle)
        {
            this.ModelObject = drTriangle;
            _RotatMatrix = Matrix3D.Identity;
            _ShiftMatrix = Matrix3D.Identity;
            _localMatrix = Matrix3D.Identity;
        }
        public override void Move(double moveX, double moveY, double moveZ)
        {
            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(moveX, moveY, moveZ);
            _ShiftMatrix.TransformPoint(Center);
        }
        public override void Rotate(double degAroundX, double degAroundY, double degAroundZ)
        {
            return;
        }
        public override Vektor GetCenter()
        {
            return Center;
        }
    }
}
