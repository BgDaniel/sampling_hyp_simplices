using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices
{
    public class Simplex : Simplex<Vector<double>>
    {
        public Simplex(List<Vector<double>> edges)
        {
            BasePoint = edges.First();
            Dim = edges.Count - 1;           
            Edges = edges;
            DimAmbiantSpace = BasePoint.Count;
            DirectionalVectors = Enumerable.Range(1, Dim + 1).Select(i => Edges[i] + BasePoint).ToArray();
                        
            Chart = x => Parametrization(x.AsArray());
        }

        protected Vector<double> Parametrization(double[] t)
        {
            var ret = Vector<double>.Build.Dense(DimAmbiantSpace);
            
            for (int ell = 0; ell < Dim; ell++)
                ret += t[ell] * DirectionalVectors[ell];

            return ret;
        }

        public override void Negate()
        {
            if (Dim == 0)
                return;
            else
            {
                var edge0 = Edges[0].Clone();
                Edges[0] = Edges[1].Clone();
                Edges[1] = edge0;
                BasePoint = Edges.First();
                DirectionalVectors = Enumerable.Range(1, Dim + 1).Select(i => Edges[i] + BasePoint).ToArray();
            }
        }
    }
}
