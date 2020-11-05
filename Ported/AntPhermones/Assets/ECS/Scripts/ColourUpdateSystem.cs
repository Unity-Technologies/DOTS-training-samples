using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

public class ColourUpdateSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem cmdBufferSystem;

    protected override void OnCreate()
    {
        cmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = cmdBufferSystem.CreateCommandBuffer().AsParallelWriter();

        //Update ants looking for food
        Entities
            .WithAll<RequireColourUpdate, AntLookingForFood>()
            .ForEach((Entity ant, int entityInQueryIndex, ref URPMaterialPropertyBaseColor colour) =>
            {
                ecb.RemoveComponent<RequireColourUpdate>(entityInQueryIndex, ant);
                colour.Value = new float4(0.189f, 0.210f, 0.352f, 1f);

            }).ScheduleParallel();

        //Update ants looking for colony
        Entities
            .WithAll<RequireColourUpdate, AntLookingForNest>()
            .ForEach((Entity ant, int entityInQueryIndex, ref URPMaterialPropertyBaseColor colour) =>
            {
                ecb.RemoveComponent<RequireColourUpdate>(entityInQueryIndex, ant);
                colour.Value = new float4(0.720f, 0.711f, 0.397f, 1);

            }).ScheduleParallel();


        //Dependency for the cmd buffer system to complete
        cmdBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
