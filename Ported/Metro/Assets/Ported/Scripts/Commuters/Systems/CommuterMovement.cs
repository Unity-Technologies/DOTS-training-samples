using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace MetroECS.Comuting
{
    public class CommuterMovement : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities.ForEach((ref Translation translation, ref DynamicBuffer<MoveTarget> targets, in Commuter commute) =>
            {
                var currentTarget = targets[0];
                if (math.distance(translation.Value, currentTarget.Position) < 0.1f)
                {
                    targets.RemoveAt(0);
                }
                else
                {
                    translation.Value = math.lerp(translation.Value, currentTarget.Position, deltaTime);
                }
            }).ScheduleParallel();
        }
    }
}