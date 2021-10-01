using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class BeeSpawnerAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
{
    [UnityRange(0, 10000)] public int InitialBeeCount;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BeeSpawner
        {
            BlueBeeCount = InitialBeeCount,
            RedBeeCount = InitialBeeCount,
        });
    }
}
