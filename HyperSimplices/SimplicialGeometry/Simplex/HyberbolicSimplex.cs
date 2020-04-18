using HyperSimplices.CurvedGeometry;
using HyperSimplices.Utils;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry.Simplex
{
    public class HyberbolicSimplex : GenericSimplex<Vector<double>>, IEquatable<HyberbolicSimplex>
    {
        public LocalTrivialization Trivialization { get; private set; }
        public Vector<double>[] DirectionalVectors { get; set; }
        public int DimAmbiantSpace { get; set; }        
        public List<int> Indices => Edges.Select(edge => edge.Item1).ToList();
        public double Volume { get; private set; }

        public HyberbolicSimplex(Tuple<int, Vector<double>>[] edges) : base(edges)
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

            Trivialization = new LocalTrivialization(chart, pushForward, new BeltramiKlein(DimAmbiantSpace));
        }

        public void Integrate(int meshSteps)
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

        public HyberbolicSimplex RemoveEdge(int index)
        {
            var newEdges = new List<Tuple<int, Vector<double>>>();
            
            for(int ell = 0; ell < Edges.Length; ell++)
            {
                if (ell != index)
                    newEdges.Add(new Tuple<int, Vector<double>>(Edges[ell].Item1, (Vector<double>)Edges[ell].Item2.Clone()));
            }

            return new HyberbolicSimplex(newEdges.ToArray());
        }

        public HyberbolicSimplex AddEdge(Tuple<int, Vector<double>> edge)
        {
            var newEdges = new List<Tuple<int, Vector<double>>>();

            for (int ell = 0; ell < Dim + 1; ell++)
                newEdges.Add(new Tuple<int, Vector<double>>(Edges[ell].Item1, Edges[ell].Item2.Clone()));

            newEdges.Add(edge);
            return new HyberbolicSimplex(newEdges.ToArray());
        }

        public List<HyberbolicSimplex> Faces
        {
            get
            {
                var ret = new List<HyberbolicSimplex>();
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

        public HyberbolicSimplex GetOppositeFace(int index)
        {
            return RemoveEdge(index);
        }

        public List<HyberbolicSimplex> GetAdjacentFaces(int index)
        {
            var adjacentFaces = new List<HyberbolicSimplex>();

            for (int ell = 0; ell < Dim + 1; ell++)
            {
                if (ell != index)
                    adjacentFaces.Add(RemoveEdge(ell));
            }

            return adjacentFaces;
        }

        public HyberbolicSimplex Negate()
        {
            if (Dim == 0)
                return (HyberbolicSimplex)Clone();
            else
            {
                var edgesCloned = new Tuple<int, Vector<double>>[Dim + 1];

                for (int ell = 0; ell < Dim + 1; ell++)
                    edgesCloned[ell] = new Tuple<int, Vector<double>>( Edges[ell].Item1, Edges[ell].Item2.Clone() );

                var firstIndex = edgesCloned[0].Item1;
                var firstEdge = edgesCloned[0].Item2.Clone();
                edgesCloned[0] = edgesCloned[1];
                edgesCloned[1] = new Tuple<int, Vector<double>>( firstIndex, firstEdge );
            
                return new HyberbolicSimplex(edgesCloned);
            }
        }

        bool IEquatable<HyberbolicSimplex>.Equals(HyberbolicSimplex other)
        {
            var edgeIndices = new HashSet<int>(other.Indices);
            return edgeIndices.SetEquals(new HashSet<int>(Indices));
        }

        public override int GetHashCode()
        {
            return Indices.GetHashCode();
        }

        public static HyberbolicSimplex RandomSample(int dim, double maxNorm = 1.0)
        {
            return RandomSamples(1, dim, maxNorm)[0];
        }

        public static List<HyberbolicSimplex> RandomSamples(int nbSamples, int dim, double maxNorm = double.NaN) 
        {
            var ret = new List<HyberbolicSimplex>();
            var hyperbolicNorm = Math.Tanh(maxNorm);
            var rndVectors = ArrayHelpers.RandomVectors(nbSamples * (dim + 1), dim, hyperbolicNorm);

            for (int i = 0; i < nbSamples; i++)
            {
                var edges = new Tuple<int, Vector<double>>[dim + 1];

                for (int j = 0; j <= dim; j++)
                    edges[j] = new Tuple<int, Vector<double>>(j + 1, rndVectors[i * dim + j]);
                
                ret.Add(new HyberbolicSimplex(edges));
            }
                
            return ret; 
        }

        public HyberbolicSimplex Clone()
        {
            var edgesCloned = new Tuple<int, Vector<double>>[Dim + 1];
            
            for(int ell = 0; ell < Dim + 1; ell++)
                edgesCloned[ell] = new Tuple<int, Vector<double>>( Edges[ell].Item1, Edges[ell].Item2.Clone() );
            
            return new HyberbolicSimplex(edgesCloned);
        }
    }
}
