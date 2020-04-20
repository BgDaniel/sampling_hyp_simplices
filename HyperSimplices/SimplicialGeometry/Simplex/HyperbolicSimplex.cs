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

        public override void Integrate(int meshSteps, bool calcAnalytical = false)
        {
            if (calcAnalytical && (Dim == 1 || Dim == 2))
            {
                if (Dim == 1 && calcAnalytical)
                    Volume = BeltramiKlein2Dim.Distance(this);

                if (Dim == 2 && calcAnalytical)
                    Volume = BeltramiKlein2Dim.Surface(this);
            }
            else
                base.Integrate(meshSteps);                       
        }
    }
}
