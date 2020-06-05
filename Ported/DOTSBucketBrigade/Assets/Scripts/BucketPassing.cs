using Unity.Entities;

namespace DefaultNamespace
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class BucketPassing : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_Barrier;
        protected override void OnCreate()
        {
            m_Barrier =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var targetBucketComponent = GetComponentDataFromEntity<TargetBucket>();
            var ecb = m_Barrier.CreateCommandBuffer().ToConcurrent();
            
            Entities.ForEach((Entity entity, int entityInQueryIndex, in BucketChangeRequest bucketAction) => 
            {
                if (bucketAction.To != Entity.Null)
                {
                    var toBucket = targetBucketComponent[bucketAction.To];
                    if (toBucket.Target == Entity.Null)
                    {
                        toBucket.Target = bucketAction.Bucket;
                        targetBucketComponent[bucketAction.To] = toBucket;
                    }
                }

                if (bucketAction.From != Entity.Null)
                {
                    var fromBucket = targetBucketComponent[bucketAction.From];
                    if (fromBucket.Target != Entity.Null)
                    {
                        fromBucket.Target = Entity.Null;
                        targetBucketComponent[bucketAction.From] = fromBucket;

                        if (bucketAction.To == Entity.Null)
                        {
                            ecb.AddComponent<AvailableBucketTag>(0, bucketAction.Bucket);
                        }
                    }
                }
            }).WithNativeDisableParallelForRestriction(targetBucketComponent).Schedule();
        }
    }
}