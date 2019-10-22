using System.Collections;
using System.Collections.Generic;
using HighwayRacers;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace HighwayRacers
{
    [UpdateBefore(typeof(CarMoveSystem))]
    [UpdateAfter(typeof(CarStateSystem))]
    public class AccelerationSystem : JobComponentSystem
    {
        struct AccelerationSystemJob : IJobForEach<CarState>
        {
            public float deltaTime;

            public void Execute(ref CarState state)
            {
                // increase to speed
                if (state.TargetFwdSpeed > state.FwdSpeed)
                {
                    state.FwdSpeed = Mathf.Min(state.TargetFwdSpeed, state.FwdSpeed + Game.instance.acceleration * deltaTime);
                } else {
                    state.FwdSpeed = Mathf.Max(state.TargetFwdSpeed, state.FwdSpeed - Game.instance.brakeDeceleration * deltaTime);
                }

            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Time.deltaTime <= 0)
                return new JobHandle();

            var job = new AccelerationSystemJob {deltaTime = Time.deltaTime};
            return job.Schedule(this, inputDeps);
        }
    }
}
