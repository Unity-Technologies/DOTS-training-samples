using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateBefore(typeof(CarStateSystem))]
public class GetNearestCarsSystem : JobComponentSystem
{
    struct GetNearestCarsSystemJob : IJobForEach<CarStateOnTrack,ProximityData>
    {
        public void Execute([ReadOnly] ref CarStateOnTrack trackState, ref ProximityData proximityData)
        {
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new GetNearestCarsSystemJob ();
        return job.Schedule(this, inputDeps);
        
    }
}
