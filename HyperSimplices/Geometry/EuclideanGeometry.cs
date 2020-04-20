using HyperSimplices.CurvedGeometry;
using HyperSimplices.SimplicialGeometry.Simplex;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices
{
    public class EuclideanGeometry : RiemannianSpace
    {
        public double Curvature { get; private set; }

        public EuclideanGeometry(int dim, double curvature = 1.0) : base(dim, x =>
            {
                return Matrix<double>.Build.DenseIdentity(dim);
            })
        {
            Curvature = curvature;
        }

        public static double Volume(Simplex simplex)
        {
            var A = Matrix<double>.Build.DenseOfColumnVectors(simplex.DirectionalVectors);
            var AT_A = A.Transpose() * A;
            return Math.Sqrt(AT_A.Determinant());
        }
    }
}
