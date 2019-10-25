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

        [BurstCompile]
        struct CrashPreventionSystemJob : IJobForEach<ProximityData, CarSettings, CarState>
        {
            public float minDistBetweenCars;
            public float deltaTime;

            public void Execute(
                [ReadOnly] ref ProximityData proximity,
                [ReadOnly] ref CarSettings settings,
                ref CarState state)
            {
                if (!proximity.data.HasFront)
                    return;

                var maxDistanceDiff = math.max(
                    0, proximity.data.NearestFrontMyLane.Distance - minDistBetweenCars);
                state.FwdSpeed = math.min(state.FwdSpeed, maxDistanceDiff / deltaTime);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Time.deltaTime <= 0)
                return inputDeps;

            var job = new CrashPreventionSystemJob {minDistBetweenCars = Highway.MIN_DIST_BETWEEN_CARS,deltaTime = Time.deltaTime};
            return job.Schedule(this, inputDeps);
        }
    }
}
