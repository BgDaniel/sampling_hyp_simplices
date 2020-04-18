using HyperSimplices.CurvedGeometry;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry
{


    public class GenericSimplex<T> where T : ICloneable
    {
        public Dictionary<int, T> Edges { get; private set; }
        public int Dim { get; set; }
        public int DimAmbiantSpace { get; set; }
        public virtual void Negate() { }
        public T BasePoint { get; set; }
        public T[] DirectionalVectors { get; set; }
        public Parametrization Chart { get; set; }

        public GenericSimplex<T> RemoveEdge(int index)
        {
            var newEdges = new Dictionary<int, T>();

            foreach (var edge in Edges)
            {
                if(edge.Key != index)
                    newEdges[edge.Key] = (T)edge.Value.Clone();
            }
            
            return new GenericSimplex<T>(newEdges);
        }

        public List<GenericSimplex<T>> Faces
        {
            get
            {
                var ret = new List<GenericSimplex<T>>();
                var counter = 0;

                foreach (var edge in Edges)
                {
                    var face = RemoveEdge(counter);

                    if (counter % 2 != 0)
                        face.Negate();

                    ret.Add(face);
                }

                return ret;
            }
        }

        public GenericSimplex<T> GetOppositeFace(int index)
        {
            return RemoveEdge(index);
        }

        public List<GenericSimplex<T>> GetAdjacentFaces(int index)
        {
            var adjacentFaces = new List<GenericSimplex<T>>();
            
            for(int ell = 0; ell < Dim + 1; ell++)
            {
                if(ell != index)
                    adjacentFaces.Add(RemoveEdge(ell));                                
            }
            
            return adjacentFaces;
        }

        public GenericSimplex(Dictionary<int, T> edges)
        {
            Edges = edges;
        }

        public GenericSimplex<T> Clone()
        {
            var edgesCloned = new Dictionary<int, T>();

            foreach (var edge in Edges)
                edgesCloned[edge.Key] = (T)edge.Value.Clone();

            return new GenericSimplex<T>(edgesCloned);
        }
    }

    
}
