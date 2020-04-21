using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HyperSimplices.Geometry;
using HyperSimplices.SampleGeneration;
using HyperSimplices.SimplicialGeometry.Simplex;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit;
using NUnit.Framework;

namespace HyperSimplices.UnitTests
{
    [TestClass]
    public class VolumeTest
    {
        public const int NUMBER_SAMPLES = 100;
        public const double MAX_NORM = 1.0;
        public const int MESH_STEPS = 5000;
        public const double TOLERANCE = 10e-5;

        [TestCase(CurvatureType.FLAT)]
        [TestCase(CurvatureType.NEGATIVE)]
        public void TestLength(CurvatureType curvatureType)
        {
            var ambiantSpace = Geometries.GetSpace(CurvatureType.NEGATIVE, 2);
            var randomSamples = Simplex.RandomSamples(NUMBER_SAMPLES, 2, ambiantSpace, true, MAX_NORM);
            var counter = 0;

            foreach (var simplex in randomSamples)
            {
                foreach(var face in simplex.Faces)
                {
                    face.Integrate(MESH_STEPS, true);
                    var volumeAnalytical = face.Volume;
                    face.Integrate(MESH_STEPS, false);
                    var volume = face.Volume;

                    if (Math.Abs(volumeAnalytical - volume) >= TOLERANCE)
                        throw new Exception($"Deviation to high for simplex number {counter}");
                }

                counter++;
            }
        }

        [Test]
        public void TestHyperbolicTriangleDefect()
        {
            var integrate = true;   
            var zeroAmongEdges = true;
            var computeAnlges = true;
            var computeAnalytical = false;

            var sampleFactory = new SampleFactory(10, 2, integrate, MESH_STEPS, MAX_NORM, zeroAmongEdges, 
                computeAnlges, computeAnalytical, CurvatureType.NEGATIVE);

            (var randomSimplices, var hyperRandomComplexes) = sampleFactory.RandomSamples();

            for(int i = 0; i < randomSimplices.Count; i++)
            {
                var surfaceVolume = hyperRandomComplexes[i].Simplex0.Volume;
                var angles = hyperRandomComplexes[i].SimplexPairs[1].Select(simplexPair => simplexPair.Angle).ToList();
                var defect = Math.PI - angles.Sum();
            }
        }
    }
}
