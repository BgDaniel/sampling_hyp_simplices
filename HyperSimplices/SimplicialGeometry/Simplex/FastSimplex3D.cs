using Accord.Math.Integration;
using Aspose.Words.Fields;
using FileHelpers;
using HyperSimplices.Geometry.CurvedGeometry.GeometricMotions;
using HyperSimplices.Utils;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry.Simplex
{
    [DelimitedRecord(";")]
    public class FastSimplex3D
    {
        [FieldHidden]
        private double[] m_A;

        [FieldHidden]
        private double[] m_B;

        [FieldHidden]
        private double[] m_C;

        [FieldHidden]
        private double[] m_D;

        public double Volume;

        public double SurfaceABC;
        public double SurfaceABD;
        public double SurfaceACD;
        public double SurfaceBCD;

        public double LengthAB;
        public double LengthAC;
        public double LengthAD;
        public double LengthBC;
        public double LengthBD;
        public double LengthCD;

        public double AngleABC;
        public double AngleABD;
        public double AngleACD;
        public double AngleBAC;
        public double AngleBCD;
        public double AngleBAD;
        public double AngleCAB;
        public double AngleCBD;
        public double AngleCAD;
        public double AngleDAB;
        public double AngleDBC;
        public double AngleDAC;

        public FastSimplex3D()
        {

        }

        public FastSimplex3D(double[] A, double[] B, double[] C, double[] D)
        {
            m_A = A;
            m_B = B;
            m_C = C;
            m_D = D;
        }

        protected double[] GetPoint(String P)
        {
            switch(P)
            {
                case "A":
                    return m_A;
                case "B":
                    return m_B;
                case "C":
                    return m_C;
                case "D":
                    return m_D;
                default:
                    return null;
            }
        }

        public void Compute()
        {
            Volume = Vol();

            SurfaceABC = Surface("A", "B", "C");
            SurfaceABD = Surface("A", "B", "D");
            SurfaceACD = Surface("A", "C", "D");
            SurfaceBCD = Surface("B", "C", "D");

            LengthAB = Length("A", "B");
            LengthAC = Length("A", "C");
            LengthAD = Length("A", "D");
            LengthBC = Length("B", "C");
            LengthBD = Length("B", "D");
            LengthCD = Length("C", "D");

            AngleABC = Angle("A", "B", "C");
            AngleABD = Angle("A", "B", "D");
            AngleACD = Angle("A", "C", "D");
            AngleBAC = Angle("B", "A", "C");
            AngleBCD = Angle("B", "C", "D");
            AngleBAD = Angle("B", "A", "D");
            AngleCAB = Angle("C", "A", "B");
            AngleCBD = Angle("C", "B", "D");
            AngleCAD = Angle("C", "A", "D");
            AngleDAB = Angle("D", "A", "B");
            AngleDBC = Angle("D", "B", "C");
            AngleDAC = Angle("D", "A", "C");
        }

        protected double[] FirstToZero(double[] B, double[] P)
        {
            return GeometricMotions.MoveBToZeroInBeltramiKlein(B, P);
        }

        protected double Vol(int meshSteps = 10000)
        {
            var __B = FirstToZero(m_A, m_B);
            var __C = FirstToZero(m_A, m_C);
            var __D = FirstToZero(m_A, m_D);
            var span = Matrix<double>.Build.DenseOfColumnArrays(new List<double[]>() { __B, __C, __D});
            var detSpan = span.Determinant();

            Func<double, double, double, double> gramDet = (r, s, t) =>
            {
                var y = new double[3] { __B[0] * r + __C[0] * s + __D[0] * t,
                                        __B[1] * r + __C[0] * s + __D[0] * t,
                                        __B[0] * r + __C[0] * s + __D[0] * t };

                var yNorm = y.Norm();
                var yNormSq = yNorm * yNorm;

                return 1.0 / ((1.0 - yNormSq) * (1.0 - yNormSq) * (1.0 - yNormSq));
            };

            var mesh = MeshFactory.Instance.GetMesh(3, meshSteps);            
            var dVol = 1.0 / (meshSteps * meshSteps * meshSteps);

            var volume = .0;
            var meshPoints = MeshFactory.Instance.GetMesh(meshSteps, 3);

            foreach (var meshPoint in meshPoints)
                volume += gramDet(meshPoint[0], meshPoint[1], meshPoint[2]) * dVol;

            return volume;
        }

        protected double Angle(double[] v, double[] w)
        {
            var vw = Enumerable.Range(0, v.Length).Select(ell => v[ell] * w[ell]).Sum();
            var vv = Enumerable.Range(0, v.Length).Select(ell => v[ell] * v[ell]).Sum();
            var ww = Enumerable.Range(0, v.Length).Select(ell => w[ell] * w[ell]).Sum();

            return Math.Acos(vw / Math.Sqrt(vv * ww));
        }

        protected double Surface(String _P, String _Q, String _R)
        {
            return Math.PI - Angle(_P, _Q, _R) - Angle(_Q, _R, _P) - Angle(_R, _Q, _P);
        }

        protected double Angle(String _B, String _Q, String _R)
        {
            var _base = GetPoint(_B);
            var __Q = FirstToZero(_base, GetPoint(_Q));
            var __R = FirstToZero(_base, GetPoint(_R));
            return Angle(__Q, __R);
        }

        protected double Angle(String _P, String _B, String _Q, String _S)
        {
            var _base = GetPoint(_B);
            var __P = FirstToZero(_base, GetPoint(_P));
            var __Q = FirstToZero(_base, GetPoint(_Q));
            var __S = FirstToZero(_base, GetPoint(_S));

            var normalPQ = __P.Normal(__Q);
            var normalPS = __P.Normal(__S);
            return Angle(normalPQ, normalPS);
        }

        public double Length(String _P, String _Q)
        {
            var __Q = FirstToZero(GetPoint(_P), GetPoint(_Q));
            var r = __Q.Norm();
            return Math.Atan(r);
        }

        public static List<FastSimplex3D> RandomSamples(int nbSamples, double maxNorm = 1.0)
        {
            var maxHyperbolicNorm = Math.Tanh(maxNorm);
            var rndVectors = VariousHelpers.RandomVectors(nbSamples * 4, 3, maxHyperbolicNorm);
            var ret = new List<FastSimplex3D>();

            for (int i = 0; i < nbSamples; i++)
            {
                ret.Add(new FastSimplex3D(rndVectors[4 * i].ToArray(), rndVectors[4 * i + 1].ToArray(), 
                    rndVectors[4 * i + 2].ToArray(), rndVectors[4 * i + 3].ToArray()));
            }

            return ret;
        }
    }
}
