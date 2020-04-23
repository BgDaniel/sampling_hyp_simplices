using HyperSimplices.SimplicialGeometry.Simplex;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices
{
    public static class VariousHelpers
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

        public static List<double[]> CreateMesh(int meshSteps, int dim, double scale = 1.0)
        {
            var delta = 1.0 / meshSteps;
            var deltas = Enumerable.Range(0, meshSteps).Select(i => i * delta).ToArray();
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

            foreach (var meshPoint in meshPoints)
            {
                if (meshPoint.Sum() <= 1.0)
                    meshPointsFinal.Add(meshPoint);
            }

            return meshPointsFinal;
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

        public static Vector<double> GetNormalVector(Simplex simplex)
        {
            if (simplex.Dim != simplex.DimAmbiantSpace - 1)
                throw new ArgumentException($"Dimension of simplex must be of codimennsion one but is {simplex.DimAmbiantSpace - simplex.Dim}!");

            var unitVectors = Enumerable.Range(0, simplex.DimAmbiantSpace).Select(iDim => {
                double[] array_i = new double[simplex.DimAmbiantSpace];
                array_i[iDim] = 1.0;
                return Vector<double>.Build.DenseOfArray(array_i);
            }).ToList(); 

            foreach(var unitVector in unitVectors)
            {
                try
                {
                    return GetNormalVector(simplex, unitVector);
                }
                catch(Exception ex)
                {

                }
            }

            return null;
        }

        public static Vector<double> GetNormalVector(Simplex simplex, Vector<double> b)
        {
            var A = Matrix<double>.Build.DenseOfColumnVectors(simplex.DirectionalVectors);
            var A_T = A.Transpose();
            var AT_B = A_T * b;
            var AT_A = A_T * A;
            var x_0 = AT_A.Solve(AT_B);

            var normal = b - A * x_0;

            var cols = A.EnumerateColumns().ToList();
            cols.Add(b);
            var AB = Matrix<double>.Build.DenseOfColumnVectors(cols);
            var AB_T = AB.Transpose();
            var sign = Math.Sign(AB_T.Determinant());

            return sign * normal / normal.L2Norm();
        }

        public static double Angle(Vector<double> a, Vector<double> b)
        {
            return Math.Acos((a.DotProduct(b)) / (a.L2Norm() * b.L2Norm()));
        }

        public static void WriteToFile(string filepath, List<Vector<double>> vectors)
        {
            var matrix = Matrix<double>.Build.DenseOfColumnVectors(vectors);

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filepath))
            {
                for (int i = 0; i < matrix.RowCount; i++)
                {
                    string outstr = "";
                    for (int j = 0; j < matrix.ColumnCount; j++)
                    {
                        outstr += ";" + matrix.ToArray()[i, j];
                    }
                    sw.WriteLine(outstr);
                }
            }
        }
    }

    public static class ArrayExtensions
    {
        public static double Norm(this double[] array)
        {
            var normSqr = .0;

            for (int i = 0; i < array.Length; i++)
                normSqr += array[i] * array[i];

            return Math.Sqrt(normSqr);
        }

        public static double Dot(this double[] array1, double[] array2)
        {
            var dotProd = .0;

            for (int i = 0; i < array1.Length; i++)
                dotProd += array1[i] * array2[i];

            return dotProd;
        }

        public static double[] Subtract(this double[] array1, double[] array2)
        {
            return Enumerable.Range(0, array1.Length).Select(ell => array1[ell] - array2[ell]).ToArray();
        }

        public static double[] Mult(this double[] array, double factor)
        {
            var arrayMult = new double[array.Length];

            for (int ell = 0; ell < array.Length; ell++)
                arrayMult[ell] = array[ell] * factor;

            return arrayMult;
        }

        public static double[] Normal(this double[] array1, double[] array2)
        {
            var a1 = array1[0];
            var a2 = array1[1];
            var a3 = array1[2];
            var b1 = array2[0];
            var b2 = array2[1];
            var b3 = array2[2];

            return new double[3] { a2 * b3 - b2 * a3, a1 * b3 - b1 * a3, a1 * b2 - b1 * a2 };
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
