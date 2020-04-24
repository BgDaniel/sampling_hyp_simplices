//#define MULTITHREADS

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
    public class Triangle
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double Alpha { get; set; }
        public double Beta { get; set; }
        public double Gamma { get; set; }
        public double Surface { get; set; }

        public Triangle(double a, double b, double c, double alpha, double beta, double gamma)
        {
            A = a;
            B = b;
            C = c;
            Alpha = alpha;
            Beta = beta;
            Gamma = gamma;
        }
    }

    [DelimitedRecord(";")]
    public class Simplex3D
    {
        [FieldHidden]
        private double[] m_A;

        [FieldHidden]
        private double[] m_B;

        [FieldHidden]
        private double[] m_C;

        [FieldHidden]
        private double[] m_D;

        [FieldHidden]
        public Triangle ABC => new Triangle(LengthBC, LengthAC, LengthAB, AngleABC, AngleBAC, AngleCAB);

        [FieldHidden]
        public Triangle ABD => new Triangle(LengthAB, LengthAD, LengthBD, AngleDAB, AngleBAD, AngleABD);

        [FieldHidden]
        public Triangle ACD => new Triangle(LengthAC, LengthAD, LengthCD, AngleDAC, AngleCAD, AngleACD);

        [FieldHidden]
        public Triangle BCD => new Triangle(LengthBC, LengthBD, LengthCD, AngleDBC, AngleCBD, AngleBCD);

        [FieldHidden]
        public List<Triangle> Triangles => new List<Triangle>() { ABC, ABD, ACD, BCD };


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

        public double AngleABCD;
        public double AngleACBD;
        public double AngleADBC;
        public double AngleBCAD;
        public double AngleBDAC;
        public double AngleCDAB;

        public Simplex3D()
        {

        }

        public Simplex3D(double[] A, double[] B, double[] C, double[] D)
        {
            m_A = A;
            m_B = B;
            m_C = C;
            m_D = D;
        }

        public double[] GetPoint(String P)
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

        public void Compute(int meshSteps, bool computeVolume = true)
        {
            if(computeVolume)
                Volume = Vol(meshSteps);

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

            AngleABCD = Angle("A", "B", "C", "D");
            AngleACBD = Angle("A", "C", "B", "D");
            AngleADBC = Angle("A", "D", "B", "C");
            AngleBCAD = Angle("B", "C", "A", "D");
            AngleBDAC = Angle("B", "D", "A", "C");
            AngleCDAB = Angle("C", "D", "A", "B");
        }

        protected double Vol(int meshSteps = 2500)
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


#if MULTITHREADS
            var nbThreads = 4;
            var vols = new double[nbThreads];

            var partition = VariousHelpers.GetPartition(meshSteps, nbThreads, 1.0 / 3.0);

            Parallel.For(0, nbThreads, ell =>
            {
#else
            var partition = new int[1][]{ new int[2] { 0, meshSteps } };
                var vols = new double[1];
                var ell = 0; 
#endif
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
#if MULTITHREADS
            });
#endif

            return vols.Sum() * Math.Sqrt(detSpan * detSpan) * dt * dt * dt;
        }

        protected double SurfaceNumeric(String Base, String Q, String R, int meshSteps = 2500)
        {
            var _Base = Vector<double>.Build.DenseOfArray(GetPoint(Base));
            var _Q = Vector<double>.Build.DenseOfArray(GetPoint(Q));
            var _R = Vector<double>.Build.DenseOfArray(GetPoint(R));
            var _V = _Q - _Base;
            var _W = _R - _Base;            
            var span = Matrix<double>.Build.DenseOfColumnVectors(new List<Vector<double>>() { _V, _W });
            var spanT = span.Transpose();
            var spanTspan = spanT * span;
            var dt = 1.0 / (double)meshSteps;
            var surface = .0;
            var weight = 1.0;

            for (int _v = 0; _v < meshSteps; _v++)
            {
                for (int _w = 0; _w < meshSteps - _v; _w++)
                {
                    if (_w == meshSteps - _v - 1)
                        weight = .5;

                    var _X = Vector<double>.Build.DenseOfArray(new double[2] { _v * dt, _w * dt });
                    var _Y = _Base + span * _X;
                    var __Y = Matrix<double>.Build.DenseOfRowVectors(new List<Vector<double>>() { _Y });
                    var __YT = __Y.Transpose();
                    var _G = spanTspan / (1.0 - _Y.L2Norm() * _Y.L2Norm())
                        + spanT * __YT * __Y * span / ((1.0 - _Y.L2Norm() * _Y.L2Norm()) * (1.0 - _Y.L2Norm() * _Y.L2Norm()));
                    var gramDet = _G.Determinant();
                    surface += weight * gramDet * dt * dt;
                }
            }

            return surface;
        }

        protected double LengthNumeric(String Base, String Q, int meshSteps = 2500)
        {
            var _Base = Vector<double>.Build.DenseOfArray(GetPoint(Base));
            var _Q = Vector<double>.Build.DenseOfArray(GetPoint(Q));
            var _V = _Q - _Base;
            var span = Matrix<double>.Build.DenseOfColumnVectors(new List<Vector<double>>() { _V });
            var spanT = span.Transpose();
            var spanTspan = spanT * span;
            var dt = 1.0 / (double)meshSteps;
            var length = .0;

            for (int _v = 0; _v < meshSteps; _v++)
            {
                var _Y = _Base + _V * _v * dt;
                var __Y = Matrix<double>.Build.DenseOfRowVectors(new List<Vector<double>>() { _Y });
                var __YT = __Y.Transpose();
                var _G = spanTspan / (1.0 - _Y.L2Norm() * _Y.L2Norm())
                    + spanT * __YT * __Y * span / ((1.0 - _Y.L2Norm() * _Y.L2Norm()) * (1.0 - _Y.L2Norm() * _Y.L2Norm()));
                var gramDet = _G.Determinant();
                length += gramDet * dt;
            }

            return length;
        }

        public static double Angle(double[] x, double[] v, double[] w)
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

        public double[] NormalOnFace(String Base, String Q, String R)
        {
            var _Base = GetPoint(Base);
            var _Q = GetPoint(Q);
            var _R = GetPoint(R);
            var vecQ = _Q.Subtract(_Base);
            var vecR = _R.Subtract(_Base);

            return Normal(_Base, vecQ, vecR); ;
        }

        protected double Angle(String Base, String Q, String R, String S)
        {
            var _Base = GetPoint(Base);
            var normalBaseQR = NormalOnFace(Base, Q, R);
            var normalBaseSQ = NormalOnFace(Base, S, Q);
            return Math.PI - Angle(_Base, normalBaseQR, normalBaseSQ);
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

        public static List<Simplex3D> RandomSamples(int nbSamples, double maxNorm = 1.0)
        {
            var maxHyperbolicNorm = Math.Tanh(maxNorm);
            var rndVectors = VariousHelpers.RandomVectors(nbSamples * 4, 3, maxHyperbolicNorm);
            var ret = new List<Simplex3D>();

            for (int i = 0; i < nbSamples; i++)
            {
                ret.Add(new Simplex3D(rndVectors[4 * i].ToArray(), rndVectors[4 * i + 1].ToArray(), 
                    rndVectors[4 * i + 2].ToArray(), rndVectors[4 * i + 3].ToArray()));
            }

            return ret;
        }

        private static double G(double[] x, double[] v, double[] w)
        {
            var _vT = Matrix<double>.Build.DenseOfColumnArrays(v).Transpose();
            var _w = Matrix<double>.Build.DenseOfColumnArrays(w);
            
            return (_vT * G(x) * _w).ToArray()[0,0];
        }

        private static Matrix<double> G(double[] x)
        {
            var xNorm = x.Norm();
            var y = 1.0 - xNorm * xNorm;
            var id = Matrix<double>.Build.DenseIdentity(3);
            var X = Matrix<double>.Build.DenseOfColumnArrays(x);
            var XT = X.Transpose();

            return id / y + X * XT / (y * y);
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
                var AT_G_e = A_T * G(x) * _e;
                var AT_G_A = A_T * G(x) * A;
                var x_0 = AT_G_A.Solve(AT_G_e);
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

        public static Simplex3D GetStandardSimplex()
        {
            return new Simplex3D(new double[3] { .0, .0, .0 }, new double[3] { .5, .0, .0 }, 
                new double[3] { .0, .5, .0 }, new double[3] { .0, .0, .5 });
        }
    }
}
