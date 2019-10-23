using System.Collections;
using System.Collections.Generic;
using HighwayRacers;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacers
{
    [UpdateBefore(typeof(CarMoveSystem))]
    [UpdateAfter(typeof(CarStateSystem))]
    public class AccelerationSystem : JobComponentSystem
    {
        struct AccelerationSystemJob : IJobForEach<CarState>
        {
            public float accelRate;
            public float decelRate;

            public void Execute(ref CarState state)
            {
                if (state.TargetFwdSpeed > state.FwdSpeed)
                    state.FwdSpeed = math.min(state.TargetFwdSpeed, state.FwdSpeed + accelRate);
                else
                    state.FwdSpeed = math.max(state.TargetFwdSpeed, state.FwdSpeed - decelRate);
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
