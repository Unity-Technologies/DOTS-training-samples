using Components;
using Metro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct TrainSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<StationIDComponent>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var trains = CollectionHelper.CreateNativeArray<Entity>(1, Allocator.Temp);

        var em = state.EntityManager;
        em.Instantiate(config.TrainEntity, trains);

        foreach (var entity in trains)
        {
            var track = SystemAPI.GetSingletonBuffer<TrackPoint>(true);
            TrackPoint currentPoint = track[0];

            var transform = em.GetComponentData<LocalTransform>(entity);
            
            em.SetComponentData(entity, transform);
            var train = em.GetComponentData<Train>(entity);
            
            train.Offset = transform.Position;
            transform.Position = currentPoint.Position;

            em.SetComponentData(entity, train);
            em.SetComponentData(entity, transform);
            
            em.SetComponentEnabled<EnRouteComponent>(entity, true);
            em.SetComponentEnabled<LoadingComponent>(entity, false);
        }

        state.Enabled = false;
    }
}
