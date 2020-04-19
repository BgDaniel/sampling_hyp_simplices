using HyperSimplices.SimplicialGeometry.Simplex;
using MathNet.Numerics.LinearAlgebra;
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
        public double Angle { get; private set; }

        public void ComputeAngle()
        {
            var ind1 = Simplex1.Indices;
            var ind2 = Simplex2.Indices;
            
            var ind1_ext = ind2.Except(ind1).ToArray()[0];
            var vec1 = Simplex2.GetEdgeByIndex(ind1_ext);

            var ind2_ext = ind1.Except(ind2).ToArray()[0];
            var vec2 = Simplex1.GetEdgeByIndex(ind2_ext);

            var norm1 = VariousHelpers.GetNormalVector(Simplex1, vec1 - Simplex1.BasePoint);
            var norm2 = VariousHelpers.GetNormalVector(Simplex2, vec2 - Simplex2.BasePoint);

            Angle = norm1.DotProduct(norm2) / (norm1.L2Norm() * norm2.L2Norm());
        }

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

        public void Integrate(int meshSteps, List<int> dimensions = null, bool computeLengthAnalytical = false)
        {
            foreach(var item in Chain)
            {
                if (dimensions != null && !dimensions.Contains(item.Key))
                    continue;

                if (item.Key == 0)
                    return;

                foreach(var simplex in item.Value)
                    simplex.Integrate(meshSteps, computeLengthAnalytical);                
            }
        }

        public void ComputeAngles()
        {
            foreach (var item in SimplexPairs)
            {
                foreach (var simplexPair in item.Value)
                    simplexPair.ComputeAngle();
            }
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
