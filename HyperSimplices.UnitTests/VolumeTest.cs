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
        public const double MAX_NORM = 1.0;
        public const int MESH_STEPS = 20000;
        public const double TOLERANCE = 10e-5;

        [TestCase(CurvatureType.FLAT)]
        [TestCase(CurvatureType.NEGATIVE)]
        public void TestLength(CurvatureType curvatureType)
        {
            var ambiantSpace = Geometries.GetSpace(curvatureType, 2);
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
    }
}
