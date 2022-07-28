using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    partial struct Intersection:IComponentData
    {
        public bool IsOccupied;
    }
}
