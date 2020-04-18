using HyperSimplices.CurvedGeometry;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices
{
    public class BeltramiKlein : RiemannianSpace
    {
        public double Curvature { get; private set; }

        public BeltramiKlein(int dim, double curvature = 1.0) : base(dim, x =>
            {
                var norm_x = x.L2Norm();
                var y = 1.0 - norm_x * norm_x;
                var id = Matrix<double>.Build.DenseIdentity(dim);
                var X = Matrix<double>.Build.DenseOfColumnVectors(new List<Vector<double>>() { x });
                var XT = X.Transpose();
                
                return id / y + X * XT / (y * y);
            })
        {
            Curvature = curvature;
        }
    }

    public class ChartOverSimplex : LocalTrivialization
    {   
        public ChartOverSimplex(Simplex simplex) : base(simplex.Chart, 
            x => Matrix<double>.Build.DenseOfColumnVectors(simplex.DirectionalVectors), 
            new BeltramiKlein(simplex.Dim))
        {
        }
    }
}
