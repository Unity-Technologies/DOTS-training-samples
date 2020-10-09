using Unity.Entities;

[UpdateAfter(typeof(MoveTowardsTargetSystem))]
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
            .WithNone<ChainPosition>()
            .ForEach((Entity botEntity, int entityInQueryIndex,
                ref HasBucket hasBucket,
                in Target target) =>
            {
                if (hasBucket.PickingUpBucket && !hasBucket.PickedUp && target.ReachedTarget)
                {
                    hasBucket.PickedUp = true;
                    hasBucket.PickingUpBucket = false;
                    hasBucket.Entity = target.Entity;
                    
                    ecb.AddComponent(entityInQueryIndex, hasBucket.Entity, new BucketOwner {Entity = botEntity});
                }
            }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}