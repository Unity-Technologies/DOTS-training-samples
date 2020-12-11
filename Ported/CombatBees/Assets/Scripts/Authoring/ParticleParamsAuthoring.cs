using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;

public class ParticleParamsAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject bloodPrefab;
    public float minBloodSize;
    public float maxBloodSize;
    public int numberOfBloodParticles;
    public GameObject flashPrefab;
    public float minFlashSize;
    public float maxFlashSize;
    public int numberOfFlashParticles;
    public float speedStretch;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var particleParams = new ParticleParams
        {
            bloodPrefab = conversionSystem.GetPrimaryEntity(this.bloodPrefab),
            minBloodSize = this.minBloodSize,
            maxBloodSize = this.maxBloodSize,
            numberOfBloodParticles = this.numberOfBloodParticles,
            flashPrefab = conversionSystem.GetPrimaryEntity(this.flashPrefab),
            minFlashSize = this.minFlashSize,
            maxFlashSize = this.maxFlashSize,
            numberOfFlashParticles = this.numberOfFlashParticles,
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
    public int numberOfBloodParticles;
    public Entity flashPrefab;
    public int numberOfFlashParticles;
    public float minFlashSize;
    public float maxFlashSize;
    public float speedStretch;
}