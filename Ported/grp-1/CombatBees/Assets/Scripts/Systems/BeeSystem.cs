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
        
        NativeList<Entity> beeArray = new NativeList<Entity>(100, Allocator.TempJob);
        
        
        
        
        /* --------------------------------------------------------------------------------- */
        var ecb0 = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .ForEach((Entity beeEntity, in Bee bee) =>
            {
                beeArray.Add(beeEntity);
            }).Run();
        ecb0.Playback(EntityManager);
        ecb0.Dispose();
        
    
        var random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
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
        ///////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////Compute Movement//////////////////////////////////////
        


        //////////////////////////////////////////////////////////////////////////////////////////

        beeArray.Dispose();
        //
        //var random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
        //var ecb = new EntityCommandBuffer(Allocator.TempJob);
        //Entities
        //   .WithAll<Bee>()
        //   .WithNone<BeeMoveToTarget>()
        //   .ForEach((Entity bee, ref Velocity velocity, in Translation pos, in Bee beeData) =>
        //   {
        //       //BeeMoveToTarget BeeTarget;
        //       //ecb.AddComponent<BeeMoveToTarget>(bee, BeeTarget);
        //     
        //   }).ScheduleParallel();
        //    ecb.Playback(EntityManager);
        //    ecb.Dispose();

        //Entities
        //    .ForEach((ref Translation translation, in Bee bee, in Velocity newPos) =>
        //    {
        //        translation.Value.x += deltaTime;
        //    }).ScheduleParallel();
        // beeArray.Dispose();


        //
        //var ecb = CommandBufferSystem.CreateCommandBuffer();
        //var cdfe = GetComponentDataFromEntity<Translation>();
        //
        //Entities
        //    // Random access to components for writing can be a race condition.
        //    // Here, we know for sure that prefabs don't share their entities.
        //    // So explicitly request to disable the safety system on the CDFE.
        //    .WithNativeDisableContainerSafetyRestriction(cdfe)
        //    .WithStoreEntityQueryInField(ref RequirePropagation)
        //    .WithAll<Bee>()
        //    .ForEach((in DynamicBuffer<LinkedEntityGroup> group
        //        , in Position newPos) =>
        //    {
        //        for (int i = 1; i < group.Length; ++i)
        //        {
        //            cdfe[group[i].Value].Value.x = (float)(newPos.position.x + time);
        //        }
        //    }).ScheduleParallel();
    }
}
