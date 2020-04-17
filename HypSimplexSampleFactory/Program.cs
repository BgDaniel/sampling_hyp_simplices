using HyperSimplices;
using System;

namespace HypSimplexSampleFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            var standardSimplex = StandardSimplexFactory.Instance.GetStandardSimplex(3, 1000);
            standardSimplex = StandardSimplexFactory.Instance.GetStandardSimplex(3, 1000);
        }
    }
}
