using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class ConfigAuthoring : MonoBehaviour
{
    #region Prefabs

    [Header("Prefabs")]

    public GameObject bucketPrefab;
    public GameObject botPrefab;
    public GameObject flameCellPrefab;
    public GameObject waterPrefab;

    #endregion

    #region Bot Params

    [Header("Bot Params")]

    [Range(0.0001f,1f)]
    public float botSpeed = 0.1f;
    
    [Range(0.0001f,1f)]
    public float botArriveThreshold = 0.1f;
    
    [Range(0.001f,1f)]
    public float waterCarryEffect = 0.5f;

    public int numOmnibots = 0;

    public Color botScoopColor;
    public Color botFullColor;
    public Color botEmptyColor;
    public Color botThrowColor;
    public Color botOmniColor;

    #endregion

    #region Fire Params
    [Header("Fire Params")]
    [Tooltip("How many random fires do you want to battle?")]
    public int startingFireCount = 1;
    [Tooltip("How high the flames reach at max temperature")]
    public float maxFlameHeight = 0.1f;
    [Tooltip("Size of an individual flame. Full grid will be (rows * cellSize)")]
    [Range(0.001f, 10f)]
    public float flickerRate = 0.1f;
    [Range(0f, 1f)]
    public float flickerRange = 0.1f;
    public float cellSize = 0.05f;
    [Tooltip("How many cells WIDE the simulation will be")]
    public int numRows = 20;
    [Tooltip("How many cells DEEP the simulation will be")]
    public int numColumns = 20;
    [Tooltip("When temperature reaches *flashpoint* the cell is on fire")]
    public float flashpoint = 0.5f;
    [Tooltip("How far does heat travel? Note: Higher heat radius significantly increases CPU usafge")]
    public int heatRadius = 1;
    [Tooltip("How fast will adjascent cells heat up?")]
    public float heatTransferRate = 0.7f;
    
    [Range(0.0001f, 2f)]
    public float fireSimUpdateRate = 0.5f;
    public Color fireNeutralColor;
    public Color fireCoolColor;
    public Color fireHotColor;

    #endregion

    #region Water Params

    [Header("Water Params")]

    [Tooltip("Water sources will refill by this amount per second")]
    public float refillRate = 0.0001f;

    [Range(3f, 10000f)]
    public float maxCapacity = 5f;

    public Color waterColor;

    #endregion

    #region Bucket Params

    [Header("Bucket Params")]

    [Range(1,5)]
    [Tooltip("Number of cells affected by a bucket of water")]
    public int splashRadius = 3;
    [Tooltip("Water bucket reduces fire temperature by this amount")]
    public float coolingStrength = 1f;
    [Tooltip("Splash damage of water bucket. (1 = no loss of power over distance)")]
    public float coolingStrengthFalloff = 0.75f;
    [Range(0, 100)]
    public int totalBuckets = 3;
    [Tooltip("How much water does a bucket hold?")]
    public float bucketCapacity = 3f;
    [Tooltip("Buckets fill up by this much per second")]
    public float bucketFillRate = 0.01f;
    [Tooltip("Visual scale of bucket when EMPTY (no effect on water capacity)")]
    public float bucketSizeEmpty = 0.2f;
    [Tooltip("Visual scale of bucket when FULL (no effect on water capacity)")]
    public float bucketSizeFull = 0.4f;
    public Color bucketEmptyColor;
    public Color bucketFullColor;

    #endregion

    class Baker : Baker <ConfigAuthoring> 
    {

        public override void Bake(ConfigAuthoring authoring)
        {
            AddComponent(new Config
            {
                bucketPrefab = GetEntity(authoring.bucketPrefab),
                botPrefab = GetEntity(authoring.botPrefab),
                flameCellPrefab = GetEntity(authoring.flameCellPrefab),
                waterPrefab = GetEntity(authoring.waterPrefab),
                botSpeed =  authoring.botSpeed,
                botArriveThreshold = authoring.botArriveThreshold,
                waterCarryEffect = authoring.waterCarryEffect,
                numOmnibots = authoring.numOmnibots,
                botScoopColor = authoring.botScoopColor.ToFloat4(),
                botFullColor = authoring.botFullColor.ToFloat4(),
                botEmptyColor = authoring.botEmptyColor.ToFloat4(),
                botThrowColor = authoring.botThrowColor.ToFloat4(),
                botOmniColor = authoring.botOmniColor.ToFloat4(),
                startingFireCount = authoring.startingFireCount,
                maxFlameHeight = authoring.maxFlameHeight,
                flickerRate = authoring.flickerRate,
                flickerRange = authoring.flickerRange,
                cellSize = authoring.cellSize,
                numRows = authoring.numRows,
                numColumns = authoring.numColumns,
                flashpoint = authoring.flashpoint,
                heatRadius = authoring.heatRadius,
                heatTransferRate = authoring.heatTransferRate,
                fireSimUpdateRate = authoring.fireSimUpdateRate,
                fireNeutralColor = authoring.fireNeutralColor.ToFloat4(),
                fireCoolColor = authoring.fireCoolColor.ToFloat4(),
                fireHotColor = authoring.fireHotColor.ToFloat4(),
                splashRadius = authoring.splashRadius,
                coolingStrength = authoring.coolingStrength,
                coolingStrengthFalloff = authoring.coolingStrengthFalloff,
                refillRate = authoring.refillRate,
                maxCapacity = authoring.maxCapacity,
                waterColor = authoring.waterColor.ToFloat4(),
                totalBuckets = authoring.totalBuckets,
                bucketCapacity = authoring.bucketCapacity,
                bucketFillRate = authoring.bucketFillRate,
                bucketSizeEmpty = authoring.bucketSizeEmpty,
                bucketSizeFull = authoring.bucketSizeFull,
                bucketEmptyColor = authoring.bucketEmptyColor.ToFloat4(),
                bucketFullColor = authoring.bucketFullColor.ToFloat4(),
                simulationWidth =  authoring.numRows * authoring.cellSize,
                simulationDepth = authoring.numColumns * authoring.cellSize
            });
            AddBuffer<FlameHeat>();
        }
    }

    public struct Config : IComponentData
    {
    
        public Entity bucketPrefab;
        public Entity botPrefab;
        public Entity flameCellPrefab;
        public Entity waterPrefab;
        public float botSpeed;
        public float botArriveThreshold;
        public float waterCarryEffect;
        public int numOmnibots;
        public float4 botScoopColor;
        public float4 botFullColor;
        public float4 botEmptyColor;
        public float4 botThrowColor;
        public float4 botOmniColor;
        public int startingFireCount;
        public float maxFlameHeight;
        public float flickerRate;
        public float flickerRange;
        public float cellSize;
        public int numRows;
        public int numColumns;
        public float flashpoint;
        public int heatRadius;
        public float heatTransferRate;
        public float fireSimUpdateRate;
        public float4 fireNeutralColor;
        public float4 fireCoolColor;
        public float4 fireHotColor;
        public int splashRadius;
        public float coolingStrength;
        public float coolingStrengthFalloff;
        public float refillRate;
        public float maxCapacity;
        public float4 waterColor;
        public int totalBuckets;
        public float bucketCapacity;
        public float bucketFillRate;
        public float bucketSizeEmpty;
        public float bucketSizeFull;
        public float4 bucketEmptyColor;
        public float4 bucketFullColor;
        public float simulationWidth;
        public float simulationDepth;

    }
    
    public struct FlameHeat : IBufferElementData
    {
        public float Value;
    }
}
