using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;

public class ParticleParamsAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject bloodPrefab;
    public GameObject flashPrefab;
    public float speedStretch;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var particleParams = new ParticleParams
        {
            bloodPrefab = conversionSystem.GetPrimaryEntity(this.bloodPrefab),
            flashPrefab = conversionSystem.GetPrimaryEntity(this.flashPrefab),
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
    public Entity flashPrefab;
    public float speedStretch;
}