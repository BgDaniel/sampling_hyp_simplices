using CommandLine;
using FileHelpers;
using HyperSimplices;
using HyperSimplices.SampleGeneration;
using HyperSimplices.SimplicialGeometry.ShearMapping;
using HyperSimplices.SimplicialGeometry.Simplex;
using HyperSimplices.Utils;
using HypSimplexSampleFactory.CommandLineParser;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace HypSimplexSampleFactory
{
    class Program
    {
        private static string _computing = "Generating random samples ...";
        private static object _simplexLock = new object();

        static void Main(string[] args)
        {
            //Execute(args);
            Execute(ExampleConfigs.ExampleConfigurations.Hyper_4);
        }

        private static void Execute(Options options)
        {
            var path = "C:\\Users\\bergerd\\simplices.csv";

            var samples = ShearTriangle2D.RandomSamples(50000);
            //foreach(var sample in samples)
            //    VariousHelpers.SampleToFile(path, sample);            


            Console.WriteLine(MessageAtStart(options));

            var randomSimplices = Simplex3D.RandomSamples(options.NumberSamples);
            var counter = 0;
            var progressBar = new ProgressBar();
            var nbThreads = 7;
            var vols = new double[nbThreads];                        
            
            if (File.Exists(path))
                File.Delete(path);

            var partition = VariousHelpers.GetPartition(options.NumberSamples, nbThreads);

            Parallel.For(0, nbThreads, ell =>
            {
                for (int i = partition[ell][0]; i < partition[ell][1]; i++)
                {
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
                    randomSimplices[i].Compute(3000);
                    stopWatch.Stop();
                    counter += 1;
                    Debug.WriteLine($"Successfully generated {counter} random samples.");

                    lock(_simplexLock)
                    {
                        progressBar.Report(counter, options.NumberSamples, stopWatch.Elapsed.TotalSeconds);
                        VariousHelpers.SampleToFile(path, randomSimplices[i]);
                    }                    
                }
            });

                                                               
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
