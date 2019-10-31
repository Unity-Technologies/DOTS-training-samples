using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[Serializable]
public struct BarSpawner : IComponentData
{
    // Fields you can modify in the editor.
    public Entity particlePrefab;
    public int buildingCount;
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class BarSpawnerComponent : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject prefab; //mesh and material
    public int buildingCount;


    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
    {
        gameObjects.Add(prefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new BarSpawner
        {
            // Make an entity reference to the game object/prefab we got from the editor
            particlePrefab = conversionSystem.GetPrimaryEntity(prefab),
            buildingCount = buildingCount,
        };
        dstManager.AddComponentData(entity, data);
    }
}