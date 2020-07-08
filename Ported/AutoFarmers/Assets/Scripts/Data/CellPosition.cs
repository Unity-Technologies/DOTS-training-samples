using Unity.Entities;
using Unity.Mathematics;

namespace AutoFarmers
{
    [GenerateAuthoringComponent]
    public struct CellPosition : IComponentData
    {
        public int2 Value;
    }
}