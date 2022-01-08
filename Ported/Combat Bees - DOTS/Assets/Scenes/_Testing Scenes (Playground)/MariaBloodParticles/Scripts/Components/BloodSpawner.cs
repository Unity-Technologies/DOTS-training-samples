using Unity.Entities;

namespace Combatbees.Testing.Maria
{
    [GenerateAuthoringComponent]
    public struct BloodSpawner : IComponentData
    {
        public Entity bloodEntity;
    }
}