using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class TinCanManagerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject TinCanPrefab;
    public int TinCanCount = 1000;
    public static float TinCanGravityStrength = 20;
    public static Vector3 SpawnBoxMin = new Vector3(-10, 4f, 15f);
    public static Vector3 SpawnBoxMax = new Vector3(-100, 16f, 15f);
    public static Vector3 MoverInitialVelocity = new Vector3(1f, 0f, 0f);

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

        dstManager.AddComponent<ResetPosition>(entityPrefab);
        // Start with scale 0 and grow the can
        dstManager.AddComponentData(entityPrefab, new Scale { Value = 0.0f });
        // TODO Add tag and Angular Velocity components only when hit
        // TODO: angular velocity should be init to : can.angularVelocity = Random.onUnitSphere * rockVelocity.magnitude * 40f;
        // dstManager.AddComponentData(entityPrefab, new FlyingTag());
        dstManager.AddComponentData(entityPrefab, new AngularVelocity());

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
