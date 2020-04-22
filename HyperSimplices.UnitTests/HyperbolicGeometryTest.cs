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
        public const int NUMBER_SAMPLES = 500;
        public const double MAX_NORM = 2.0;
        public const double TOLERANCE = 10e-5;

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

        [Test]
        public void TestFastSimplex3D()
        {
            var simplex = new FastSimplex3D(new double[] { .0, .0, .0, .0 },
                new double[] { .0, .5, .0, .0 },
                new double[] { .0, .0, .5, .0 },
                new double[] { .0, .0, .5, 1.0 });

            simplex.Compute();
        }
    }
}
