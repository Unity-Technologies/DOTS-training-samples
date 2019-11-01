using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;

public class TornadoPositionSystem : JobComponentSystem
{
    [BurstCompile]
    public struct ChangePosition : IJobForEach<TornadoPosition>
    {
        public float time;
        public void Execute(ref TornadoPosition tornadoPos)
        {
            tornadoPos.position = new float3(Mathf.Cos(time / 6f) * 30f, 0,
            Mathf.Sin(time / 6f * 1.618f) * 30f);
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new ChangePosition()
        {
            time = Time.time
        }.Schedule(this, inputDeps);
        return job;
    }
}
