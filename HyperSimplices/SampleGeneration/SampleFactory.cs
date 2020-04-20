using HyperSimplices.Geometry;
using HyperSimplices.SimplicialGeometry.Simplex;
using HyperSimplices.SimplicialGeometry.SimplexComplex;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SampleGeneration
{
    public class SampleFactory
    {
        public int NbSamples { get; private set; }
        public int Dim { get; private set; }
        public double MaxNorm { get; private set; } 
        public bool Integrate { get; private set; }
        public int MeshSteps { get; private set; }
        public bool ZeroAmongEdges { get; private set; }
        public bool ComputeAngles { get; private set; }
        public bool ComputeAnalytical { get; private set; }
        public CurvatureType CurvatureType { get; private set; }

        public event EventHandler<SampleCreationEventArgs> RaiseSampleCreatorEvent;

        public SampleFactory(int nbSamples, int dim, bool integrate = false, int meshSteps = 1000, double maxNorm = 1.0, 
            bool zeroAmongEdges = false, bool computeAngles = false, bool computeAnalytical = false, CurvatureType curvatureType = CurvatureType.NEGATIVE)
        {
            NbSamples = nbSamples;
            Dim = dim;
            MaxNorm = maxNorm;
            Integrate = integrate;
            MeshSteps = meshSteps;
            ZeroAmongEdges = zeroAmongEdges;
            ComputeAngles = computeAngles;
            ComputeAnalytical = computeAnalytical;
            CurvatureType = curvatureType;
        }

        public (List<Simplex>, List<SimplexComplex>) RandomSamples()
        {
            var randomSimplices = Simplex.RandomSamples(NbSamples, Dim, Geometries.GetSpace(CurvatureType, Dim), ZeroAmongEdges, MaxNorm);
            var hyperRandomComplexes = new List<SimplexComplex>();
            var counter = 0;

            foreach (var randomSimplex in randomSimplices)                
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var simplexComplex = new SimplexComplex(randomSimplex);
                simplexComplex.Propagate();
                
                if(Integrate)
                    simplexComplex.Integrate(MeshSteps, null, ComputeAnalytical);

                if (ComputeAngles)
                    simplexComplex.ComputeAngles();

                counter++;

                stopWatch.Stop();
                var sampleCreationEventArgs = new SampleCreationEventArgs(counter, NbSamples, stopWatch.Elapsed.TotalSeconds);
                OnSampleCreationEvent(sampleCreationEventArgs);
            }
                
            return (randomSimplices, hyperRandomComplexes);
        }

        protected virtual void OnSampleCreationEvent(SampleCreationEventArgs e)
        {
            EventHandler<SampleCreationEventArgs> handler = RaiseSampleCreatorEvent;

            if(handler != null)
                handler(this, e);            
        }
    }

    public class SampleCreationEventArgs: EventArgs
    {
        public int CreatedSamples { get; set; }
        public int TotalSamples { get; set; }
        public double ElapsedTime { get;  set; }

        public SampleCreationEventArgs(int createdSamples, int totalSamples, double elapsedTime)
        {
            CreatedSamples = createdSamples;
            TotalSamples = totalSamples;
            ElapsedTime = elapsedTime;
        }
    }
}
