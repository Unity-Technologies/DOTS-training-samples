using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ParticleSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject ParticlePrefab;

    public int ParticleCount;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(ParticlePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ParticleSpawner
        {
            ParticlePrefab = conversionSystem.GetPrimaryEntity(ParticlePrefab),
            ParticleCount = ParticleCount,
        });
    }
}
