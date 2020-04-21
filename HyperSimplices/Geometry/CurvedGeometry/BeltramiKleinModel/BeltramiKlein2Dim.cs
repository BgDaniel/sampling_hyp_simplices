using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyperSimplices.SimplicialGeometry.Simplex;

namespace HyperSimplices.CurvedGeometry.BeltramiKleinModel
{
    delegate Vector<double> Line(double t);

    public class BeltramiKlein2Dim : BeltramiKlein
    {
        public BeltramiKlein2Dim(double curvature = 1) : base(2, curvature) { }

        public static double Distance(Simplex line)
        {
            var p = line.Edges[0].Item2;
            var q = line.Edges[1].Item2;
            Line line_pq = t => p + (q - p) * t;
            Func<double, double> f = t => line_pq(t).L2Norm() - 1.0;

            var t_a = FindRoots.OfFunction(f, - 3.0 / (p - q).L2Norm(), .0);
            var t_b = FindRoots.OfFunction(f, 1.0, 3.0 / (p - q).L2Norm());

            var a = line_pq(t_a);
            var b = line_pq(t_b);

            var aq = (a - q).L2Norm();
            var ap = (a - p).L2Norm();
            var pb = (p - b).L2Norm();
            var qb = (q - b).L2Norm();

            return .5 * Math.Log((aq * pb) / (ap * qb));
        }

        public static double Surface(Simplex surface)
        {
            var a = surface.Edges[0].Item2;
            var b = surface.Edges[1].Item2;
            var c = surface.Edges[2].Item2;
            var alpha = VariousHelpers.Angle(b, c);
            var beta = VariousHelpers.Angle(a, c);
            var gamma = VariousHelpers.Angle(a, b);

            return Math.PI - alpha - beta - gamma;
        }
    }
}
