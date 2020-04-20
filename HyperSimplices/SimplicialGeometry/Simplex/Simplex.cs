using HyperSimplices.CurvedGeometry;
using HyperSimplices.CurvedGeometry.BeltramiKleinModel;
using HyperSimplices.Utils;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry.Simplex
{
    public class Simplex : GenericSimplex<Vector<double>>, IEquatable<Simplex>
    {
        public LocalTrivialization Trivialization { get; private set; }
        public Vector<double>[] DirectionalVectors { get; set; }              
        public List<int> Indices => Edges.Select(edge => edge.Item1).ToList();
        public double Volume { get; set; }

        public Simplex(Tuple<int, Vector<double>>[] edges, RiemannianSpace ambiantSpace) : base(edges)
        {
            var directionalVectors = new List<Vector<double>>();

            foreach(var item in edges.Skip(1))
                directionalVectors.Add(item.Item2 - BasePoint);

            DirectionalVectors = directionalVectors.ToArray();

            DimAmbiantSpace = BasePoint.Count();

            Parametrization chart = x => {
                var dir = Matrix<double>.Build.DenseOfColumnVectors(DirectionalVectors);
                return dir * x;
            };

            PushForward pushForward = x => {
                return Matrix<double>.Build.DenseOfColumnVectors(DirectionalVectors);
            };

            AmbiantSpace = ambiantSpace;
            Trivialization = new LocalTrivialization(chart, pushForward, ambiantSpace);
        }

        public Vector<double> GetEdgeByIndex(int index)
        {
            if (!Indices.Contains(index))
                return null;
            else
            {
                for(int ell = 0; ell < Edges.Length; ell++)
                {
                    if (Edges[ell].Item1 == index)
                        return Edges[ell].Item2;
                }

                return null;
            }
        }

        public virtual void Integrate(int meshSteps, bool calcAnalytical = false)
        {
            if (Dim == 0)
                return;

            var mesh = MeshFactory.Instance.GetMesh(Dim, meshSteps);
            Volume = .0;
            var dVol = Math.Pow(1.0 / meshSteps, Dim);

            foreach (var meshPoint in mesh)
                Volume += Trivialization.GramDeterminant(meshPoint) * dVol;
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

        public Simplex RemoveEdge(int index)
        {
            var newEdges = new List<Tuple<int, Vector<double>>>();
            
            for(int ell = 0; ell < Edges.Length; ell++)
            {
                if (ell != index)
                    newEdges.Add(new Tuple<int, Vector<double>>(Edges[ell].Item1, (Vector<double>)Edges[ell].Item2.Clone()));
            }

            return new Simplex(newEdges.ToArray(), AmbiantSpace);
        }

        public Simplex AddEdge(Tuple<int, Vector<double>> edge)
        {
            var newEdges = new List<Tuple<int, Vector<double>>>();

            for (int ell = 0; ell < Dim + 1; ell++)
                newEdges.Add(new Tuple<int, Vector<double>>(Edges[ell].Item1, Edges[ell].Item2.Clone()));

            newEdges.Add(edge);
            return new Simplex(newEdges.ToArray(), AmbiantSpace);
        }

        public List<Simplex> Faces
        {
            get
            {
                var ret = new List<Simplex>();
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

        public Simplex GetOppositeFace(int index)
        {
            return RemoveEdge(index);
        }

        public List<Simplex> GetAdjacentFaces(int index)
        {
            var adjacentFaces = new List<Simplex>();

            for (int ell = 0; ell < Dim + 1; ell++)
            {
                if (ell != index)
                    adjacentFaces.Add(RemoveEdge(ell));
            }

            return adjacentFaces;
        }

        public Simplex Negate()
        {
            if (Dim == 0)
                return (Simplex)Clone();
            else
            {
                var edgesCloned = new Tuple<int, Vector<double>>[Dim + 1];

                for (int ell = 0; ell < Dim + 1; ell++)
                    edgesCloned[ell] = new Tuple<int, Vector<double>>( Edges[ell].Item1, Edges[ell].Item2.Clone() );

                var firstIndex = edgesCloned[0].Item1;
                var firstEdge = edgesCloned[0].Item2.Clone();
                edgesCloned[0] = edgesCloned[1];
                edgesCloned[1] = new Tuple<int, Vector<double>>( firstIndex, firstEdge );
            
                return new Simplex(edgesCloned, AmbiantSpace);
            }
        }

        bool IEquatable<Simplex>.Equals(Simplex other)
        {
            var edgeIndices = new HashSet<int>(other.Indices);
            return edgeIndices.SetEquals(new HashSet<int>(Indices));
        }

        public override int GetHashCode()
        {
            return Indices.GetHashCode();
        }

        public static Simplex RandomSample(int nbSamples, int dim, RiemannianSpace ambiantSpace, bool zeroAmongEdges = true, double maxNorm = double.NaN)
        {
            return RandomSamples(1, dim, ambiantSpace, zeroAmongEdges, maxNorm)[0];
        }

        public static List<Simplex> RandomSamples(int nbSamples, int dim, RiemannianSpace ambiantSpace, bool zeroAmongEdges = true, double maxNorm = double.NaN) 
        {
            var ret = new List<Simplex>();
            var hyperbolicNorm = Math.Tanh(maxNorm);

            var nbEdgesPerSimplex = zeroAmongEdges ? dim : dim + 1; ;            
            var rndVectors = VariousHelpers.RandomVectors(nbSamples * nbEdgesPerSimplex, dim, hyperbolicNorm);

            for (int i = 0; i < nbSamples; i++)
            {
                var edges = new Tuple<int, Vector<double>>[dim + 1];

                if(zeroAmongEdges)
                {
                    edges[0] = new Tuple<int, Vector<double>>(1, Vector<double>.Build.Dense(new double[dim]));
                    for (int j = 1; j <= dim; j++)
                        edges[j] = new Tuple<int, Vector<double>>(j + 1, rndVectors[i * (dim - 1) + j]);
                }
                else
                {
                    for (int j = 0; j <= dim; j++)
                        edges[j] = new Tuple<int, Vector<double>>(j + 1, rndVectors[i * dim + j]);
                }
                                
                ret.Add(new Simplex(edges, ambiantSpace));
            }
                
            return ret; 
        }

        public Simplex Clone()
        {
            var edgesCloned = new Tuple<int, Vector<double>>[Dim + 1];
            
            for(int ell = 0; ell < Dim + 1; ell++)
                edgesCloned[ell] = new Tuple<int, Vector<double>>( Edges[ell].Item1, Edges[ell].Item2.Clone() );
            
            return new Simplex(edgesCloned, AmbiantSpace);
        }
    }
}
