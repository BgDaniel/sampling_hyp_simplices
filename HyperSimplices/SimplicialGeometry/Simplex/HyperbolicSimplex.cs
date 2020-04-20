using HyperSimplices.CurvedGeometry;
using HyperSimplices.CurvedGeometry.BeltramiKleinModel;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry.Simplex
{
    public class HyperbolicSimplex : Simplex
    {
        public HyperbolicSimplex(Tuple<int, Vector<double>>[] edges) : base(edges, new BeltramiKlein(edges[0].Item2.Count()))
        {
        }
    }
}
