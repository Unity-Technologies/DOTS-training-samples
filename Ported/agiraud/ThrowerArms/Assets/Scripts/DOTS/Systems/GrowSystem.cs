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
    [BurstCompile]
    struct GrowSystemJob : IJobForEachWithEntity<Scale>
    {
        public float deltaTime;

        public void Execute(Entity entity, int index, ref Scale scale)
        {
            scale.Value += (1f - scale.Value) * 2.0f * deltaTime;
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
