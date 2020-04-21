using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyperSimplices.Geometry;
using HyperSimplices.SimplicialGeometry.Simplex;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit;
using NUnit.Framework;

namespace HyperSimplices.UnitTests
{
    [TestClass]
    public class EuclideanGeometryTest
    {
        public const int NUMBER_SAMPLES = 500;
        public const double MAX_NORM = 2.0;
        public const double TOLERANCE = 10e-5;

        [Test]
        public void TestNormVectorComputation()
        {
            for (int iDim = 2; iDim < 10; iDim++)
            {
                var ambiantSpace = Geometries.GetSpace(CurvatureType.FLAT, iDim);
                var randomSamples = Simplex.RandomSamples(NUMBER_SAMPLES, iDim, ambiantSpace, true, MAX_NORM);
                var counter = 0;

                foreach (var simplex in randomSamples)
                {
                    var normalVectorSum = Vector<double>.Build.DenseOfArray(new double[iDim]);

                    foreach (var face in simplex.Faces)
                    {
                        var normalVector = VariousHelpers.GetNormalVector(face);
                        face.Integrate(0, true);
                        var volumeFace = face.Volume;
                        var normNormalVector = normalVector.L2Norm();
                        normalVectorSum += volumeFace * normalVector;
                    }

                    if (Math.Abs(normalVectorSum.L2Norm()) >= TOLERANCE)
                        throw new Exception($"Deviation to high for simplex number {counter}");

                    counter++;
                }
            }
        }

        [Test]
        public void TestNormPerpendicularToFace()
        {
            for (int iDim = 2; iDim < 10; iDim++)
            {
                var ambiantSpace = Geometries.GetSpace(CurvatureType.FLAT, iDim);
                var randomSamples = Simplex.RandomSamples(NUMBER_SAMPLES, iDim, ambiantSpace, true, MAX_NORM);
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

        [Test]
        public void TestAngleComputation2Dim()
        {
            var ambiantSpace = Geometries.GetSpace(CurvatureType.FLAT, 2);
            var randomSamples = Simplex.RandomSamples(NUMBER_SAMPLES, 2, ambiantSpace, true, MAX_NORM);
            var counter = 0;

            foreach (var simplex in randomSamples)
            {
                var faceA = simplex.Faces[0];
                var faceB = simplex.Faces[1];
                var faceC = simplex.Faces[2];
                
                var normVectorA = VariousHelpers.GetNormalVector(faceA);
                var normVectorB = VariousHelpers.GetNormalVector(faceB);
                var normVectorC = VariousHelpers.GetNormalVector(faceC);

                var alpha = Math.PI - VariousHelpers.Angle(normVectorB, normVectorC);
                var beta = Math.PI - VariousHelpers.Angle(normVectorA, normVectorC);
                var gamma = Math.PI - VariousHelpers.Angle(normVectorA, normVectorB);

                if (Math.Abs(alpha + beta + gamma - Math.PI) >= TOLERANCE)
                    throw new Exception($"Deviation to high for simplex number {counter}");

                counter++;
            }
        }

        [Test]
        public void TestFaceSum2Dim()
        {
            var ambiantSpace = Geometries.GetSpace(CurvatureType.FLAT, 2);
            var randomSamples = Simplex.RandomSamples(NUMBER_SAMPLES, 2, ambiantSpace, true, MAX_NORM);
            var counter = 0;

            foreach (var simplex in randomSamples)
            {
                var faceSum = Vector<double>.Build.DenseOfArray(new double[2]);

                foreach (var face in simplex.Faces)
                    faceSum += face.DirectionalVectors[0];

                if (Math.Abs(faceSum.L2Norm()) >= TOLERANCE)
                    throw new Exception($"Deviation to high for simplex number {counter}");

                counter++;
            }
        }

        [Test]
        public void TestRelations()
        {
            var ambiantSpace = Geometries.GetSpace(CurvatureType.FLAT, 2);
            var randomSamples = Simplex.RandomSamples(NUMBER_SAMPLES, 2, ambiantSpace, true, MAX_NORM);
            var counter = 0;

            foreach (var simplex in randomSamples)
            {
                var faceSum = Vector<double>.Build.DenseOfArray(new double[2]);

                foreach (var face in simplex.Faces)
                    faceSum += face.DirectionalVectors[0];

                if (Math.Abs(faceSum.L2Norm()) >= TOLERANCE)
                    throw new Exception($"Deviation to high for simplex number {counter}");

                counter++;
            }
        }
    }
}
