using HyperSimplices.SimplicialGeometry.Simplex;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry.SimplexComplex
{
    public class EuclideanSimplexPair : GenericSimplexPlair<Vector<double>>
    {
        public double Angle()
        {
            return .0;
        }

        public EuclideanSimplexPair(EuclideanSimplex commonBase, EuclideanSimplex simplex1, EuclideanSimplex simplex2) : base(commonBase, simplex1, simplex2)
        {
        }
    }

    public class HyperbolicSimplexComplex<T> : GenericSimplexComplex<Vector<double>>
    {
        public HyperbolicSimplexComplex(HyperbolicSimplex simplex0) : base(simplex0)
        {
        }
    }
}
