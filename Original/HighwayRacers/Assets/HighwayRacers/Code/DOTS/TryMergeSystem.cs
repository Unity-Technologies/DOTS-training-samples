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
        struct TryMergeSystemJob : IJobForEach<CarState, ProximityData, CarSettings>
        {
            [ReadOnly] public DotsHighway DotsHighway;
	        public float currentTime;

	        bool IsMerging(CarState.State state)
	        {
		        return state != CarState.State.MERGE_LEFT && state != CarState.State.MERGE_RIGHT;
	        }

	        bool CanMergeLeft(
                float lane, float mergeSpace,
                [ReadOnly] ref ProximityData proximityData)
	        {
                return (proximityData.data.NearestFrontLeft.CarId == 0
                        || proximityData.data.NearestFrontLeft.Distance >= mergeSpace)
                    && (proximityData.data.NearestRearLeft.CarId == 0
                        || proximityData.data.NearestRearLeft.Distance >= mergeSpace);
	        }

	        bool CanMergeRight(
                float lane, float mergeSpace,
                [ReadOnly] ref ProximityData proximityData)
	        {
                return (proximityData.data.NearestFrontRight.CarId == 0
                        || proximityData.data.NearestFrontRight.Distance >= mergeSpace)
                    && (proximityData.data.NearestRearRight.CarId == 0
                        || proximityData.data.NearestRearRight.Distance >= mergeSpace);
	        }

	        bool WantsToMergeLeft(
                [ReadOnly] ref CarState trackState,
                [ReadOnly] ref ProximityData proximityData,
                [ReadOnly] ref CarSettings settings)
	        {
		        return trackState.Lane + 1 < DotsHighway.NumLanes // left lane exists
		               && proximityData.data.NearestFrontMyLane.Distance < settings.LeftMergeDistance // close enough to car in front
		               && settings.OvertakeEagerness >
		               proximityData.data.NearestFrontMyLane.Speed / settings.DefaultSpeed; // car in front is slow enough
	        }

	        bool WantsToMergeRight(
                [ReadOnly] ref CarState state,
                [ReadOnly] ref ProximityData proximityData,
                [ReadOnly] ref CarSettings settings)
	        {
		        return state.Lane > 0
                    && state.CurrentState != CarState.State.MERGE_LEFT
			        && proximityData.data.NearestFrontRight.Distance
                        < settings.LeftMergeDistance // close enough to car in front
                    && settings.OvertakeEagerness
                        > proximityData.data.NearestFrontRight.Speed
                            / settings.DefaultSpeed; // car in front is slow enough
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
				    if (WantsToMergeLeft(ref state, ref proximityData, ref settings)
                        && CanMergeLeft(state.Lane, settings.MergeSpace, ref proximityData))
				    {
					    state.CurrentState = CarState.State.MERGE_LEFT;
					    state.TargetLane = Mathf.Round(state.Lane + 1);
					    state.TimeOvertakeCarSet = currentTime;
				    }
				    else if (WantsToMergeRight(ref state, ref proximityData,ref settings)
                        && CanMergeRight(state.Lane, settings.MergeSpace, ref proximityData))
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
                currentTime =  Time.time
            };
            var deps = job.Schedule(this, inputDeps);
            DotsHighway.RegisterReaderJob(deps);
            return deps;
        }
    }
}
