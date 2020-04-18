using HyperSimplices.SimplicialGeometry.Simplex;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry.Simplex
{
    public class HyperbolicSimplex : EuclideanSimplex
    {
        public ChartOverSimplex Trivialization { get; private set; }

        public HyperbolicSimplex(Tuple<int, Vector<double>>[] edges) : base(edges)
        {
            Trivialization = new ChartOverSimplex(this);
        }
        
        public double CalculateVolume(int meshSteps)
        {
            var mesh = CreateMesh(meshSteps);
            var volume = .0;
            var dVol = Math.Pow(1.0 / meshSteps, Dim);

            foreach(var meshPoint in mesh)
                volume += Trivialization.GramDeterminant(meshPoint) * dVol;

            return volume;
        }

        public static new List<HyperbolicSimplex> RandomSamples(int nbSamples, int dim, double maxNorm = 1.0)
        {
            return EuclideanSimplex.RandomSamples(nbSamples, dim, Math.Tanh(maxNorm))
                .Select(euclideanSimplex => new HyperbolicSimplex(euclideanSimplex.Edges)).ToList();
        }
    }
}
