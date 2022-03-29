using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct VerletPoints : IComponentData
    {
        public VerletPoints(VerletPoints other)
        {
            oldPosition = other.oldPosition;
            currentPosition = other.currentPosition;
            anchored = other.anchored;
            neighborCount = other.neighborCount;
        }

        public float3 oldPosition;
        public float3 currentPosition;

        public byte anchored;
        public int neighborCount;
    }
}