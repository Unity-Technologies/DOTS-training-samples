using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class TryMergeSystem : JobComponentSystem
{
	const int k_NumLanes = 4;
    struct TryMergeSystemJob : IJobForEach<CarState, CarStateOnTrack, ProximityData, CarSettings>
    {
	    public int numLanes;
	    public float currentTime;
	    
	    bool isMerging(CarState.State state)
	    {
		    return state != CarState.State.MERGE_LEFT && state != CarState.State.MERGE_RIGHT;
	    }

	    bool WantsToMergeLeft(ref CarStateOnTrack trackState, ref ProximityData proximityData, ref CarSettings settings)
	    {
		    return trackState.Lane + 1 < numLanes // left lane exists
		           && proximityData.NearestFrontMyLane < settings.LeftMergeDistance // close enough to car in front
		           && settings.OvertakeEagerness >
		           proximityData.NearestFrontMyLaneSpeed / settings.DefaultSpeed; // car in front is slow enough
	    }

	    bool CanMergeLeft(float lane)
	    {
		    return false; //TODO Highway.instance.CanMergeToLane
	    }

	    bool WantsToMergeRight([ReadOnly] ref CarStateOnTrack trackState, [ReadOnly] ref CarState state, [ReadOnly] ref ProximityData proximityData, [ReadOnly] ref CarSettings settings)
	    {
		    // detect merging to right lane
		    var tryMergeRight = state.CurrentState != CarState.State.MERGE_LEFT;
		    tryMergeRight = !(trackState.Lane - 1 < 0) && tryMergeRight;

		    if (tryMergeRight) {
			    // don't merge if just going to merge back
			    // condition for merging to left lane
			    if (proximityData.NearestFrontRight < settings.LeftMergeDistance// close enough to car in front
			        && settings.OvertakeEagerness > proximityData.NearestFrontRightSpeed / settings.DefaultSpeed // car in front is slow enough
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
	    
        public void Execute(ref CarState state, ref CarStateOnTrack trackState, [ReadOnly] ref ProximityData proximityData, [ReadOnly] ref CarSettings settings)
        {
            // detect merging
			if (!isMerging(state.CurrentState))
			{
				// detect merging to left lane
				if (WantsToMergeLeft(ref trackState,ref proximityData,ref settings) && CanMergeLeft(trackState.Lane + 1))
				{
					state.CurrentState = CarState.State.MERGE_LEFT;
					trackState.TargetLane = Mathf.Round(trackState.Lane + 1);
					trackState.TimeOvertakeCarSet = currentTime;
				}
				else if (WantsToMergeRight(ref trackState, ref state,ref proximityData,ref settings) && CanMergeRight(trackState.Lane - 1))
				{
					state.CurrentState = CarState.State.MERGE_RIGHT;
					trackState.TargetLane = Mathf.Round(trackState.Lane - 1);
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
