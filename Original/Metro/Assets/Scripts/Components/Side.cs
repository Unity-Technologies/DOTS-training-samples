using Unity.Entities;

namespace dots_src.Components
{
    [GenerateAuthoringComponent]
    public struct Side : IComponentData
    {
        public bool IsLeft;
    }
}
