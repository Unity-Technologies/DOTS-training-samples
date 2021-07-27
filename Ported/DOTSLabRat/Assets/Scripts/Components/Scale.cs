using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRATS
{ 
    public struct Scale : IComponentData
    {
        public float currentScale;
        public float targetScale;
    }
}

