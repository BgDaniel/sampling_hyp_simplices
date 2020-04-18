using HyperSimplices.SimplicialGeometry.Simplex;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry
{
    public struct SimplexKey
    {
        public int Dim { get; }
        public int MeshSteps { get; }

        public SimplexKey(int dim, int meshSteps)
        {
            Dim = dim;
            MeshSteps = meshSteps;
        }
    }

    public sealed class StandardSimplex : EuclideanSimplex
    {
        private static Tuple<int, Vector<double>>[] GetStandardEdges(int dim)
        {
            var ret = new Tuple<int, Vector<double>>[dim + 1];

            for(int ell = 0; ell <= dim + 1; ell++)
            {
                var arr = new double[dim];
                arr[ell] = 1.0;
                ret[ell] = new Tuple<int, Vector<double>>( ell + 1, Vector<double>.Build.DenseOfArray(arr) );
            }

            return ret;
        }

        public double Delta { get; private set; }
        public List<double[]> Mesh { get; private set; }

        public StandardSimplex(int dim) : base(GetStandardEdges(dim))
        {
        }

        public void CreateMesh(int meshSteps)
        {
            Delta = 1.0 / meshSteps;

        }

        public StandardSimplex(SimplexKey simplexKey) : base(GetStandardEdges(simplexKey.Dim))
        {
            CreateMesh(simplexKey.MeshSteps);
        }      
    }
}
