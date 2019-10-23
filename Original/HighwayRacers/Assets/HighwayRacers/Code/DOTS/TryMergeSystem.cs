using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace HighwayRacers
{
    public class TryMergeSystem : JobComponentSystem
    {
        [BurstCompile]
        struct TryMergeSystemJob : IJobForEach<CarState, ProximityData, CarSettings>
        {
            [ReadOnly] public DotsHighway DotsHighway;
	        public float overtakeMaxDuration;
	        bool IsMerging(CarState.State state)
	        {
		        return state == CarState.State.MERGE_LEFT || state == CarState.State.MERGE_RIGHT;
	        }

	        bool WantsToMergeLeft(
                [ReadOnly] ref CarState trackState,
                [ReadOnly] ref ProximityData proximityData,
                [ReadOnly] ref CarSettings settings)
	        {
                // Is there a lane to the left?
		        if (trackState.Lane >= DotsHighway.NumLanes - 1)
                    return false;

                // Is somebody in front?
                if (proximityData.data.NearestFrontMyLane.CarId == 0)
                    return false;

                // Is he annoying?
                if (proximityData.data.NearestFrontMyLane.Distance
                        > settings.LeftMergeDistance // close enough to car in front
                    || settings.OvertakeEagerness
                        < proximityData.data.NearestFrontMyLane.Speed
                            / settings.DefaultSpeed) // car in front is slow enough
                {
                    return false;
                }

                // Is there room for me to merge left?
                return (proximityData.data.NearestFrontLeft.CarId == 0
                        || proximityData.data.NearestFrontLeft.Distance >= settings.MergeSpace)
                    && (proximityData.data.NearestRearLeft.CarId == 0
                        || proximityData.data.NearestRearLeft.Distance >= settings.MergeSpace);
	        }

	        bool WantsToMergeRight(
                [ReadOnly] ref CarState state,
                [ReadOnly] ref ProximityData proximityData,
                [ReadOnly] ref CarSettings settings)
	        {
		        if (state.Lane == 0 || IsMerging(state.CurrentState))
			        return false;

                // Is there space for me in the right lane?
                if (!(proximityData.data.NearestFrontRight.CarId == 0
                        || proximityData.data.NearestFrontRight.Distance >= settings.MergeSpace)
                    && (proximityData.data.NearestRearRight.CarId == 0
                        || proximityData.data.NearestRearRight.Distance >= settings.MergeSpace))
                {
                    return false; // nope - stay where we are
                }

                // If we were in the right lane, would we want to merge left?
                if ((proximityData.data.NearestFrontRight.Distance < settings.LeftMergeDistance)
                    && (settings.OvertakeEagerness > proximityData.data.NearestFrontRight.Speed / settings.DefaultSpeed))
                {
				    return false; // yes we would, stay where we are
		        }
		        return true;
	        }

            public void Execute(
                ref CarState state,
                [ReadOnly] ref ProximityData proximityData,
                [ReadOnly] ref CarSettings settings)
            {
                // detect merging
			    if (!IsMerging(state.CurrentState))
			    {
				    // detect merging to left lane
				    if (WantsToMergeLeft(ref state, ref proximityData, ref settings))
				    {
					    state.CurrentState = CarState.State.MERGE_LEFT;
					    state.TargetLane = Mathf.Round(state.Lane + 1);
				    }
				    else if (WantsToMergeRight(ref state, ref proximityData,ref settings))
				    {
					    state.CurrentState = CarState.State.MERGE_RIGHT;
					    state.TargetLane = Mathf.Round(state.Lane - 1);
				    }
			    }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new TryMergeSystemJob
            {
                DotsHighway = Highway.instance.DotsHighway,
            };
            var deps = job.Schedule(this, inputDeps);
            DotsHighway.RegisterReaderJob(deps);
            return deps;
        }
    }
}
