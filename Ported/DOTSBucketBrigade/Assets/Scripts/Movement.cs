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
            var positionComponent = GetComponentDataFromEntity<Translation>();
            var availableBucketComponent = GetComponentDataFromEntity<AvailableBucketTag>();
            
            Entities.ForEach((Entity entity, in TargetPosition targetPosition, in TargetBucket targetBucket)
                =>
            {
                var currentPosition = positionComponent[entity];
                var currentMinsTarget = targetPosition.Target - currentPosition.Value;
                if (math.lengthsq(currentMinsTarget) > config.MovementTargetReachedThreshold)
                {
                    var direction = math.normalize(currentMinsTarget);
                    currentPosition.Value += direction * config.AgentSpeed * dt;
                    positionComponent[entity] = currentPosition;
                }
                
                if (targetBucket.Target != Entity.Null && !availableBucketComponent.HasComponent(targetBucket.Target))
                {
                    var bucketTranslation = positionComponent[targetBucket.Target];
                    bucketTranslation.Value = currentPosition.Value;
                    bucketTranslation.Value.y += config.CarriedBucketHeightOffset;
                    positionComponent[targetBucket.Target] = bucketTranslation;
                }
            }).WithReadOnly(availableBucketComponent).WithNativeDisableParallelForRestriction(positionComponent).ScheduleParallel();
        }
    }
}