using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateBefore(typeof(CarStateSystem))]
public class GetNearestCarsSystem : JobComponentSystem
{
    struct GetNearestCarsSystemJob : IJobForEach<CarStateOnTrack>
    {
        public void Execute(ref CarStateOnTrack trackState)
        {
            
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new GetNearestCarsSystemJob ();
        return job.Schedule(this, inputDeps);
        
    }
}
