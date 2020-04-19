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
        public HyberbolicSimplex CommonBase { get; private set; }
        public HyberbolicSimplex Simplex1 { get; private set; }
        public HyberbolicSimplex Simplex2 { get; private set; }

        public SimplexPair(HyberbolicSimplex commonBase, HyberbolicSimplex simplex1, HyberbolicSimplex simplex2)
        {
            CommonBase = commonBase;
            Simplex1 = simplex1;
            Simplex2 = simplex2;
        }
    }

    public class SimplexComplex
    {
        public HyberbolicSimplex Simplex0 { get; private set; }
        public Dictionary<int, List<HyberbolicSimplex>> Chain { get; private set; }
        public Dictionary<int, List<SimplexPair>> SimplexPairs { get; private set; }

        public SimplexComplex(HyberbolicSimplex simplex0)
        {
            Simplex0 = simplex0;
            Chain = new Dictionary<int, List<HyberbolicSimplex>> { [Simplex0.Dim] = new List<HyberbolicSimplex> { Simplex0 } };
            SimplexPairs = new Dictionary<int, List<SimplexPair>>();            
        }

        public void Integrate(int meshSteps, List<int> dimensions = null)
        {
            foreach(var item in Chain)
            {
                if (dimensions != null && !dimensions.Contains(item.Key))
                    continue;

                if (item.Key == 0)
                    return;

                foreach(var simplex in item.Value)
                    simplex.Integrate(meshSteps);                
            }
        }

        public void ComputeAngles()
        {

        }

        public void Propagate()
        {
            for(var dim = Simplex0.Dim; dim > 0; dim--)
            {
                var simplices = Chain[dim];

                foreach(var simplex in simplices)
                {
                    if (Chain.ContainsKey(dim - 1))
                    {
                        foreach(var newFace in simplex.Faces)
                        {
                            if (!Chain[dim - 1].Contains(newFace))
                                Chain[dim - 1].Add(newFace);
                        }
                    }
                    else
                        Chain[dim - 1] = new List<HyberbolicSimplex>(simplex.Faces);                    
                }                    
            }

            foreach(var equalDimSimplices in Chain)
            {
                if (Simplex0.Dim - equalDimSimplices.Key < 2)
                    continue;

                SimplexPairs[equalDimSimplices.Key + 1] = new List<SimplexPair>();

                foreach (var simplex in equalDimSimplices.Value)
                {
                    var complementaryEdges = Simplex0.ComplementaryEdges(simplex.Edges);

                    for (int i = 0; i < complementaryEdges.Count; i++)
                    {
                        var simplex_i = simplex.AddEdge(complementaryEdges[i]);

                        for (int j = i + 1; j < complementaryEdges.Count; j++)
                        {
                            var simplex_j = simplex.AddEdge(complementaryEdges[j]);
                            SimplexPairs[equalDimSimplices.Key + 1].Add(new SimplexPair(simplex, simplex_i, simplex_j));
                        }
                    }
                }                
            }
        }
    }
}
