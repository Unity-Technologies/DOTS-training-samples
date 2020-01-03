using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;


[UpdateInGroup(typeof(FixedTimeStepSystemGroup))]
public class HeadingSystem : JobComponentSystem
{
  protected override JobHandle OnUpdate(JobHandle inputDependencies)
  {
    var jobHandle = Entities
      .ForEach((ref Rotation rotation, ref VelocityComponent velocity, in SpeedComponent speed, in HeadingComponent facingAngle) =>
      {
        velocity.Value.x = speed.Value * Mathf.Cos(facingAngle.Value);
        velocity.Value.y = speed.Value * Mathf.Sin(facingAngle.Value);
        rotation.Value = Quaternion.Euler(0f, 0f, facingAngle.Value * Mathf.Rad2Deg);
      })
      .Schedule(inputDependencies);

    return jobHandle;
  }
}

