using System.Collections;
using System.Collections.Generic;
using HighwayRacers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacers
{
    [UpdateBefore(typeof(TryMergeSystem))]
    public class CarStateSystem : JobComponentSystem
    {
        [BurstCompile]
        struct CarStateSystemJob : IJobForEach<CarState, CarSettings, ProximityData>
        {
            public float switchLaneSpeed;
            public float deltaTime;

            public void Execute(
                ref CarState state,
                [ReadOnly]ref CarSettings settings,
                [ReadOnly] ref ProximityData proximityData)
            {
                var targetSpeed = settings.DefaultSpeed;
                switch (state.CurrentState) {
                    case CarState.State.NORMAL:
                        state.LeftSpeed = 0;

                        // Don't hit the guy in front
                        if (proximityData.data.HasFront
                            && proximityData.data.NearestFrontMyLane.Distance < settings.LeftMergeDistance)
                        {
                            targetSpeed = math.min(
                                targetSpeed, proximityData.data.NearestFrontMyLane.Speed);
                        }
                        break;

                    case CarState.State.MERGE_LEFT:

                        state.LeftSpeed = switchLaneSpeed;
                        // detect ending merge
                        if (state.Lane + state.LeftSpeed * deltaTime >= state.TargetLane) {
                            // set veloicty to not overshoot lane
                            state.LeftSpeed = (state.TargetLane - state.Lane) / deltaTime;
                            if (state.Lane >= state.TargetLane) { // end when frame started in the target lane
                                state.CurrentState = CarState.State.OVERTAKING;
                            }
                        }
                        break;

                    case CarState.State.OVERTAKING:
                        targetSpeed = settings.OvertakePercent * settings.DefaultSpeed;
                        state.LeftSpeed = 0;
                        break;

                    case CarState.State.MERGE_RIGHT:
                        state.LeftSpeed = -switchLaneSpeed;
                        // detect ending merge
                        if (state.Lane + state.LeftSpeed * deltaTime <= state.TargetLane) {
                            // set veloicty to not overshoot lane
                            state.LeftSpeed = (state.TargetLane - state.Lane) / deltaTime;
                            if (state.Lane <= state.TargetLane) { // end when frame started in the target lane
                                state.CurrentState = CarState.State.NORMAL;
                            }
                        }
                        break;
                }
                state.TargetFwdSpeed = targetSpeed;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new CarStateSystemJob() { switchLaneSpeed =  Game.instance.switchLanesSpeed, deltaTime =  Time.deltaTime};
            return job.Schedule(this, inputDeps);
        }
    }
}
