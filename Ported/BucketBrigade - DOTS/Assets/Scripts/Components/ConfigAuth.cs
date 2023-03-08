using Unity.Entities;
using UnityEngine;

public class ConfigAuth : MonoBehaviour
{
    [Header("WATER")]
    public GameObject Water;
    [Range(1,5)]
    [Tooltip("Number of cells affected by a bucket of water")]
    public int splashRadius = 3;
    [Tooltip("Water sources will refill by this amount per second")]
    public float refillRate = 0.0001f;
    [Range(0, 100)]
    public int totalBuckets = 3;
    [Tooltip("How much water does a bucket hold?")]
    public float bucketCapacity = 3f;
    [Tooltip("Buckets fill up by this much per second")]
    public float bucketFillRate = 0.01f;
    [Tooltip("Visual scale of bucket when EMPTY (no effect on water capacity)")]
    public float bucketSize_EMPTY= 0.2f;
    [Tooltip("Visual scale of bucket when FULL (no effect on water capacity)")]
    public float bucketSize_FULL= 0.4f;
    public int gridSize;
    
    [Header("FIRE")]
    [Tooltip("Prefabs / FlameCell")]
    public GameObject FlameCell;
    [Tooltip("How many random fires do you want to battle?")]
    public int startingFireCount = 1;
    [Tooltip("How high the flames reach at max temperature")]
    public float maxFlameHeight = 0.1f;
    [Tooltip("Size of an individual flame. Full grid will be (rows * cellSize)")]
    public float cellSize = 0.05f;
    [Tooltip("How many cells WIDE the simulation will be")]
    public int rows = 20;
    [Tooltip("How many cells DEEP the simulation will be")]
    public int columns = 20;
    [Tooltip("How far does heat travel? Note: Higher heat radius significantly increases CPU usafge")]
    public int heatRadius = 1;
    
    [Header("BOTS")]
    public GameObject Bot;
    public GameObject Bucket;
    public GameObject Ground;
    [Range(0.0001f, 1f)]
    public float botSpeed = 0.1f;
    [Range(0.001f, 1f)]
    public float waterCarryAffect = 0.5f;



    class Baker : Baker<ConfigAuth>
    {
        public override void Bake(ConfigAuth authoring)
        {
            AddComponent(new Config
            {
                splashRadius = authoring.splashRadius,
                refillRate = authoring.refillRate,
                totalBuckets = authoring.totalBuckets,
                bucketCapacity = authoring.bucketCapacity,
                bucketFillRate = authoring.bucketFillRate,
                bucketSize_EMPTY = authoring.bucketSize_EMPTY,
                bucketSize_FULL = authoring.bucketSize_FULL,
                gridSize = authoring.gridSize,
                startingFireCount = authoring.startingFireCount,
                maxFlameHeight = authoring.maxFlameHeight,
                cellSize = authoring.cellSize,
                rows = authoring.rows,
                columns = authoring.columns,
                heatRadius = authoring.heatRadius,
                botSpeed = authoring.botSpeed,
                waterCarryAffect = authoring.waterCarryAffect,
                Bot = GetEntity(authoring.Bot),
                Bucket = GetEntity(authoring.Bucket),
                FlameCell = GetEntity(authoring.FlameCell),
                Ground = GetEntity(authoring.Ground),
                Water = GetEntity(authoring.Water)

            });
        }
    }
}

public struct Config : IComponentData
{
    public int splashRadius;
    public float refillRate;
    public int totalBuckets;
    public float bucketCapacity;
    public float bucketFillRate;
    public float bucketSize_EMPTY;
    public float bucketSize_FULL;
    public int gridSize;
    
    public int startingFireCount;
    public float maxFlameHeight;
    public float cellSize;
    public int rows;
    public int columns;
    public int heatRadius;

    public float botSpeed;
    public float waterCarryAffect;


    public Entity Bot;
    public Entity Bucket;
    public Entity FlameCell;
    public Entity Ground;
    public Entity Water;
}
