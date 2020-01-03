using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedTimeStepSystemGroup))]
public class HeadingSystem : JobComponentSystem
{
  protected override JobHandle OnUpdate(JobHandle inputDependencies)
  {
    var jobHandle = Entities
      .ForEach((ref Rotation rotation, ref VelocityComponent velocity, in SpeedComponent speed, in HeadingComponent facingAngle) =>
      {
        velocity.Value.x = speed.Value * math.cos(facingAngle.Value);
        velocity.Value.y = speed.Value * math.sin(facingAngle.Value);
        rotation.Value = quaternion.Euler(new float3(0f, 0f, facingAngle.Value));
      })
      .Schedule(inputDependencies);

    return jobHandle;
  }
}
