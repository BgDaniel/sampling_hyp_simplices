﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices
{
    public class StandardSimplexFactory
    {
        private static StandardSimplexFactory m_instance = null;
        private Dictionary<SimplexKey, StandardSimplex> m_standardSimplices;

        private StandardSimplexFactory()
        {
            m_standardSimplices = new Dictionary<SimplexKey, StandardSimplex>();
        }

        public static StandardSimplexFactory Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new StandardSimplexFactory();
                }
                return m_instance;
            }
        }

        public StandardSimplex GetStandardSimplex(int dimension, int meshSteps)
        {
            var simplexKey = new SimplexKey(dimension, meshSteps);

            if (m_standardSimplices.ContainsKey(simplexKey))
                return m_standardSimplices[simplexKey];
            else
            {
                var simplex = new StandardSimplex(simplexKey);
                m_standardSimplices[simplexKey] = simplex;
                return simplex;
            }

        }
    }
}