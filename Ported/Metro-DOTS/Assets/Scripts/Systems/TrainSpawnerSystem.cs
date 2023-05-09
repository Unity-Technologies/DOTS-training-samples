using Components;
using Metro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct TrainSpawnerSystem : ISystem
{
    private EntityQuery m_Group;
    // EntityQuery myQuery;
    // ComponentTypeHandle<LocalTransform> transformHandle;
    // ComponentTypeHandle<TrainIDComponent> trainIdHandle;
    // EntityTypeHandle entityHandle;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        
        // var builder = new EntityQueryBuilder(Allocator.Temp);
        // builder.WithAll<LocalTransform, TrainIDComponent, EnRouteComponent, LoadingComponent>();
        // builder.WithNone<Banana>();
        // myQuery = state.GetEntityQuery(builder);
    }
    
    public void OnUpdate(ref SystemState state)
    {
        var stationConfig = SystemAPI.GetSingleton<Config>();
        var trains = CollectionHelper.CreateNativeArray<Entity>(1, Allocator.Temp);

        var em = state.EntityManager;
        em.Instantiate(stationConfig.TrainEntity, trains);

        var q = em.CreateEntityQuery(typeof(TrainIDComponent));
        foreach (var entity in q.ToEntityArray(Allocator.Temp))
        {
            var transform = em.GetComponentData<LocalTransform>(entity);
            var train = em.GetComponentData<TrainIDComponent>(entity);
            train.Offset = transform.Position;
            em.SetComponentData(entity, train);
            
            em.SetComponentEnabled<EnRouteComponent>(entity, true);
            em.SetComponentEnabled<LoadingComponent>(entity, false);
        }

        // var query = SystemAPI.Query<RefRO<LocalTransform>, RefRW<TrainIDComponent>, RefRW<EnRouteComponent>, RefRW<LoadingComponent>>();
        // foreach (var (transform, train, enRoute, loading) in query)
        // {
        //     train.ValueRW.Offset = transform.ValueRO.Position;
        // }

        state.Enabled = false;
    }
}
