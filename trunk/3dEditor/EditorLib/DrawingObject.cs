using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RayTracerLib;
using Mathematics;

namespace EditorLib
{
    public class DrawObjectComparer : IComparer<DrawingObject>
    {
        /// <summary>
        /// typ porovnavani: 
        /// ASC - vzestupne
        /// DESC - sestupne
        /// pouzivame DESC, protoze objekty ze seznamu vykreslujeme podle poradi, v jakem jsou umisteny v seznamu, 
        /// a tedy na zacatku seznamu ma byt nejvzdalenejsi objekt
        /// </summary>
        public enum SortType { ASC, DESC };

        Vektor POV;
        Matrix3D matrTransp;
        SortType sortType;

        public DrawObjectComparer(Vektor pov, Matrix3D matrFor, SortType sortType)
        {
            POV = new Vektor(pov);
            matrTransp = matrFor.Transpose();
            matrTransp.TransformPoint(POV);

            this.sortType = sortType;
        }

        public int Compare(DrawingObject drob1, DrawingObject drob2)
        {
            if (drob1 == null) return 1;
            if (drob2 == null) return -1;
            if (drob1 == drob2) return 0;
            Vektor vec1 = drob1.GetCenter();
            vec1 = matrTransp.Transform2NewPoint(vec1);
            vec1 = POV - vec1;
            double len1 = vec1.Size();

            Vektor vec2 = drob2.GetCenter();
            vec2 = matrTransp.Transform2NewPoint(vec2);
            vec2 = POV - vec2;
            double len2 = vec2.Size();

            if (sortType == SortType.ASC)
            {
                if (len1 < len2) return -1;
                if (len1 > len2) return 1;
                return 0;
            }
            else
            {
                if (len1 > len2) return -1;
                if (len1 < len2) return 1;
                return 0;
            }
        }
    }

    public abstract class DrawingObject
    {
        /// <summary>
        /// popisek, ktery je prirazen k Modelovanemu objektu
        /// </summary>
        public String Label
        {
            get
            {
                if (ModelObject is LabeledShape)
                {
                    LabeledShape ls = ModelObject as LabeledShape;
                    return ls.Label;
                }
                if (ModelObject is Camera)
                    return "camera";
                return "";
            }
            set {
                if (ModelObject is LabeledShape)
                {
                    LabeledShape ls = ModelObject as LabeledShape;
                    ls.Label = value;
                }
            }
        }
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

        /// <summary>
        /// Aplikuje rotacni matici na vsechny body sveta editoru.
        /// Nemeni modelovy objekt.
        /// </summary>
        /// <param name="rotationMatrix"></param>
        public virtual void ApplyRotationMatrix(Matrix3D rotationMatrix)
        {
            rotationMatrix.TransformPoints(Points);
        }

        public virtual void Rotate(double degAroundX, double degAroundY, double degAroundZ)
        {
            
            DefaultShape ds = (DefaultShape)ModelObject;
            if (ds != null)
                ds.Rotate(degAroundX, degAroundY, degAroundZ);

            Matrix3D newRot = Matrix3D.NewRotateByDegrees(degAroundX, degAroundY, degAroundZ);

            Matrix3D transpRot = _RotatMatrix.Transpose();
            Matrix3D transpShift = _ShiftMatrix.GetOppositeShiftMatrix();

            transpShift.TransformPoints(Points);
            transpRot.TransformPoints(Points);
            
            this._RotatMatrix = newRot;
            _localMatrix = _RotatMatrix * _ShiftMatrix;
            _localMatrix.TransformPoints(Points);
            //this.SetModelObject(this.ModelObject);
        }



        public virtual void Move(double moveX, double moveY, double moveZ)
        {
            DefaultShape ds = (DefaultShape)ModelObject;
            ds.MoveToPoint(moveX, moveY, moveZ);

            Matrix3D transpShift = _ShiftMatrix.GetOppositeShiftMatrix();
            transpShift.TransformPoints(Points);

            _ShiftMatrix = Matrix3D.PosunutiNewMatrix(moveX, moveY, moveZ);

            _ShiftMatrix.TransformPoints(Points);
            _localMatrix = _RotatMatrix * _ShiftMatrix;
        }

        public void Scale(double scale) { }


        public virtual double[] GetRotationAngles()
        {
            //return _RotatMatrix.GetAnglesFromMatrix();
            return ((DefaultShape)ModelObject)._RotatMatrix.GetAnglesFromMatrix();
        }

        public override string ToString()
        {
            return ModelObject.ToString();
            //return Label + " {" + ModelObject.ToString() + "}";
        }

        //#region LABEL
        //private String _label;
        ///// <summary>
        ///// oznaceni objektu, jeho popisek. Maximalni delka = 9 znaku
        ///// </summary>
        //public String Label
        //{
        //    get { return _label; }
        //    set
        //    {
        //        String str;
        //        if (value.Length > 9)
        //            str = value.Substring(0, 9);
        //        else
        //            str = value;
        //        str = str.ToLower();
        //        if (!labels.Contains(str))
        //        {
        //            _label = str;
        //            labels.Add(str);
        //        }
        //    }
        //}

        //protected int counter;
        //protected String labelPrefix;

        ///// <summary>
        ///// nastavi prefix labelu a prejmenuje podle daneho prefixu rovnou label
        ///// </summary>
        ///// <param name="prefix"></param>
        //public void SetLabelPrefix(String prefix)
        //{
        //    counter = 0;
        //    String str;
        //    if (prefix.Length > 6)
        //        str = prefix.Substring(0, 6);
        //    else
        //        str = prefix;
        //    str = str.ToLower();
        //    labelPrefix = str;
        //    Label = GetUniqueName();
        //}
        //public static bool IsAvailable(String label)
        //{
        //    return !labels.Contains(label.ToLower());
        //}
        //protected static List<String> labels = new List<string>();
        
        //public virtual string GetUniqueName()
        //{
        //    String label;
        //    do
        //    {
        //        counter++;
        //        label = labelPrefix + counter;
        //    }
        //    while (labels.Contains(label));
        //    return label;
        //}
        //#endregion

        public virtual void InitForRaytracer(Matrix3D rotMatrix)
        {
            return;
        }
    }
}
