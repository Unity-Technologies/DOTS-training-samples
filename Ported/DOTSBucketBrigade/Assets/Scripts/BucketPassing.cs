using Unity.Entities;
using UnityEngine;

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
                bool success = false;
                if (bucketAction.To != Entity.Null)
                {
                    var toBucket = targetBucketComponent[bucketAction.To];
                    if (toBucket.Target == Entity.Null)
                    {
                        toBucket.Target = bucketAction.Bucket;
                        targetBucketComponent[bucketAction.To] = toBucket;
                        success = true;
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
                            ecb.AddComponent<AvailableBucketTag>(entityInQueryIndex, bucketAction.Bucket);
                        }

                        success = true;
                    }
                }

                if (success)
                {
                    ecb.RemoveComponent<BucketChangeRequest>(entityInQueryIndex,entity );
                }
            }).WithNativeDisableParallelForRestriction(targetBucketComponent).Schedule();
        }
    }
}