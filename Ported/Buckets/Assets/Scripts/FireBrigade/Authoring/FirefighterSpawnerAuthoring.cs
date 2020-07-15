using Unity.Entities;

namespace FireBrigade.Authoring
{
    [GenerateAuthoringComponent]
    public struct FirefighterSpawnerAuthoring : IComponentData
    {
        public Entity firefighterPrefab;
        public int numPerGroup;
        public int minGroups;
        public int maxGroups;
    }
}