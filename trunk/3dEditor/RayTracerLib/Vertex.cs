using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics;

namespace RayTracerLib
{
    public class Vertex : Vektor
    {
        public Vektor Normal { get; set; }
        public List<Triangle> NeighFaces { get; private set; }

        public Vertex() : this(Vektor.ZeroVektor) { }
        public Vertex(Vektor vector) : base(vector)
        {
            this.Normal = Vektor.ZeroVektor;
            NeighFaces = new List<Triangle>(8);
        }

        /// <summary>
        /// Prida plosku do seznamu sousednich plosek.
        /// Byla-li jiz plozka soucasti seznamu, vrati FALSE a neprida ji.
        /// Jinak ji prida a vrati TRUE.
        /// </summary>
        /// <param name="face">ploska obsahujici tento vrchol</param>
        /// <returns>TRUE, kdyz byla ploska pridana. Jinak FALSE - ploska byla jiz predtim pritomna v seznamu</returns>
        public bool AddFace(Triangle face)
        {
            if (NeighFaces.Contains(face)) return false;
            NeighFaces.Add(face);
            return true;
        }

        /// <summary>
        /// Nastavi normalu z normal okolnich plosek
        /// </summary>
        public void SetNorm()
        {
            foreach (Triangle face in NeighFaces)
            {
                this.Normal += face.Norm;
            }
            this.Normal.Normalize();
        }
    }
}
