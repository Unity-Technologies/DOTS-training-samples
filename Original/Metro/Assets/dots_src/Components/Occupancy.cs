using Unity.Entities;

namespace dots_src.Components
{
    [GenerateAuthoringComponent]
    public struct Occupancy : IComponentData
    {
        public Entity Train;
        public float TimeLeft;
    }
}
