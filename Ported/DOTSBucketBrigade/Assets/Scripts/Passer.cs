using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DefaultNamespace
{
    public class Passer : SystemBase
    {
        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            var config = GetSingleton<BucketBrigadeConfig>();
            var chainComponent = GetComponentDataFromEntity<Chain>();
            var availableBucketComponent = GetComponentDataFromEntity<AvailableBucketTag>();
            var translationComponent = GetComponentDataFromEntity<Translation>();
            var targetBucketComponent = GetComponentDataFromEntity<TargetBucket>();
            
            // ref TargetBucket targetBucket, in Translation position,
            Entities.WithNone<ScooperState, ThrowerState>().ForEach((Entity entity, ref TargetPosition targetPosition, in Agent agent,  in NextInChain nextInChain)
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
                    targetPosition.Target = translationComponent[nextInChain.Next].Value;
                    
                    var nextDistSq = math.distancesq(targetPosition.Target.xz, position.Value.xz);

                    if (nextDistSq < config.MovementTargetReachedThreshold)
                    {
                        var nextInChainTargetBucket = targetBucketComponent[nextInChain.Next];
                        if (nextInChainTargetBucket.Target == Entity.Null)
                        {
                            nextInChainTargetBucket.Target = targetBucket.Target;
                            targetBucketComponent[nextInChain.Next] = nextInChainTargetBucket;
                            targetBucket.Target = Entity.Null;
                            targetBucketComponent[entity] = targetBucket;
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
            }).WithReadOnly(translationComponent).WithReadOnly(availableBucketComponent).WithNativeDisableParallelForRestriction(targetBucketComponent).WithReadOnly(chainComponent).ScheduleParallel();
        }
    }
}