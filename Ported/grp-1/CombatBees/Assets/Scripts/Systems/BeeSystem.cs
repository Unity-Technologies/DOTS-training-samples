using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
//public struct UpdateTranslation : IJobChunk
//{
//    public ComponentTypeHandle<Position> TypeHandlePosition;
//    public ComponentTypeHandle<Translation> TypeHandleTranslation;
//
//    public ExecuteAlways(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
//    {
//        NativeArray<Translation> arrayTranslation = chunk.GetNativeArray<Translation>(TypeHandleTranslation);
//        NativeArray<Position> arrayPosition = chunk.GetNativeArray<Position>(TypeHandlePosition);
//        for (int i = 0; i < arrayTranslation.Length || arrayTranslation.Length != arrayPosition.Length; i++)
//        {
//
//        }
//
//    }
//}
public class PositionSystem : SystemBase
{
    private EntityQuery RequirePropagation;
    private EntityCommandBufferSystem CommandBufferSystem;
    protected override void OnCreate()
    {
        // Looking up another system in the world is an expensive operation.
        // In order to not do it every frame, we store the reference in a field.
        CommandBufferSystem
            = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        /////////////////////Setting the bee target witch is an Entity ( bee, resource, base) 
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities
           .WithAll<Bee>()
           .ForEach((Entity bee, ref Translation pos, ref Bee beeData) =>
           {
               if (beeData.currentTargetEntity == Entity.Null)
               {
                   beeData.currentTargetTransform = beeData.baseTargetTransform;
                   beeData.currentTargetEntity = beeData.baseEntity;
               }

               float3 direction = beeData.currentTargetTransform.Value - pos.Value;
               float v = (direction.x * direction.x + direction.y * direction.y + direction.z * direction.z);
               direction /= v;
               if (v < 10f)
               {
                   if (beeData.currentTargetEntity == beeData.baseEntity)
                   {
                       beeData.currentTargetTransform = beeData.CenterTargetTransform;
                       beeData.currentTargetEntity = beeData.centerEntity;
                   }
                   else
                       beeData.currentTargetEntity = Entity.Null;
               }
               //direction
               float speed = 100;
               float3 newPos = math.lerp(pos.Value, pos.Value + direction, deltaTime);
               pos.Value = pos.Value + direction * deltaTime * speed;
               // check near target

           }).ScheduleParallel();
            ecb.Playback(EntityManager);
            ecb.Dispose();
    }
}
