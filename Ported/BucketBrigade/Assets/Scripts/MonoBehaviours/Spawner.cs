using UnityEngine;
using UnityEngine.Rendering;
using Unity.Jobs;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Rendering;
using Unity.Transforms;
using System.Linq;

public class Spawner : MonoBehaviour
{
    [Header("WATER")]
    [Range(1, 5)]
    [Tooltip("Number of cells affected by a bucket of water")]
    public int splashRadius = 3;
    [Tooltip("Water bucket reduces fire temperature by this amount")]
    public float coolingStrength = 1f;
    [Tooltip("Splash damage of water bucket. (1 = no loss of power over distance)")]
    public float coolingStrength_falloff = 0.75f;
    [Tooltip("Water sources will refill by this amount per second")]
    public float refillRate = 0.0001f;
    [Range(0, 100)]
    public int totalBuckets = 3;
    [Tooltip("How much water does a bucket hold?")]
    public float bucketCapacity = 3f;
    [Tooltip("Buckets fill up by this much per second")]
    public float bucketFillRate = 0.01f;
    [Tooltip("Visual scale of bucket when EMPTY (no effect on water capacity)")]
    public float bucketSize_EMPTY = 0.2f;
    [Tooltip("Visual scale of bucket when FULL (no effect on water capacity)")]
    public float bucketSize_FULL = 0.4f;
    [Range(0, 100)]
    [Tooltip("Visual scale of bucket when FULL (no effect on water capacity)")]
    public int waterCellCount = 10;

    [Header("FIRE")]
    [Tooltip("Terrain Cell Prefab")]
    public GameObject TerrainCellPrefab;
    [Tooltip("Water Cell Prefab")]
    public GameObject WaterCellPrefab;
    [Tooltip("Bucket Prefab")]
    public GameObject BucketPrefab;
    public Entity PrefabTest;
    public Mesh TerrainCellMesh;
    public Material TerrainCellMaterial;
    [Tooltip("Prefabs / FlameCell")]
    public GameObject prefab_flameCell;
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
    private float simulation_WIDTH, simulation_DEPTH;
    [Tooltip("When temperature reaches *flashpoint* the cell is on fire")]
    public float flashpoint = 0.5f;
    [Tooltip("How far does heat travel? Note: Higher heat radius significantly increases CPU usage")]
    public int heatRadius = 1;
    [Tooltip("How fast will adjascent cells heat up?")]
    public float heatTransferRate = 0.7f;

    [Header("FIRE FIGHTER")]
    public GameObject FireFighterPrefab;
    [Range(0, 10)]
    public float Speed = 0.5f;

    [Header("LINE")]
    public int linesCount = 2;
    public int PerLinesCount = 10;

    [Range(0.0001f, 2f)]
    [Tooltip("How often the fire cells update. 1 = once per second. Lower = faster")]
    public float fireSimUpdateRate = 0.5f;

    [Header("Colours")]
    // cell colours
    public Color colour_fireCell_neutral;
    public Color colour_fireCell_cool;
    public Color colour_fireCell_hot;
    // bot colours
    public Color WaterBringersColor;
    public Color BucketBringersColor;
    public Color BucketFillerFetcherColor;
    public Color WaterDumperColor;
    // bucket Colours
    public Color colour_bucket_empty;
    public Color colour_bucket_full;

    // Line ID
    [Header("Fire Fighter lines")]
    public GameObject FireFighterLinePrefab;
    public int FireFighterLineCount;
    public int FireFightersPerLineCount;
    
    // Start is called before the first frame update
    void Start()
    {
    }
}

class WaterSpawnerBaker : Baker<Spawner>
{
    public override void Bake(Spawner authoring)
    {
        AddComponent(new WaterCellConfig
        {
            WaterCellPrefab = GetEntity(authoring.WaterCellPrefab),
            CellCount = authoring.waterCellCount,
            CellSize = authoring.cellSize,
            GridSize = authoring.rows
        });
    }
}

class BucketSpawnerBaker : Baker<Spawner>
{
    public override void Bake(Spawner authoring)
    {
        AddComponent(new BucketConfig
        {
            Prefab = GetEntity(authoring.BucketPrefab),
            Count = authoring.totalBuckets,
            GridSize = authoring.rows,
            CellSize = authoring.cellSize
        });
    }
}

class TerrainCellSpawnerBaker : Baker<Spawner>
{
    public override void Bake(Spawner authoring)
    {
        AddComponent(new TerrainCellConfig
        {
            Prefab = GetEntity(authoring.TerrainCellPrefab),
            CellSize = authoring.cellSize,
            GridSize = authoring.rows
        });
    }
}

class FiremanSpawnerBaker : Baker<Spawner>
{
    public override void Bake(Spawner authoring)
    {
        AddComponent(new FireFighterConfig
        {
            WaterBringersColor = authoring.WaterBringersColor,
            BucketBringersColor = authoring.BucketBringersColor,
            BucketFillerFetcherColor = authoring.BucketFillerFetcherColor,
            WaterDumperColor = authoring.WaterDumperColor,
            FireFighterPrefab = GetEntity(authoring.FireFighterPrefab),
            LinesCount = authoring.linesCount,
            PerLinesCount = authoring.PerLinesCount,
            GridSize = authoring.rows,
            CellSize = authoring.cellSize,
            Speed = authoring.Speed
        });
    }
}

class FireFighterLineSpawnerBaker : Baker<Spawner>
{
    public override void Bake(Spawner authoring)
    {
        AddComponent(new FireFighterLineConfig
        {
            Prefab = GetEntity(authoring.FireFighterLinePrefab),
            Count = authoring.FireFighterLineCount,
            FireFightersPerLine = authoring.FireFightersPerLineCount
        });
    }
}


