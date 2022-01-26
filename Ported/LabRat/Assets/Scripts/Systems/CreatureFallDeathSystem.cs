using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class CreatureFallDeathSystem : SystemBase
{
    private EntityCommandBufferSystem mECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        mECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithAll<Creature>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation trans) =>
            {
                if (trans.Value.y < -3.0f)
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        mECBSystem.AddJobHandleForProducer(Dependency);
    }
}