using System;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Rendering;

struct BucketTeamCollectInfoSingleton : IComponentData
{
};

[BurstCompile]
[UpdateBefore(typeof(FireSystem))]
public class BucketTeamCollectInfoSystem : SystemBase
{
    Entity m_Singleton;

    // jiv fixme: put on Singleton entity
    static public NativeArray<float2> s_ThrowerCoords;
    static public NativeArray<float2> s_FetcherCoords;
    static public NativeArray<float2> s_FetcherCurrentCoords;
    static public NativeArray<float2> s_FetcherTargetCoords;
    static public NativeArray<float2> s_TeamDirection;

    protected override void OnCreate()
    {
        s_ThrowerCoords = new NativeArray<float2>(FireSimConfig.maxTeams, Allocator.Persistent);
        s_FetcherCoords = new NativeArray<float2>(FireSimConfig.maxTeams, Allocator.Persistent);
        s_FetcherCurrentCoords = new NativeArray<float2>(FireSimConfig.maxTeams, Allocator.Persistent);
        s_FetcherTargetCoords = new NativeArray<float2>(FireSimConfig.maxTeams, Allocator.Persistent);
        s_TeamDirection = new NativeArray<float2>(FireSimConfig.maxTeams, Allocator.Persistent);

        ComponentType[] compTypes = {
            ComponentType.ReadOnly<BucketTeamCollectInfoSingleton>()
        };
        m_Singleton = EntityManager.CreateEntity(compTypes);
    }

    protected override void OnDestroy()
    {
        s_ThrowerCoords.Dispose();
        s_FetcherCoords.Dispose();
        s_FetcherCurrentCoords.Dispose();
        s_FetcherTargetCoords.Dispose();
        s_TeamDirection.Dispose();
    }

    protected override void OnUpdate()
    {
        NativeArray<float2> throwerCoords        = s_ThrowerCoords;
        NativeArray<float2> fetcherCoords        = s_FetcherCoords;
        NativeArray<float2> fetcherCurrentCoords = s_FetcherCurrentCoords;
        NativeArray<float2> fetcherTargetCoords  = s_FetcherTargetCoords;
        NativeArray<float2> teamDirection        = s_TeamDirection;

        Entities.WithAll<Fetcher>().ForEach((in Position position, in TeamIndex teamIndex) =>
        {
            fetcherTargetCoords[teamIndex.Value] = position.coord;
        }).Run();

        Entities.WithAll<Fetcher>().WithNone<MovingBot, FetcherFindWaterSource>().ForEach((in Position position, in TeamIndex teamIndex) =>
        {
            fetcherCurrentCoords[teamIndex.Value] = position.coord;
        }).Run();

        Entities.WithAll<Thrower>().ForEach((in Thrower thrower, in TeamIndex teamIndex) =>
        {
            throwerCoords[teamIndex.Value] = thrower.GridPosition;
            teamDirection[teamIndex.Value] = thrower.GridPosition - fetcherCoords[teamIndex.Value];
        }).Run();

        int numTeams = FireSimConfig.maxTeams;
		float currentDeltaTime = Time.DeltaTime;
        float botSpeed = FireSimConfig.kBotSpeed;

        Entities
            .WithReadOnly(fetcherCurrentCoords)
            .ForEach((in BucketTeamCollectInfoSingleton bucketTeamCollectInfoSingleton) =>
        {
            for (int i=0; i<numTeams; ++i)
            {
                float2 start = fetcherCurrentCoords[i];
                float2 dir = fetcherTargetCoords[i] - start;
                float ls = math.lengthsq(dir);
                if (ls >= math.EPSILON)
                {
                    float2 dirn = math.normalize(dir);
                    fetcherCoords[i] = start + dirn * currentDeltaTime * botSpeed;
                }
            }
        }).Run();
    }
}
