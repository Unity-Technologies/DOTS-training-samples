using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
[UpdateBefore(typeof(PhysicsSystem))]
public class GrowSystem : JobComponentSystem
{
    [BurstCompile(FloatMode = FloatMode.Fast)]
    struct GrowSystemJob : IJobForEach<Scale>
    {
        public float deltaTime;

        public void Execute(ref Scale scale)
        {
            scale.Value += (1f - scale.Value) * 0.4f * deltaTime;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new GrowSystemJob();
        job.deltaTime = Time.deltaTime;
        var jobHandle = job.Schedule(this, inputDeps);
        return jobHandle;
    }
}
