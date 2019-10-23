using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacers
{
    public class CarMoveSystem : JobComponentSystem
    {
        [BurstCompile]
        struct MoveSystemJob : IJobForEach<CarState>
        {
            [ReadOnly] public DotsHighway DotsHighway;
            public float deltaTime;
            public void Execute(ref CarState state)
            {
                //forward position
                var pos = state.PositionOnTrack + state.FwdSpeed * deltaTime;
                //lateral position
                var lane = state.Lane + state.LeftSpeed * deltaTime;
                var roundLane = math.round(lane);
                lane = math.select(lane, roundLane, math.abs(roundLane - lane) < .0001f);

                pos = DotsHighway.GetEquivalentDistance(pos, state.Lane, lane);
                state.PositionOnTrack = pos;
                state.Lane = lane;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new MoveSystemJob
            {
                deltaTime = Time.deltaTime,
                DotsHighway = Highway.instance.DotsHighway
            };
            var deps = job.Schedule(this, inputDeps);
            DotsHighway.RegisterReaderJob(deps);
            return deps;
        }
    }
}
