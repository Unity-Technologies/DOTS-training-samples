using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial class RenderFireSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var field = GetBuffer<FireHeat>(GetSingletonEntity<FireField>());
        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        var config = GetSingleton<GameConstants>(); 
        var fieldSize = config.FieldSize;
        var maxColor = config.FireMaxColor;
        var minColor = config.FireMinColor;
        var oscScale = config.FireOSCRange;
        var deltaTime = Time.DeltaTime;
        
        
        Entities
            .WithReadOnly(field)
            .ForEach((Entity e, int entityInQueryIndex, ref NonUniformScale scale, ref URPMaterialPropertyBaseColor color,
            in Translation pos, in FireRenderer renderer) =>
        {
            int id = (int) ((pos.Value.x + fieldSize.x * pos.Value.z) / 0.3f);
            float heat = field[id];
            if (heat < 0.2f)
            {
                ecb.DestroyEntity(entityInQueryIndex, e);
                return;
            }
            
            float4 finalColor = math.lerp(minColor, maxColor, heat);
            color.Value = finalColor;
            float oscHeat = math.clamp(heat + oscScale * math.sin(deltaTime + (float) id), 0.1f, 1f);
            scale.Value = new float3(0.3f, oscHeat, 0.3f);
            //TODO: setup with noise.pnoise;
        }).ScheduleParallel();
        
        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
