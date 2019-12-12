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
    EntityQuery m_Group;
    public NativeArray<int> IndexList;
    public NativeArray<int2> Buckets;
    public NativeArray<float> PheromoneMap;
    public NativeArray<AntOutput> AntOutput;
    public Material PheromoneMaterial;
    public Texture2D PheromoneTexture;

    RenderMesh renderMesh;

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

    unsafe protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        AntSettings settings = GetSingleton<AntSettings>();
        Init(settings);

        var TimeDelta = Time.DeltaTime;
        //for (int i = 0; i < PheromoneMap.Length; i++) PheromoneMap[i] = 0;
        var antEntities = m_Group.ToEntityArray(Allocator.TempJob);
        
        // sort ants into buckets for pheromone processing
        var antComponents = new NativeArray<AntComponent>(settings.antCount, Allocator.TempJob);
        var antPositions = new NativeArray<Translation>(settings.antCount, Allocator.TempJob);

        var antRandomDirections = new NativeArray<float>(settings.antCount, Allocator.TempJob);

        for (int i = 0; i < settings.antCount; i++)
        {
            antComponents[i] = EntityManager.GetComponentData<AntComponent>(antEntities[i]);

            var ant = antComponents[i];
            ant.index = i;
            antComponents[i] = ant;
            EntityManager.SetComponentData(antEntities[i], antComponents[i]);

            antPositions[i] = EntityManager.GetComponentData<Translation>(antEntities[i]);

            antRandomDirections[i] = UnityEngine.Random.Range(-settings.randomSteering, settings.randomSteering);
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
            LookAheadDistance = 1.5f / settings.mapSize
        };
       
        var obstacleJobHandle = obstacleJob.Schedule(this, steeringJobHandle);

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
        var movementSystemJobHandle = movementJob.Schedule(this, obstacleJobHandle);
        
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
