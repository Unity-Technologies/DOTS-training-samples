using System.Collections;
using System.Collections.Generic;
using HighwayRacers;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacers
{
    [UpdateBefore(typeof(CarMoveSystem))]
    [UpdateAfter(typeof(TryMergeSystem))]
    public class AccelerationSystem : JobComponentSystem
    {
        [BurstCompile]
        struct AccelerationSystemJob : IJobForEach<CarState>
        {
            public float accelRate;
            public float decelRate;

            public void Execute(ref CarState state)
            {
                state.FwdSpeed = math.select(
                    math.max(state.TargetFwdSpeed, state.FwdSpeed - decelRate),
                    math.min(state.TargetFwdSpeed, state.FwdSpeed + accelRate),
                    state.TargetFwdSpeed > state.FwdSpeed);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Time.deltaTime <= 0)
                return inputDeps;

            var job = new AccelerationSystemJob
            {
                accelRate = Game.instance.acceleration * Time.deltaTime,
                decelRate = Game.instance.brakeDeceleration * Time.deltaTime
            };
            return job.Schedule(this, inputDeps);
        }
    }
}
