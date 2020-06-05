using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DefaultNamespace
{
    public class Passer : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_Barrier;
        protected override void OnCreate()
        {
            m_Barrier =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        public struct BucketChangeRequest
        {
            public Entity From;
            public Entity To;
            public Entity Bucket;
        }

        protected override void OnUpdate()
        {
            var config = GetSingleton<BucketBrigadeConfig>();
            var chainComponent = GetComponentDataFromEntity<Chain>();
            var availableBucketComponent = GetComponentDataFromEntity<AvailableBucketTag>();
            var translationComponent = GetComponentDataFromEntity<Translation>();
            var targetBucketComponent = GetComponentDataFromEntity<TargetBucket>();
            var ecb = m_Barrier.CreateCommandBuffer().ToConcurrent();
            
            var bucketChangeQueue = new NativeQueue<BucketChangeRequest>(Allocator.TempJob);
            var bucketChangeQueueParallel = bucketChangeQueue.AsParallelWriter();

            // ref TargetBucket targetBucket, in Translation position,
            Entities.WithNone<ScooperState, ThrowerState>().ForEach((Entity entity, int entityInQueryIndex, ref TargetPosition targetPosition, in Agent agent,  in NextInChain nextInChain)
                =>
            {
                var chain = chainComponent[agent.MyChain];
                
                // direction & perp can be computed once when the chain changes 
                var direction = math.normalize(chain.ChainEndPosition - chain.ChainStartPosition) * agent.Forward;
                var perpendicular = new float3(direction.z, 0f, -direction.x);
                var targetBucket = targetBucketComponent[entity];
                var position = translationComponent[entity];

                if (targetBucket.Target != Entity.Null && !availableBucketComponent.HasComponent(targetBucket.Target))
                {
                    if (nextInChain.Next == Entity.Null)
                    {
                        targetPosition.Target = chain.ChainStartPosition;
                    }
                    else
                    {
                        targetPosition.Target = translationComponent[nextInChain.Next].Value;
                    }

                    var nextDistSq = math.distancesq(targetPosition.Target.xz, position.Value.xz);

                    if (nextDistSq < config.MovementTargetReachedThreshold)
                    {
                        if (nextInChain.Next == Entity.Null)
                        {
                            var targetBucketPosition = translationComponent[targetBucket.Target];
                            targetBucketPosition.Value.y -= config.CarriedBucketHeightOffset;
                            translationComponent[targetBucket.Target] = targetBucketPosition;
                            
                            bucketChangeQueueParallel.Enqueue(new BucketChangeRequest { Bucket = targetBucket.Target, From = entity, To = Entity.Null});
                            
                            //targetBucket.Target = Entity.Null;
                            //targetBucketComponent[entity] = targetBucket;
                        }
                        else
                        {
                            var nextInChainTargetBucket = targetBucketComponent[nextInChain.Next];
                            if (nextInChainTargetBucket.Target == Entity.Null)
                            {
                                bucketChangeQueueParallel.Enqueue(new BucketChangeRequest { Bucket = targetBucket.Target, From = entity, To = nextInChain.Next});
                                
                                //nextInChainTargetBucket.Target = targetBucket.Target;
                                //targetBucketComponent[nextInChain.Next] = nextInChainTargetBucket;
                                //targetBucket.Target = Entity.Null;
                                //targetBucketComponent[entity] = targetBucket;
                            }
                        }
                    }
                }
                else
                {
                    if (agent.Forward > 0)
                    {
                        targetPosition.Target = ChainInit.CalculateChainPosition(agent, perpendicular, chain.ChainStartPosition, chain.ChainEndPosition);
                    }
                    else
                    {
                        targetPosition.Target = ChainInit.CalculateChainPosition(agent, perpendicular, chain.ChainEndPosition, chain.ChainStartPosition);
                    }
                }
            })
                .WithNativeDisableParallelForRestriction(translationComponent)
                .WithReadOnly(availableBucketComponent)
                .WithReadOnly(targetBucketComponent)
                //.WithNativeDisableParallelForRestriction(targetBucketComponent)
                .WithReadOnly(chainComponent)
                .ScheduleParallel();

            Job.WithCode(() =>
            {
                while (bucketChangeQueue.TryDequeue(out var bucketAction))
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
                }
            }).WithNativeDisableParallelForRestriction(targetBucketComponent).Schedule();
            
            m_Barrier.AddJobHandleForProducer(Dependency);

            Dependency = bucketChangeQueue.Dispose(Dependency);
        }
    }
}