using HypSimplexSampleFactory.CommandLineParser;
using System;
using System.Collections.Generic;
using System.Text;

namespace HypSimplexSampleFactory.ExampleConfigs
{
    static class ExampleConfigurations
    {
        public static Options Hyper3_1000_5 = new Options
        {
            NumberSamples = 1000,
            Dimension = 3,
            MaxNorm = 5.0
        };
    }
}
