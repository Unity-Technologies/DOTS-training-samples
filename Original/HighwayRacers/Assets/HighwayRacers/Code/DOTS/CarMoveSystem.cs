using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class CarMoveSystem : JobComponentSystem
{

    struct MoveSystemJob : IJobForEach<CarState,CarStateOnTrack>
    {
        public float deltaTime;
        public void Execute([ReadOnly] ref CarState state, ref CarStateOnTrack trackState)
        {
            //forward position
            trackState.PositionOnTrackSegmentLane0 += state.FwdSpeed * deltaTime;
            trackState.Lane += state.LeftSpeed * deltaTime;
            
            //lateral position
            var roundLane = Mathf.Round(trackState.Lane);
            if (Mathf.Abs (roundLane - trackState.Lane) < .0001f) 
            {
                trackState.Lane = roundLane;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new MoveSystemJob {deltaTime = Time.deltaTime};
        return job.Schedule(this, inputDeps);
    }
}
