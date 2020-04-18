using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices
{
    public static class ArrayHelpers
    {
        private static List<double[]> FillNextEntry(double[] arr, int atPosition, double[] values)
        {
            var ret = new List<double[]>();
            var arrLength = arr.Length;

            for(int ell = 0; ell < values.Length; ell++)
            {
                var newArr = new double[arrLength];
                Array.Copy(arr, newArr, arrLength);
                newArr[atPosition] = values[ell];
                ret.Add(newArr);
            }

            return ret;
        }

        public static List<Vector<double>> CreateSimplexMesh(int meshSteps, int dim, double scale = 1.0)
        {
            var delta = 1.0 / meshSteps;
            var deltas = Enumerable.Range(0, meshSteps + 1).Select(i => i * delta).ToArray();         
            var meshPoints = new List<double[]>() { new double[dim] };
            List<double[]> meshPointsNew = null;

            for (int iDim = 0; iDim < dim; iDim++)
            {
                meshPointsNew = new List<double[]>();

                foreach (var meshPoint in meshPoints)
                    meshPointsNew.AddRange(FillNextEntry(meshPoint, iDim, deltas));

                meshPoints = meshPointsNew.CopyDoubleArrayList(); 
            }

            var meshPointsFinal = new List<double[]>();

            foreach(var meshPoint in meshPoints)
            {
                if (meshPoint.Sum() <= 1.0)
                    meshPointsFinal.Add(meshPoint);
            }

            return meshPointsFinal.Select(meshPt => Vector<double>.Build.DenseOfArray(meshPt)).ToList();
        }   
        
        public static List<Vector<double>> RandomVectors(int nbSamples, int dim, double maxNorm = 1.0)
        {
            var ret = new List<Vector<double>>();
            var counter = 0;
            var continuousUniform = new ContinuousUniform(- maxNorm, + maxNorm);

            while(counter < nbSamples)
            {
                var meetsCondition = false;
                Vector<double> rndVector = null;

                while (!meetsCondition)
                {
                    rndVector = Vector<double>.Build.Random(dim, continuousUniform);
                    meetsCondition = rndVector.L2Norm() < maxNorm;
                }

                ret.Add(rndVector);
                counter++;
            }

            return ret;
        }
    }

    public static class ListExtension
    {
        public static List<double[]> CopyDoubleArrayList(this List<double[]> dblArrList)
        {
            var clone = new List<double[]>();

            foreach(var arr in dblArrList)
            {
                var length = arr.Length;
                var newArr = new double[length];
                Array.Copy(arr, newArr, length);
                clone.Add(newArr);
            }

            return clone;
        }
    }
}
