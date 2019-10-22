using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace HighwayRacers
{
    [UpdateBefore(typeof(CarMoveSystem))]
    [UpdateAfter(typeof(AccelerationSystem))]
    public class CrashPreventionSystem : JobComponentSystem
    {
        const float k_MinDistBetweenCars = 0.7f;
        struct CrashPreventionSystemJob : IJobForEach<ProximityData,CarState>
        {
            public float deltaTime;

            public void Execute(
                [ReadOnly] ref ProximityData proximityData, ref CarState state)
            {
                var maxDistanceDiff = Mathf.Max(0, proximityData.data.NearestFrontMyLane.Distance - k_MinDistBetweenCars);
                state.FwdSpeed = Mathf.Min(state.FwdSpeed, maxDistanceDiff / deltaTime);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Time.deltaTime <= 0)
                return new JobHandle();

            var job = new CrashPreventionSystemJob {deltaTime = Time.deltaTime};
            return job.Schedule(this, inputDeps);
        }
    }
}
