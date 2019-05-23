using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[Serializable]
public struct Spawner : IComponentData
{
    // Fields you can modify in the editor.
    public Entity particlePrefab;
    public int particleCount;
    public float spawnRadius;
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class SpawnerComponent : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject prefab; //mesh and material
    public int count;
    public float radius; //we spawn particles in a sphere around the position of the spawner


    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
    {
        gameObjects.Add(prefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new Spawner
        {
            // Make an entity reference to the game object/prefab we got from the editor
            particlePrefab = conversionSystem.GetPrimaryEntity(prefab),
            particleCount = count,
            spawnRadius = radius,
        };
        dstManager.AddComponentData(entity, data);
    }
}