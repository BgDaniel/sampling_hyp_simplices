using HyperSimplices.Geometry;
using HyperSimplices.SimplicialGeometry.Simplex;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.UnitTests.TriangleRelations
{
    [TestClass]
    public class TriangleRelationsTest
    {
        public const int NUMBER_SAMPLES = 500;
        public const double MAX_NORM = 2.0;
        public const double TOLERANCE = 10e-5;

        public delegate double RelationAnlges(double alpha, double beta, double gamma);

        static RelationAnlges TANGENT1 = (alpha, beta, gamma) 
            => (Math.Tan(alpha) + Math.Tan(beta)) / (1.0 - Math.Tan(alpha) * Math.Tan(beta)) + Math.Tan(gamma);

        static RelationAnlges TANGENT2 = (alpha, beta, gamma)
            => Math.Tan(alpha) + Math.Tan(beta) + Math.Tan(gamma) - Math.Tan(alpha) * Math.Tan(beta) * Math.Tan(gamma);

        static RelationAnlges COTANGENT = (alpha, beta, gamma)
            => 1.0 / (Math.Tan(beta) * Math.Tan(gamma)) + 1.0 / (Math.Tan(gamma) * Math.Tan(alpha)) + 1.0 / (Math.Tan(alpha) * Math.Tan(beta)) - 1.0;

        protected Dictionary<string, RelationAnlges> GetAnlgeRelations()
        {
            var relationAnlges = new Dictionary<string, RelationAnlges>();
            relationAnlges["TANGENT1"] = TANGENT1;
            relationAnlges["TANGENT2"] = TANGENT1;
            relationAnlges["COTANGENT"] = COTANGENT;
            return relationAnlges;
        }

        //ToBeContinued ...

        [TestCase("TANGENT1")]
        [TestCase("TANGENT2")]
        [TestCase("COTANGENT")]
        public void TestRelations(string relation)
        {
            var ambiantSpace = Geometries.GetSpace(CurvatureType.FLAT, 2);
            var randomSamples = Simplex.RandomSamples(NUMBER_SAMPLES, 2, ambiantSpace, true, MAX_NORM);
            var counter = 0;
            var anlgeRelations = GetAnlgeRelations();

            foreach (var simplex in randomSamples)
            {
                foreach(var angleRelation in anlgeRelations)
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

                    var value = angleRelation.Value(alpha, beta, gamma);

                    if (Math.Abs(value) >= TOLERANCE)
                        throw new Exception($"Deviation to high for simplex number {counter}");
                }

                counter++;
            }
        }
    }
}
