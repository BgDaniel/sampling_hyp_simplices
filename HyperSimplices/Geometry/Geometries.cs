using HyperSimplices.CurvedGeometry;
using HyperSimplices.CurvedGeometry.BeltramiKleinModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.Geometry
{
    public enum CurvatureType
    {
        FLAT,
        NEGATIVE,
        POSITIVE
    }

    public static class Geometries
    {
        public static RiemannianSpace GetSpace(CurvatureType curvatureType, int dim)
        {
            switch(curvatureType)
            {
                case CurvatureType.FLAT:
                    return new EuclideanGeometry(dim);
                case CurvatureType.NEGATIVE:
                    return new BeltramiKlein(dim);
                case CurvatureType.POSITIVE:
                default:
                    throw new NotSupportedException($"Curvature type {curvatureType} is not yet supported!");
            }
        }
    }
}
