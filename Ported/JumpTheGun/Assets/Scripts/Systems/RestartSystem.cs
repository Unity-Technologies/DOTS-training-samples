using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Color = UnityEngine.Color;


public partial class RestartSystem : SystemBase
{
    private EntityQuery query;
    private EntityCommandBufferSystem beginEcbSystem;
    
    protected override void OnCreate()
    {
        query = GetEntityQuery(typeof (RestartTag));
        this.RequireForUpdate(query);
        
        beginEcbSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
    } 
    protected override void OnUpdate()
    {
        var ecb = beginEcbSystem.CreateCommandBuffer();
        var ecbParallel = ecb.AsParallelWriter();

        ecb.RemoveComponentForEntityQuery<RestartTag>(query);
        ecb.DestroyEntity(GetSingletonEntity<OccupiedElement>());
        
        Entities
            .WithAny<CannonBallTag, Tank, Brick>()
            .ForEach((int entityInQueryIndex, Entity entity) =>
            {
                ecbParallel.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();
        
        beginEcbSystem.AddJobHandleForProducer(Dependency);
    }
}