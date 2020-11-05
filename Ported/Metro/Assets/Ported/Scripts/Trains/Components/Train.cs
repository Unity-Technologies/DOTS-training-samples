using Unity.Entities;

namespace MetroECS.Trains
{
    public struct Train : IComponentData
    {
        public float Position;
        public int CarriageCount;
        public Entity Path;
    }
}