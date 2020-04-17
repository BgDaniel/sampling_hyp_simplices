using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices
{
    public interface INegatable
    {
        void Negate();
    }

    public abstract class ChainComplexElement
    {
        public List<INegatable> Rand(List<INegatable> chain)
        {
            var ret = new List<INegatable>();
            var counter = 0;

            foreach (var chainElement in chain)
            {
                if (counter % 2 != 0)
                    chainElement.Negate();
                ret.Add(chainElement);
            }

            return ret;
        }
    }
}
