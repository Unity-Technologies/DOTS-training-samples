using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedTimeStepSystemGroup))]
public class VelocitySystem : JobComponentSystem
{
  protected override JobHandle OnUpdate(JobHandle inputDependencies)
  {
    float deltaTime = Time.DeltaTime;

    var jobHandle = Entities
      .ForEach((ref Translation translation, in VelocityComponent velocity) =>
      {
        translation.Value.x += velocity.Value.x * deltaTime;
        translation.Value.y += velocity.Value.y * deltaTime;
      })
      .Schedule(inputDependencies);

    return jobHandle;
  }
}
