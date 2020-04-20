using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry.Simplex
{
    public class EuclideanSimplex : Simplex
    {
        public EuclideanSimplex(Tuple<int, Vector<double>>[] edges) : base(edges, new EuclideanGeometry(edges[0].Item2.Count()))
        {
        }
    }
}
