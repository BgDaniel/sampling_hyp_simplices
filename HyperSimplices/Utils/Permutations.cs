using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.Utils
{
    public class Permutations
    {
        private List<int[]> _elements;
        public int Dim { get; private set; }

        public List<int[]> Elements => _elements;

        public Permutations(int dim)
        {
            Dim = dim;
        }

        public static int[] Apply(int[] element, int[] x)
        {
            return x.Select(i => x[element[i]]).ToArray();
        }

        public List<int[]> Orbit(int[] x)
        {
            var ret = new List<int[]>();

            foreach (var element in _elements)
                ret.Add(Permutations.Apply(element, x));

            return ret;
        }
    }
}
