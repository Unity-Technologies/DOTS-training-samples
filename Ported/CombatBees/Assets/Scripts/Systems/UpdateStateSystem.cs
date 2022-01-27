using System;
using System.Net;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditorInternal;
using UnityEngine;
using Utils;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(SpawnerSystem))]
public partial class UpdateStateSystem : SystemBase
{
    private EntityQuery beeQuery;
    private EntityQuery foodQuery;
    private EntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // Get NativeArray data for bees translation and entity data
        //beeQuery = GetEntityQuery(typeof(BeeTag));

        int beeCount = beeQuery.CalculateEntityCount();
        NativeArray<Translation> beeTranslationData = new NativeArray<Translation>(beeCount, Allocator.TempJob);
        NativeArray<Entity> beeEntityData = new NativeArray<Entity>(beeCount, Allocator.TempJob);
        NativeArray<int> beeTeamData = new NativeArray<int>(beeCount, Allocator.TempJob);

        Entities
            .WithStoreEntityQueryInField(ref beeQuery)
            .WithAll<BeeTag>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation, in BeeTeam beeTeam) =>
            {
                beeTranslationData[entityInQueryIndex] = translation;
                beeEntityData[entityInQueryIndex] = entity;
                beeTeamData[entityInQueryIndex] = (int)beeTeam.Value;
            }).WithName("GetBeeData")
            .ScheduleParallel();


        // Get NativeArray data for food translation and entity data
        foodQuery = GetEntityQuery(typeof(Food));
        int foodCount = foodQuery.CalculateEntityCount();
        NativeArray<Translation> foodTranslationData = new NativeArray<Translation>(foodCount, Allocator.TempJob);
        NativeArray<Entity> foodEntityData = new NativeArray<Entity>(foodCount, Allocator.TempJob);

        Entities
            .WithStoreEntityQueryInField(ref foodQuery)
            .WithAll<Food>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
            {
                foodTranslationData[entityInQueryIndex] = translation;
                foodEntityData[entityInQueryIndex] = entity;
            }).WithName("GetFoodData")
            .ScheduleParallel();

        // Used to determine if two translations are "equal enough"
        const float distanceDelta = 0.1f;

        // Parallel ECB for recording component add / remove
        // NOTE: Not necessary if only modifying pre-existing component values
        var randomSeed = (uint)max(1,
            DateTime.Now.Millisecond +
            DateTime.Now.Second +
            DateTime.Now.Minute +
            DateTime.Now.Day +
            DateTime.Now.Month +
            DateTime.Now.Year);
        // var random = new Random(randomSeed);

        // Parallel ECB for recording component add / remove
        // NOTE: Not necessary if only modifying pre-existing component values
        var ecb = commandBufferSystem.CreateCommandBuffer();
        var parallelWriter = ecb.AsParallelWriter();

        var spawner = GetSingleton<Spawner>();

        // Get "close enough" Food based on distance calculation
        Entities.WithAll<BeeTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref State state, ref PP_Movement movement,
                ref CarriedEntity carriedEntity, ref TargetedEntity targetedEntity, in Translation translation, in BeeTeam team) =>
            {
                var random = Random.CreateFromIndex((uint)entityInQueryIndex + randomSeed);

                // If Bee is idle -> seeking
                //           carrying -> continue
                //           seeking -> check for attack option then check for carry option
                //           attacking -> check for carry option then check for seeking
                if (state.value == StateValues.Carrying)
                {

                    if (!(translation.Value.x < spawner.ArenaExtents.x + 0.5f) && team.Value == TeamValue.Yellow ||
                        !(translation.Value.x > -spawner.ArenaExtents.x - 0.5f) && team.Value == TeamValue.Blue)
                    {
                        // Add updated movement information to food entity
                        parallelWriter.AddComponent(entityInQueryIndex, carriedEntity.Value,
                            PP_Movement.Create(translation.Value + float3(0f, -1f, 0f), translation.Value.Floored()));

                        parallelWriter.AddComponent(entityInQueryIndex, carriedEntity.Value,
                            new Food { isBeeingCarried = false });

                        state.value = StateValues.Idle;
                    }
                }
                else if (state.value == StateValues.Seeking)
                {
                    // Also potentially check for attack here
                    for (int i = 0; i < foodCount; i++)
                    {
                        float translationDistance = distance(translation.Value, foodTranslationData[i].Value);
                        if (translationDistance <= distanceDelta && foodEntityData[i] != Entity.Null)
                        {
                            state.value = StateValues.Carrying;
                            carriedEntity.Value = foodEntityData[i];

                            var minBeeBounds = SpawnerSystem.GetBeeMinBounds(spawner);
                            var maxBeeBounds = SpawnerSystem.GetBeeMaxBounds(spawner, minBeeBounds);

                            var beeRandomY = SpawnerSystem.GetRandomBeeY(ref random, minBeeBounds, maxBeeBounds);
                            var beeRandomZ = SpawnerSystem.GetRandomBeeZ(ref random, minBeeBounds, maxBeeBounds);

                            // Calculate end location based on team value;
                            float3 endLocation;
                            if (team.Value == TeamValue.Yellow)
                            {
                                var beeRandomX =
                                    SpawnerSystem.GetRandomYellowBeeX(ref random, minBeeBounds, maxBeeBounds);
                                endLocation = float3(beeRandomX, beeRandomY, beeRandomZ);
                            }
                            else
                            {
                                var beeRandomX =
                                    SpawnerSystem.GetRandomBlueBeeX(ref random, minBeeBounds, maxBeeBounds);
                                endLocation = float3(beeRandomX, beeRandomY, beeRandomZ);
                            }


                            movement.GoTo(translation.Value, endLocation);


                            // Add updated movement information to food entity
                            parallelWriter.AddComponent(entityInQueryIndex, foodEntityData[i],
                                PP_Movement.Create(movement.startLocation + float3(0f, -1f, 0f),
                                    movement.endLocation + float3(0f, -1f, 0f)));

                            parallelWriter.AddComponent(entityInQueryIndex, foodEntityData[i],
                                new Food { isBeeingCarried = true });

                            break;
                        }
                    }

                    if (movement.t >= 0.99f)
                    {
                        // If we have finished moving and are still seeking, go back to idle status
                        state.value = StateValues.Idle;
                    }
                }
                else if (state.value == StateValues.Attacking)
                {
                    // Also potentially check for attack here
                    for (int i = 0; i < beeCount; i++)
                    {
                        if (beeEntityData[i] == targetedEntity.Value)
                        {
                            if (beeEntityData[i] != Entity.Null)
                            {
                                if (beeEntityData[i] == targetedEntity.Value)
                                {
                                    float translationDistance =
                                        distancesq(translation.Value, beeTranslationData[i].Value);

                                    if (translationDistance <= 2)
                                    {
                                        if (carriedEntity.Value != Entity.Null)
                                        {
                                            //if the enemy Bee is carrying something, set it to no longer be
                                            parallelWriter.AddComponent(entityInQueryIndex, carriedEntity.Value,
                                                    PP_Movement.Create(
                                                        beeTranslationData[i].Value + float3(0f, -1f, 0f),
                                                        beeTranslationData[i].Value.Floored()));

                                            parallelWriter.AddComponent(entityInQueryIndex, carriedEntity.Value,
                                            new Food { isBeeingCarried = false });
                                        }

                                        //carriedEntity.Value = Entity.Null;

                                        //destroy enemy bee
                                        parallelWriter.DestroyEntity(entityInQueryIndex, beeEntityData[i]);
                                        Debug.Log("Killed A Bee!");

                                        //spawn a bee bit
                                        var bitsEntity = parallelWriter.Instantiate(entityInQueryIndex,
                                            spawner.BeeBitsPrefab);

                                        parallelWriter.SetComponent(entityInQueryIndex, bitsEntity,
                                            new Translation { Value = beeTranslationData[i].Value });

                                        parallelWriter.SetComponent(entityInQueryIndex, bitsEntity,
                                            PP_Movement.Create(
                                                beeTranslationData[i].Value, beeTranslationData[i].Value.Floored()));

                                        state.value = StateValues.Idle;
                                    }
                                }
                            }
                            else //if (beeEntityData[i] != Entity.Null)
                            {
                                Debug.Log("Someone else killed my bee!");
                                targetedEntity.Value = Entity.Null;
                                state.value = StateValues.Idle;
                            }
                        }
                    }

                    if (movement.t >= 0.99f)
                    {
                        // If we have finished moving and are still seeking, go back to idle status
                        state.value = StateValues.Idle;
                    }
                }
                else if (state.value == StateValues.Idle)
                {
                    float foodOrEnemy = random.NextFloat();
                    if (foodOrEnemy > 0.5f)
                    {
                        // Choose food at random here
                        // TODO: Exclude food that is being carried or dropped?
                        int randomInt = random.NextInt(foodTranslationData.Length); // Note: random int/uint values are non-inclusive of the maximum value
                        Translation randomFoodTranslation = foodTranslationData[randomInt];
                        movement.GoTo(translation.Value, randomFoodTranslation.Value);
                        state.value = StateValues.Seeking;
                    }
                    else
                    {
                        float smallestDistance = float.MaxValue;
                        Entity nearestEntSoFar = Entity.Null;
                        float3 nearestTargetLocation = float3.zero;


                        for (int i = 0; i < beeTranslationData.Length; i++)
                        {
                            if (beeTeamData[i] != (int)team.Value)
                            {
                                float dist = distancesq(translation.Value, beeTranslationData[i].Value);
                                if ( dist < smallestDistance)
                                {
                                    smallestDistance = dist;
                                    nearestEntSoFar = beeEntityData[i];
                                    nearestTargetLocation = beeTranslationData[i].Value;
                                }
                            }
                        }

                        targetedEntity.Value = nearestEntSoFar;
                        movement.GoTo(translation.Value, nearestTargetLocation);
                        state.value = StateValues.Attacking;
                        Debug.Log("Set to Attacking!");
                    }
                }
            }).WithReadOnly(beeTranslationData)
            .WithReadOnly(beeEntityData)
            .WithReadOnly(beeTeamData)
            .WithReadOnly(foodTranslationData)
            .WithReadOnly(foodEntityData)
            .WithDisposeOnCompletion(beeTranslationData)
            .WithDisposeOnCompletion(beeEntityData)
            .WithDisposeOnCompletion(beeTeamData)
            .WithDisposeOnCompletion(foodTranslationData)
            .WithDisposeOnCompletion(foodEntityData)
            .WithName("ProcessBeeState")
            .ScheduleParallel();



        commandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}