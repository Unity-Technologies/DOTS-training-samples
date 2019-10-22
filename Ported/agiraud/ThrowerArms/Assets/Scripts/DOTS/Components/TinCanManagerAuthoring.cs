using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TinCanManagerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject TinCanPrefab;
    public int TinCanCount = 1000;
    public static float TinCanGravityStrength = 20;
    
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(TinCanPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var entityPrefab = conversionSystem.TryGetPrimaryEntity(TinCanPrefab);
        if (entityPrefab == Entity.Null)
            throw new Exception($"Something went wrong while creating an Entity for the rig prefab: {TinCanPrefab.name}");

        // Here we should add some components to our entity prefab
        var tintag = new TinCanTag();
        dstManager.AddComponentData(entityPrefab, tintag);
        
        var spawnerData = new SpawnerData
        {
            // The referenced prefab will be converted due to DeclareReferencedPrefabs.
            // So here we simply map the game object to an entity reference to that prefab.
            EntityPrefab = entityPrefab,
            Count = TinCanCount
        };
        dstManager.AddComponentData(entity, spawnerData);
        
    }
}
