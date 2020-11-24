using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public class BeeControlParamsAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public float minBeeSize;
    public float maxBeeSize;
    public float speedStretch;
    public float rotationStiffness;
    [Space(10)]
    [Range(0f, 1f)]
    public float aggression;
    public float flightJitter;
    public float teamAttraction;
    public float teamRepulsion;
    [Range(0f, 1f)]
    public float damping;
    public float chaseForce;
    public float carryForce;
    public float grabDistance;
    public float attackDistance;
    public float attackForce;
    public float hitDistance;
    public float maxSpawnSpeed;
    [Space(10)]
    public int maxBeeCount;

    public BeeSpawnerAuthoring blueSpawnerPrefab;
    public BeeSpawnerAuthoring yellowSpawnerPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var beeParams = new BeeControlParams
        {
            blueSpawnerPrefab = conversionSystem.GetPrimaryEntity(this.blueSpawnerPrefab),
            yellowSpawnerPrefab = conversionSystem.GetPrimaryEntity(this.yellowSpawnerPrefab),
            minBeeSize = this.minBeeSize,
            maxBeeSize = this.maxBeeSize,
            speedStretch = this.speedStretch,
            rotationStiffness = this.rotationStiffness,
            aggression = this.aggression,
            flightJitter = this.flightJitter,
            teamAttraction = this.teamAttraction,
            teamRepulsion = this.teamRepulsion,
            damping = this.damping,
            chaseForce = this.chaseForce,
            carryForce = this.carryForce,
            grabDistance = this.grabDistance,
            attackDistance = this.attackDistance,
            attackForce = this.attackForce,
            hitDistance = this.hitDistance,
            maxSpawnSpeed = this.maxSpawnSpeed,
            maxBeeCount = this.maxBeeCount
        };

        dstManager.AddComponentData(entity, beeParams);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(this.blueSpawnerPrefab.gameObject);
        referencedPrefabs.Add(this.yellowSpawnerPrefab.gameObject);
    }
}

public struct BeeControlParams : IComponentData
{
    public float minBeeSize;
    public float maxBeeSize;
    public float speedStretch;
    public float rotationStiffness;
    public float aggression;
    public float flightJitter;
    public float teamAttraction;
    public float teamRepulsion;
    public float damping;
    public float chaseForce;
    public float carryForce;
    public float grabDistance;
    public float attackDistance;
    public float attackForce;
    public float hitDistance;
    public float maxSpawnSpeed;
    public int maxBeeCount;

    public Entity blueSpawnerPrefab;
    public Entity yellowSpawnerPrefab;
}
