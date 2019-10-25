using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacers
{
    [UpdateAfter(typeof(CarStateSystem))]
    public class TryMergeSystem : JobComponentSystem
    {
        [BurstCompile]
        struct TryMergeSystemJob : IJobForEach<CarState, ProximityData, CarSettings>
        {
            [ReadOnly] public DotsHighway DotsHighway;

	        bool IsMerging(CarState.State state)
	        {
		        return state == CarState.State.MERGE_LEFT || state == CarState.State.MERGE_RIGHT;
	        }

	        bool WantsToMergeLeft(
                [ReadOnly] ref CarState state,
                [ReadOnly] ref ProximityData proximity,
                [ReadOnly] ref CarSettings settings)
	        {
                // Is there a lane to the left?
		        if (state.Lane >= DotsHighway.NumLanes - 1)
                    return false;

                // Is somebody in front?
                if (!proximity.data.HasFront)
                    return false;

                // Is he annoying?
                if (proximity.data.NearestFrontMyLane.Distance
                        > settings.LeftMergeDistance // far enough from car in front
                    || settings.OvertakeEagerness
                        < proximity.data.NearestFrontMyLane.Speed
                            / state.FwdSpeed) // car in front is fast enough
                {
                    return false;
                }

                // Is there space for me to merge left?
                return proximity.data.NearestFrontLeft.Distance >= settings.MergeSpace
                    && proximity.data.NearestRearLeft.Distance >= settings.MergeSpace;
	        }

	        bool WantsToMergeRight(
                [ReadOnly] ref CarState state,
                [ReadOnly] ref ProximityData proximity,
                [ReadOnly] ref CarSettings settings)
	        {
		        if (state.Lane < 1)
			        return false;

                // Is there space for me in the right lane?
                if (proximity.data.NearestFrontRight.Distance < settings.MergeSpace
                    || proximity.data.NearestRearRight.Distance < settings.MergeSpace)
                {
                    return false; // nope - stay where we are
                }

                // If we were in the right lane, would we want to merge left?
                if (proximity.data.NearestFrontRight.Distance < settings.LeftMergeDistance
                    && settings.OvertakeEagerness > proximity.data.NearestFrontRight.Speed / state.FwdSpeed)
                {
				    return false; // yes we would, stay where we are
		        }
		        return true;
	        }

            public void Execute(
                ref CarState state,
                [ReadOnly] ref ProximityData proximity,
                [ReadOnly] ref CarSettings settings)
            {
                // detect merging
			    if (!IsMerging(state.CurrentState))
			    {
				    // detect merging to left lane
				    if (WantsToMergeLeft(ref state, ref proximity, ref settings))
				    {
					    state.CurrentState = CarState.State.MERGE_LEFT;
					    state.TargetLane = math.round(state.Lane + 1);
				    }
				    else if (WantsToMergeRight(ref state, ref proximity,ref settings))
				    {
					    state.CurrentState = CarState.State.MERGE_RIGHT;
					    state.TargetLane = math.round(state.Lane - 1);
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
