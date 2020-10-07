using Unity.Entities;

[UpdateAfter(typeof(MoveTowardsTargetSystem))]
[UpdateBefore(typeof(StartNextCommandSystem))]
public class PickupTargetBucketSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithName("PickupTargetBucket")
            .ForEach((Entity botEntity, int entityInQueryIndex,
                ref HasBucket hasBucket,
                in Target target, in CurrentBotCommand currentCommand) =>
            {
                if (!hasBucket.Has &&
                    target.ReachedTarget && currentCommand.Command == Command.FindOrKeepBucket)
                {
                    hasBucket.Has = true;
                    hasBucket.Entity = target.Entity;
                    
                    ecb.AddComponent(entityInQueryIndex, hasBucket.Entity, new BucketOwner {Entity = botEntity});
                }
            }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}