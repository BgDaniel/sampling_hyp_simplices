using HyperSimplices.Geometry;
using HyperSimplices.SimplicialGeometry.Simplex;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.UnitTests
{
    [TestClass]
    public class HyperbolicGeometryTest
    {
        public const int NUMBER_SAMPLES = 5000;
        public const double MAX_NORM = 2.0;
        public const double TOLERANCE = 10e-5;
        public const int MESH_STEPS = 2500;

        [Test]
        public void TestDefect2Dim()
        {
            var ambiantSpace = Geometries.GetSpace(CurvatureType.NEGATIVE, 2);
            var randomSamples = Simplex.RandomSamples(NUMBER_SAMPLES, 2, ambiantSpace, true, MAX_NORM);
            var counter = 0;

            foreach (var simplex in randomSamples)
            {
                foreach (var face in simplex.Faces)
                {
                    var normalVector = VariousHelpers.GetNormalVector(face);

                    foreach (var dirVector in face.DirectionalVectors)
                    {
                        var scalarProduct = dirVector.DotProduct(normalVector);

                        if (Math.Abs(scalarProduct) >= TOLERANCE)
                            throw new Exception($"Deviation to high for simplex number {counter}");
                    }
                    counter++;
                }
            }
        }        
    }
}
