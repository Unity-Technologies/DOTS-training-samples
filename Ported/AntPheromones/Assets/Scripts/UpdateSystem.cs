using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class UpdateSystem : JobComponentSystem
{
    private float m_TimeScale = 1.0f;
    EntityQuery m_Group;
    public NativeArray<int> IndexList;
    public NativeArray<int2> Buckets;
    public NativeArray<float> PheromoneMap;
    public NativeArray<AntOutput> AntOutput;
    public Material PheromoneMaterial;
    public Texture2D PheromoneTexture;

    RenderMesh renderMesh;

    Material carryMaterial;
    Material searchMaterial;

    bool init = true;

    protected override void OnCreate()
    {
        m_Group = GetEntityQuery(typeof(AntComponent));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Buckets.Dispose();
        IndexList.Dispose();
        AntOutput.Dispose();
        PheromoneMap.Dispose();
        PheromoneTexture = null;
    }

    protected void Init(AntSettings settings)
    {
        if (!init)
            return;

        Buckets = new NativeArray<int2>(settings.mapSize, Allocator.Persistent);
        IndexList = new NativeArray<int>(settings.antCount, Allocator.Persistent);

        AntOutput = new NativeArray<AntOutput>(settings.antCount, Allocator.Persistent);

        PheromoneTexture = new Texture2D(settings.mapSize, settings.mapSize, TextureFormat.RFloat, false);
        PheromoneTexture.wrapMode = TextureWrapMode.Mirror;
        PheromoneTexture.Apply();

        PheromoneMap = new NativeArray<float>(settings.mapSize * settings.mapSize, Allocator.Persistent);

        searchMaterial = new Material(Shader.Find("Custom/InstancedColor"));
        searchMaterial.color = settings.searchColor;

        carryMaterial = new Material(Shader.Find("Custom/InstancedColor"));
        carryMaterial.color = settings.carryColor;

        init = false;
    }

    protected void UpdateTexture()
    {
        var entities = GetEntityQuery(typeof(PheromoneMapComponent), typeof(RenderMesh)).ToEntityArray(Allocator.TempJob);
        Debug.Assert(entities.Length == 1, "There should only be one PheromoneMapComponent in the scene.");

        renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entities[0]);
        PheromoneMaterial = renderMesh.material;
        PheromoneMaterial.mainTexture = PheromoneTexture;

        //PheromoneMaterial.color = Color.HSVToRGB(UnityEngine.Random.Range(0.0f, 1.0f), 1, 1);
        renderMesh.material = PheromoneMaterial;

        var destData = PheromoneTexture.GetRawTextureData<float>();
        PheromoneMap.CopyTo(destData);
        PheromoneTexture.Apply();
        
        EntityManager.SetSharedComponentData<RenderMesh>(entities[0], renderMesh);
        entities.Dispose();
    }

    [BurstCompile]
    struct SetupAntsJob : IJobForEachWithEntity<AntComponent, Translation>
    {
        [NativeDisableParallelForRestriction] public NativeArray<AntComponent> Ants;
        [NativeDisableParallelForRestriction] public NativeArray<Translation> Translations;

        public void Execute(Entity entity, int index, ref AntComponent ant, ref Translation pos)
        {
            ant.index = index;
            Ants[index] = ant;
            Translations[index] = pos;
        }
    }

    unsafe protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        AntSettings settings = GetSingleton<AntSettings>();
        Init(settings);

        var TimeDelta = Time.DeltaTime * m_TimeScale;
        //for (int i = 0; i < PheromoneMap.Length; i++) PheromoneMap[i] = 0;
        var antEntities = m_Group.ToEntityArray(Allocator.TempJob);
        
        // sort ants into buckets for pheromone processing
        var antComponents = new NativeArray<AntComponent>(settings.antCount, Allocator.TempJob);
        var antPositions = new NativeArray<Translation>(settings.antCount, Allocator.TempJob);

        var antRandomDirections = new NativeArray<float>(settings.antCount, Allocator.TempJob);

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            m_TimeScale = 1f;
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            m_TimeScale = 2f;
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            m_TimeScale = 3f;
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            m_TimeScale = 4f;
        } else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            m_TimeScale = 5f;
        } else if (Input.GetKeyDown(KeyCode.Alpha6)) {
            m_TimeScale = 6f;
        } else if (Input.GetKeyDown(KeyCode.Alpha7)) {
            m_TimeScale = 7f;
        } else if (Input.GetKeyDown(KeyCode.Alpha8)) {
            m_TimeScale = 8f;
        } else if (Input.GetKeyDown(KeyCode.Alpha9)) {
            m_TimeScale = 9f;
        }

        var setupAntsJob = new SetupAntsJob() { Ants = antComponents, Translations = antPositions };
        var setupAntsJobHandle = setupAntsJob.Run(this, inputDeps);

        for (int i = 0; i < settings.antCount; i++)
        {
            var ant = antComponents[i];
            if (ant.stateSwitch)
            {
                var antRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(antEntities[i]);
                antRenderMesh.material = ant.state == 1 ? carryMaterial : searchMaterial;
                EntityManager.SetSharedComponentData<RenderMesh>(antEntities[i], antRenderMesh);
                ant.stateSwitch = false;
            }
        }

        var pheromoneBucketsJob = new UpdatePheromoneBuckets()
        {
            AntCount = settings.antCount,
            MapSize = settings.mapSize,
            Buckets = (int2*)Buckets.GetUnsafePtr(),
            IndexList = (int*)IndexList.GetUnsafePtr(),
            AntEntities = antEntities,
            AntComponents = antComponents,
            AntPositions = antPositions
        };
        var pheromoneBucketsJobHandle = pheromoneBucketsJob.Schedule(inputDeps);

        var steeringJob = new AntSteeringJob()
        {
            ColonyPosition = RuntimeManager.instance.colonyPosition,
            ResourcePosition = RuntimeManager.instance.resourcePosition,
            MapSize = settings.mapSize,
            RandomDirections = antRandomDirections,
            PheromoneMap = PheromoneMap,
            TargetSteeringStrength = settings.goalSteerStrength,
            ObstacleBucketDimensions = RuntimeManager.instance.obstacleBucketDimensions, 
            ObstacleBuckets = RuntimeManager.instance.obstacleBuckets, 
            CachedObstacles = RuntimeManager.instance.cachedObstacles
        };
        var steeringJobHandle = steeringJob.Schedule(this, pheromoneBucketsJobHandle);

        var obstacleJob = new AntObstacleAvoidanceJob
        {
            Dimensions = RuntimeManager.instance.obstacleBucketDimensions, 
            ObstacleBuckets = RuntimeManager.instance.obstacleBuckets, 
            CachedObstacles = RuntimeManager.instance.cachedObstacles,
            LookAheadDistance = 1.5f / settings.bucketResolution
        };
        var obstacleJobHandle = obstacleJob.Schedule(this, steeringJobHandle);

        var inwardOutwardStrengthJob = new AntInwardOutwardStrengthJob()
        {
            ColonyPosition = RuntimeManager.instance.colonyPosition.xy,
            InwardStrength = settings.inwardStrength,
            OutwardStrength = settings.outwardStrength,
        };
        var inwardOutwardStrengthJobHandle = inwardOutwardStrengthJob.Schedule(this, obstacleJobHandle);

        var movementJob = new AntMovementJob()
        {
            TimeDelta = TimeDelta,
            AntMaxSpeed =  settings.antSpeed,
            PheromoneSteeringStrength = settings.pheromoneSteerStrength,
            WallSteeringStrength = settings.wallSteerStrength,
            ColonyPosition = RuntimeManager.instance.colonyPosition.xy,
            ResourcePosition = RuntimeManager.instance.resourcePosition.xy,
            TargetRadius = 4.0f / settings.mapSize,
        };
        var movementSystemJobHandle = movementJob.Schedule(this, inwardOutwardStrengthJobHandle);
        
        var resolvePositionJob = new AntResolvePositionJob()
        {
           CachedObstacles = RuntimeManager.instance.cachedObstacles,
           Dimensions = RuntimeManager.instance.obstacleBucketDimensions,
           ObstacleBuckets = RuntimeManager.instance.obstacleBuckets,
        };
        var resolvePositionJobHandle = resolvePositionJob.Schedule(this, movementSystemJobHandle);

        var antOutputJob = new AntOutputJob()
        {
            TimeDelta = TimeDelta,
            Settings = settings,
            Positions = antPositions,
            Ants = antComponents,
            Output = AntOutput
        };
        var antOutputJobHandle = antOutputJob.Schedule(antComponents.Length, 64, resolvePositionJobHandle);

        var dropPheromonesJob = new DropPheromonesJob()
        {
            Settings = settings,
            Buckets = Buckets,
            IndexList = IndexList,
            AntOutput = AntOutput,
            PheromoneMap = PheromoneMap
        };
        var dropPheromonesJobHandle = dropPheromonesJob.Schedule(Buckets.Length, 1, antOutputJobHandle);
        
        dropPheromonesJobHandle.Complete();

        for (int i = 0; i < PheromoneMap.Length; i++)
        {
            PheromoneMap[i] *= settings.trailDecay;// math.pow(settings.trailDecay, TimeDelta / 60.0f);
        }
        
        UpdateTexture();

        return new JobHandle();
    }
}
