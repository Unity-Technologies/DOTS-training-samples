using System.Collections;
using System.Collections.Generic;
using HighwayRacers;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
[UpdateBefore(typeof(TryMergeSystem))]
public class CarStateSystem : JobComponentSystem
{
    struct CarStateSystemJob : IJobForEach<CarState,CarStateOnTrack,CarSettings, ProximityData>
    {
        public float switchLaneSpeed;
        public float deltaTime;
        
        public void Execute(ref CarState state,[ReadOnly] ref CarStateOnTrack trackState, [ReadOnly]ref CarSettings settings, [ReadOnly] ref ProximityData proximityData)
        {
            var targetSpeed = settings.DefaultSpeed;
            var distToCarInFront = proximityData.NearestFrontMyLane;
            switch (state.CurrentState) {
                case CarState.State.NORMAL:
                    state.LeftSpeed = 0;

                    // if won't merge, match car in front's speed
                    if (distToCarInFront < settings.LeftMergeDistance) {
                        targetSpeed = Mathf.Min(targetSpeed, proximityData.NearestFrontMyLaneSpeed);
                    }

                    break;

                case CarState.State.MERGE_LEFT:

                    state.LeftSpeed = switchLaneSpeed;
                    // detect ending merge
                    if (trackState.Lane + state.LeftSpeed * deltaTime >= trackState.TargetLane) {
                        // set veloicty to not overshoot lane
                        state.LeftSpeed = (trackState.TargetLane - trackState.Lane) / deltaTime;
                        if (trackState.Lane >= trackState.TargetLane) { // end when frame started in the target lane
                            state.CurrentState = CarState.State.OVERTAKING;
                        }
                    }

                    break;

                case CarState.State.OVERTAKING:
                    state.LeftSpeed = 0;

                    break;

                case CarState.State.MERGE_RIGHT:

                    state.LeftSpeed = -switchLaneSpeed;
                    // detect ending merge
                    if (trackState.Lane + state.LeftSpeed * deltaTime <= trackState.TargetLane) {
                        // set veloicty to not overshoot lane
                        state.LeftSpeed = (trackState.TargetLane - trackState.Lane) / deltaTime;
                        if (trackState.Lane <= trackState.TargetLane) { // end when frame started in the target lane
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
