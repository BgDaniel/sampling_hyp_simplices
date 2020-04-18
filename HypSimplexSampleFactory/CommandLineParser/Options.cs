using CommandLine;
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

        [Option('m', "maximal norm", Required = false, Default = 1.0, HelpText = "Hyperbolic norm of ball in which all random samples are contained.")]
        public double MaxNorm { get; set; }

    }
}
