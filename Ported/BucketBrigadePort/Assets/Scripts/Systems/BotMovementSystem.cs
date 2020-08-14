using System;
using System.Transactions;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

public class BotMovementSystem : SystemBase
{
    private EntityQuery m_bucketQuery;
    private EntityCommandBufferSystem m_ECBSystem;
    protected override void OnCreate()
    {
        m_bucketQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
           {
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<WaterAmount>(),
            },

            None = new[]
           {
                ComponentType.ReadOnly<WaterRefill>()
            }
        });
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var bucketSpawner = GetSingleton<BucketSpawner>();
        var lineSpawner = GetSingleton<LineSpawner>();
        float deltaTime = Time.DeltaTime;
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithName("TranslationSpeedSystem")
            .WithNone<BucketCarry>()
            .ForEach((int entityInQueryIndex, Entity entity, ref Translation translation, ref TargetPosition targetPosition) =>
            {
                // TODO: Move to function
                var maxMovement = lineSpawner.BotSpeed * deltaTime;
                var vector = targetPosition.Value - translation.Value;
                vector = math.normalizesafe(vector);
                var magnitude = math.distance(targetPosition.Value, translation.Value) * 2f;
                
                if(magnitude < 0.0001f) // epsilon?
                {
                    ecb.RemoveComponent<TargetPosition>(entityInQueryIndex, entity);
                }
                else
                {
                    var actualMovement = math.min(maxMovement, magnitude);
                    translation.Value += vector * actualMovement;
                }

            })
            .ScheduleParallel();

        Entities
            .WithName("TranslationWithBucketSpeedSystem")
            .ForEach((int entityInQueryIndex, Entity entity, ref Translation translation, ref TargetPosition targetPosition, ref BucketRef bucketRef, in BucketCarry bucketCarry ) =>
            {
                // TODO: Move to function
                var maxMovement = lineSpawner.BotSpeed * deltaTime;
                var vector = targetPosition.Value - translation.Value;
                vector = math.normalizesafe(vector);
                var magnitude = math.distance(targetPosition.Value, translation.Value);
                
                if (magnitude < 0.0001f) // epsilon?
                {
                    //var bucketMovement = new BucketMovement() { Value = translation.Value + new float3(0, 0,0)}; // TODO: Tag to put bucket down
                    //ecb.AddComponent<BucketMovement>(entityInQueryIndex, bucketRef.Value, bucketMovement);
                    ecb.RemoveComponent<TargetPosition>(entityInQueryIndex, entity);
                }
                else
                {
                    var actualMovement = math.min(maxMovement, magnitude);
                    translation.Value += vector * actualMovement;
                    //var bucketMovement = new BucketMovement() { Value = translation.Value + new float3(0,0.5f,0) }; // TODO: Move bucket to directly above the bot
                    var bucketMovement = new BucketMovement() { Value = translation.Value }; // Move bucket directly to the bot
                    ecb.AddComponent<BucketMovement>(entityInQueryIndex, bucketRef.Value, bucketMovement);
                }            
            })
            .ScheduleParallel();

        Entities
            .WithName("TranslationBucketSystem")
            .ForEach((int entityInQueryIndex, Entity entity, ref Translation translation, ref BucketMovement bucketMovement) =>
            {
                // // TODO: Move to function
                // var maxMovement = lineSpawner.BotSpeed * deltaTime;
                // var vector = bucketMovement.Value - translation.Value;
                // vector = math.normalizesafe(vector);
                // var magnitude = math.distance(bucketMovement.Value, translation.Value);
                // var actualMovement = math.min(maxMovement, magnitude);
                // translation.Value += vector * actualMovement;

                translation.Value = bucketMovement.Value;
                ecb.RemoveComponent<BucketMovement>(entityInQueryIndex, entity);
            })
            .ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}