using HyperSimplices.CurvedGeometry;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry.Simplex
{
    public class GenericSimplex<T> where T : ICloneable
    {
        public Dictionary<int, T> Edges { get; private set; }
        public int Dim => Edges.Count - 1;
        public virtual void Negate() { }
        public T BasePoint
        {
            get
            {
                return Edges.Values.First();
            }
            set
            {
                BasePoint = value;
            }
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
