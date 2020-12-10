using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

//public class ParticleSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
public class ParticleSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    //public GameObject bloodPrefab;
    //public GameObject flashPrefab;
    public int count;
    public ParticleType.Type type;
    public float velocity;
    public float velocityJitter;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawner = new ParticleSpawner
        {
            //bloodPrefab = conversionSystem.GetPrimaryEntity(bloodPrefab),
            //flashPrefab = conversionSystem.GetPrimaryEntity(flashPrefab),
            type = this.type,
            count = this.count,
            velocity = this.velocity,
            velocityJitter = this.velocityJitter
        };

        dstManager.AddComponent<ParticleSpawner>(entity);
    }
    
    /*
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(this.bloodPrefab);
        referencedPrefabs.Add(this.flashPrefab);
    }
    */
}


public struct ParticleSpawner : IComponentData
{
    //public Entity bloodPrefab;
    //public Entity flashPrefab;
    public int count;
    public ParticleType.Type type;
    public float3 velocity;
    public float velocityJitter;
}