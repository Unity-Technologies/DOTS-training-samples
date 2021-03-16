using Unity.Entities;

namespace src.DOTS.Components
{
    public struct CommuterSpawner : IComponentData
    {
        public Entity commuterPrefab;
        public int amountToSpawn;
    }
}