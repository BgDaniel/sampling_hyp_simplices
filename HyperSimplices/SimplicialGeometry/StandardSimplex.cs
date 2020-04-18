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
        private static Dictionary<int, Vector<double>> GetStandardEdges(int dim)
        {
            var ret = new Dictionary<int, Vector<double>>();

            for(int i = 1; i <= dim; i++)
            {
                var arr = new double[dim];
                arr[i - 1] = 1.0;
                ret[i] = Vector<double>.Build.DenseOfArray(arr);
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
