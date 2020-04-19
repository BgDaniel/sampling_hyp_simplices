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
        public bool Integrate { get; private set; }
        public int MeshSteps { get; private set; }
        public bool ZeroAmongEdges { get; private set; }
        public bool ComputeAngles { get; private set; }

        public SampleFactory(int nbSamples, int dim, bool integrate = false, int meshSteps = 1000, double maxNorm = 1.0, 
            bool zeroAmongEdges = false, bool computeAngles = false)
        {
            NbSamples = nbSamples;
            Dim = dim;
            MaxNorm = maxNorm;
            Integrate = integrate;
            MeshSteps = meshSteps;
            ZeroAmongEdges = zeroAmongEdges;
            ComputeAngles = computeAngles;
        }

        public (List<HyberbolicSimplex>, List<SimplexComplex>) RandomSamples()
        {
            var randomSimplices = HyberbolicSimplex.RandomSamples(NbSamples, Dim, ZeroAmongEdges, MaxNorm);
            var hyperRandomComplexes = new List<SimplexComplex>();

            foreach (var randomSimplex in randomSimplices)                
            {
                var simplexComplex = new SimplexComplex(randomSimplex);
                simplexComplex.Propagate();
                
                if(Integrate)
                    simplexComplex.Integrate(MeshSteps);

                if (ComputeAngles)
                    simplexComplex.ComputeAngles();
            }
                
            return (randomSimplices, hyperRandomComplexes);
        }
    }
}
