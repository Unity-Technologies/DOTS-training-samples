using Unity.Entities;
using Unity.Mathematics;

namespace AutoFarmers
{
    [GenerateAuthoringComponent]
    public struct CellSize : IComponentData
    {
        public int2 Value;
    }
}