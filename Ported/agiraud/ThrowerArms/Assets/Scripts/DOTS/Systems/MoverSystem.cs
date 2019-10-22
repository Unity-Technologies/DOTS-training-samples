using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public class MoverSystem : JobComponentSystem
{
    [BurstCompile]
    struct MoverSystemJob : IJobForEach<Mover, Translation>
    {
        public float deltaTime;

        public void Execute([ReadOnly] ref Mover mover, ref Translation translation)
        {
            translation.Value += mover.velocity * deltaTime;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new MoverSystemJob();
        job.deltaTime = Time.deltaTime;
        return job.Schedule(this, inputDeps);
    }
}
