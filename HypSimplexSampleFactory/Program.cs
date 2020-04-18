using CommandLine;
using HyperSimplices;
using HypSimplexSampleFactory.CommandLineParser;
using System;

namespace HypSimplexSampleFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            Execute(args);
            Execute(ExampleConfigs.ExampleConfigurations.Hyper3_1000_5);
        }

        private static void Execute(Options options)
        {
            Console.WriteLine(MessageAtStart(options));
        }

        private static void Execute(string[] args)
        {
            var options = new Options();
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       Console.WriteLine(MessageAtStart(o));

                       
                   });
        }

        private static String MessageAtStart(Options options)
        {
            return $"Generation of random hyperbolic simplices started ... \n Current Arguments: \n" +
                           $"-n : {options.NumberSamples} \n" +
                           $"-d : {options.Dimension} \n" +
                           $"-m : {options.MaxNorm} \n";
        }
    }
}
