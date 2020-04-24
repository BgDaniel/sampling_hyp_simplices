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
    public class TestFastSimplex3D
    {
        public const int NUMBER_SAMPLES = 5000;
        public const double MAX_NORM = 2.0;
        public const double TOLERANCE = 10e-5;
        public const int MESH_STEPS = 2500;

        public delegate double TrigonometricRelation(Triangle triangle);

        public List<TrigonometricRelation> Relations()
        {
            TrigonometricRelation LawOfCosineI1 = (triangle) => Math.Cosh(triangle.A) - Math.Cosh(triangle.B) * Math.Cosh(triangle.C)
                + Math.Sinh(triangle.B) * Math.Sinh(triangle.C) * Math.Cos(triangle.Alpha);

            TrigonometricRelation LawOfCosineI2 = (triangle) => Math.Cosh(triangle.B) - Math.Cosh(triangle.A) * Math.Cosh(triangle.C)
                + Math.Sinh(triangle.A) * Math.Sinh(triangle.C) * Math.Cos(triangle.Beta);

            TrigonometricRelation LawOfCosineI3 = (triangle) => Math.Cosh(triangle.C) - Math.Cosh(triangle.A) * Math.Cosh(triangle.B)
                + Math.Sinh(triangle.A) * Math.Sinh(triangle.B) * Math.Cos(triangle.Gamma);

            TrigonometricRelation LawOfCosineII1 = (triangle) => Math.Cos(triangle.Gamma) + Math.Cos(triangle.Alpha) * Math.Cos(triangle.Beta)
                - Math.Sin(triangle.Alpha) * Math.Sin(triangle.Beta) * Math.Cosh(triangle.C);

            TrigonometricRelation LawOfCosineII2 = (triangle) => Math.Cos(triangle.Beta) + Math.Cos(triangle.Alpha) * Math.Cos(triangle.Gamma)
                - Math.Sin(triangle.Alpha) * Math.Sin(triangle.Gamma) * Math.Cosh(triangle.B);

            TrigonometricRelation LawOfCosineII3 = (triangle) => Math.Cos(triangle.Alpha) + Math.Cos(triangle.Beta) * Math.Cos(triangle.Gamma)
                - Math.Sin(triangle.Beta) * Math.Sin(triangle.Gamma) * Math.Cosh(triangle.A);

            TrigonometricRelation LawOfSine1 = (triangle) => Math.Sinh(triangle.A) * Math.Sin(triangle.Beta)
                - Math.Sinh(triangle.B) * Math.Sin(triangle.Alpha);

            TrigonometricRelation LawOfSine2 = (triangle) => Math.Sinh(triangle.A) * Math.Sin(triangle.Gamma)
                - Math.Sinh(triangle.C) * Math.Sin(triangle.Alpha);

            TrigonometricRelation LawOfSine3 = (triangle) => Math.Sinh(triangle.B) * Math.Sin(triangle.Gamma)
                - Math.Sinh(triangle.C) * Math.Sin(triangle.Beta);

            return new List<TrigonometricRelation>()
            {
                LawOfCosineI1,
                LawOfCosineI2,
                LawOfCosineI3,
                LawOfCosineII1,
                LawOfCosineII2,
                LawOfCosineII3,
                LawOfSine1,
                LawOfSine2,
                LawOfSine3
            };
        }

        [Test]
        public void TestStandardSimplex()
        {
            var standardSimplex = Simplex3D.GetStandardSimplex();
            standardSimplex.Compute(MESH_STEPS, false);

            var trianlges = standardSimplex.Triangles;

            foreach (var triangle in trianlges)
            {
                var trigonometricRelations = Relations();

                foreach (var trigonometricRelation in trigonometricRelations)
                {
                    var deviation = Math.Abs(trigonometricRelation(triangle) - .0);

                    if (deviation >= TOLERANCE)
                        throw new Exception($"Deviation to high!");
                }
            }
        }

        [Test]
        public void TestTrigonometricRelations()
        {
            var randomSimplices = Simplex3D.RandomSamples(NUMBER_SAMPLES);

            foreach (var randomSimplex in randomSimplices)
            {
                randomSimplex.Compute(MESH_STEPS, false);
                var trianlges = randomSimplex.Triangles;

                foreach (var triangle in trianlges)
                {
                    var trigonometricRelations = Relations();

                    foreach (var trigonometricRelation in trigonometricRelations)
                    {
                        var deviation = Math.Abs(trigonometricRelation(triangle) - .0);

                        if (deviation >= TOLERANCE)
                            throw new Exception($"Deviation to high!");
                    }
                }
            }
        }

        [Test]
        public void TestAngleBetweenFaces()
        {
            var randomSimplices = Simplex3D.RandomSamples(NUMBER_SAMPLES);

            foreach (var randomSimplex in randomSimplices)
            {
                var normalABC = randomSimplex.NormalOnFace("A", "B", "C");
                var normalABD = randomSimplex.NormalOnFace("A", "D", "B");
                var A = randomSimplex.GetPoint("A");
                var angleA = Math.PI - Simplex3D.Angle(A, normalABC, normalABD);

                var normalBCA = randomSimplex.NormalOnFace("B", "C", "A");
                var normalBAD = randomSimplex.NormalOnFace("B", "A", "D");
                var B = randomSimplex.GetPoint("B");
                var angleB = Math.PI - Simplex3D.Angle(B, normalBCA, normalBAD);

                if (Math.Abs(angleA - angleB) >= TOLERANCE)
                    throw new Exception($"Deviation to high!");
            }
        }

        [Test]
        public void TestSurfacesComputation()
        {
            var randomSimplices = Simplex3D.RandomSamples(NUMBER_SAMPLES);

            foreach (var randomSimplex in randomSimplices)
            {
                randomSimplex.Compute(MESH_STEPS, false);
                var trianlges = randomSimplex.Triangles;

                foreach (var triangle in trianlges)
                {
                    var trigonometricRelations = Relations();

                    foreach (var trigonometricRelation in trigonometricRelations)
                    {
                        var deviation = Math.Abs(trigonometricRelation(triangle) - .0);

                        if (deviation >= TOLERANCE)
                            throw new Exception($"Deviation to high!");
                    }
                }
            }
        }
    }
}
