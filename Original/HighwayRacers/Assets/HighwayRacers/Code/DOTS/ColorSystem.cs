using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace HighwayRacers
{
    [UpdateAfter(typeof(CarMoveSystem))]
    public class ColorSystem : JobComponentSystem
    {
        struct ColorSystemJob : IJobForEach<CarSharedData, CarState, CarSettings, CarColor>
        {
            public void Execute(
                [ReadOnly] ref CarSharedData shared,
                [ReadOnly] ref CarState state,
                [ReadOnly] ref CarSettings settings,
                ref CarColor color)
            {
                if (state.FwdSpeed > settings.DefaultSpeed)
                {
                    color.Value = Color.Lerp (shared.defaultColor, shared.maxSpeedColor, (state.FwdSpeed - settings.DefaultSpeed) / (settings.DefaultSpeed * settings.OvertakePercent - settings.DefaultSpeed));
                }
                else if (state.FwdSpeed < settings.DefaultSpeed)
                {
                    color.Value = Color.Lerp (shared.minSpeedColor, shared.defaultColor, state.FwdSpeed / settings.DefaultSpeed);
                }
                else
                {
                    color.Value = shared.defaultColor;
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new ColorSystemJob();
            return job.Schedule(this, inputDeps);
        }
    }
}
