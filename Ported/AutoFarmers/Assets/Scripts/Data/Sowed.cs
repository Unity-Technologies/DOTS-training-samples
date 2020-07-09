using Unity.Entities;

namespace AutoFarmers
{
    [GenerateAuthoringComponent]
    struct Sowed : IComponentData
    {
        public Entity Plant;
    }
}