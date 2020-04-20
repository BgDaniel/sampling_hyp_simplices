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

    public abstract class RiemannianSpace
    {
        public int Dim { get; set; }
        public MetricTensor Metric { get; set; }

        public RiemannianSpace(int dim, MetricTensor metric)
        {
            Dim = dim;
            Metric = metric;
        }

        public MetricTensor PullBack(Parametrization chart, PushForward pushForward)
        {
            return x =>
            {
                var _pushForward = pushForward(x);
                var _pushForwardT = pushForward(x).Transpose();

                return _pushForwardT * Metric(chart(x)) * _pushForward;
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
        public MetricTensor LocalMetric { get; set; }

        public LocalTrivialization(Parametrization chart, PushForward pushForward, RiemannianSpace chartDomain) :
            base(chartDomain.Dim, chartDomain.Metric)
        {
            LocalMetric = PullBack(chart, pushForward);
        }

        public double GramDeterminant(Vector<double> x)
        {
            var metricTensor = LocalMetric(x);
            var determinant = metricTensor.Determinant();

            return Math.Sqrt(determinant);
        }
    }
}
