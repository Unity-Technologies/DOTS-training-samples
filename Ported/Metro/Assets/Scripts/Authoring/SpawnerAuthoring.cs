using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class SpawnerAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public UnityGameObject LanePrefab;
    [UnityRange(0, 1000)] public int LaneCount;

    public UnityGameObject CarPrefab;
    [UnityRange(0, 1)] public float CarFrequency;

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(LanePrefab);
        referencedPrefabs.Add(CarPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Spawner
        {
            LanePrefab = conversionSystem.GetPrimaryEntity(LanePrefab),
            LaneCount = LaneCount,
            CarPrefab = conversionSystem.GetPrimaryEntity(CarPrefab),
            CarFrequency = CarFrequency
        });
    }
}