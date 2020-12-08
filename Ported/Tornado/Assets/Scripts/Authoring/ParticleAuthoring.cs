using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class ParticleAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject ParticlePrefab;
    [Range(0, 1000)] public int ParticleCount;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(ParticlePrefab);
    }
    public void Convert(Entity entity, EntityManager dstManager
    , GameObjectConversionSystem conversionSystem)
    {
        // GetPrimaryEntity fetches the entity that resulted from the conversion of
        // the given GameObject, but of course this GameObject needs to be part of
        // the conversion, that's why DeclareReferencedPrefabs is important here.
        dstManager.AddComponentData(entity, new ParticleSpawner
        {
            ParticlePrefab = conversionSystem.GetPrimaryEntity(ParticlePrefab),
            ParticleCount = ParticleCount,
        });
    }
}
