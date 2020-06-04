using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DefaultNamespace
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class Movement : SystemBase
    {
        protected override void OnUpdate()
        {
            var config = GetSingleton<BucketBrigadeConfig>();
            var dt = Time.DeltaTime;
            var bucketPositionComponent = GetComponentDataFromEntity<Translation>();
            
            Entities.ForEach((ref Translation currentPosition, in TargetPosition targetPosition, in TargetBucket targetBucket)
                =>
            {
                var currentMinsTarget = targetPosition.Target - currentPosition.Value;
                if (math.lengthsq(currentMinsTarget) > config.MovementTargetReachedThreshold)
                {
                    var direction = math.normalize(currentMinsTarget);
                    currentPosition.Value += direction * config.AgentSpeed * dt;
                }
                
                if (targetBucket.Target != Entity.Null)
                {
                    var bucketTranslation = bucketPositionComponent[targetBucket.Target];
                    bucketTranslation.Value = currentPosition.Value;
                    bucketTranslation.Value.y += 1f;
                    bucketPositionComponent[targetBucket.Target] = bucketTranslation;
                }
            }).WithNativeDisableParallelForRestriction(bucketPositionComponent).ScheduleParallel();
        }
    }
}