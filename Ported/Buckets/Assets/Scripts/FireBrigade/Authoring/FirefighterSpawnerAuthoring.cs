using FireBrigade.Components;
using Unity.Entities;
using UnityEngine;

namespace FireBrigade.Authoring
{
    public class FirefighterSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public GameObject prefab;
        [Min(4)]
        public int firefightersPerGroup = 20;

        [Min(1)] public int minGroups = 1;
        [Min(2)] public int maxGroups = 4;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var spawner = new FirefighterSpawner();
            conversionSystem.DeclareReferencedPrefab(prefab);
            spawner.firefighterPrefab = conversionSystem.GetPrimaryEntity(prefab);
            spawner.maxGroups = maxGroups;
            spawner.minGroups = minGroups;
            spawner.numPerGroup = firefightersPerGroup;
        }
    }
}