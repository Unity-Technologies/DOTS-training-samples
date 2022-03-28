using Unity.Entities;

namespace Components
{
    public struct Link : IComponentData
    {
        public int startIndex;
        public int endIndex;
        public float length;
    }
}