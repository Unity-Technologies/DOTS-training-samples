using Unity.Entities;
using Random = Unity.Mathematics.Random;

namespace Combatbees.Testing.Maria
{
    [GenerateAuthoringComponent]
    public struct BloodSpawner : IComponentData
    {
        public Entity bloodEntity;
        public Random random;
    }
}