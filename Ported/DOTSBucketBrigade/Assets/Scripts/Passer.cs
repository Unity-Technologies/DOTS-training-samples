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
            
            Entities.WithNone<ScooperState, ThrowerState>().ForEach((ref TargetPosition targetPosition, ref TargetBucket targetBucket, in Translation position, in Agent agent,  in NextInChain nextInChain)
                =>
            {
                var chain = chainComponent[agent.MyChain];
                
                // direction & perp can be computed once when the chain changes 
                var direction = math.normalize(chain.ChainEndPosition - chain.ChainStartPosition);
                var perpendicular = new float3(direction.z, 0f, -direction.x);

                if (targetBucket.Target != Entity.Null && !availableBucketComponent.HasComponent(targetBucket.Target))
                {
                    targetPosition.Target = translationComponent[nextInChain.Next].Value;
                    
                    var nextDistSq = math.distancesq(targetPosition.Target.xz, position.Value.xz);

                    if (nextDistSq < config.MovementTargetReachedThreshold)
                    {
                        var nextInChainTargetBucket = targetBucketComponent[nextInChain.Next];
                        nextInChainTargetBucket.Target = targetBucket.Target;
                        targetBucketComponent[nextInChain.Next] = nextInChainTargetBucket;
                        targetBucket.Target = Entity.Null;
                    }
                }
                else
                {
                    targetPosition.Target = ChainInit.CalculateChainPosition(agent, chain, perpendicular);
                }
            }).WithReadOnly(translationComponent).WithReadOnly(availableBucketComponent).WithNativeDisableParallelForRestriction(targetBucketComponent).WithReadOnly(chainComponent).ScheduleParallel();
        }
    }
}