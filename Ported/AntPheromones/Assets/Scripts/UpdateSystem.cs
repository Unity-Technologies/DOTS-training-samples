using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class UpdateSystem : JobComponentSystem
{
    EntityQuery m_Group;
    public NativeArray<int> IndexList;
    public NativeArray<int2> Buckets;

    bool init = true;

    protected override void OnCreate()
    {
        m_Group = GetEntityQuery(typeof(AntComponent), typeof(Translation));
    }

    unsafe protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        AntSettings settings = GetSingleton<AntSettings>();

        if (init)
        {
            Buckets = new NativeArray<int2>(settings.mapSize, Allocator.Persistent);
            IndexList = new NativeArray<int>(settings.antCount, Allocator.Persistent);
            init = false;
        }

        var antEntities = m_Group.ToEntityArray(Allocator.TempJob);
        
        // sort ants into buckets for pheromone processing
        var antComponents = new NativeArray<AntComponent>(settings.antCount, Allocator.TempJob);
        var antPositions = new NativeArray<Translation>(settings.antCount, Allocator.TempJob);

        for (int i = 0; i < settings.antCount; i++)
        {
            antComponents[i] = EntityManager.GetComponentData<AntComponent>(antEntities[i]);
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
        return movementSystemJobHandle;
    }
}
