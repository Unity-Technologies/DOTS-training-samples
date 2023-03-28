using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ConfigAuth : MonoBehaviour
{
    [Header("GRID")]
    [Range(0.4f, 3f)]
    [Tooltip("Size of an individual flame. Full grid will be (rows * cellSize)")]
    public float cellSize = 1f;
    [Tooltip("How many cells WIDE the simulation will be")]
    public int rows = 20;
    [Tooltip("How many cells DEEP the simulation will be")]
    public int columns = 20;
    
    [Header("WATER")]
    public GameObject Water;
    [Range(1,5)]
    [Tooltip("Number of cells affected by a bucket of water")]
    public int splashRadius = 3;
    [Tooltip("Water sources will refill by this amount per second")]
    public float refillRate = 0.0001f;
    [Tooltip("Number of water sources")]
    public int waterSourcesCount = 4;

    [Header("BUCKET")]
    public GameObject Bucket;
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
    private float bucketStartingYPosition = 0.1f;
    
    [Header("FIRE")]
    [Tooltip("Prefabs / GroundTile")]
    public GameObject Ground;
    [Tooltip("How many random fires do you want to battle?")]
    public int startingFireCount = 1;
    [Tooltip("How high the flames reach at max temperature")]
    public float maxFlameHeight = 0.1f;
    [Range(0.001f, 10f)]
    public float flickerRate = 0.1f;
    [Range(0f, 1f)]
    public float flickerRange = 0.1f;
    [Tooltip("When temperature reaches *flashpoint* the cell is on fire")]
    public float flashpoint = 0.5f;
    [Tooltip("How far does heat travel?")]
    [Range(1,10)]
    public int heatRadius = 1;
    [Tooltip("How fast will adjascent cells heat up?")]
    public float heatTransferRate = 0.7f;
    [Range(0.0001f, 2f)]
    [Tooltip("How often the fire cells update. 1 = once per second. Lower = faster")]
    public float fireSimUpdateRate = 0.5f;

    [Header("BOTS")]
    public GameObject Bot;
    public GameObject Omniworker;
    [Range(0.0001f, 1f)]
    public float botSpeed = 0.1f;
    [Range(1, 100)]
    public int totalBots = 10;
    [Range(1, 100)]
    public int totalOmniworkers = 10;
    [Range(0.001f, 1f)]
    public float waterCarryAffect = 0.5f;
    public float arriveThreshold= 0.2f;
    private float botStartingYPosition = 0.5f;
    
    [Header("Colours")]
    public Color colour_fireCell_neutral = new Color(125, 202, 117);
    public Color colour_fireCell_cool = new Color(255, 255, 131);
    public Color colour_fireCell_hot = new Color(255, 0, 0);
    public Color colour_bucket_empty;
    public Color colour_bucket_full;

    class Baker : Baker<ConfigAuth>
    {
        public override void Bake(ConfigAuth authoring)
        {
            AddComponent(new Config
            {
                //GRID
                cellSize = authoring.cellSize,
                rows = authoring.rows,
                columns = authoring.columns,
                
                //WATER
                Water = GetEntity(authoring.Water),
                splashRadius = authoring.splashRadius,
                refillRate = authoring.refillRate,
                waterSourcesCount = authoring.waterSourcesCount,
                
                //BUCKET
                Bucket = GetEntity(authoring.Bucket),
                totalBuckets = authoring.totalBuckets,
                bucketCapacity = authoring.bucketCapacity,
                bucketFillRate = authoring.bucketFillRate,
                bucketSize_EMPTY = authoring.bucketSize_EMPTY,
                bucketSize_FULL = authoring.bucketSize_FULL,
                bucketStartingYPosition = authoring.bucketStartingYPosition,
                
                //FIRE
                Ground = GetEntity(authoring.Ground),
                startingFireCount = authoring.startingFireCount,
                maxFlameHeight = authoring.maxFlameHeight,
                flickerRate = authoring.flickerRate,
                flickerRange = authoring.flickerRange,
                flashpoint = authoring.flashpoint,
                heatRadius = authoring.heatRadius,

                //BOT
                Bot = GetEntity(authoring.Bot),
                Omniworker = GetEntity(authoring.Omniworker),
                botSpeed = authoring.botSpeed,
                TotalBots = authoring.totalBots,
                TotalOmniworkers = authoring.totalOmniworkers,
                waterCarryAffect = authoring.waterCarryAffect,
                arriveThreshold = authoring.arriveThreshold,
                botStartingYPosition = authoring.botStartingYPosition,

                //COLORS
                colour_fireCell_neutral = authoring.colour_fireCell_neutral,
                colour_fireCell_cool = authoring.colour_fireCell_cool,
                colour_fireCell_hot = authoring.colour_fireCell_hot,
                colour_bucket_empty = authoring.colour_bucket_empty,
                colour_bucket_full = authoring.colour_bucket_full
            }); ;
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
    public float bucketStartingYPosition;

    public int startingFireCount;
    public float maxFlameHeight;
    public float flickerRate;
    public float flickerRange;
    public float cellSize;
    public int rows;
    public int columns;
    public int heatRadius;
    public float botStartingYPosition;

    public int TotalBots;
    public int TotalOmniworkers;

    public float heatTransferRate;
    public float fireSimUpdateRate;
    public float flashpoint;

    public float botSpeed;
    public float waterCarryAffect;
    public float arriveThreshold;
    public int waterSourcesCount;

    public Entity Bot;
    public Entity Omniworker;
    public Entity Bucket;
    public Entity Ground;
    public Entity Water;

    public Color colour_fireCell_neutral;
    public Color colour_fireCell_cool;
    public Color colour_fireCell_hot;
    public Color colour_bucket_empty;
    public Color colour_bucket_full;
}
