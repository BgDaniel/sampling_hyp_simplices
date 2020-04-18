using HyperSimplices.SimplicialGeometry.Simplex;
using HyperSimplices.SimplicialGeometry.SimplexComplex;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SampleGeneration
{
    public class SampleFactory
    {
        public int NbSamples { get; private set; }
        public int Dim { get; private set; }
        public double MaxNorm { get; private set; }        

        public SampleFactory(int nbSamples, int dim, double maxNorm = 1.0)
        {
            NbSamples = nbSamples;
            Dim = dim;
            MaxNorm = maxNorm;
        }

        public (List<HyperbolicSimplex>, List<SimplexComplex>) RandomSamples()
        {
            var randomSimplices = HyperbolicSimplex.RandomSamples(NbSamples, Dim, MaxNorm);
            var hyperRandomComplexes = new List<SimplexComplex>();

            foreach (var randomSimplex in randomSimplices)                
            {
                var simplexComplex = new SimplexComplex(randomSimplex);
                simplexComplex.Propagate();
            }
                

            return (randomSimplices, null);
        }
    }
}
