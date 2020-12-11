using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;

public class ParticleParamsAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject bloodPrefab;
    public float minBloodSize;
    public float maxBloodSize;
    public GameObject flashPrefab;
    public float minFlashSize;
    public float maxFlashSize;
    public float speedStretch;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var particleParams = new ParticleParams
        {
            bloodPrefab = conversionSystem.GetPrimaryEntity(this.bloodPrefab),
            minBloodSize = this.minBloodSize,
            maxBloodSize = this.maxBloodSize,
            flashPrefab = conversionSystem.GetPrimaryEntity(this.flashPrefab),
            minFlashSize = this.minFlashSize,
            maxFlashSize = this.maxFlashSize,
            speedStretch = this.speedStretch
        };

        dstManager.AddComponentData(entity, particleParams);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(this.bloodPrefab);
        referencedPrefabs.Add(this.flashPrefab);
    }
}


public struct ParticleParams : IComponentData
{
    public Entity bloodPrefab;
    public float minBloodSize;
    public float maxBloodSize;
    public Entity flashPrefab;
    public float speedStretch;
    public float minFlashSize;
    public float maxFlashSize;
}