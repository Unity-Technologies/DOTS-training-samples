using Unity.Entities;


public partial class CannonBallUpdateSystem : SystemBase
{
    private EntityCommandBufferSystem ecsSystem;
    protected override void OnCreate()
    {
        ecsSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {

        var ecb = ecsSystem.CreateCommandBuffer();
        var ecbParallel = ecb.AsParallelWriter();
        Entities
            .ForEach((int entityInQueryIndex, Entity entity, in CannonBallTag tag, in NormalizedTime time) =>
            {
                if (time.value >= 1.0f)
                {
                    ecbParallel.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
        
            ecsSystem.AddJobHandleForProducer(Dependency);
    }

}