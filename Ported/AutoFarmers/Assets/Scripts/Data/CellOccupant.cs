using Unity.Entities;

namespace AutoFarmers
{
    [GenerateAuthoringComponent]
    struct CellOccupant : IComponentData
    {
        public Entity Value;
    }
}