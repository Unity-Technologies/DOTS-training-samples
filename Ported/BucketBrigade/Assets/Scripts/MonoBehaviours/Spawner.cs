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
    
    // Example Burst job that creates many entities
    [BurstCompatible]
    public struct SpawnJob : IJobParallelFor
    {
        public Entity Prototype;
        public int EntityCount;
        public EntityCommandBuffer.ParallelWriter Ecb;
        public float cellSize;
        public int rows;
        public int columns;

        public void Execute(int index)
        {
            // Clone the Prototype entity to create a new entity.
            var e = Ecb.Instantiate(index, Prototype);
            // Prototype has all correct components up front, can use SetComponent to
            // set values unique to the newly created entity, such as the transform.
            Ecb.SetComponent(index, e, new LocalToWorld { Value = ComputeTransform(index, 1.0f) });
        }

        public float4x4 ComputeTransform(int index, float scale)
        {
            var transform = float4x4.TRS(new float3(Mathf.Ceil(index % rows) * cellSize, 0, Mathf.Ceil(index / rows) * cellSize), Quaternion.identity, new float3(scale, 1.0f, scale));
            
            return transform;
        }
    }

    // Start is called before the first frame update
    //void Start()
    //{
    //    var world = World.DefaultGameObjectInjectionWorld;
    //    var entityManager = world.EntityManager;
    //    
    //    EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
    //    
    //    //Terrain cells description
    //    var renderer = TerrainCellPrefab.GetComponent<Renderer>();
    //    // public RenderMeshDescription(ShadowCastingMode shadowCastingMode, bool receiveShadows = false, MotionVectorGenerationMode motionVectorGenerationMode = MotionVectorGenerationMode.Camera, int layer = 0, uint renderingLayerMask = uint.MaxValue, LightProbeUsage lightProbeUsage = LightProbeUsage.Off, bool staticShadowCaster = false);
    //    var desc = new RenderMeshDescription(shadowCastingMode: ShadowCastingMode.Off, receiveShadows: true);
    //    
    //    // Create empty base entity
    //    var prototype = entityManager.CreateEntity();
    //
    //    RenderMesh rendermesh = new RenderMesh(TerrainCellPrefab.GetComponent<Renderer>(), TerrainCellMesh);
    //      //RenderMesh rendermesh = new RenderMesh { mesh = TerrainCellMesh, material = TerrainCellMaterial };
    //
    //    RenderMeshUtility.AddComponents(
    //        prototype,
    //        entityManager,
    //        desc,
    //        rendermesh);
    //        //new RenderMesh { mesh = TerrainCellMesh, material = TerrainCellMaterial });
    //    
    //    entityManager.AddComponentData(prototype, new LocalToWorld());
    //    NativeArray<Entity> array = new NativeArray<Entity>(rows * columns, Allocator.Persistent);
    //    entityManager.Instantiate(prototype);
    //
    //    //Spawn Terrain Cell
    //    var spawnJob = new SpawnJob
    //    {
    //        Prototype = prototype,
    //        Ecb = ecb.AsParallelWriter(),
    //        EntityCount = rows * columns,
    //        cellSize = cellSize,
    //        rows = rows,
    //        columns = columns
    //    };
    //    
    //    var spawnHandle = spawnJob.Schedule(rows * columns, 128);
    //    spawnHandle.Complete();
    //    
    //    ecb.Playback(entityManager);
    //    ecb.Dispose();
    //    entityManager.DestroyEntity(prototype);
    //}
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
            GridSize = authoring.rows,
            NeutralCol = authoring.colour_fireCell_neutral,
            CoolCol = authoring.colour_fireCell_cool,
            HotCol = authoring.colour_fireCell_hot,
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
            CellSize = authoring.cellSize
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

class BrigadeSpawnerBaker : Baker<Spawner>
{
    public override void Bake(Spawner authoring)
    {
        AddComponent(new BrigadeConfig
        {
            Speed = authoring.Speed
        });
    }
}

