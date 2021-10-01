using Unity.Entities;

namespace dots_src.Components
{
    [GenerateAuthoringComponent]
    public struct PlatformRef : IComponentData
    {
        public Entity Platform;
    }
}
