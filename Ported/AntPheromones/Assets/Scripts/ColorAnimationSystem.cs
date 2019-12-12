
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class ColorAnimationSystem : JobComponentSystem
{
    [BurstCompile]
    struct ColorAnimationJob : IJobForEach<AntComponent, MaterialColor>
    {
        public float4 SearchColor;
        public float4 CarryColor;

        public void Execute([ReadOnly] ref AntComponent ant, ref MaterialColor color)
        {
            var targetColor = ant.state == 0 ? SearchColor : CarryColor;
            color.Value += (targetColor * ant.brightness - color.Value) * .05f;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        AntSettings settings = GetSingleton<AntSettings>();

        var jobColor = new ColorAnimationJob()
        {
            SearchColor = (Vector4)settings.searchColor,
            CarryColor = (Vector4)settings.carryColor
        };
        var jobColorHandle = jobColor.Schedule(this, inputDependencies);

        return jobColorHandle;
    }
}
