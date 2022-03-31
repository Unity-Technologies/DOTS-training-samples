using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct Link : IComponentData
    {
        public int point1Index;
        public int point2Index;
        public float length;

        public float3 direction;
        public byte dirtyRotation;
        public ushort materialID;
    }
}