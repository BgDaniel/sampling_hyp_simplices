using HyperSimplices.SimplicialGeometry.Simplex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry.SimplexComplex
{
    public class SimplexPair
    {
        public EuclideanSimplex CommonBase { get; private set; }
        public EuclideanSimplex Simplex1 { get; private set; }
        public EuclideanSimplex Simplex2 { get; private set; }

        public SimplexPair(EuclideanSimplex commonBase, EuclideanSimplex simplex1, EuclideanSimplex simplex2)
        {
            CommonBase = commonBase;
            Simplex1 = simplex1;
            Simplex2 = simplex2;
        }
    }

    public class SimplexComplex
    {
        public EuclideanSimplex Simplex0 { get; private set; }
        public Dictionary<int, List<EuclideanSimplex>> Chain { get; private set; }
        public Dictionary<int, List<SimplexPair>> SimplexPairs { get; private set; }

        public SimplexComplex(EuclideanSimplex simplex0)
        {
            Simplex0 = simplex0;
            Chain = new Dictionary<int, List<EuclideanSimplex>> { [Simplex0.Dim] = new List<EuclideanSimplex> { Simplex0 } };
            SimplexPairs = new Dictionary<int, List<SimplexPair>>();            
        }

        public List<EuclideanSimplex> CommonBaseSimplices(EuclideanSimplex baseSimplex)
        {
            var commonBaseSimplices = new List<EuclideanSimplex>();
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
                    commonBaseSimplices.Add(new EuclideanSimplex(edgesExtended));
                }                    
            }

            return commonBaseSimplices;
        }

        public void Propagate()
        {
            for(var dim = Simplex0.Dim; dim > 0; dim--)
            {
                var simplices = Chain[dim];

                foreach(var simplex in simplices)
                {
                    if (Chain.ContainsKey(dim - 1))
                        Chain[dim - 1].AddRange(simplex.Faces);
                    else
                        Chain[dim - 1] = new List<EuclideanSimplex>(simplex.Faces);                    
                }                    
            }

            foreach(var equalDimSimplices in Chain)
            {
                if (Simplex0.Dim - equalDimSimplices.Key < 2)
                    continue;

                SimplexPairs[equalDimSimplices.Key] = new List<SimplexPair>();

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
                        var simplex_i = new EuclideanSimplex(edgesExtended_i);

                        for (int j = i + 1; j < complementaryIndices.Length; j++)
                        {
                            var index_j = complementaryIndices[j];
                            var edgesExtended_j = simplex.Clone().Edges;
                            edgesExtended_j[index_j] = Simplex0.Edges[index_j];
                            var simplex_j = new EuclideanSimplex(edgesExtended_j);

                            SimplexPairs[equalDimSimplices.Key].Add(new SimplexPair(simplex, simplex_i, simplex_j));
                        }
                    }
                }                
            }
        }
    }
}
