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
        public RiemannianSpace AmbiantSpace { get; set; }

        public HyperbolicSimplex(List<Vector<double>> edges) : base(edges)
        {
            AmbiantSpace = new HyperbolicSpace(DimAmbiantSpace);
        }
        
        public double Volume()
        {
            //var metricInChart =

            return .0;
        }
    }
}
