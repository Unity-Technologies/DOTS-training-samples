using Unity.Mathematics;

namespace Magneto.Track
{
    public struct SplineData
    {
        public int3 StartPosition;
        public int3 EndPosition;
        public int3 StartNormal;
        public int3 EndNormal;
        public int3 StartTangent;
        public int3 EndTangent;
        
        public float3 Anchor1;
        public float3 Anchor2;
    }
}