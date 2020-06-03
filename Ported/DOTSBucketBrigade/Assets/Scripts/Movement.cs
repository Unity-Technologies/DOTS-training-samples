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
            
            Entities.ForEach((ref Translation currentPosition, in TargetPosition targetPosition)
                =>
            {
                var currentMinsTarget = targetPosition.Target - currentPosition.Value;
                if (math.lengthsq(currentMinsTarget) > 0.005f)
                {
                    var direction = math.normalize(currentMinsTarget);
                    currentPosition.Value += direction * config.AgentSpeed * dt;
                }
            }).ScheduleParallel();
        }
    }
}