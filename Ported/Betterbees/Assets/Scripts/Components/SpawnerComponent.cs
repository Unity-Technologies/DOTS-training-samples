using Unity.Entities;

    public struct SpawnerComponent : IComponentData
    {
        public int initialSpawnAmount;
        public Entity beePrefab;
    }