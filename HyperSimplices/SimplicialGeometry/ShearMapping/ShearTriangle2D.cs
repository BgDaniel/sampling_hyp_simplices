using FileHelpers;
using HyperSimplices.SimplicialGeometry.Simplex;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;
using System;
using System.Collections.Generic;

namespace HyperSimplices.SimplicialGeometry.Triangle
{
    [DelimitedRecord(";")]
    public class ShearTriangle2D
    {
        [FieldHidden]
        private double[] m_A => new double[3];

        [FieldHidden]
        private double[] m_B;

        [FieldHidden]
        private double[] m_C;

        public double Surface;
        public double Alpha;
        public double Beta;
        public double Gamma;
        public double A;
        public double B;
        public double C;

        public ShearTriangle2D()
        {

        }

        public ShearTriangle2D(double[] _B, double[] _C)
        {
            m_B = _B;
            m_C = _C;

            Alpha = Simplex3D.Angle(m_A, m_B, m_C);
            Beta = Simplex3D.Angle(m_B, m_A.Subtract(m_B), m_C.Subtract(m_B));
            Gamma = Simplex3D.Angle(m_C, m_A.Subtract(m_C), m_B.Subtract(m_C));
            A = Length(m_B, m_C);
            B = Length(m_A, m_C);
            C = Length(m_A, m_B);
            Surface = Math.PI - Alpha - Beta - Gamma;
        }

        protected static double Length(double[] P, double[] Q)
        {
            var _P = Vector<double>.Build.DenseOfArray(P);
            var _Q = Vector<double>.Build.DenseOfArray(Q);

            Func<double, Vector<double>> line_pq = t => _P + (_Q - _P) * t;
            Func<double, double> f = t => line_pq(t).L2Norm() - 1.0;

            var t_A = FindRoots.OfFunction(f, - 3.0 / (_P - _Q).L2Norm(), .0);
            var t_B = FindRoots.OfFunction(f, 1.0, 3.0 / (_P - _Q).L2Norm());

            var _A = line_pq(t_A);
            var _B = line_pq(t_B);

            var aq = (_A - _Q).L2Norm();
            var ap = (_A - _P).L2Norm();
            var pb = (_P - _B).L2Norm();
            var qb = (_Q - _B).L2Norm();

            return .5 * Math.Log((aq * pb) / (ap * qb));
        }

        public static List<ShearTriangle2D> RandomSamples(int nbSamples)
        {
            var samples = new List<ShearTriangle2D>();
            var counter = 0;
            var rnd = new Random();

            while (counter < nbSamples)
            {
                // choose B: (b_x, b_y)
                var b_x = .0;
                var b_y = .0;  
                var B_InDisc = false;

                while (!B_InDisc)
                {
                    b_x = rnd.NextDouble();
                    b_y = rnd.NextDouble();
                    B_InDisc = b_x * b_x + b_y * b_y < 1.0;
                }

                // choose C: (c_x, c_y)                
                var c_x = .0;
                var c_y = .0;
                var C_InDisc = false;

                while (!C_InDisc)
                {
                    c_x = rnd.NextDouble();
                    c_y = rnd.NextDouble();
                    C_InDisc = c_x * c_x + c_y * c_y < 1.0;
                }
              
                samples.Add(new ShearTriangle2D(new double[3] { .0, b_x, b_y }, new double[3] { .0, c_x, c_y }));
                counter++;
            }

            return samples;
        }

        public static List<Tuple<ShearTriangle2D, ShearTriangle2D>> RandomShearSamples(int nbSamples)
        {
            var samples = new List<Tuple<ShearTriangle2D, ShearTriangle2D>>();
            var counter = 0;
            var rnd = new Random();

            while (counter < nbSamples)
            {
                // choose B on y-axis: (0, b)
                var b = rnd.NextDouble();

                // choose C: (c_x, c_y)
                var c_x = rnd.NextDouble();
                double c_y = .0;
                var C_InDisc = false;
                
                while (!C_InDisc)
                {
                    c_y = rnd.NextDouble();
                    C_InDisc = c_x * c_x + c_y * c_y < 1.0;
                }

                // choose C_sheared: (c_x, c_y_sheared)
                double c_y_sheared = .0;
                var C_sheared_InDisc = false;

                while (!C_sheared_InDisc)
                {
                    c_y_sheared = rnd.NextDouble();
                    C_sheared_InDisc = c_x * c_x + c_y_sheared * c_y_sheared < 1.0;
                }

                var triangle = new ShearTriangle2D(new double[3] { .0, .0, b }, new double[3] { .0, c_x, c_y });
                var triangleSheared = new ShearTriangle2D(new double[3] { .0, .0, b }, new double[3] { .0, c_x, c_y_sheared });

                samples.Add(new Tuple<ShearTriangle2D, ShearTriangle2D>(triangle, triangleSheared));
                counter++;
            }

            return samples;
        }
    }
}
