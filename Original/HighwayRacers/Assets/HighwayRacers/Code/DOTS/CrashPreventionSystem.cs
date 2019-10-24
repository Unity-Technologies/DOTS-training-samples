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
    [UpdateBefore(typeof(CarMoveSystem))]
    [UpdateAfter(typeof(AccelerationSystem))]
    public class CrashPreventionSystem : JobComponentSystem
    {
        const float k_MinDistBetweenCars = Highway.MIN_DIST_BETWEEN_CARS;

        [BurstCompile]
        struct CrashPreventionSystemJob : IJobForEach<ProximityData, CarSettings, CarState>
        {
            public float deltaTime;

            public void Execute(
                [ReadOnly] ref ProximityData proximity,
                [ReadOnly] ref CarSettings settings,
                ref CarState state)
            {
                if (!proximity.data.HasFront)
                    return;

                var maxDistanceDiff = math.max(
                    0, proximity.data.NearestFrontMyLane.Distance - k_MinDistBetweenCars);
                state.FwdSpeed = math.min(state.FwdSpeed, maxDistanceDiff / deltaTime);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Time.deltaTime <= 0)
                return inputDeps;

            var job = new CrashPreventionSystemJob {deltaTime = Time.deltaTime};
            return job.Schedule(this, inputDeps);
        }
    }
}
