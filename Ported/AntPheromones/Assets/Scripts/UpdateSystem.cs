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

        //for (int i = 0; i < PheromoneMap.Length; i++) PheromoneMap[i] = 0;
        var antEntities = m_Group.ToEntityArray(Allocator.TempJob);
        
        // sort ants into buckets for pheromone processing
        var antComponents = new NativeArray<AntComponent>(settings.antCount, Allocator.TempJob);
        var antPositions = new NativeArray<Translation>(settings.antCount, Allocator.TempJob);

        for (int i = 0; i < settings.antCount; i++)
        {
            antComponents[i] = EntityManager.GetComponentData<AntComponent>(antEntities[i]);

            var ant = antComponents[i];
            ant.index = i;
            antComponents[i] = ant;
            EntityManager.SetComponentData(antEntities[i], antComponents[i]);

            antPositions[i] = EntityManager.GetComponentData<Translation>(antEntities[i]);
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

        var movementJob = new AntMovementJob()
        {
            TimeDelta = Time.DeltaTime
        };
        var movementSystemJobHandle = movementJob.Schedule(this, pheromoneBucketsJobHandle);
        
        var obstacleJob = new AntObstacleAvoidanceJob(RuntimeManager.instance.obstacleBucketDimensions, RuntimeManager.instance.obstacleBuckets, RuntimeManager.instance.cachedObstacles);
        var obstacleJobHandle = obstacleJob.Schedule(this, movementSystemJobHandle);

        var antOutputJob = new AntOutputJob()
        {
            TimeDelta = Time.DeltaTime,
            Settings = settings,
            Positions = antPositions,
            Ants = antComponents,
            Output = AntOutput
        };
        var antOutputJobHandle = antOutputJob.Schedule(antComponents.Length, 64, obstacleJobHandle);

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

        UpdateTexture();
        return new JobHandle();
    }
}
