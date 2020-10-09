using Unity.Entities;

[UpdateAfter(typeof(MoveTowardsTargetSystem))]
public class DropOffFullBucket : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        var bucketsArrayEntity = GetSingletonEntity<BucketInChain>();
        var bucketsArray = EntityManager.GetBuffer<BucketInChain>(bucketsArrayEntity);
        
        Entities
            .WithName("DropOffFullBucket")
            .WithNone<ChainPosition>()
            .WithoutBurst()
            .ForEach((Entity botEntity, int entityInQueryIndex, ref Target target,
                ref DroppingOffBucket droppingOffBucket, ref HasBucket hasBucket, ref FillingBucket fillingBucket) =>
            {
                if (!droppingOffBucket.DroppingOff || !target.ReachedTarget)
                    return;

                droppingOffBucket.DroppingOff = false;
                // Add the ChainPosition component to the bucket
                Entity bucketEntity = hasBucket.Entity;
                ecb.AddComponent(entityInQueryIndex, bucketEntity, new ChainPosition { Value = 0 });
                ecb.AddComponent(entityInQueryIndex, bucketEntity, new ChainObjectType { Value = ObjectType.Bucket });
                ecb.AddComponent(entityInQueryIndex, bucketEntity, new Speed { Value = 1f });
                ecb.AddComponent(entityInQueryIndex, bucketEntity, new Target() { ReachedTarget = true });
                ecb.AddSharedComponent(entityInQueryIndex, bucketEntity, droppingOffBucket.chain);

                ecb.RemoveComponent<BucketOwner>(entityInQueryIndex, bucketEntity);
                bucketsArray.Add(new BucketInChain() { chainID = droppingOffBucket.chain.chainID, bucketPos = 0, bucketShift = 0 });
                
                ecb.SetComponent(entityInQueryIndex, botEntity, 
                    new HasBucket { PickedUp = false, PickingUpBucket = false, Entity = Entity.Null});
                ecb.SetComponent(entityInQueryIndex, botEntity, 
                    new FillingBucket { Filling = false, Full = false, Entity = Entity.Null});
            }).Schedule();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}