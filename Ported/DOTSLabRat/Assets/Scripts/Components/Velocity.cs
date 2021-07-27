using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct Velocity : IComponentData
    {
        public Direction Direction;
        public float Speed;
    }
}
