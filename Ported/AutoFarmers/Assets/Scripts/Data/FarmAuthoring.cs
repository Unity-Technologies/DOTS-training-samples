using Unity.Entities;
using Unity.Mathematics;

namespace AutoFarmers
{
    [GenerateAuthoringComponent]
    struct Farm : IComponentData
    {
        public int2 MapSize;
        public Entity GroundPrefab;
    }
}