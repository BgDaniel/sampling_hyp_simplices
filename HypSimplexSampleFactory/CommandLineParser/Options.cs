using CommandLine;
using HyperSimplices.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace HypSimplexSampleFactory.CommandLineParser
{
    public class Options
    {
        [Option('n', "number", Required = true, HelpText = "Number of random samples. Defaults to 1000.")]
        public int NumberSamples { get; set; }

        [Option('d', "dimension", Required = true, HelpText = "Dimension of generated random simplices.")]
        public int Dimension { get; set; }

        [Option('b', "maximal norm", Required = false, Default = 1.0, HelpText = "Hyperbolic norm of ball in which all random samples are contained.")]
        public double MaxNorm { get; set; }

        [Option('i', "integrate", Required = false, Default = false, HelpText = "Flag whether determination of volumina should be performed or not.")]
        public bool Integrate { get; set; }

        [Option('m', "discretization steps for integration", Required = false, Default = 1000, HelpText = "Number of discretization steps used for each dimension while integrating.")]
        public int MeshSteps { get; set; }

        [Option('z', "zero among edges", Required = false, Default = true, HelpText = "Simulate random simplices with zero vector among edges.")]
        public bool ZeroAmongEdges { get; set; }

        [Option('a', "compute angles", Required = false, Default = false, HelpText = "Flag whether determination of angles should be performed or not.")]
        public bool ComputeAngles { get; set; }

        [Option('c', "compute hyperbolic length analytical", Required = false, Default = false, HelpText = "Flag whether computation of hyperbolic length will be performed analytical or via Monte Carlo integration.")]
        public bool ComputeAnalytical { get; set; }

        [Option('t', "type of curvature", Required = false, Default = CurvatureType.NEGATIVE, HelpText = "Type of ambiant space curvature.")]
        public CurvatureType CurvatureType { get; set; }
    }
}
