using HyperSimplices.SimplicialGeometry.Simplex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry.SimplexComplex
{
    public class GenericSimplexPlair<T> where T: ICloneable
    {
        public GenericSimplex<T> CommonBase { get; private set; }
        public GenericSimplex<T> Simplex1 { get; private set; }
        public GenericSimplex<T> Simplex2 { get; private set; }

        public GenericSimplexPlair(GenericSimplex<T> commonBase, GenericSimplex<T> simplex1, GenericSimplex<T> simplex2)
        {
            CommonBase = commonBase;
            Simplex1 = simplex1;
            Simplex2 = simplex2;
        }
    }

    public class GenericSimplexComplex<T> where T : ICloneable
    {
        public GenericSimplex<T> Simplex0 { get; private set; }
        public Dictionary<int, List<GenericSimplex<T>>> Chain { get; private set; }
        public Dictionary<int, List<GenericSimplexPlair<T>>> SimplexPairs { get; private set; }

        public GenericSimplexComplex(GenericSimplex<T> simplex0)
        {
            Simplex0 = simplex0;
            Chain = new Dictionary<int, List<GenericSimplex<T>>> { [Simplex0.Dim] = new List<GenericSimplex<T>> { Simplex0 } };
            SimplexPairs = new Dictionary<int, List<GenericSimplexPlair<T>>>();
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

            foreach(var equalDimSimplices in Chain)
            {
                if (Simplex0.Dim - equalDimSimplices.Key < 2)
                    continue;

                SimplexPairs[equalDimSimplices.Key] = new List<GenericSimplexPlair<T>>();

                foreach (var simplex in equalDimSimplices.Value)
                {
                    var complementaryIndices = Simplex0.Edges.Keys.Except(simplex.Edges.Keys).ToArray();
                    var baseSimplex = simplex.Clone();
                    var baseEdges = simplex.Clone().Edges;

                    for (int i = 0; i < complementaryIndices.Length; i++)
                    {
                        var index_i = complementaryIndices[i];
                        var edgesExtended_i = simplex.Clone().Edges;
                        edgesExtended_i[index_i] = Simplex0.Edges[index_i];
                        var simplex_i = new GenericSimplex<T>(edgesExtended_i);

                        for (int j = i + 1; j < complementaryIndices.Length; j++)
                        {
                            var index_j = complementaryIndices[j];
                            var edgesExtended_j = simplex.Clone().Edges;
                            edgesExtended_j[index_j] = Simplex0.Edges[index_j];
                            var simplex_j = new GenericSimplex<T>(edgesExtended_j);

                            SimplexPairs[equalDimSimplices.Key].Add(new GenericSimplexPlair<T>(simplex, simplex_i, simplex_j));
                        }
                    }
                }                
            }
        }
    }
}
