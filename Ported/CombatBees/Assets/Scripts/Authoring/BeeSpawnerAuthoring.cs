using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class BeeSpawnerAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject BlueBeePrefab;
    public UnityGameObject RedBeePrefab;

    [UnityRange(0, 1000)] public int InitialBeeCount;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BeeSpawner
        {
            BlueBeePrefab = conversionSystem.GetPrimaryEntity(BlueBeePrefab),
            RedBeePrefab = conversionSystem.GetPrimaryEntity(RedBeePrefab),
            BlueBeeCount = InitialBeeCount,
            RedBeeCount = InitialBeeCount,
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BlueBeePrefab);
        referencedPrefabs.Add(RedBeePrefab);
    }
}
