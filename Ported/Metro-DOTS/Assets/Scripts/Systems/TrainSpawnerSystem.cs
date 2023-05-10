using Components;
using Metro;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public partial struct TrainSpawnerSystem : ISystem
{
    EntityQuery m_trackQuery;
    EntityTypeHandle entityHandle;
    
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAny<Track>();
        m_trackQuery = state.GetEntityQuery(builder);
        entityHandle = state.GetEntityTypeHandle();
        
        state.RequireForUpdate<Config>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        entityHandle.Update(ref state);
        var trackEntities = m_trackQuery.ToEntityArray(Allocator.Temp);
        if (trackEntities.Length != 0)
        {
            var stationConfig = SystemAPI.GetSingleton<Config>();
            var trains = CollectionHelper.CreateNativeArray<Entity>(1, Allocator.Temp);

            var em = state.EntityManager;
            em.Instantiate(stationConfig.TrainEntity, trains);

            int trackIndex = 0;
            foreach (var (transform, train, entity) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<TrainIDComponent>>()
                         .WithEntityAccess())
            {
                var trackEntity = trackEntities[trackIndex++];
                var track = em.GetBuffer<TrackPoint>(trackEntity);
                train.ValueRW.TrackEntity = trackEntity;
                train.ValueRW.Offset = transform.ValueRO.Position;

                for (int i = 0; i < track.Length; i++)
                {
                    var trackPoint = track[i];
                    if (trackPoint.IsEnd)
                    {
                        transform.ValueRW.Position = trackPoint.Position;
                        train.ValueRW.TrackPointIndex = i;

                        bool isStation = trackPoint.IsStation;
                        em.SetComponentEnabled<EnRouteComponent>(entity, !isStation);
                        em.SetComponentEnabled<LoadingComponent>(entity, false);
                        em.SetComponentEnabled<UnloadingComponent>(entity, isStation);
                        em.SetComponentEnabled<ArrivingComponent>(entity, false);
                        em.SetComponentEnabled<DepartingComponent>(entity, false);
                    }
                }
            }

            state.Enabled = false;
        }
    }
}
