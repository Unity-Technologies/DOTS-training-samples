using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEditor.Rendering;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial class PassengerColorizerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
       
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, in Passenger passenger, in EntityColor color) =>
            {
                ecb.AddComponent(passenger.Body, new URPMaterialPropertyBaseColor {Value = new float4(color.Value.r, color.Value.g,color.Value.b, color.Value.a)});
                ecb.RemoveComponent<EntityColor>(entity);
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}