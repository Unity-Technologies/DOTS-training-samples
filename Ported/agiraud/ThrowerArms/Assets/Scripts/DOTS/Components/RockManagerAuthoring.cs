using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RockManagerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject RockPrefab;
    public int RockCount = 1000;
    public Vector3 velocity;

    public static float RockGravityStrength = 25;
    public static Vector3 SpawnBoxMin = new Vector3(10, 0f, 0f);
    public static Vector3 SpawnBoxMax = new Vector3(-100, 0f, 0f);
    public static Vector3 MoverInitialVelocity = new Vector3(-1f, 0f, 0f);

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
        var rockTag = new RockTag();
        dstManager.AddComponentData(entityPrefab, rockTag);
        dstManager.AddComponentData(entityPrefab, new ResetPosition { needReset = true });
        dstManager.AddComponentData(entityPrefab, new Physics { velocity = velocity, angularVelocity = float3.zero, flying = false, GravityStrength = RockGravityStrength});

        // TODO: only add this component when rock is thrown
        // dstManager.AddComponentData(entityPrefab, new FlyingTag());

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
