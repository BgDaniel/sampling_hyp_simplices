using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices
{
    public class HyperbolicSimplex : Simplex
    {
        public ChartOverSimplex Trivialization { get; private set; }

        public HyperbolicSimplex(List<Vector<double>> edges) : base(edges)
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
    }
}
