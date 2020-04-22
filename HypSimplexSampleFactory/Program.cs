using CommandLine;
using FileHelpers;
using HyperSimplices.SampleGeneration;
using HyperSimplices.SimplicialGeometry.Simplex;
using HyperSimplices.Utils;
using HypSimplexSampleFactory.CommandLineParser;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace HypSimplexSampleFactory
{
    class Program
    {
        private static string _computing = "Generating random samples ...";

        static void Main(string[] args)
        {
            //Execute(args);
            Execute(ExampleConfigs.ExampleConfigurations.Hyper_4);
        }

        private static void Execute(Options options)
        {
            Console.WriteLine(MessageAtStart(options));

            var randomSimplices = FastSimplex3D.RandomSamples(options.NumberSamples);

            foreach(var randomSimplex in randomSimplices)
                randomSimplex.Compute();
            
            var engine = new FileHelperEngine(typeof(FastSimplex3D));
            engine.HeaderText = engine.GetFileHeader();
            engine.WriteFile("C:\\Users\\bergerd\\simplices.csv", randomSimplices);



            /*
            var sampleFactory = new SampleFactory(options.NumberSamples, options.Dimension, options.Integrate, 
                options.MeshSteps, options.MaxNorm, options.ZeroAmongEdges, options.ComputeAngles, options.ComputeAnalytical);

            var progressBar = new ProgressBar();
            sampleFactory.RaiseSampleCreatorEvent += progressBar.HandleSampleCreationEvent;

            Console.WriteLine(_computing);            
            (var randomSimplices, var hyperRandomComplexes) = sampleFactory.RandomSamples();   
            */
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
                           $"-m = {options.MaxNorm} \n" +
                           $"-z = {options.ZeroAmongEdges} \n" +
                           $"-a = {options.ComputeAngles}" +
                           $"-c = {options.ComputeAnalytical}";
        }
    }
}
