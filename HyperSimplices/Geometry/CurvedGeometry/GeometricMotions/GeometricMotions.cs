using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.Geometry.CurvedGeometry.GeometricMotions
{
    public class GeometricMotions
    {
        public static double[] FromBeltramKleinToPoincareDisc(double[] x)
        {
            var xNorm = x.Norm();
            var scale = 1.0 / (1.0 + Math.Sqrt(1 - xNorm * xNorm));
            return x.Mult(scale);
        }

        public static double[] FromPoincareDiscToBeltramKlein(double[] x)
        {
            var xNorm = x.Norm();
            var scale = 2.0 / (1.0 + xNorm * xNorm);
            return x.Mult(scale);
        }

        public static double[] FromPoincareDiscToHalfPlane(double[] x)
        {
            var u = 2 * x[0] / (x[0] * x[0] + (1.0 - x[1]) * (1.0 - x[1]));
            var y = (1.0 - x[0] * x[0] - x[1] * x[1]) / (x[0] * x[0] + (1.0 - x[1]) * (1.0 - x[1]));
            return new double[2] { u, y };
        }

        public static double[] FromHalfPlaneToPoincareDisc(double[] x)
        {
            var u = 2 * x[0] / (x[0] * x[0] + (1.0 + x[1]) * (1.0 + x[1]));
            var y = (x[0] * x[0] + x[1] * x[1] - 1.0) / (x[0] * x[0] + (1.0 + x[1]) * (1.0 + x[1]));
            return new double[2] { u, y };
        }

        public static double[] FromBeltramKleinToHalfPlane(double[] x)
        {
            var _x = FromBeltramKleinToPoincareDisc(x);
            return FromPoincareDiscToHalfPlane(_x);
        }

        public static double[] FromHalfPlaneToBeltramKlein(double[] x)
        {
            var _x = FromHalfPlaneToPoincareDisc(x);
            return FromPoincareDiscToBeltramKlein(_x);
        }

        public static double[] MoveBToImaginaryUnitInPoincareHalfPlane(double[] B, double[] P)
        {
            return new double[2] { (P[0] - B[0]) / B[1], P[1] / B[1] };
        }

        public static double[] MoveBToZeroInPoincareDisc(double[] B, double[] P)
        {
            var BDotP = B.Dot(P);
            var normB = B.Norm();
            var normP = P.Norm();
            var nom = P.Mult(1.0 - 2.0 * BDotP + normB * normB).Subtract(B.Mult(1.0 - normP * normP)); 
            var denom = 1.0 - 2.0 * BDotP + normB * normB * normP * normP;
            return nom.Mult(1.0 / denom);
        }

        public static double[] MoveBToZeroInBeltramiKlein(double[] B, double[] P)
        {
            var _B = FromBeltramKleinToPoincareDisc(B);
            var _P = FromBeltramKleinToPoincareDisc(P);
            var _Q = MoveBToZeroInPoincareDisc(_B, _P);
            return FromPoincareDiscToBeltramKlein(_Q);
        }
    }
}
