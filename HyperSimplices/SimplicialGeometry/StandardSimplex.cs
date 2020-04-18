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
        private static List<Vector<double>> GetStandardEdges(int dim)
        {
            return Enumerable.Range(0, dim).Select(i =>
            {
                var array = new double[dim];
                array[i] = 1.0;
                
                return Vector<double>.Build.Dense(array);
            }).ToList();
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
