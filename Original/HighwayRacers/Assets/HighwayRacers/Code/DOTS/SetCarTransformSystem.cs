using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;

namespace HighwayRacers
{
    [UpdateAfter(typeof(ColorSystem))]
    public class SetCarTransformSystem : JobComponentSystem
    {
        [BurstCompile]
        struct SetTransformJob : IJobForEach<CarState, Translation, Rotation>
        {
            [ReadOnly] public DotsHighway DotsHighway;

            public void Execute(
                [ReadOnly] ref CarState state,
                ref Translation translation,
                ref Rotation rotation)
            {
                DotsHighway.GetWorldPosition(
                    state.PositionOnTrack, state.Lane, out float3 pos, out quaternion q);
                translation.Value = pos;
                rotation.Value = q;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new SetTransformJob
            {
                DotsHighway = Highway.instance.DotsHighway
            };
            return job.Schedule(this, inputDeps);
        }
    }
}
