using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace HighwayRacers
{
    public class TryMergeSystem : JobComponentSystem
    {
	    const int k_NumLanes = 4;
        struct TryMergeSystemJob : IJobForEach<CarState, ProximityData, CarSettings>
        {
	        public int numLanes;
	        public float currentTime;

	        bool isMerging(CarState.State state)
	        {
		        return state != CarState.State.MERGE_LEFT && state != CarState.State.MERGE_RIGHT;
	        }

	        bool WantsToMergeLeft(
                ref CarState trackState, ref ProximityData proximityData, ref CarSettings settings)
	        {
		        return trackState.Lane + 1 < numLanes // left lane exists
		               && proximityData.data.NearestFrontMyLane.Distance < settings.LeftMergeDistance // close enough to car in front
		               && settings.OvertakeEagerness >
		               proximityData.data.NearestFrontMyLane.Speed / settings.DefaultSpeed; // car in front is slow enough
	        }

	        bool CanMergeLeft(float lane)
	        {
		        return false; //TODO Highway.instance.CanMergeToLane
	        }

	        bool WantsToMergeRight(
                [ReadOnly] ref CarState state,
                [ReadOnly] ref ProximityData proximityData,
                [ReadOnly] ref CarSettings settings)
	        {
		        // detect merging to right lane
		        var tryMergeRight = state.CurrentState != CarState.State.MERGE_LEFT;
		        tryMergeRight = !(state.Lane - 1 < 0) && tryMergeRight;

		        if (tryMergeRight) {
			        // don't merge if just going to merge back
			        // condition for merging to left lane
			        if (proximityData.data.NearestFrontRight.Distance < settings.LeftMergeDistance// close enough to car in front
			            && settings.OvertakeEagerness > proximityData.data.NearestFrontRight.Speed / settings.DefaultSpeed // car in front is slow enough
			        )
			        {
				        tryMergeRight = false;
			        }

		        }

		        return tryMergeRight;

	        }

	        bool CanMergeRight(float lane)
	        {
		        return false; //TODO Highway.instance.CanMergeToLane
	        }

            public void Execute(
                ref CarState state,
                [ReadOnly] ref ProximityData proximityData,
                [ReadOnly] ref CarSettings settings)
            {
                // detect merging
			    if (!isMerging(state.CurrentState))
			    {
				    // detect merging to left lane
				    if (WantsToMergeLeft(ref state, ref proximityData, ref settings)
                        && CanMergeLeft(state.Lane + 1))
				    {
					    state.CurrentState = CarState.State.MERGE_LEFT;
					    state.TargetLane = Mathf.Round(state.Lane + 1);
					    state.TimeOvertakeCarSet = currentTime;
				    }
				    else if (WantsToMergeRight(ref state, ref proximityData,ref settings)
                        && CanMergeRight(state.Lane - 1))
				    {
					    state.CurrentState = CarState.State.MERGE_RIGHT;
					    state.TargetLane = Mathf.Round(state.Lane - 1);
				    }
			    }

            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new TryMergeSystemJob {numLanes = k_NumLanes, currentTime =  Time.time};
            return job.Schedule(this, inputDeps);
        }
    }
}
