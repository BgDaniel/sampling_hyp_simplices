using System;
using System.Collections.Generic;
using HyperSimplices.Geometry;
using HyperSimplices.SimplicialGeometry.Simplex;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;

namespace HyperSimplices.UnitTests
{
    [TestClass]
    public class VolumeTest
    {
        public const int NUMBER_SAMPLES = 100;
        public const double MAX_NORM = 2.0;
        public const int MESH_STEPS = 500;

        [Theory]
        [InlineData(CurvatureType.FLAT)]
        public void TestEulcideanVolume(CurvatureType curvatureType)
        {
            List<Simplex> randomSamples;

            for (int iDim = 1; iDim < 5; iDim++)
            {
                var ambiantSpace = Geometries.GetSpace(curvatureType, iDim);
                randomSamples = Simplex.RandomSamples(NUMBER_SAMPLES, iDim, ambiantSpace, true, MAX_NORM);

                foreach(var simplex in randomSamples)
                {
                    simplex.Integrate(MESH_STEPS, true);
                    var volumeAnalytical = simplex.Volume;
                    simplex.Integrate(MESH_STEPS, false);
                    var volume = simplex.Volume;
                }
            }
        }
    }
}
