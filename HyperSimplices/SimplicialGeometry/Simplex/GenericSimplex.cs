﻿using HyperSimplices.CurvedGeometry;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.SimplicialGeometry.Simplex
{
    public class GenericSimplex<T> where T : ICloneable
    {
        public Tuple<int, T>[] Edges { get; private set; }
        public int Dim => Edges.Length - 1;
        public int DimAmbiantSpace { get; set; }  
        public T BasePoint
        {
            get
            {
                return Edges[0].Item2;
            }
            set
            {
                BasePoint = value;
            }
        }

        public List<T> EdgesAsSet()
        {
            return Edges.Select(edge => edge.Item2).ToList();
        }

        public RiemannianSpace AmbiantSpace { get; set; }

        public GenericSimplex(Tuple<int, T>[] edges)
        {
            Edges = edges;
        }
    }    
}
