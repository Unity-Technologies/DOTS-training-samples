using Unity.Entities;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct InPlay : IComponentData
    {
    }

    public struct InPause : IComponentData
    {
    }
}
