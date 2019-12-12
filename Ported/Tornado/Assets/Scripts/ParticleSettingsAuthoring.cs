using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ParticleSettingsAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject Prefab;
    public float spinRate;
    public float upwardSpeed;
    [Range(0f, 1f)]
    public float minSize;
    [Range(0f, 1f)]
    public float maxSize;

    
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Prefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var particle = new ParticleSettings
        {
            prefab = conversionSystem.GetPrimaryEntity(Prefab),
            spinRate = spinRate,
            upwardSpeed = 6,
            minSize = minSize,
            maxSize = maxSize
        };

        dstManager.AddComponentData(entity, particle);
    }
}