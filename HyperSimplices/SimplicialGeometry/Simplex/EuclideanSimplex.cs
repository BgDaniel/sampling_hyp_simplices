using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry.Simplex
{
    public class EuclideanSimplex : GenericSimplex<Vector<double>>, IEquatable<EuclideanSimplex>
    {
        public EuclideanSimplex(Dictionary<int, Vector<double>> edges) : base(edges)
        {
            BasePoint = edges.Values.First();
            Dim = edges.Count - 1;
            DimAmbiantSpace = BasePoint.Count;
            DirectionalVectors = Enumerable.Range(1, Dim + 1).Select(i => Edges[i] + BasePoint).ToArray();

            Chart = x => Parametrization(x.AsArray());
        }

        protected Vector<double> Parametrization(double[] t)
        {
            var ret = Vector<double>.Build.Dense(DimAmbiantSpace);

            for (int ell = 0; ell < Dim; ell++)
                ret += t[ell] * DirectionalVectors[ell];

            return ret;
        }
        public List<Vector<double>> CreateMesh(int meshSteps)
        {
            var mesh = new List<Vector<double>>();
            var delta = 1.0 / meshSteps;
            var meshPointsInCube = ArrayHelpers.CreateSimplexMesh(delta, Dim);

            foreach (var meshPoint in meshPointsInCube)
            {
                if (meshPoint.Sum() <= 1.0)
                    mesh.Add(Vector<double>.Build.DenseOfArray(meshPoint));
            }

            return mesh;
        }

        public override void Negate()
        {
            if (Dim == 0)
                return;
            else
            {
                var edge0 = Edges[0].Clone();
                Edges[0] = Edges[1].Clone();
                Edges[1] = edge0;
                BasePoint = Edges.Values.First();
                DirectionalVectors = Enumerable.Range(1, Dim + 1).Select(i => Edges[i] + BasePoint).ToArray();
            }
        }

        bool IEquatable<EuclideanSimplex>.Equals(EuclideanSimplex other)
        {
            var edgeIndices = new HashSet<int>(other.Edges.Keys);
            return edgeIndices.SetEquals(new HashSet<int>(Edges.Keys));
        }

        public override int GetHashCode()
        {
            return Edges.Keys.GetHashCode();
        }

        public static EuclideanSimplex RandomSample(int dim, double maxNorm = 1.0)
        {
            return RandomSamples(1, dim, maxNorm)[0];
        }

        public static List<EuclideanSimplex> RandomSamples(int nbSamples, int dim, double maxNorm = 1.0) 
        {
            var ret = new List<EuclideanSimplex>();
            var rndVectors = ArrayHelpers.RandomVectors(nbSamples * dim, dim, maxNorm);

            for (int i = 0; i < nbSamples; i++)
            {
                var edges = new Dictionary<int, Vector<double>>();

                for (int j = 0; j < dim; j++)
                    edges[1 + j] = rndVectors[i * dim + j];
                
                ret.Add(new EuclideanSimplex(edges));
            }
                
            return ret; 
        }
    }
}
