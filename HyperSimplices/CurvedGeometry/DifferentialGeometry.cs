using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.CurvedGeometry
{
    public delegate Vector<double> Parametrization(Vector<double> x);
    public delegate Matrix<double> PushForward(Vector<double> x);
    public delegate Matrix<double> MetricTensor(Vector<double> x);

    public class RiemannianSpace
    {
        public int Dim { get; set; }
        public MetricTensor Metric { get; set; }

        public RiemannianSpace(int dim, MetricTensor metric)
        {
            Dim = dim;
            Metric = metric;
        }

        public MetricTensor PullBack(Parametrization f, PushForward df)
        {
            return x =>
            {
                return df(x).Transpose() * Metric(f(x)) * df(x);
            };
        }

        private double G(Vector<double> x, Vector<double> v, Vector<double> w)
        {
            return v.DotProduct(Metric(x) * w);
        }

        public double Angle(Vector<double> x, Vector<double> v, Vector<double> w)
        {
            var g_vw = G(x, v, w);
            var g_vv = G(x, v, v);
            var g_ww = G(x, w, w);

            return g_vw / (Math.Sqrt(g_vv * g_ww));
        }
    }

    public class LocalTrivialization : RiemannianSpace
    {
        public LocalTrivialization(Parametrization diffeo, PushForward ddiffeo, RiemannianSpace chartDomain) :
            base(chartDomain.Dim, null)
        {
            Metric = PullBack(diffeo, ddiffeo);
        }

        public double GramDeterminant(Vector<double> x)
        {
            return Math.Sqrt(Metric(x).Determinant());
        }
    }
}
