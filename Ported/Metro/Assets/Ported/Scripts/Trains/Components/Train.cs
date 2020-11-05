using Unity.Entities;

namespace MetroECS.Trains
{
    public struct Train : IComponentData
    {
        public const float MAX_SPEED = 0.002f;
        
        public int ID;
        public float Position;
        public int CarriageCount;
        public Entity Path;
    }
}