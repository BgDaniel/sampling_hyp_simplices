using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices
{
    public abstract class Simplex<T> : ChainComplexElement, INegatable
    {
        public List<T> Edges { get; set; }
        public int Dim { get; set; }
        public int DimAmbiantSpace { get; set; }
        public abstract void Negate();
        public T BasePoint { get; set; }
        public T[] DirectionalVectors { get; set; }
        public abstract T Parametrization(double[] t);
    }
}
