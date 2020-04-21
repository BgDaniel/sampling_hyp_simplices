using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HyperSimplices.Geometry;
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
        public const double MAX_NORM = 2.0;
        public const int MESH_STEPS = 500;
        public const double TOLERANCE = 10e-5;

        [TestCase(CurvatureType.FLAT)]
        public void TestVolume(CurvatureType curvatureType)
        {
            for (int iDim = 1; iDim < 5; iDim++)
            {
                var ambiantSpace = Geometries.GetSpace(curvatureType, iDim);
                var randomSamples = Simplex.RandomSamples(NUMBER_SAMPLES, iDim, ambiantSpace, true, MAX_NORM);
                var counter = 0;
                var meshSteps = MESH_STEPS * (int)Math.Pow(10, iDim);
                      
                foreach(var simplex in randomSamples)
                {
                    simplex.Integrate(meshSteps, true);
                    var volumeAnalytical = simplex.Volume;
                    simplex.Integrate(meshSteps, false);
                    var volume = simplex.Volume;

                    if (Math.Abs(volumeAnalytical - volume) >= TOLERANCE)
                        throw new Exception($"Deviation to high for simplex number {counter}");

                    counter++;
                }
            }
        }
    }
}
