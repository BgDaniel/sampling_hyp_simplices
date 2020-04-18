using HyperSimplices.CurvedGeometry;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices
{
    public abstract class Simplex<T>
    {
        public List<T> Edges { get; set; }
        public int Dim { get; set; }
        public int DimAmbiantSpace { get; set; }
        public abstract void Negate();
        public T BasePoint { get; set; }
        public T[] DirectionalVectors { get; set; }
        public Diffeomorphism Chart { get; set; }
        public abstract List<T> CreateMesh(int meshSteps);
        public List<Simplex<T>> Faces { get; }
    }

    public class Simplex : Simplex<Vector<double>>
    {
        public List<Simplex> Faces
        {
            get
            {
                var ret = new List<Simplex>();
                var counter = 0;

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

        protected Simplex RemoveEdge(int index)
        {
            var newEdges = Enumerable.Range(0, Edges.Count).Select(i => Edges[i].Clone()).ToList();
            newEdges.RemoveAt(index);
            return new Simplex(newEdges);
        }

        public Simplex(List<Vector<double>> edges)
        {
            BasePoint = edges.First();
            Dim = edges.Count - 1;           
            Edges = edges;
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
        public override List<Vector<double>> CreateMesh(int meshSteps)
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
                BasePoint = Edges.First();
                DirectionalVectors = Enumerable.Range(1, Dim + 1).Select(i => Edges[i] + BasePoint).ToArray();
            }
        }

        Simplex Clone()
        {
            return new Simplex(Enumerable.Range(0, Edges.Count).Select(i => Edges[i].Clone()).ToList());
        }
    }
}
