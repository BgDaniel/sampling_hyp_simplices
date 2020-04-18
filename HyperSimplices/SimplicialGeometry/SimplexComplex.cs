using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry
{
    public class GenericSimplexComplex<T> where T : ICloneable
    {
        public GenericSimplex<T> Simplex0 { get; private set; }
        public Dictionary<int, List<GenericSimplex<T>>> Chain { get; private set; }

        public GenericSimplexComplex(GenericSimplex<T> simplex0)
        {
            Simplex0 = simplex0;
            Chain = new Dictionary<int, List<GenericSimplex<T>>> { [Simplex0.Dim] = new List<GenericSimplex<T>> { Simplex0 } };
            Propagate();
        }

        public List<GenericSimplex<T>> CommonBaseSimplices(GenericSimplex<T> baseSimplex)
        {
            var commonBaseSimplices = new List<GenericSimplex<T>>();
            var baseIndices = Simplex0.Edges.Keys;
            List<int> indicesExtended;

            foreach (var index in baseIndices)
            {
                indicesExtended = new List<int>(baseIndices);                

                if (!baseSimplex.Edges.Keys.Contains(index))
                {
                    indicesExtended.Add(index);
                    var edgesExtended = Simplex0.Edges.Where(edge => indicesExtended.Contains(edge.Key))
                        .ToDictionary(edge => edge.Key, edge => edge.Value);
                    commonBaseSimplices.Add(new GenericSimplex<T>(edgesExtended));
                }                    
            }

            return commonBaseSimplices;
        }

        private void Propagate()
        {
            for(var dim = Simplex0.Dim; dim > 0; dim--)
            {
                var simplices = Chain[dim];

                foreach(var simplex in simplices)
                {
                    if (Chain.ContainsKey(dim - 1))
                        Chain[dim - 1].AddRange(simplex.Faces);
                    else
                        Chain[dim - 1] = new List<GenericSimplex<T>>(simplex.Faces);                    
                }
                    
            }
        }
    }
}
