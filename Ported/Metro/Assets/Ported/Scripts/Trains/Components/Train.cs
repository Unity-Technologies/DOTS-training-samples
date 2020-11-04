using Unity.Entities;

namespace MetroECS.Trains
{
    [GenerateAuthoringComponent]
    public struct Train : IComponentData
    {
        public float Position;
        public int CarriageCount;
    }
}