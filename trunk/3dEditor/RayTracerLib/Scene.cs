using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{

    /// <summary>
    /// Popis zobrazovane sceny. Obsahuje:
    ///     kameru, jenz generuje paprsky pro snimani sceny.
    ///     svetla osvetlujici scenu
    ///     zakladni okolni barvu - "vzduchu"
    ///     barvu pozadi
    ///     seznam zobrazovanych objektu ve scene (koule, ..)
    ///     
    /// Nejdulezitejsi je metoda GetIntersectPoint, ktera vraci nejblizsi bod pruniku, jenz vznikne protnutim
    /// zadaneho paprsku s objekty ve scene
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// generator paprsku
        /// </summary>
        public Camera Camera { get; set; }

        /// <summary>
        /// seznam svetel
        /// </summary>
        public List<Light> Lights { get; set; }

        /// <summary>
        /// okolni barva
        /// </summary>
        public Colour AmbientLightColor { get; set; }

        /// <summary>
        /// barva pozadi sceny
        /// </summary>
        public Colour BgColor { get; set; }

        /// <summary>
        /// vsechny objekty ve scene, ktere osvetlujeme
        /// </summary>
        public List<DefaultShape> SceneObjects { get; set; }

        public RTree R_Tree { get; private set; }
        public bool IsOptimizing { get; set; }

        private Optimalizer Optimaliz;

        private string _caption;

        /// <summary>
        /// celkovy pocet vsech pruniku paprsku s objekty ve scene
        /// </summary>
        private static ulong totalInters = 0;

        /// <summary>
        /// nazev/popisek sceny
        /// </summary>
        public string Caption
        {
            get
            {
                if (_caption == null)
                    return this.ToString();

                return _caption;
            }
            set 
            {
                _caption = value;
            }
        }

        public Scene()
        {
            SetDefaultCamera();
            SetDefaultLights();
            SetDefaultColors();
            SetDefaultSceneObjects();
        }

        public Scene(Scene old)
        {
            Camera = new Camera(old.Camera);
            Lights = new List<Light>(old.Lights.Count);
            foreach (Light l in old.Lights)
            {
                Light nl = new Light(l);
                Lights.Add(nl);
            }
            AmbientLightColor = new Colour(old.AmbientLightColor);
            BgColor = new Colour(old.BgColor);

            SceneObjects = new List<DefaultShape>(old.SceneObjects.Count);
            foreach (DefaultShape ds in old.SceneObjects)
            {
                if (ds.GetType() == typeof(Sphere))
                {
                    Sphere sp = new Sphere((Sphere)ds);
                    SceneObjects.Add(sp);
                }
                else if (ds.GetType() == typeof(Box))
                {
                    Box b = new Box((Box)ds);
                    SceneObjects.Add(b);
                }
                else if (ds.GetType() == typeof(Cylinder))
                {
                    Cylinder c = new Cylinder((Cylinder)ds);
                    SceneObjects.Add(c);
                }
                else if (ds.GetType() == typeof(Plane))
                {
                    Plane p = new Plane((Plane)ds);
                    SceneObjects.Add(p);
                }
                else
                {
                    SceneObjects.Add(ds);
                }
            }
            Optimaliz = old.Optimaliz;
        }

        /// <summary>
        /// nastavi zakladni kameru - generator paprsku
        /// </summary>
        public void SetDefaultCamera()
        {
            Camera = new Camera();
        }

        /// <summary>
        /// nastavi zakladni svetelne zdroje
        /// </summary>
        public void SetDefaultLights()
        {
            
            Lights = new List<Light>();
            Light source1 = new Light();
            Light source2 = new Light(new Vektor(5, 0, -6), new Colour(0.9, 0.9, 0, 1));
            this.AddLight(source1);
            this.AddLight(source2);

        }

        /// <summary>
        /// Odebere objekt ze sceny
        /// </summary>
        /// <param name="itemToRemove"></param>
        public void RemoveObject(object itemToRemove)
        {

            if (itemToRemove.GetType() == typeof(Sphere))
            {
                Sphere sph = (Sphere)itemToRemove;
                this.SceneObjects.Remove(sph);
            }
            // vymazani roviny ze sceny
            else if (itemToRemove.GetType() == typeof(Plane))
            {
                Plane p = (Plane)itemToRemove;
                this.SceneObjects.Remove(p);
            }
            // vymazani krychle ze sceny
            else if (itemToRemove.GetType() == typeof(Box))
            {
                Box b = (Box)itemToRemove;
                this.SceneObjects.Remove(b);
            }
                // vymazani krychle ze sceny
            else if (itemToRemove.GetType() == typeof(Cube))
            {
                Cube c = (Cube)itemToRemove;
                this.SceneObjects.Remove(c);
            }
            else if (itemToRemove.GetType() == typeof(Cylinder))
            {
                Cylinder cyl = (Cylinder)itemToRemove;
                this.SceneObjects.Remove(cyl);
            }

            // vymazeme svetlo ze sceny
            else if (itemToRemove.GetType() == typeof(Light))
            {
                Light l = (Light)itemToRemove;
                this.Lights.Remove(l);
            }

            else if (itemToRemove is Triangle)
            {
                Triangle tr = itemToRemove as Triangle;
                this.SceneObjects.Remove(tr);
            }
            else if (itemToRemove is CustomObject)
            {
                CustomObject cust = itemToRemove as CustomObject;
                this.SceneObjects.Remove(cust);
            }
        }
        public void AddLight(Light l)
        {
            Lights.Add(l);
        }

        public void AddSceneObject(DefaultShape ds)
        {
            SceneObjects.Add(ds);
        }

        /// <summary>
        /// nastavi zakladni barvy sceny: barvu pozadi a ambientni
        /// </summary>
        public void SetDefaultColors()
        {
            AmbientLightColor = new Colour(0.5, 0.0, 0.0, 1);
            BgColor = new Colour(0.2, 0.2, 0.2, 1);
        }

        /// <summary>
        /// zavolat pred spustenim raytraceru
        /// </summary>
        public void SetBeforeRayTr(RayImage rayimg)
        {
            Scene.totalInters = 0;
            Cuboid.TotalTested = 0;
            SetOptmimizer(rayimg.OptimizType);
            BgColor = rayimg.BackgroundColor;
            IsOptimizing = rayimg.IsOptimalizing;
            if (this.Optimaliz == null) IsOptimizing = false;
        }
        private void SetOptmimizer(Optimalizer.OptimizeType optimType)
        {
            this.Optimaliz = new Optimalizer(optimType, this.SceneObjects);
        }

        /// <summary>
        /// nastavi zakladni objekty ve scene
        /// </summary>
        public void SetDefaultSceneObjects()
        {
            this.SceneObjects = new List<DefaultShape>();

            Sphere sp1 = new Sphere(new Vektor(-1.5, 0, -6), 1);
            Sphere sp2 = new Sphere(new Vektor(0.0, 0, -6), 1);

            this.SceneObjects.Add(sp1);
            this.SceneObjects.Add(sp2);

            Plane p = new Plane(new Vektor(1, -9, 1), 1.9);
           this.SceneObjects.Add(p);

            Box b = new Box(new Vektor(0, 0, -2), 1);
            this.SceneObjects.Add(b);
        }

        /// <summary>
        /// spocita nejblizsi bod pruniku pro paprsek
        /// </summary>
        /// <param name="P0">pocatek paprsku</param>
        /// <param name="P1">smerovy vektro paprsku</param>
        /// <returns>nejblizsi bod, ktery protne paprsek a ma byt tedy vykreslen</returns>
        public SolidPoint GetIntersectPoint(Vektor P0, Vektor P1)
        {
            if (P0 == null || P1 == null)
                return null;

            
            List<SolidPoint> interSolids = new List<SolidPoint>();

            //P1.Normalize();

            //if (IsOptimizing)
            //{
                //if (R_Tree == null) this.SetRtree();
                //R_Tree.TestIntersection(P0, P1, ref interSolids);
                Optimaliz.Optimizer.Intersection(P0, P1, ref interSolids);

                // rovina se do RStromu nepridava - nutno overit zlvast vsechny roviny
                foreach (DefaultShape obj in this.SceneObjects)
                {
                    if (obj is Plane)
                        obj.Intersects(P0, P1, ref interSolids);
                }
            //}
            //else
            //{
            //    // pro kazdy objekt ve scene otestujeme, zda je protnut paprskem a zjistime body pruniku
            //    foreach (DefaultShape obj in this.SceneObjects)
            //    {
            //        obj.Intersects(P0, P1, ref interSolids);
            //    }
            //}

            // paprsek neprotina zadny objekt ve scene:
            if (interSolids.Count == 0)
                return null;

            Scene.totalInters = Scene.totalInters + (uint)interSolids.Count;

            if (interSolids.Count > 1)
                interSolids.Sort();

            // vybereme prvni bod pruniku, jenz je protnut
            foreach (SolidPoint sp in interSolids)
            {
                if (sp.T > 0 && sp.T * sp.T > MyMath.EPSILON)
                    return sp;
            }

            return null;
        }

        public SolidPoint GetIntersectPointForShadow(Vektor P0, Vektor P1)
        {
            if (P0 == null || P1 == null)
                return null;

            List<SolidPoint> interSolids = new List<SolidPoint>();
            bool isIntersected = false;

            if (IsOptimizing)
            {
                //R_Tree.TestIntersection(P0, P1, ref interSolids);
                Optimaliz.Optimizer.Intersection(P0, P1, ref interSolids);
                if (interSolids.Count == 0)
                {

                    // rovina se do RStromu nepridava - nutno overit zlvast vsechny roviny
                    foreach (DefaultShape obj in this.SceneObjects)
                    {
                        if (obj is Plane)
                        {
                            isIntersected = obj.Intersects(P0, P1, ref interSolids);
                            if (isIntersected) break;
                        }
                    }
                }
            }
            else
            {
                // pro kazdy objekt ve scene otestujeme, zda je protnut paprskem a zjistime body pruniku
                foreach (DefaultShape obj in this.SceneObjects)
                {
                    isIntersected = obj.Intersects(P0, P1, ref interSolids);
                    if (isIntersected) break;
                }
            }

            if (interSolids.Count > 0) return interSolids[0];
            return null;
        }


        /// <summary>
        /// Zjisti vsechna svetla, ktera osvetluji dany bod
        /// </summary>
        /// <param name="sp">bod, na nejz ma svitit nejake svetlo</param>
        /// <returns>seznam svetel</returns>
        public Light[] GetAllLightningsToPoint(SolidPoint sp)
        {
            if (sp == null || Lights==null || Lights.Count ==0 )
                return null;

            List<Light> lightsList = new List<Light>();
            List<Light> softListLights;

            foreach (Light l in Lights)
            {
                if (l.IsSoftLight)
                {
                    
                    if (l.LightningSoft2(sp, this, out softListLights))
                        lightsList.AddRange(softListLights);
                }
                else
                {
                    if (l.Lightning(sp, this))
                    {
                        lightsList.Add(l);
                    }
                }
            }

            return lightsList.ToArray();
        }

        internal void SetRtree()
        {
            this.R_Tree = new RTree(SceneObjects.ToArray());
        }

        public override string ToString()
        {
            return String.Format("Scena: {0} svetel, {1} objektu", Lights.Count, SceneObjects.Count);
        }


        ///////////////////////////////////////////////////////
        //// PREDNASTAVENE SCENY
        ///////////////////////////////////////////////////////

        /// <summary>
        /// Prednastavena scena 1
        /// </summary>
        public void SetDefaultScene1()
        {
            // SVETLA:
            Light source1 = new Light();
            Light source2 = new Light(new Vektor(5, 0, -6), new Colour(0.9, 0.9, 0, 1));
            Lights = new List<Light>();
            Lights.Add(source1);
            Lights.Add(source2);

            // OBJEKTY:
            this.SceneObjects = new List<DefaultShape>();

            Sphere sp1 = new Sphere(new Vektor(-1.5, 0, -6), 1);
            Sphere sp2 = new Sphere(new Vektor(0.0, 0.5, -6), 1);
            Sphere sp3 = new Sphere(new Vektor(1.5, 1.0, -6), 1);

            this.SceneObjects.Add(sp1);
            this.SceneObjects.Add(sp2);
            this.SceneObjects.Add(sp3);

            Plane p = new Plane(new Vektor(1, -9, 1), 1.9);
            this.SceneObjects.Add(p);

            this.Caption = "3 joined spheres above plane";
        }

        /// <summary>
        /// Prednastavena scena 2
        /// </summary>
        public void SetDefaultScene2()
        {
            // SVETLA:
            Light source1 = new Light(new Vektor(5, 0, -6), new Colour(0.9, 0.9, 0, 1));
            Light source2 = new Light(new Vektor(0, 0, 3), new Colour(1, 1, 1, 1));
            
            Lights = new List<Light>();
            Lights.Add(source1);
            Lights.Add(source2);

            
            // OBJEKTY:
            this.SceneObjects = new List<DefaultShape>();

            Sphere sp1 = new Sphere(new Vektor(0, -3, -12), 2);
            sp1.Material.Color = new Colour(0.3, 0.86, 0.01, 1);
            Sphere sp2 = new Sphere(new Vektor(0.8, 0, -6), 1.5);

            Sphere sp3 = new Sphere(new Vektor(3, -2, -7), 1);
            sp3.Material = Material.Glass;

            this.SceneObjects.Add(sp1);
            this.SceneObjects.Add(sp2);
            this.SceneObjects.Add(sp3);

            Plane p0 = new Plane(new Vektor(1, 0, 4), 9);
            p0.MinX = -2;
            p0.MaxX = -0.8;
            Plane p1 = new Plane(new Vektor(0, 0, 4), 9);
            p1.MinX = -0.3;
            p1.MaxX = 0.2;
            Plane p2 = new Plane(new Vektor(-1, 0, 4), 9);
            p2.MinX = 0.7;
            p2.MaxX = 1.2;
            Plane p3 = new Plane(new Vektor(-2, 0, 4), 9);
            p3.MinX = 1.7;
            p3.MaxX = 3;
            this.SceneObjects.Add(p0);
            this.SceneObjects.Add(p1);
            this.SceneObjects.Add(p2);
            this.SceneObjects.Add(p3);

            this.Caption = "Planes around and between spheres";
        }


        /// <summary>
        /// Prednastavena scena 3
        /// </summary>
        public void SetDefaultScene3()
        {
            this.SceneObjects = new List<DefaultShape>();

            Plane p = new Plane(new Vektor(1, -9, 1), 1.9);
            p.MinX = -2.0;
            p.MaxX = 2.0;
            p.MinY = -2.0;
            p.MaxY = 2.0;
            this.SceneObjects.Add(p);

            Cube c = new Cube(new Vektor(0, 0, -7), new Vektor(0.2, -0.5, 0.6), 1);
            this.SceneObjects.Add(c); 

            Sphere sp = new Sphere(new Vektor(-2, 0, -8), 1);
            sp.Material.Color = new Colour(0.9, 0.2, 0.3, 0);
            this.SceneObjects.Add(sp);

            this.Caption = "Cube with sphere above plane";
        }
        public void SetDefaultScene4()
        {
            // SVETLA:
            Light source1 = new Light();
            Light source2 = new Light(new Vektor(-1, -10, 1), new Colour(0.9, 0.9, 0, 1));
            Lights = new List<Light>(3);
            Lights.Add(source1);
            Lights.Add(source2);

            // OBJEKTY:
            this.SceneObjects = new List<DefaultShape>();

            //Plane p = new Plane(new Vektor(1, -9, 1), 2, new Colour(0.06, 0.89, 0.933, 1));
            Plane2 p = new Plane2();
            //p.MinX = -2.0;
            //p.MaxX = 3.0;
            //p.MinY = -2.0;
            //p.MaxY = 3.0;

            Sphere sp1 = new Sphere(new Vektor(-2, 1, -7), 1);
            sp1.Material.Color = new Colour(0.2, 0.9, 0.1, 1);

            Sphere sp2 = new Sphere(new Vektor(0, 0, -4), 1, new Colour(0.2, 0.9, 0.1, 1));
            sp2.IsActive = true;
            Cylinder c = new Cylinder(new Vektor(0.5, 0, -8), new Vektor(0.5, 0.2, -0.4), 1, 4);

            this.Camera.SetNormAndUp(new Vektor(0, 1, -1), this.Camera.Up);
            this.Camera.Source = new Vektor(-2, -10, 5);

            this.SceneObjects.Add(c);
            this.SceneObjects.Add(sp1);
            this.SceneObjects.Add(sp2);
            this.SceneObjects.Add(p);

            this.Caption = "Cylinder onto sphere";
        }

        public void SetDefaultScene5()
        {
            // SVETLA:
            Light source1 = new Light();
            Light source2 = new Light(new Vektor(5, 0, -6), new Colour(0.9, 0.9, 0, 1));
            source1.IsActive = false;
            source2.IsActive = false;
            Light source3 = new Light(new Vektor(-3, -3, -5), new Colour(1, 1, 1, 1));
            Lights = new List<Light>(3);
            Lights.Add(source1);
            Lights.Add(source2);
            source3.IsSoftLight = true;
            source3.SoftEpsilon = 0.5;
            Lights.Add(source3);

            this.Camera.Source = new Vektor(1, 1, 1);

            // OBJEKTY:
            this.SceneObjects = new List<DefaultShape>();

            Plane p = new Plane(new Vektor(1, -9, 1), 2, new Colour(0.06, 0.89, 0.933, 1));
            p.MinX = -2.0;
            p.MaxX = 3.0;
            p.MinY = -2.0;
            p.MaxY = 3.0;

            Plane p2 = new Plane(new Vektor(0, -9, 0), 1.1);
            p2.MinX = -1.0;
            p2.MaxX = 1.0;
            p2.MinY = -2.0;
            p2.MaxY = 2.0;
            p2.MinZ = -5;
            p2.MaxZ = -2.5;

            this.SceneObjects.Add(p);
            this.SceneObjects.Add(p2);

            this.Caption = "Plane above plane";
        }

        public void SetDefaultScene6()
        {
            BgColor = new Colour(0.06, 0.06, 0.06, 1);

            // SVETLA:
            Light source1 = new Light();
            Light source2 = new Light(new Vektor(5, 0, -6), new Colour(0.9, 0.9, 0, 1));
            Light source3 = new Light(new Vektor(-1.0, 0, -11), new Colour(1, 1, 1, 1));
            Light source4 = new Light(new Vektor(-3.0, 2, -2), new Colour(1, 1, 1, 1));
            Lights = new List<Light>(4);
            Lights.Add(source1);
            Lights.Add(source2);
            Lights.Add(source3);
            Lights.Add(source4);

            // OBJEKTY:
            this.SceneObjects = new List<DefaultShape>();

            Sphere sp1 = new Sphere(new Vektor(4, -1.5, -8), 1);
            Sphere sp2 = new Sphere(new Vektor(1.5, 1.0, -6), 1, new Colour(0.2, 0.7, 0.3, 1));
            sp2.IsActive = false;
            sp2.Material = Material.Mirror;
            Sphere sp3 = new Sphere(new Vektor(-3, -1, -7), 1, new Colour(0.96, 0.14, 0.0, 1));
            sp3.Material = Material.Glass;
            sp3.Material.KT = 1.0;
            sp3.Material.N = 1.6;
            Sphere sp4 = new Sphere(new Vektor(4, -0.5, -2), 1, new Colour(0.957, 0.043, 0.777, 1));
            sp4.Material = Material.Glass;
            sp4.Material.Color = new Colour(0.957, 0.043, 0.777, 1);

            Sphere sp5 = new Sphere(new Vektor(-2, -0.5, -1), 1, new Colour(0.035, 0.035, 0.984, 1));

            Sphere sp6 = new Sphere(new Vektor(1.5, -5.5, -6), 3);
            sp6.Material = Material.Mirror;

            this.SceneObjects.Add(sp1);
            this.SceneObjects.Add(sp2);
            this.SceneObjects.Add(sp3);
            this.SceneObjects.Add(sp4);
            this.SceneObjects.Add(sp5);
            this.SceneObjects.Add(sp6);

            Plane p1 = new Plane(new Vektor(1, -9, 1), 1.9);
            p1.MaxX=1.5;
            p1.MaxY= 1.5;
            p1.MinY= -4.5;
            p1.MaxZ= -2;
            p1.MinZ = -7;

            Plane p2 = new Plane(new Vektor(1, -9, 1), 2, new Colour(0.854, 0.87, 0.129, 0.0));
            p2.MaxX = 5.0;
            p2.MinX = 1.0;
            p2.MaxY = 1.5;
            p2.MinY = -4.5;
            p2.MaxZ = -7.0;

            Plane p3 = new Plane(new Vektor(1, -9, 1), 2, new Colour(0.188, 0.769, 0.812, 0.0));
            p3.MaxX= 3;
            p3.MinX= 0.5;
            p3.MinY=-3.5;
            p3.MinZ = -6;

            Plane p4 = new Plane(new Vektor(0.8, -9, 1), 2, new Colour(0.984, 0.137, 0.016, 0.0));
            p4.MinX = 2;
            p4.MaxZ = -4.5;
            p4.MinZ = -7;

            this.SceneObjects.Add(p1);
            this.SceneObjects.Add(p2);
            this.SceneObjects.Add(p3);
            this.SceneObjects.Add(p4);

            Cylinder c1 = new Cylinder(new Vektor(1.5, 1.0, -6), new Vektor(0, 1, 0), 2, 10);
            c1.Material = Material.Mirror;
            this.SceneObjects.Add(c1);

            Cube cube1 = new Cube(new Vektor(-5, 0.5, -3), new Vektor(-0.3, 1, -0.5), 1);
            cube1.Material.Color = new Colour(1, 1, 1, 1);
            Cube cube2 = new Cube(new Vektor(1.5, 1.9, 3), new Vektor(0, 1, -1), 1);
            cube2.Material.Color = new Colour(1, 1, 1, 1);
            cube2.Material.Ka = 1;
            cube2.Material.Kd = 0.1;
            cube2.Material.Ks = 0;
            Cube cube3 = new Cube(new Vektor(6, 1, -5.5), new Vektor(0, 1, -0.5), 1);
            cube3.Material.Color = new Colour(1, 1, 1, 1);
            cube3.Material.Ka = 0.1;
            cube3.Material.Kd = 1;
            cube3.Material.Ks = 0;
            Cube cube4 = new Cube(new Vektor(2, 0, -12), new Vektor(-1, 0.5, 0.2), 1.5);
            cube4.Material = Material.Mirror;
            cube4.Material.Color = new Colour(0.04, 0.9, 0.6, 1);

            this.SceneObjects.Add(cube1);
            this.SceneObjects.Add(cube2);
            this.SceneObjects.Add(cube3);
            this.SceneObjects.Add(cube4);

            this.Caption = "MEGA: planes, spheres and cubes around cylinder";
        }

        public void SetDefaultScene7()
        {
            Camera cam = new Camera();
            cam.Source = new Vektor(-1, -4, 1);
            cam.SetNormAndUp(new Vektor(0, 0.4, -1), new Vektor(0, -1, 0));
            this.Camera = cam;

            Light light1 = new Light(new Vektor(1, -4, -1), new Colour(1, 1, 1, 1));
            Lights = new List<Light>(5);
            Lights.Add(light1);

            this.SceneObjects = new List<DefaultShape>();

            Plane p = new Plane(new Vektor(2, -6, 0), 3);
            p.Material.Color = new Colour(0.3, 0.6, 0.92, 1);
            this.SceneObjects.Add(p);

            Cylinder c = new Cylinder(new Vektor(0, -1, -6), new Vektor(0.5, 0.2, -0.4), 1, 4);
            c.Material.Color = new Colour(0.97, 0.99, 0.25, 1);
            this.SceneObjects.Add(c);

            Sphere sp1 = new Sphere(new Vektor(0, -0.5, -5.5), 1);
            sp1.Material = Material.Rubber;
            sp1.Material.Color = new Colour(0.80784, 0, 0, 0);
            Sphere sp2 = new Sphere(new Vektor(0, -2, -6.5), 1);
            sp2.Material = Material.Rubber;
            sp2.Material.Color = new Colour(0.80784, 0, 0, 0);
            Sphere sp3 = new Sphere(new Vektor(-1.5, -1.5, -5), 1);
            sp3.Material = Material.Rubber;
            sp3.Material.Color = new Colour(0.80784, 0, 0, 0);
            Sphere sp4 = new Sphere(new Vektor(01.5, -0.5, -7), 1);
            sp4.Material = Material.Rubber;
            sp4.Material.Color = new Colour(0.80784, 0, 0, 0);
            this.SceneObjects.Add(sp1);
            this.SceneObjects.Add(sp2);
            this.SceneObjects.Add(sp3);
            this.SceneObjects.Add(sp4);


            this.Caption = "4 spheres within cylinder shading onto a plane";
        }


        public void SetDefaultScene8()
        {
            BgColor = new Colour(1, 1, 1, 1);

            Camera cam = new Camera();
            cam.Source = new Vektor(4, 2.5, 3);
            cam.SetNormAndUp(new Vektor(0, 0, -1), new Vektor(0, -1, 0));
            this.Camera = cam;

            Light light1 = new Light(new Vektor(-3, 0, 0), new Colour(10, 10, 10, 1));
            Lights = new List<Light>(5);
            Lights.Add(light1);

            this.SceneObjects = new List<DefaultShape>();

            Plane p = new Plane(new Vektor(-1, -9, 2.5), 7);
            p.Material.Color = new Colour(1, 1, 1, 1);
            p.Material.Ka = 1;
            p.Material.Kd = 1;
            p.Material.Ks = 0;
            p.MinZ = -55;
            this.SceneObjects.Add(p);

            Cylinder c1 = new Cylinder(new Vektor(1, 2.5, -6), new Vektor(0.5, 0, 0), 0.5, 3);
            c1.Material.Color = new Colour(0.7492, 0, 0.07451, 1);
            c1.Material.Ka = 1;
            c1.Material.Kd = 0.3;
            c1.Material.Ks = 0;

            Cylinder c2 = new Cylinder(new Vektor(1, -0.5, -6), new Vektor(0.5, 0, 0), 0.5, 3);
            c2.Material.Color = new Colour(0.04706, 0.42745, 0.89412, 1);
            c2.Material.Ka = 1;
            c2.Material.Kd = 0.3;
            c2.Material.Ks = 0;

            Cylinder c3 = new Cylinder(new Vektor(0, 3, -6), new Vektor(0, 0.5, 0), 0.5, 7);
            c3.Material.Color = new Colour(0.5, 0.1, 0.6, 1);
            c3.Material.Ka = 1;
            c3.Material.Kd = 0.3;
            c3.Material.Ks = 0;

            Cylinder c4 = new Cylinder(new Vektor(2, 1, -6), new Vektor(0, 0.5, 0), 0.5, 3);
            c4.Material.Color = new Colour(0.82745, 0.85098, 0.0902, 1);
            c4.Material.Ka = 1;
            c4.Material.Kd = 0.3;
            c4.Material.Ks = 0;

            Cylinder c5 = new Cylinder(new Vektor(1.5, 4.5, -6), new Vektor(0.4, 0.6, 0), 0.5, 4.5);
            c5.Material.Color = new Colour(0.30980, 0.85098, 0.14902, 1);
            c5.Material.Ka = 1;
            c5.Material.Kd = 0.3;
            c5.Material.Ks = 0;

            this.SceneObjects.Add(c1);
            this.SceneObjects.Add(c2);
            this.SceneObjects.Add(c3);
            this.SceneObjects.Add(c4);
            this.SceneObjects.Add(c5);


            this.Caption = "R shadow R";
        }

        public void SetDefaultScene9()
        {
            BgColor = new Colour(1, 1, 1, 1);

            Camera cam = new Camera();
            cam.Source = new Vektor(-3, -3.7, -5);
            cam.SetNormAndUp(new Vektor(0, 0.5, -0.5), new Vektor(0, -1, 0));
            this.Camera = cam;

            Lights = new List<Light>(5);
            Light light1 = new Light(new Vektor(-3, -7, -10), new Colour(1, 1, 1, 1));
            Light light2 = new Light(new Vektor(-3, 8.4, -11.5), new Colour(1, 1, 1, 1));
            Lights.Add(light1);
            Lights.Add(light2);

            this.SceneObjects = new List<DefaultShape>();

            Sphere sph = new Sphere(new Vektor(-3, 0, -10), 2.5);
            sph.R = 2.5;
            sph.Material.Ks = 1;
            sph.Material.Ka = 0.1;
            sph.Material.Kd = 0.1;
            sph.Material.Color = new Colour(1, 1, 1, 1);

            this.SceneObjects.Add(sph);

            Cylinder c1 = new Cylinder(new Vektor(-0.5, 1, -6), new Vektor(0.4, -0.6, 0), 0.5, 3);
            c1.Material.Color = new Colour(0.74920, 0, 0.07451, 1);
            c1.Material.Ka = 1;
            c1.Material.Kd = 1;
            c1.Material.Ks = 0;

            Cylinder c2 = new Cylinder(new Vektor(-3.3, 0, -6), new Vektor(1, 0, 0), 0.5, 7.5);
            c2.Material.Color = new Colour(0.74920, 0, 0.07451, 1);
            c2.Material.Ka = 1;
            c2.Material.Kd = 1;
            c2.Material.Ks = 0;

            Cylinder c3 = new Cylinder(new Vektor(-0.5, -1, -6), new Vektor(0.4, 0.6, 0), 0.5, 3);
            c3.Material.Color = new Colour(0.74920, 0, 0.07451, 1);
            c3.Material.Ka = 1;
            c3.Material.Kd = 1;
            c3.Material.Ks = 0;

            this.SceneObjects.Add(c1);
            this.SceneObjects.Add(c2);
            this.SceneObjects.Add(c3);

            this.Caption = "Logo";
        }




        
        public ulong GetTotalIntersections()
        {
            return totalInters;
        }
    }
}
