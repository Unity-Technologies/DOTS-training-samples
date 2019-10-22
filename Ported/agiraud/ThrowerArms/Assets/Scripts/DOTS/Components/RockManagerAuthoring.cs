using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RockManagerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject RockPrefab;
    public int RockCount = 1000;
    public float RockGravityStrength = 25;

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
        var state = new RockStatus
        {
            Value = RockState.Conveyor
        };
        dstManager.AddComponentData(entityPrefab, state);

        var mover = new Mover
        {
            velocity = 0
        };
        dstManager.AddComponentData(entityPrefab, mover);

        var gravityStength = new GravityStrength
        {
            Value = RockGravityStrength
        };
        dstManager.AddComponentData(entityPrefab, gravityStength);

        dstManager.AddComponentData(entityPrefab, new FlyingTag());

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
