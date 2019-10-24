using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;

namespace HighwayRacers
{
    [UpdateAfter(typeof(CarMoveSystem))]
    public class SetCarTransformSystem : JobComponentSystem
    {
        [BurstCompile]
        struct SetTransformJob
        : IJobForEach<CarState, CarSettings, Translation, Rotation, ColorComponent>
        {
            [ReadOnly] public DotsHighway DotsHighway;
            public float4 defaultColor;
            public float4 maxSpeedColor;
            public float4 minSpeedColor;

            public void Execute(
                [ReadOnly] ref CarState state,
                [ReadOnly] ref CarSettings settings,
                ref Translation translation,
                ref Rotation rotation,
                ref ColorComponent color)
            {
                DotsHighway.GetWorldPosition(
                    state.PositionOnTrack, state.Lane, out float3 pos, out quaternion q);
                translation.Value = pos;
                rotation.Value = q;

                bool isFaster = state.FwdSpeed > settings.DefaultSpeed;
                float maxSpeed = math.select(
                    settings.DefaultSpeed, settings.DefaultSpeed * settings.OvertakePercent, isFaster);
                float minSpeed = math.select(0, settings.DefaultSpeed, isFaster);
                float4 minColor = math.select(minSpeedColor, defaultColor, isFaster);
                float4 maxColor = math.select(defaultColor, maxSpeedColor, isFaster);
                color.Value = math.lerp(
                    minColor, maxColor, (state.FwdSpeed - minSpeed) / (maxSpeed - minSpeed));
            }
        }

        float4 AsFloat4(Color c) { return new float4(c.r, c.g, c.b, c.a); }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new SetTransformJob
            {
                DotsHighway = Highway.instance.DotsHighway,
                defaultColor = AsFloat4(Game.instance.defaultColor),
                maxSpeedColor = AsFloat4(Game.instance.maxSpeedColor),
                minSpeedColor = AsFloat4(Game.instance.minSpeedColor)
            };
            return job.Schedule(this, inputDeps);
        }
    }
}
