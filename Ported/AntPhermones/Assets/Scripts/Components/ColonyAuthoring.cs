using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

[ChunkSerializable]
public struct Bucket: IBufferElementData
{
    public UnsafeList<float2> obstacles;
}

[Serializable]
public struct Colony: IComponentData
{
    public float antTargetSpeed;
    public float antAccel;
    public int antCount;
    public float antScale;

    public float pheromoneGrowthRate;
    public float pheromoneDecayRate;

    public float obstacleSize;
    public int ringCount;
    public int bucketResolution;
    public float randomSteerStrength;
    public float pheromoneSteerStrength;
    public float pheromoneSteerDistance;
    public float wallSteerStrength;
    public float wallSteerDistance;
    public float resourceSteerStrength;
    public float wallPushbackUnits;

    public float mapSize;

    public Entity homePrefab;
    public Entity obstaclePrefab;
    public Entity antPrefab;
    public Entity resourcePrefab;

    //public NativeArray<UnsafeList<float2>> buckets;
}

public class ColonyAuthoring : MonoBehaviour
{
    public Colony colony;
    public GameObject homePrefab;
    public GameObject obstaclePrefab;
    public GameObject antPrefab;
    public GameObject resourcePrefab;

    class Baker : Baker<ColonyAuthoring>
    {
        public override void Bake(ColonyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            var colony = authoring.colony;

            colony.homePrefab = GetEntity(authoring.homePrefab, TransformUsageFlags.Renderable);
            colony.obstaclePrefab = GetEntity(authoring.obstaclePrefab, TransformUsageFlags.Renderable);
            colony.antPrefab = GetEntity(authoring.antPrefab, TransformUsageFlags.Renderable);
            colony.resourcePrefab = GetEntity(authoring.resourcePrefab, TransformUsageFlags.Renderable);
            AddComponent<Colony>(entity, colony);
            AddBuffer<Bucket>(entity);
        }
    }
}
