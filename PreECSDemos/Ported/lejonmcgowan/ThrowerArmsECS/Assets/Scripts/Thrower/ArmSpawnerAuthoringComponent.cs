using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[RequiresEntityConversion]
[AddComponentMenu("ECS Thrower/Hand Renderer")]
public class ArmSpawnerAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject meshPrefab;
    public int numArms;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        float3 position = transform.position;
        quaternion rotation = transform.rotation;
        MeshSpawnComponentData spawnComponent = new MeshSpawnComponentData()
        {
            prefab = conversionSystem.GetPrimaryEntity(meshPrefab),
            position = new Translation()
            {
                Value = position
            },
            right = new float3(transform.right),
            numToSpawn = numArms
        };
        
        //give converted spawner game object component data for initial spawn
        dstManager.AddComponentData(entity, spawnComponent);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(meshPrefab);
    }
}