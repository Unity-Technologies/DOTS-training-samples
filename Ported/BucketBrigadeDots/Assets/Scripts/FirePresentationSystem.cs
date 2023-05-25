using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateBefore(typeof(TransformSystemGroup)), UpdateAfter(typeof(FireSpreadSystem))]
public partial struct FirePresentationSystem : ISystem
{
    private static readonly float4 green = new float4(0f, 1f, 0f, 1f);
    private static readonly float4 red = new float4(1f, 0f, 0f, 1f);
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FireCell>();
        state.RequireForUpdate<GameSettings>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameSettings = SystemAPI.GetSingleton<GameSettings>();
        var temperatures = SystemAPI.GetSingletonBuffer<FireTemperature>();

        float time = (float)SystemAPI.Time.ElapsedTime * 10f;

        int rowsAndColumns = gameSettings.RowsAndColumns;
        if (gameSettings.FirePresentationJobMode == JobMode.RunWithoutJobs)
        {
            var index = 0;
            foreach (var (localTransform, baseColor, entity) in SystemAPI
                         .Query<RefRW<LocalTransform>, RefRW<URPMaterialPropertyBaseColor>>().WithAll<FireCell>()
                         .WithEntityAccess())
            {
                SetColorAndTransformForHeat(
                    ref temperatures,
                    ref localTransform.ValueRW,
                    ref baseColor.ValueRW,
                    entity,
                    index,
                    rowsAndColumns,
                    time);
                
                ++index;
            }
        }
        else
        {
            // If we're using jobs, we need a job for all scheduling modes.
            var job = new FirePresentationJob()
            {
                temperatures = temperatures,
                rowsAndColumns = gameSettings.RowsAndColumns,
                time = time
            };

            switch (gameSettings.FirePresentationJobMode)
            {
                case JobMode.JobParallel:
                    job.ScheduleParallel();
                    break;
                
                case JobMode.JobSingleThread:
                    job.Schedule();
                    break;
                
                case JobMode.RunWithoutJobs:
                    Debug.LogError("Coding error: non-job invocations should be handled above!");
                    break;
            }
        }
    }

    [BurstCompile]
    private static void SetColorAndTransformForHeat(
        [ReadOnly] ref DynamicBuffer<FireTemperature> temperatures,
        ref LocalTransform localTransform,
        ref URPMaterialPropertyBaseColor baseColor,
        Entity entity,
        int index,
        int rowsAndColumns,
        float time)
    {
        var heat = temperatures[index];
        var x = index % rowsAndColumns;
        var z = index / rowsAndColumns;
        var sine = math.sin(time + entity.Index) + 1f;
        var y = heat - heat * sine * .05f - 1f;
        localTransform.Position = new float3(x * .3f, y, z * .3f);

        baseColor.Value = math.lerp(green, red, heat);
    }

    [BurstCompile, WithAll(typeof(FireCell))]
    partial struct FirePresentationJob : IJobEntity
    {
        [ReadOnly] public DynamicBuffer<FireTemperature> temperatures;
        public int rowsAndColumns;
        public float time;
        
        public void Execute(
            ref LocalTransform localTransform,
            ref URPMaterialPropertyBaseColor baseColor,
            Entity entity,
            [EntityIndexInQuery] int index)
        {
            SetColorAndTransformForHeat(
                ref temperatures,
                ref localTransform,
                ref baseColor,
                entity,
                index,
                rowsAndColumns,
                time);
        }
    }
}

