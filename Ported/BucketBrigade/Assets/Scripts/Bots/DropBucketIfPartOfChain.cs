using Unity.Entities;

[UpdateBefore(typeof(MoveTowardsTargetSystem))]
public class DropBucketIfPartOfChain : SystemBase
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
            .WithName("DroppingBucketIfInChain")
            .WithAll<ChainPosition>()
            .ForEach((Entity botEntity, int entityInQueryIndex,
                ref HasBucket hasBucket) =>
            {
                if (hasBucket.PickedUp)
                {
                    ecb.RemoveComponent<BucketOwner>(entityInQueryIndex, hasBucket.Entity);
                    
                    hasBucket.PickedUp = false;
                    hasBucket.PickingUpBucket = false;
                    hasBucket.Entity = Entity.Null;
                }
            }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}