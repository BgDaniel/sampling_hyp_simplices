using HyperSimplices.CurvedGeometry;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry.Simplex
{
    public class EuclideanSimplex : GenericSimplex<Vector<double>>, IEquatable<EuclideanSimplex>, ICloneable
    {
        public Vector<double>[] DirectionalVectors { get; set; }
        public int DimAmbiantSpace { get; set; }

        public EuclideanSimplex(Dictionary<int, Vector<double>> edges) : base(edges)
        {
            DirectionalVectors = Enumerable.Range(2, Dim).Select(i => Edges[i] - BasePoint).ToArray();
            DimAmbiantSpace = BasePoint.Count();
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

        public Parametrization Chart { get; set; }

        public EuclideanSimplex RemoveEdge(int index)
        {
            var newEdges = new Dictionary<int, Vector<double>>();

            foreach (var edge in Edges)
            {
                if (edge.Key != index)
                    newEdges[edge.Key] = (Vector<double>)edge.Value.Clone();
            }

            return new EuclideanSimplex(newEdges);
        }

        public List<EuclideanSimplex> Faces
        {
            get
            {
                var ret = new List<EuclideanSimplex>();
                var counter = 1;

                foreach (var edge in Edges)
                {
                    var face = RemoveEdge(counter);

                    if (counter % 2 != 0)
                        face.Negate();

                    ret.Add(face);
                }

                return ret;
            }
        }

        public EuclideanSimplex GetOppositeFace(int index)
        {
            return RemoveEdge(index);
        }

        public List<EuclideanSimplex> GetAdjacentFaces(int index)
        {
            var adjacentFaces = new List<EuclideanSimplex>();

            for (int ell = 0; ell < Dim + 1; ell++)
            {
                if (ell != index)
                    adjacentFaces.Add(RemoveEdge(ell));
            }

            return adjacentFaces;
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
            var rndVectors = ArrayHelpers.RandomVectors(nbSamples * (dim + 1), dim, maxNorm);

            for (int i = 0; i < nbSamples; i++)
            {
                var edges = new Dictionary<int, Vector<double>>();

                for (int j = 0; j <= dim; j++)
                    edges[1 + j] = rndVectors[i * dim + j];
                
                ret.Add(new EuclideanSimplex(edges));
            }
                
            return ret; 
        }

        object ICloneable.Clone()
        {
            var edgesCloned = new Dictionary<int, Vector<double>>();

            foreach (var edge in Edges)
                edgesCloned[edge.Key] = (Vector<double>)edge.Value.Clone();

            return new GenericSimplex<Vector<double>>(edgesCloned);
        }
    }
}
