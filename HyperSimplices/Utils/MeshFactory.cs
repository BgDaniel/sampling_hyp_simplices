using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSimplices.Utils
{
    public class MeshKey : IEquatable<MeshKey>
    {
        public int Dim { get; private set; }
        public int MeshSteps { get; private set; }

        public MeshKey(int dim, int meshSteps)
        {
            Dim = dim;
            MeshSteps = meshSteps;
        }

        bool IEquatable<MeshKey>.Equals(MeshKey other)
        {
            return other.Dim == Dim && other.MeshSteps == MeshSteps;
        }
    }

    public sealed class MeshFactory
    {
        private Dictionary<MeshKey, List<double[]>> _meshs;
        private static MeshFactory _instance;

        private MeshFactory() 
        {
            _meshs = new Dictionary<MeshKey, List<double[]>>();
        }

        public static MeshFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MeshFactory();
                
                return _instance;
            }
        }

        public List<double[]> GetMesh(int dim, int meshSteps)
        {
            return GetMesh(new MeshKey(dim, meshSteps));
        }

        public List<double[]> GetMesh(MeshKey meshKey)
        {
            if (_meshs.ContainsKey(meshKey))
                return _meshs[meshKey];
            else
            {
                _meshs[meshKey] = VariousHelpers.CreateMesh(meshKey.MeshSteps, meshKey.Dim);
                return _meshs[meshKey];
            }
        }
    }
}
