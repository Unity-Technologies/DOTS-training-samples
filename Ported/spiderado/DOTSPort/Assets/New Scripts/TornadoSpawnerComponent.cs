using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct Spawner : IComponentData
{
    // Fields you can modify in the editor.
    public Entity particlePrefab;
    public int particleCount;
}

[RequiresEntityConversion]
public class TornadoSpawnerComponent : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject prefab;
    public int count;
    
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
            particleCount = count
        };
        dstManager.AddComponentData(entity, data);
        
    }
}
