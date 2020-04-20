using HypSimplexSampleFactory.CommandLineParser;
using System;
using System.Collections.Generic;
using System.Text;

namespace HypSimplexSampleFactory.ExampleConfigs
{
    static class ExampleConfigurations
    {
        public static Options Hyper2_1000_500_2 = new Options
        {
            NumberSamples = 1000,
            Dimension = 2,
            Integrate = true,
            MeshSteps = 500,
            MaxNorm = 2.0
        };

        public static Options Hyper3_1000_5 = new Options
        {
            NumberSamples = 1000,
            Dimension = 3,
            MaxNorm = 5.0
        };

        public static Options Hyper5_1000_5 = new Options
        {
            NumberSamples = 1000,
            Dimension = 5,
            MaxNorm = 5.0
        };

        public static Options Hyper7_1000_5 = new Options
        {
            NumberSamples = 1000,
            Dimension = 7,
            MaxNorm = 5.0
        };

        public static Options Hyper_4= new Options
        {
            NumberSamples = 500,
            Dimension = 2,
            Integrate = true,
            MeshSteps = 500,
            MaxNorm = 2.0,
            ComputeAngles = true,
            ZeroAmongEdges = true,
            ComputeAnalytical = true
        };
    }
}
