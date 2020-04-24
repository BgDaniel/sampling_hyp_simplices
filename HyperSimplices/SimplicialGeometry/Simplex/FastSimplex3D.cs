using Accord.Math.Integration;
using Aspose.Words.Fields;
using FileHelpers;
using HyperSimplices.Geometry.CurvedGeometry.GeometricMotions;
using HyperSimplices.Utils;
using MathNet.Numerics;
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

        public double AngleCABD;
        public double AngleBACD;
        public double AngleCADB;
        public double AngleABCD;
        public double AngleABDC;
        public double AngleACDB;

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

            AngleCABD = Angle("C", "A", "B", "D");
            AngleBACD = Angle("B", "A", "C", "D");
            AngleCADB = Angle("C", "A", "D", "B");
            AngleABCD = Angle("A", "B", "C", "D");
            AngleABDC = Angle("A", "B", "D", "C");
            AngleACDB = Angle("A", "C", "D", "B");
        }

        protected double[] ToZero(double[] B, double[] P)
        {
            return GeometricMotions.MoveBToZeroInBeltramiKlein(B, P);
        }

        protected double Vol(int meshSteps = 2000)
        {
            var _P = m_A; 
            var _V = m_B.Subtract(m_A);
            var _W = m_C.Subtract(m_A);
            var _U = m_D.Subtract(m_A);
            var span = Matrix<double>.Build.DenseOfColumnArrays(new List<double[]>() { _V, _W, _U});
            var detSpan = span.Determinant();            
            var dt = 1.0 / (double)meshSteps;
            var aSixth = 1.0 / 6.0;
            var fiveSixth = 5.0 / 6.0;
            var nbThreads = 4;
            var vols = new double[nbThreads];

            var partition = Enumerable.Range(0, nbThreads).Select(ell => new int[2] 
                {
                    (int)(meshSteps * Math.Pow((double)(nbThreads - ell - 1) / (double)nbThreads, 1.0 / 3.0)),
                    (int)(meshSteps * Math.Pow((double)(nbThreads - ell) / (double)nbThreads, 1.0 / 3.0))                    
                }).ToArray();

            Parallel.For(0, nbThreads, ell =>
            {
                var volume = .0;
                double weight;
                var _Y = new double[3];

                for (int _v = partition[ell][0]; _v < partition[ell][1]; _v++)
                {
                    for (int _w = 0; _w < meshSteps - _v; _w++)
                    {
                        for (int _u = 0; _u < meshSteps - _w - _v; _u++)
                        {
                            weight = 1.0;

                            if (_u == meshSteps - _w - _v - 1)
                                weight = aSixth;
                            else if (_u == meshSteps - _w - _v - 2)
                                weight = fiveSixth;

                            for (int ekk = 0; ekk < 3; ekk++)
                                _Y[ekk] = _P[ekk] + (_v * _V[ekk] + _w * _W[ekk] + _u * _U[ekk]) * dt;

                            var yNormSq = _Y.Dot(_Y);
                            volume += weight * (1.0 / ((1.0 - yNormSq) * (1.0 - yNormSq) * (1.0 - yNormSq)));
                        }
                    }
                }

                vols[ell] = volume;
            });
            
            return vols.Sum() * Math.Sqrt(detSpan * detSpan) * dt * dt * dt;
        }

        protected double Angle(double[] x, double[] v, double[] w)
        {
            var g_vv = G(x, v, v);
            var g_vw = G(x, v, w);
            var g_ww = G(x, w, w);

            return Math.Acos(g_vw / Math.Sqrt(g_vv * g_ww));
        }

        protected double Surface(String _P, String _Q, String _R)
        {
            return Math.PI - Angle(_P, _Q, _R) - Angle(_Q, _R, _P) - Angle(_R, _Q, _P);
        }

        protected double Angle(String Base, String Q, String R)
        {
            var _Base = GetPoint(Base);
            var _Q = GetPoint(Q);
            var _R = GetPoint(R);
            var vecQ = _Q.Subtract(_Base);
            var vecR = _R.Subtract(_Base);

            return Angle(_Base, vecQ, vecR);
        }

        protected double Angle(String Base, String Q, String R, String S)
        {
            var _Base = GetPoint(Base);
            var _Q = GetPoint(Q);
            var _R = GetPoint(R);
            var _S = GetPoint(S);
            var vecQ = _Q.Subtract(_Base);
            var vecR = _R.Subtract(_Base);
            var vecS = _S.Subtract(_Base);

            var normalBaseQR = Normal(_Base, vecQ, vecR);
            var normalBaseQS = Normal(_Base, vecQ, vecS);
            return Math.PI - Angle(_Base, normalBaseQR, normalBaseQS);
        }

        public double Length(String P, String Q)
        {
            var _P = Vector<double>.Build.DenseOfArray(GetPoint(P));
            var _Q = Vector<double>.Build.DenseOfArray(GetPoint(Q));

            Func<double, Vector<double>> line_pq = t => _P + (_Q - _P) * t;
            Func<double, double> f = t => line_pq(t).L2Norm() - 1.0;

            var t_A = FindRoots.OfFunction(f, -3.0 / (_P - _Q).L2Norm(), .0);
            var t_B = FindRoots.OfFunction(f, 1.0, 3.0 / (_P - _Q).L2Norm());

            var _A = line_pq(t_A);
            var _B = line_pq(t_B);

            var aq = (_A - _Q).L2Norm();
            var ap = (_A - _P).L2Norm();
            var pb = (_P - _B).L2Norm();
            var qb = (_Q - _B).L2Norm();

            return .5 * Math.Log((aq * pb) / (ap * qb));
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

        private double G(double[] x, double[] v, double[] w)
        {
            var xNorm = x.Norm();
            var y = 1.0 - xNorm * xNorm;
            var id = Matrix<double>.Build.DenseIdentity(3);
            var X = Matrix<double>.Build.DenseOfColumnArrays(x);
            var XT = X.Transpose();
            var _vT = Matrix<double>.Build.DenseOfColumnArrays(v).Transpose();
            var _w = Matrix<double>.Build.DenseOfColumnArrays(w);
            
            return (_vT * (id / y + X * XT / (y * y)) * _w).ToArray()[0,0];
        }

        private double[] Normal(double[] x, double[] v, double[] w)
        {
            var e_1 = new double[3];
            e_1[0] = 1.0;
            var e_2 = new double[3];
            e_2[1] = 1.0;
            var e_3 = new double[3];
            e_1[2] = 1.0;

            var e_i = new List<double[]>() { e_1, e_2, e_3 };
            var A = Matrix<double>.Build.DenseOfColumnArrays(v, w);
            Vector<double> normal = null;
            var zero = Vector<double>.Build.DenseOfArray(new double[3]);

            foreach (var e in e_i)
            {
                var _e = Vector<double>.Build.DenseOfArray(e);
                var A_T = A.Transpose();
                var AT_e = A_T * _e;
                var AT_A = A_T * A;
                var x_0 = AT_A.Solve(AT_e);
                normal = _e - A * x_0;

                if (normal.Equals(zero))
                    continue;
                else
                    break;
            }
        
            var cols = A.EnumerateColumns().ToList();
            cols.Add(normal);
            var AB = Matrix<double>.Build.DenseOfColumnVectors(cols);
            var AB_T = AB.Transpose();
            var sign = Math.Sign(AB_T.Determinant());

            return (sign * normal / normal.L2Norm()).ToArray();
        }
    }
}
