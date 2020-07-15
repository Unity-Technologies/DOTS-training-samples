using Unity.Entities;

namespace FireBrigade.Components
{
    [GenerateAuthoringComponent]
    public struct FirefighterSpawner : IComponentData
    {
        public Entity firefighterPrefab;
        public int numPerGroup;
        public int minGroups;
        public int maxGroups;
    }
}