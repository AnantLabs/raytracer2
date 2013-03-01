using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracerLib;
using Mathematics;

namespace EditorLib
{
    public class DrawingFacet : DrawingTriangle
    {
        private RayTracerLib.Triangle trian;
        public DrawingCustom DrCustObject { get; private set; }

        public DrawingFacet() : base() { }
        public DrawingFacet(RayTracerLib.Triangle trian, DrawingCustom drCust) : base(trian)
        {
            // TODO: Complete member initialization
            this.trian = trian;
            DrCustObject = drCust;
            SetLabelPrefix("facet");
        }

        private static Triangle Searching;
        private static bool TriangPredicate(Triangle tr)
        {
            if (tr.A.X == Searching.A.X && tr.A.Y == Searching.A.Y && tr.A.Z == Searching.A.Z &&
                tr.B.X == Searching.B.X && tr.B.Y == Searching.B.Y && tr.B.Z == Searching.B.Z &&
                tr.C.X == Searching.C.X && tr.C.Y == Searching.C.Y && tr.C.Z == Searching.C.Z)
                return true;
            else return false;
        }
        /// <summary>
        /// rozdeli drawingtriangle na 3 trojuhelniky a vrati dva nove.
        /// Z puvodniho draw.Tr. se stal treti novy triangl
        /// </summary>
        /// <returns>dva nove drawing trojuhelniky</returns>
        public void Split(Matrix3D rotMat)
        {
            DrCustObject.InitForRaytracer(rotMat);
            Triangle tr = ModelObject as Triangle;
            List<Triangle> trList = tr.Split();
            //this.SetModelObject(tr);

            CustomObject cust = DrCustObject.ModelObject as CustomObject;
            List<Triangle> facets = cust.FaceList;
            cust.InitializeForRayTr();
            facets.Add(trList[0]);
            facets.Add(trList[1]);
            facets.Add(trList[2]);
            Searching = trian;
            Triangle trFind = facets.Find(TriangPredicate);
            facets.Remove(trFind);
            cust.InitializeForRayTr(facets);
            DrCustObject.SetModelObject(cust);
            
        }

        public void Delete(Matrix3D rotMat)
        {
            DrCustObject.InitForRaytracer(rotMat);
            int count = DrCustObject.DrawingFacesList.Count;
            DrCustObject.DrawingFacesList.Remove(this);
            CustomObject cust = DrCustObject.ModelObject as CustomObject;
            count = cust.FaceList.Count;
            cust.FaceList.Remove(this.trian);
            cust.InitializeForRayTr(cust.FaceList);
            DrCustObject.SetModelObject(cust);
        }
    }
}
