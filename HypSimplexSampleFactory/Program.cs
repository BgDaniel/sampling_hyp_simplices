using CommandLine;
using HyperSimplices.SampleGeneration;
using HypSimplexSampleFactory.CommandLineParser;
using System;

namespace HypSimplexSampleFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            //Execute(args);
            Execute(ExampleConfigs.ExampleConfigurations.Hyper2_1000_500_2);
        }

        private static void Execute(Options options)
        {
            Console.WriteLine(MessageAtStart(options));
            var sampleFactory = new SampleFactory(options.NumberSamples, options.Dimension, options.Integrate, options.MeshSteps, options.MaxNorm);
            (var randomSimplices, var hyperRandomComplexes) = sampleFactory.RandomSamples();            
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
            return $"Generation of random hyperbolic simplices started ... \nCurrent arguments are configured as follows: \n" +
                           $"-n = {options.NumberSamples} \n" +
                           $"-d = {options.Dimension} \n" +
                           $"-b = {options.Integrate} \n" +
                           $"-i = {options.MeshSteps} \n" +
                           $"-m = {options.MaxNorm} \n";
        }
    }
}
