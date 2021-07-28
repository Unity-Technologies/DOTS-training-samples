using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct Scaling : IComponentData
    {
        public float targetScale;
        public float interpolationMax;
        public float currentInterpolation;
        public float interpolationRate;
    }
}

