using HyperSimplices.CurvedGeometry;
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
        public Vector<double>[] DirectionalVectors { get; set; }
        public int DimAmbiantSpace { get; set; }

        public List<int> Indices => Edges.Select(edge => edge.Item1).ToList();

        public EuclideanSimplex(Tuple<int, Vector<double>>[] edges) : base(edges)
        {
            DirectionalVectors = new Vector<double>[Dim];
            var ell = 0;

            foreach(var item in edges.Skip(1))
            {
                DirectionalVectors[ell] = item.Item2 - BasePoint;
                ell++;
            }                            

            DimAmbiantSpace = BasePoint.Count();
            Chart = x => Parametrization(x.AsArray());
        }

        public List<Tuple<int, Vector<double>>> ComplementaryEdges(Tuple<int, Vector<double>>[] edges)
        {
            var ret = new List<Tuple<int, Vector<double>>>();
            var currentIndices = edges.Select(edge => edge.Item1).ToList();

            for (int ell = 0; ell < Edges.Length; ell++)
            {
                if (!currentIndices.Contains(Edges[ell].Item1))
                    ret.Add(new Tuple<int, Vector<double>>( Edges[ell].Item1, Edges[ell].Item2.Clone() ));
            }

            return ret;
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
            var newEdges = new List<Tuple<int, Vector<double>>>();
            
            for(int ell = 0; ell < Edges.Length; ell++)
            {
                if (ell != index)
                    newEdges.Add(new Tuple<int, Vector<double>>(Edges[ell].Item1, (Vector<double>)Edges[ell].Item2.Clone()));
            }

            return new EuclideanSimplex(newEdges.ToArray());
        }

        public EuclideanSimplex AddEdge(Tuple<int, Vector<double>> edge)
        {
            var newEdges = new List<Tuple<int, Vector<double>>>();

            for (int ell = 0; ell < Dim + 1; ell++)
                newEdges.Add(new Tuple<int, Vector<double>>(Edges[ell].Item1, Edges[ell].Item2.Clone()));

            newEdges.Add(edge);
            return new EuclideanSimplex(newEdges.ToArray());
        }

        public List<EuclideanSimplex> Faces
        {
            get
            {
                var ret = new List<EuclideanSimplex>();
                var ell = 0;

                foreach (var edge in Edges)
                {
                    var face = RemoveEdge(ell);

                    if (ell % 2 != 0)
                        face = face.Negate();

                    ret.Add(face);
                    ell++;
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

        public EuclideanSimplex Negate()
        {
            if (Dim == 0)
                return (EuclideanSimplex)Clone();
            else
            {
                var edgesCloned = new Tuple<int, Vector<double>>[Dim + 1];

                for (int ell = 0; ell < Dim + 1; ell++)
                    edgesCloned[ell] = new Tuple<int, Vector<double>>( Edges[ell].Item1, Edges[ell].Item2.Clone() );

                var firstIndex = edgesCloned[0].Item1;
                var firstEdge = edgesCloned[0].Item2.Clone();
                edgesCloned[0] = edgesCloned[1];
                edgesCloned[1] = new Tuple<int, Vector<double>>( firstIndex, firstEdge );
            
                return new EuclideanSimplex(edgesCloned);
            }
        }

        bool IEquatable<EuclideanSimplex>.Equals(EuclideanSimplex other)
        {
            var edgeIndices = new HashSet<int>(other.Indices);
            return edgeIndices.SetEquals(new HashSet<int>(Indices));
        }

        public override int GetHashCode()
        {
            return Indices.GetHashCode();
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
                var edges = new Tuple<int, Vector<double>>[dim + 1];

                for (int j = 0; j <= dim; j++)
                    edges[j] = new Tuple<int, Vector<double>>(j + 1, rndVectors[i * dim + j]);
                
                ret.Add(new EuclideanSimplex(edges));
            }
                
            return ret; 
        }

        public EuclideanSimplex Clone()
        {
            var edgesCloned = new Tuple<int, Vector<double>>[Dim + 1];
            
            for(int ell = 0; ell < Dim + 1; ell++)
                edgesCloned[ell] = new Tuple<int, Vector<double>>( Edges[ell].Item1, Edges[ell].Item2.Clone() );
            
            return new EuclideanSimplex(edgesCloned);
        }
    }
}
