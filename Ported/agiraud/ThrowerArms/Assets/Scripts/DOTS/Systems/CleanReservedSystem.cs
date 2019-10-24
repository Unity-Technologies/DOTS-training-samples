using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
public class CleanReservedSystem : JobComponentSystem
{
    [BurstCompile]
    struct CleanReservedSystemJob : IJobForEach<Reserved>
    {
        public float deltaTime;
        public float MaxTime;

        public void Execute(ref Reserved reserved)
        {
            if (!reserved.reserved) return;
            
            reserved.Time += deltaTime;
            if (reserved.Time > MaxTime)
            {
                reserved.reserved = false;
                reserved.Time = 0f;
            }
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new CleanReservedSystemJob();
        job.deltaTime = Time.deltaTime;
        job.MaxTime = 2f;
        return job.Schedule(this, inputDependencies);
    }
}