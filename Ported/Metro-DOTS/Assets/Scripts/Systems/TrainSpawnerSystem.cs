using System.Runtime.CompilerServices;
using Metro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial struct TrainSpawnerSystem : ISystem
{
    EntityQuery m_trackQuery;
    EntityTypeHandle entityHandle;
    Random m_Random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_Random = new Random(6754);
        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAny<Track>();
        m_trackQuery = state.GetEntityQuery(builder);
        entityHandle = state.GetEntityTypeHandle();

        state.RequireForUpdate<Config>();
        state.RequireForUpdate<StationConfig>();
        state.RequireForUpdate<Track>();
        state.RequireForUpdate<StationIDComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var query = SystemAPI.QueryBuilder().WithAll<URPMaterialPropertyBaseColor>().Build();
        var queryMask = query.GetEntityQueryMask();

        // This system will only run once, so the random seed can be hard-coded.
        // Using an arbitrary constant seed makes the behavior deterministic.
        var hue = m_Random.NextFloat();

        // Set the color of the station
        // Helper to create any amount of colors as distinct from each other as possible.
        // The logic behind this approach is detailed at the following address:
        // https://martin.ankerl.com/2009/12/09/how-to-create-random-colors-programmatically/
        URPMaterialPropertyBaseColor RandomColor()
        {
            // Note: if you are not familiar with this concept, this is a "local function".
            // You can search for that term on the internet for more information.

            // 0.618034005f == 2 / (math.sqrt(5) + 1) == inverse of the golden ratio
            hue = (hue + 0.618034005f) % 1;
            var color = Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (Vector4)color };
        }

        var em = state.EntityManager;
        entityHandle.Update(ref state);

        var trackEntities = m_trackQuery.ToEntityArray(Allocator.Temp);

        var config = SystemAPI.GetSingleton<Config>();
        var stationConfig = SystemAPI.GetSingleton<StationConfig>();
        var trains = CollectionHelper.CreateNativeArray<Entity>(trackEntities.Length, Allocator.Temp);
        em.Instantiate(config.TrainEntity, trains);

        int numStations = stationConfig.NumStations;
        int trackIndex = 0;
        foreach (var (transform, train, entity) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<Train>>()
                     .WithEntityAccess())
        {
            var trackEntity = trackEntities[trackIndex++];
            var track = em.GetBuffer<TrackPoint>(trackEntity);
            train.ValueRW.TrackEntity = trackEntity;
            train.ValueRW.Offset = transform.ValueRO.Position;

            // one train per track at the moment, could there be more that one train on a track?
            train.ValueRW.TrainId = trackIndex;

            int trackPointIndex = FindRandomStationOnTrack(track, numStations);
            var trackPoint = track[trackPointIndex];
            bool forward = !(trackPointIndex >= numStations);
            bool startStopped = m_Random.NextBool();

            transform.ValueRW.Position = trackPoint.Position;
            train.ValueRW.TrackPointIndex = trackPointIndex;
            train.ValueRW.StationEntity = trackPoint.Station;
            train.ValueRW.OnPlatformA = em.GetComponentData<Track>(trackEntity).OnPlatformA;
            train.ValueRW.LineID = em.GetComponentData<Track>(trackEntity).LineID;
            train.ValueRW.Forward = forward;

            em.SetComponentEnabled<EnRouteComponent>(entity, !startStopped);
            em.SetComponentEnabled<LoadingComponent>(entity, false);
            em.SetComponentEnabled<UnloadingComponent>(entity, startStopped);
            em.SetComponentEnabled<ArrivingComponent>(entity, false);
            em.SetComponentEnabled<DepartingComponent>(entity, false);

            hue = em.GetComponentData<StationIDComponent>(trackPoint.Station).LineColor;
            ecb.SetComponentForLinkedEntityGroup(entity, queryMask, RandomColor());
        }

        state.Enabled = false;
    }

    [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
    int FindRandomStationOnTrack(DynamicBuffer<TrackPoint> track, int stationCount)
    {
        int randomStationCount = m_Random.NextInt(0, stationCount);
        int currentStationCount = 0;
        for (int i = 0; i < track.Length; i++)
        {
            var trackPoint = track[i];
            if (trackPoint.IsStation)
            {
                if (currentStationCount == randomStationCount)
                    return i;
                currentStationCount++;
            }
        }

        return 0;
    }
}
