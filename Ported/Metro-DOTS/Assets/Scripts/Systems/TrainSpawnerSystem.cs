using Metro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public partial struct TrainSpawnerSystem : ISystem
{
    EntityQuery m_trackQuery;
    EntityTypeHandle entityHandle;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAny<TrackIDComponent>();
        m_trackQuery = state.GetEntityQuery(builder);
        entityHandle = state.GetEntityTypeHandle();

        state.RequireForUpdate<Config>();
        state.RequireForUpdate<TrackIDComponent>();
        state.RequireForUpdate<StationIDComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;
        entityHandle.Update(ref state);

        var trackEntities = m_trackQuery.ToEntityArray(Allocator.Temp);

        var config = SystemAPI.GetSingleton<Config>();
        var trains = CollectionHelper.CreateNativeArray<Entity>(trackEntities.Length, Allocator.Temp);
        em.Instantiate(config.TrainEntity, trains);

        int trackIndex = 0;
        foreach (var (transform, train, entity) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<Train>>()
                     .WithEntityAccess())
        {
            var trackEntity = trackEntities[trackIndex++];
            var track = em.GetBuffer<TrackPoint>(trackEntity);
            train.ValueRW.TrackEntity = trackEntity;
            train.ValueRW.Offset = transform.ValueRO.Position;

            train.ValueRW.TrainId = -1;

            // one train per track at the moment, could there be more that one train on a track?
            train.ValueRW.TrainId = trackIndex;

            for (int i = 0; i < track.Length; i++)
            {
                var trackPoint = track[i];
                if (trackPoint.IsEnd)
                {
                    transform.ValueRW.Position = trackPoint.Position;
                    train.ValueRW.TrackPointIndex = i;
                    train.ValueRW.StationEntity = trackPoint.Station;
                    train.ValueRW.OnPlatformA = em.GetComponentData<Track>(trackEntity).OnPlatformA;

                    bool isStation = trackPoint.IsStation;
                    em.SetComponentEnabled<EnRouteComponent>(entity, !isStation);
                    em.SetComponentEnabled<LoadingComponent>(entity, false);
                    em.SetComponentEnabled<UnloadingComponent>(entity, isStation);
                    em.SetComponentEnabled<ArrivingComponent>(entity, false);
                    em.SetComponentEnabled<DepartingComponent>(entity, false);

                    break;
                }
            }
        }

        state.Enabled = false;
    }
}
