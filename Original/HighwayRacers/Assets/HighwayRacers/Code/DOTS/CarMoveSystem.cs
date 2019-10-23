using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacers
{
    public class CarMoveSystem : JobComponentSystem
    {
        struct MoveSystemJob : IJobForEach<CarState>
        {
            [ReadOnly] public DotsHighway DotsHighway;
            public float deltaTime;
            public void Execute(ref CarState state)
            {
                //forward position
                var pos = state.PositionOnTrack + state.FwdSpeed * deltaTime; // TODO: handle wraparond
                state.PositionOnTrack = DotsHighway.WrapDistance(pos, state.Lane);
                state.Lane += state.LeftSpeed * deltaTime;

                //lateral position
                var roundLane = math.round(state.Lane);
                if (math.abs(roundLane - state.Lane) < .0001f)
                {
                    state.Lane = roundLane;
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new MoveSystemJob
            {
                deltaTime = Time.deltaTime,
                DotsHighway = Highway.instance.DotsHighway

            };
            return job.Schedule(this, inputDeps);
        }
    }
}
