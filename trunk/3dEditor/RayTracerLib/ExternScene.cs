using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Mathematics;
using System.IO;
using System.Runtime.Serialization;

namespace RayTracerLib
{
    [DataContract]
    public class ExternScene
    {
        [Serializable]
        public abstract class ExternDefaultShape
        {
            public Material Material { get; set; }
            public bool IsActive { get; set; }

            public ExternDefaultShape() { }
            public ExternDefaultShape(Material mat, bool isActive)
            {
                this.Material = mat;
                this.IsActive = IsActive;
            }
            public abstract object GetShape();
        }

        public class ExternBox : ExternDefaultShape
        {
            public double Size { get; set; }
            public Vektor Center { get; set; }
            public ExternBox() { }
            public ExternBox(Box obj)
            {
                this.IsActive = obj.IsActive;
                this.Material = obj.Material;
                this.Size = obj.Size;
                this.Center = obj.Center;
            }
            public override object GetShape()
            {
                Box box = new Box(Center, Size);
                box.IsActive = this.IsActive;
                box.Material = this.Material;
                return box;
            }
        }

        //public class ExternTriangle : ExternDefaultShape
        //{
        //    public Vertex A { get; set; }
        //    public Vertex B { get; set; }
        //    public Vertex C { get; set; }
        //    public ExternTriangle() { }
        //    public ExternTriangle(Triangle obj)
        //    {
        //        this.IsActive = obj.IsActive;
        //        this.Material = obj.Material;
        //        this.A = obj.A;
        //        this.B = obj.B;
        //        this.C = obj.C;
        //    }
        //}
        public class ExternCustom : ExternDefaultShape
        {
            public Triangle[] FaceList { get; set; }
            public Vektor Center { get; set; }
            public ExternCustom() { }
            public ExternCustom(CustomObject obj)
            {
                this.IsActive = obj.IsActive;
                this.Material = obj.Material;
                this.Center = obj.Center;
                this.FaceList = obj.FaceList.ToArray();
            }
            public override object GetShape()
            {
                CustomObject cust = new CustomObject(new List<Triangle>(FaceList), Center);
                cust.IsActive = this.IsActive;
                cust.Material = this.Material;
                return cust;
            }
        }

        //////////////////////////////////////////////////
        // SCENE
        ////////////////////////////////////////////////// S C E N E
        [DataMember]
        public Light[] Lights { get; set; }
        [DataMember]
        public Camera Camera { get; set; }

        [XmlArrayItem("Default", typeof(DefaultShape)),
        XmlArrayItem("Box", typeof(Box)),
        XmlArrayItem("Cone", typeof(Cone)),
        XmlArrayItem("Cube", typeof(Cube)),
        XmlArrayItem("Custom", typeof(CustomObject)),
        XmlArrayItem("Cylinder", typeof(Cylinder)),
        XmlArrayItem("Plane", typeof(Plane)),
        XmlArrayItem("Sphere", typeof(Sphere)),
        XmlArrayItem("Triangle", typeof(Triangle))]
        [DataMember]
        public DefaultShape[] SceneObjects { get; set; }

        //////////////////////////////////////////////////
        // RAYIMAGE
        ////////////////////////////////////////////////// R A Y I M A G E
        [DataMember]
        public RayImage[] RayImages { get; set; }

        //////////////////////////////////////////////////
        // ANIMATION
        ////////////////////////////////////////////////// A N I M A T I O N
        //public Animation[] Animations { get; set; }

        //////////////////////////////////////////////////
        // EDITOR
        ////////////////////////////////////////////////// E D I T O R
        [DataMember]
        public double EditorAngleX { get; set; }
        [DataMember]
        public double EditorAngleY { get; set; }
        [DataMember]
        public double EditorAngleZ { get; set; }

        public ExternScene() { }
        public void Set(Scene scene, RayImage[] imgs)//, Animation[] anims)
        {
            this.Lights = scene.Lights.ToArray();
            this.Camera = scene.Camera;
            this.SceneObjects = scene.SceneObjects.ToArray();

            this.RayImages = imgs;

            //this.Animations = anims;



        }
        public void SerializeXML(string fullPath, Scene scene)
        {
            //StreamWriter sw = new StreamWriter(fullPath);
            //XmlSerializer serial = new XmlSerializer(typeof(Scene));
            //serial.Serialize(sw, scene);

            NetDataContractSerializer serr;
            DataContractSerializer ser = new DataContractSerializer(typeof(ExternScene), new Type[]{typeof(Vektor), typeof(Colour), typeof(Scene), 
                typeof(DefaultShape),typeof(CustomObject), typeof(Triangle), typeof(Sphere),   typeof(Plane),   typeof(Cone),typeof(Cylinder),         
                typeof(RayImage), typeof(Animation), typeof(Camera), typeof(Light), typeof(Material)});
            //using (XmlWriter xw = XmlWriter.Create(fullPath))
            //{
            //    ser.WriteObject(xw, this);
            //}
            using (XmlReader xr = XmlReader.Create(fullPath))
            {
                object ob = ser.ReadObject(xr);
            }
        }
    }
}
