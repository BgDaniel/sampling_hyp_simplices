using FileHelpers;
using HyperSimplices.SimplicialGeometry.Simplex;
using MathNet.Numerics.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ShearTriangle2D()
        {

        }

        public ShearTriangle2D(double[] B, double[] C)
        {
            m_B = B;
            m_C = C;

            Surface = Math.PI - Simplex3D.Angle(m_A, m_B, m_C)
                - Simplex3D.Angle(m_B, m_A.Subtract(m_B), m_C.Subtract(m_B))
                - Simplex3D.Angle(m_C, m_A.Subtract(m_C), m_B.Subtract(m_C));
        }

        public static List<Tuple<ShearTriangle2D, ShearTriangle2D>> RandomSamples(int nbSamples)
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
