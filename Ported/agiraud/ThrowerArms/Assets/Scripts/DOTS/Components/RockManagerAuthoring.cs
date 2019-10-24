using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RockManagerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject RockPrefab;
    public int RockCount = 1000;
    public Vector3 velocity;

    public static float RockGravityStrength = 5;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(RockPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var entityPrefab = conversionSystem.TryGetPrimaryEntity(RockPrefab);
        if (entityPrefab == Entity.Null)
            throw new Exception(
                $"Something went wrong while creating an Entity for the rig prefab: {RockPrefab.name}");

        // Here we should add some components to our entity prefab
        dstManager.AddComponentData(entityPrefab, new RockTag());
        dstManager.AddComponentData(entityPrefab, new Scale { Value = 1.0f });
        dstManager.AddComponentData(entityPrefab, new ResetPosition { needReset = true });
        dstManager.AddComponentData(entityPrefab, new Physics { velocity = velocity, angularVelocity = float3.zero, flying = false, GravityStrength = RockGravityStrength});
        dstManager.AddComponentData(entityPrefab, new ForceThrow());
        dstManager.AddComponentData(entityPrefab, new Reserved());

        var spawnerData = new SpawnerData
        {
            // The referenced prefab will be converted due to DeclareReferencedPrefabs.
            // So here we simply map the game object to an entity reference to that prefab.
            EntityPrefab = entityPrefab,
            Count = RockCount
        };
        dstManager.AddComponentData(entity, spawnerData);
    }
}
