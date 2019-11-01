using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;

[UpdateAfter(typeof(Sys_UpdateBars))]
public class ConnectionSystem : JobComponentSystem
{
    public struct KeepThingsTogether : IJobForEach_B<ConnectionBuffer>
    {
        [ReadOnly]
        public ComponentDataFromEntity<BarPoint1> barPoints1;
        [ReadOnly]
        public ComponentDataFromEntity<BarPoint2> barPoints2;
        [NativeDisableParallelForRestriction]
        public ComponentDataFromEntity<BarAveragedPoints1> barAvgPoints1;
        [NativeDisableParallelForRestriction]
        public ComponentDataFromEntity<BarAveragedPoints2> barAvgPoints2;
        public void Execute(DynamicBuffer<ConnectionBuffer> b0)
        {
            float3 averagePosition = float3.zero;
            float numConnections = 0;
            foreach (ConnectionBuffer cb in b0)
            {
                // TODO: try break
                if (cb.entity == Entity.Null) continue;
                if (cb.endpoint == 1)
                {
                    if (barPoints1[cb.entity].neighbors > 0)
                    {
                        averagePosition += barPoints1[cb.entity].pos;
                        numConnections++;
                    }
                }
                else if (cb.endpoint == 2)
                {
                    if (barPoints2[cb.entity].neighbors > 0)
                    {
                        averagePosition += barPoints2[cb.entity].pos;
                        numConnections++;
                    }
                }
            }
            if (numConnections > 0)
            {
                averagePosition /= numConnections;
            }
            foreach (ConnectionBuffer cb in b0)
            {
                // TODO: try break
                if (cb.entity == Entity.Null) continue;
                if (cb.endpoint == 1)
                {
                    if (barPoints1[cb.entity].neighbors > 0)
                    {
                        var tempcb = barAvgPoints1[cb.entity];
                        tempcb.pos = averagePosition;
                        barAvgPoints1[cb.entity] = tempcb;
                    }
                    else
                    {
                        var tempcb = barAvgPoints1[cb.entity];
                        tempcb.pos = barPoints1[cb.entity].pos;
                        barAvgPoints1[cb.entity] = tempcb;
                    }
                }
                else if (cb.endpoint == 2)
                {
                    if (barPoints1[cb.entity].neighbors > 0)
                    {
                        var tempcb = barAvgPoints2[cb.entity];
                        tempcb.pos = averagePosition;
                        barAvgPoints2[cb.entity] = tempcb;
                    }
                    else
                    {
                        var tempcb = barAvgPoints2[cb.entity];
                        tempcb.pos = barPoints2[cb.entity].pos;
                        barAvgPoints2[cb.entity] = tempcb;
                    }
                }
            }
        }
    }
    public struct CopyAverages : IJobForEach<BarPoint1, BarPoint2, BarAveragedPoints1, BarAveragedPoints2>
    {
        public void Execute(ref BarPoint1 bp1, ref BarPoint2 bp2, [ReadOnly] ref BarAveragedPoints1 bap1, [ReadOnly] ref BarAveragedPoints2 bap2)
        {
            bp1.pos = bap1.pos;
            bp2.pos = bap2.pos;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var barPoints1 = GetComponentDataFromEntity<BarPoint1>(false);
        var barPoints2 = GetComponentDataFromEntity<BarPoint2>(false);
        var barAvg1 = GetComponentDataFromEntity<BarAveragedPoints1>(false);
        var barAvg2 = GetComponentDataFromEntity<BarAveragedPoints2>(false);
        var job = new KeepThingsTogether()
        {
            barPoints1 = barPoints1,
            barPoints2 = barPoints2,
            barAvgPoints1 = barAvg1,
            barAvgPoints2 = barAvg2
        }.Schedule(this, inputDeps);
        var job2 = new CopyAverages()
        {
        }.Schedule(this, job);
        return job2;
    }
}
