using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct Intersection : IComponentData
    {
        public bool IsOccupied;
    }
}
