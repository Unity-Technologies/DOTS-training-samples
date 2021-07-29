using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct Scaling : IComponentData
    {
        public float targetScale;
    }
}

