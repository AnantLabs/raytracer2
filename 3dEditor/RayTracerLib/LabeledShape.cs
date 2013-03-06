using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace RayTracerLib
{
    [DataContract]
    [KnownType(typeof(DefaultShape))]
    [KnownType(typeof(Sphere))]
    [KnownType(typeof(CustomObject))]
    [KnownType(typeof(Cone))]
    [KnownType(typeof(Plane))]
    [KnownType(typeof(Triangle))]
    [KnownType(typeof(Cube))]
    [KnownType(typeof(Cylinder))]
    [KnownType(typeof(Light))]
    public class LabeledShape
    {
        public LabeledShape() { }
        public LabeledShape(LabeledShape old)
        {
            _label = old._label;
            counter = old.counter;
            labelPrefix = old.labelPrefix;
        }
        #region LABEL
        [DataMember(Name="Label")]
        private String _label;
        /// <summary>
        /// oznaceni objektu, jeho popisek. Maximalni delka = 9 znaku
        /// </summary>
        public String Label
        {
            get { return _label; }
            set
            {
                String str;
                if (value.Length > 9)
                    str = value.Substring(0, 9);
                else
                    str = value;
                str = str.ToLower();
                if (!labels.Contains(str))
                {
                    // odstranime stary Label ze seznamu
                    labels.Remove(_label);
                    _label = str;
                    labels.Add(str);
                }
            }
        }
        /// <summary>
        /// citac instanci daneho typu
        /// </summary>
        private int counter;
        /// <summary>
        /// prefix instanci daneho typu
        /// </summary>
        private String labelPrefix;

        /// <summary>
        /// nastavi prefix labelu a prejmenuje podle daneho prefixu rovnou label
        /// </summary>
        /// <param name="prefix"></param>
        public void SetLabelPrefix(String prefix)
        {
            counter = 0;
            String str;
            if (prefix.Length > 6)
                str = prefix.Substring(0, 6);
            else
                str = prefix;
            str = str.ToLower();
            labelPrefix = str;
            Label = GetUniqueName();
        }
        /// <summary>
        /// zjisti, zda je jmeno volne, tedy lze ho priradit nejakemu objektu
        /// </summary>
        /// <param name="label">pozadovane nove jmeno</param>
        /// <returns>TRUE, kdyz zadny objekt neni pojmenovan zadanym labelem</returns>
        public static bool IsAvailable(String label)
        {
            return !labels.Contains(label.ToLower());
        }
        protected static List<String> labels = new List<string>();

        public virtual string GetUniqueName()
        {
            String label;
            do
            {
                counter++;
                label = labelPrefix + counter;
            }
            while (labels.Contains(label));
            return label;
        }
        #endregion

        public static void ResetLabels()
        {
            LabeledShape.labels = new List<string>();
        }
    }
}
