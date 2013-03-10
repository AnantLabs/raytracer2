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
        
        //////////////////////////////////////////////////
        // SCENE
        ////////////////////////////////////////////////// S C E N E
        [DataMember]//(Order = 5)] // kdyz neni zahrnut ORDER, tak se ukladaji tridy podle abecedy
        public Light[] Lights { get; set; }
        [DataMember]//(Order = 4)]
        public Camera Camera { get; set; }
        [DataMember]//(Order = 6)]
        public DefaultShape[] SceneObjects { get; set; }

        //////////////////////////////////////////////////
        // RAYIMAGE
        ////////////////////////////////////////////////// R A Y I M A G E
        [DataMember]//(Order=7)]
        public RayImage[] RayImages { get; set; }

        //////////////////////////////////////////////////
        // ANIMATION
        ////////////////////////////////////////////////// A N I M A T I O N
        [DataMember]//(Order=8)]
        public Animation[] Animations { get; set; }

        //////////////////////////////////////////////////
        // EDITOR
        ////////////////////////////////////////////////// E D I T O R
        [DataMember]//(Order=1)]
        public double EditorAngleX { get; set; }
        [DataMember]//(Order = 2)]
        public double EditorAngleY { get; set; }
        [DataMember]//(Order = 3)]
        public double EditorAngleZ { get; set; }

        public ExternScene() { }
        /// <summary>
        /// nastaveni ukladanych objektu
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="imgs"></param>
        /// <param name="degs">seznam uhlu natoceni editoru: deg[0]=uhel okolo X, deg[1]=Y, deg[2]=Z</param>
        public void Set(Scene scene, RayImage[] imgs, Animation[] anims, double[] degs)//, Animation[] anims)
        {
            this.Lights = scene.Lights.ToArray();
            this.Camera = scene.Camera;
            this.SceneObjects = scene.SceneObjects.ToArray();

            this.RayImages = imgs;

            Animations = anims;

            EditorAngleX = degs[0];
            EditorAngleY = degs[1];
            EditorAngleZ = degs[2];

            //this.Animations = anims;
        }

        public Scene GetScene()
        {
            Scene sc = new Scene();
            sc.Camera = GetCamera();
            sc.Lights = GetLights();
            sc.SceneObjects = GetSceneObjects();
            return sc;
        }
        private List<Light> GetLights()
        {
            if (Lights == null) return null;
            List<Light> lightList = new List<Light>();
            foreach (Light l in this.Lights)
            {
                if (l == null) continue;
                Light light = Light.FromDeserial(l);
                lightList.Add(light);
            }
            return lightList;
        }

        public RayImage[] GetRayImgs()
        {
            if (RayImages == null) return null;

            List<RayImage> listImgs = new List<RayImage>();
            foreach (RayImage img in RayImages)
            {
                if (img != null)
                    listImgs.Add(RayImage.FromDeserial(img));
            }
            return listImgs.ToArray();
        }

        public Animation[] GetAnimations()
        {
            if (Animations == null) return null;
            List<Animation> anims = new List<Animation>();
            foreach (Animation anim in Animations)
            {
                if (anim != null)
                    anims.Add(anim.FromDeserial());
            }
            return Animations;
        }
        private Camera GetCamera()
        {
            if (Camera == null) return null;
            Camera cam = RayTracerLib.Camera.FromDeserial(this.Camera);
            return cam;
        }

        private List<DefaultShape> GetSceneObjects()
        {
            if (SceneObjects == null) return null;
            List<DefaultShape> dsList = new List<DefaultShape>();
            foreach (DefaultShape ds in SceneObjects)
            {
                if (ds != null)
                    dsList.Add(ds.FromDeserial());
            }
            return dsList;
        }
        public static void SerializeXML(string fullPath, ExternScene scene)
        {
            //StreamWriter sw = new StreamWriter(fullPath);
            //XmlSerializer serial = new XmlSerializer(typeof(Scene));
            //serial.Serialize(sw, scene);

            DataContractSerializer ser = new DataContractSerializer(typeof(ExternScene), new Type[]{typeof(Vektor), typeof(Colour), typeof(Scene), 
                typeof(LabeledShape), typeof(DefaultShape), typeof(CustomObject), typeof(Triangle), typeof(Sphere),typeof(Cube), typeof(Plane), typeof(Cone),
                typeof(Cylinder),         
                typeof(RayImage), typeof(Animation), typeof(Camera), typeof(Light), typeof(Material), 
                 typeof(Optimalizer.OptimizeType)});

            try
            {
                using (XmlWriter xw = XmlWriter.Create(fullPath))
                {
                    ser.WriteObject(xw, scene);
                }
            }
            catch (Exception ex) // kdyz se nepovede ukladani, smaze se soubor
            {
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
                throw ex;
            }
        }

        public static ExternScene DeserializeXML(string fullPath)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(ExternScene), new Type[]{typeof(Vektor), typeof(Colour), typeof(Scene), 
                typeof(LabeledShape), typeof(DefaultShape), typeof(CustomObject), typeof(Triangle), typeof(Sphere),typeof(Cube), typeof(Plane), typeof(Cone),
                typeof(Cylinder),         
                typeof(RayImage), typeof(Animation), typeof(Camera), typeof(Light), typeof(Material), 
                 typeof(Optimalizer.OptimizeType)});
            ExternScene ob = null;

            try
            {
                using (XmlReader xr = XmlReader.Create(fullPath))
                {
                    ob = (ExternScene)ser.ReadObject(xr);
                }
            }

            //catch (System.Runtime.Serialization.SerializationException xmlEx)
            //{
            //    throw new Exception(xmlEx.Message);
            //}
            //catch (UriFormatException urif)
            //{
            //    throw new Exception(urif.Message);
            //}
            catch (Exception)
            {
                throw new Exception("Scene could not be loaded");
            }
            return ob;
        }
    }
}