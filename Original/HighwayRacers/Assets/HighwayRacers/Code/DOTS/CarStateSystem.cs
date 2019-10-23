using System.Collections;
using System.Collections.Generic;
using HighwayRacers;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace HighwayRacers
{
    [UpdateBefore(typeof(TryMergeSystem))]
    public class CarStateSystem : JobComponentSystem
    {
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
                var distToCarInFront = proximityData.data.NearestFrontMyLane.Distance;
                switch (state.CurrentState) {
                    case CarState.State.NORMAL:
                        state.LeftSpeed = 0;

                        // if won't merge, match car in front's speed
                        if (proximityData.data.NearestFrontMyLane.CarId != 0 && distToCarInFront < settings.LeftMergeDistance)
                        {
                            targetSpeed = Mathf.Min(targetSpeed, proximityData.data.NearestFrontMyLane.Speed);
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
                        targetSpeed = settings.OvertakeEagerness *settings.DefaultSpeed;
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
