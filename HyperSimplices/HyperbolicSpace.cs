using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices
{
    public delegate double Metric(Vector<double> x, Vector<double> v, Vector<double> w);

    public delegate Vector<double> Diffeomorphism(Vector<double> x);

    public class RiemannianSpace
    {
        public int Dim { get; set; }
        public Metric Metric { get; set; }
        public Metric PullBack(Diffeomorphism f, Matrix<double> df)
        {
            return (x, v, w) =>
            {
                return Metric(f(x), df * v, df * w);
            };
        }
    }

    public class HyperbolicSpace : RiemannianSpace
    {        
        public double Curvature { get; private set; }
        
        public HyperbolicSpace(int dim, double curvature = 1.0)
        {
            Dim = dim;
            Curvature = curvature;
            Metric = (x, v, w) =>
            {

                return .0;
            };
        }
    }
}
