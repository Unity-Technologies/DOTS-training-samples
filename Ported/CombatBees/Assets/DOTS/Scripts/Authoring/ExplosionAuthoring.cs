using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ExplosionAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject ExplosionPrefab;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(ExplosionPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Explosion
        {
            ExplosionPrefab = conversionSystem.GetPrimaryEntity(ExplosionPrefab)
        });
    }
}

