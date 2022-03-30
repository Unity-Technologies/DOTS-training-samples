using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// float distance = (parabola.endPoint - parabola.startPoint).magnitude;
// float duration = distance / Constants.CANNONBALL_SPEED;
// if (duration < .0001f)
// {
//     duration = 1;
// }
public partial class ParabolaAnimationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities
            .ForEach((Entity entity, ref Translation translation, ref NormalizedTime time, in ParabolaData parabolaData) =>
            {
                float newTime = time.value + deltaTime / parabolaData.duration;
                float3 pos = new float3(
                    math.lerp(parabolaData.startPoint.x, parabolaData.endPoint.x, newTime),
                    Parabola.Solve(parabolaData, newTime),
                    math.lerp(parabolaData.startPoint.y, parabolaData.endPoint.y, newTime)
                );
                time.value = newTime;
                translation.Value = pos;
            }).ScheduleParallel();
    }
}
