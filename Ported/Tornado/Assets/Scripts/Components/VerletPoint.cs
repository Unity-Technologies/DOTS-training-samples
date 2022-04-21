using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct VerletPoint : IComponentData
    {
        public VerletPoint(VerletPoint other)
        {
            oldPosition = other.oldPosition;
            currentPosition = other.currentPosition;
            anchored = other.anchored;
            neighborCount = other.neighborCount;
            materialID = other.materialID;
        }

        public float3 oldPosition;
        public float3 currentPosition;

        public byte anchored;
        public byte neighborCount;
        public ushort materialID;

    }
}