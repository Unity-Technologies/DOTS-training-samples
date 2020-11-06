using Unity.Mathematics;

namespace Magneto.Track
{
    public struct SplineData
    {
        /// <summary>
        /// The cell coordinate of the intersection this spline starts at.
        /// </summary>
        public int3 StartIntersectionPosition;
        
        /// <summary>
        /// The cell coordinate of the intersection this spline ends at.
        /// </summary>
        public int3 EndIntersectionPosition;

        public float3 StartPosition;
        public float3 EndPosition;
        
        public int3 StartNormal;
        public int3 EndNormal;
        public int3 StartTangent;
        public int3 EndTangent;
        
        public float3 Anchor1;
        public float3 Anchor2;
    }
}