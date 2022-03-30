using Unity.Entities;

namespace Components
{
    public struct Link : IComponentData
    {
        public int point1Index;
        public int point2Index;
        public float length;

        public ushort materialID;
    }
}