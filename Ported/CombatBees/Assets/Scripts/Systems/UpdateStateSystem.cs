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
        NativeArray<Entity> beeCarriedEntityData = new NativeArray<Entity>(beeCount, Allocator.TempJob);

        Entities
            .WithStoreEntityQueryInField(ref beeQuery)
            .WithAll<BeeState>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation, in BeeTeam beeTeam, in CarriedEntity carriedEntity) =>
            {
                beeTranslationData[entityInQueryIndex] = translation;
                beeEntityData[entityInQueryIndex] = entity;
                beeTeamData[entityInQueryIndex] = (int)beeTeam.Value;
                beeCarriedEntityData[entityInQueryIndex] = carriedEntity.Value;
            }).WithName("GetBeeData")
            .ScheduleParallel();


        // Get NativeArray data for food translation and entity data
        foodQuery = GetEntityQuery(typeof(Food));
        int foodCount = foodQuery.CalculateEntityCount();
        NativeArray<Translation> foodTranslationData = new NativeArray<Translation>(foodCount, Allocator.TempJob);
        NativeArray<bool> foodCarriedData = new NativeArray<bool>(foodCount, Allocator.TempJob);
        NativeArray<Entity> foodEntityData = new NativeArray<Entity>(foodCount, Allocator.TempJob);

        Entities
            .WithStoreEntityQueryInField(ref foodQuery)
            .WithAll<Food>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation, in Food food) =>
            {
                foodTranslationData[entityInQueryIndex] = translation;
                foodCarriedData[entityInQueryIndex] = food.isBeeingCarried;
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

        // Carrying state
        Entities.WithAll<BeeState>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state, ref PP_Movement movement,
                ref CarriedEntity carriedEntity, ref TargetedEntity targetedEntity, in Translation translation, in BeeTeam team) =>
            {
                //var random = Random.CreateFromIndex((uint)entityInQueryIndex + randomSeed);
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

            }).WithName("ProcessBeeCarryingState")
            .ScheduleParallel();

        // Seeking state
        Entities.WithAll<BeeState>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state, ref PP_Movement movement,
                ref CarriedEntity carriedEntity, ref TargetedEntity targetedEntity, in Translation translation, in BeeTeam team) =>
            {
                //var random = Random.CreateFromIndex((uint)entityInQueryIndex + randomSeed);
                var random = Random.CreateFromIndex((uint)entityInQueryIndex + randomSeed);

                if (state.value == StateValues.Seeking)
                {
                    bool targetFoodStillExists = false;

                    for (int i = 0; i < foodCount; i++)
                    {
                        if (foodEntityData[i] == targetedEntity.Value)
                        {
                            if (foodCarriedData[i] ||
                                abs(foodTranslationData[i].Value.x) >= spawner.ArenaExtents.x)
                            {
                                // If the food is already being carried, then give up on that food
                                carriedEntity.Value = Entity.Null;
                                break;
                            }
                        
                            targetFoodStillExists = true;
                            movement.endLocation = foodTranslationData[i].Value;
                        }

                        float translationDistance = distance(translation.Value, foodTranslationData[i].Value);
                        if (translationDistance <= distanceDelta &&
                            HasComponent<Food>(foodEntityData[i]))
                        {
                            if (foodCarriedData[i] == false)
                            {
                                state.value = StateValues.Carrying;
                                carriedEntity.Value = foodEntityData[i];

                                var endLocation = SpawnerSystem.GetRandomGoalTarget(ref random, spawner, team.Value);

                                movement.GoTo(translation.Value, endLocation);


                                // Add updated movement information to food entity
                                parallelWriter.AddComponent(entityInQueryIndex, foodEntityData[i],
                                    PP_Movement.Create(movement.startLocation + float3(0f, -1f, 0f),
                                        movement.endLocation + float3(0f, -1f, 0f)));

                                parallelWriter.AddComponent(entityInQueryIndex, foodEntityData[i],
                                    new Food { isBeeingCarried = true });

                                break;
                            }
                            else
                            {
                                state.value = StateValues.Idle;
                                targetedEntity.Value = Entity.Null;
                                break;
                            }
                        }
                    }

                    if (!targetFoodStillExists && targetedEntity.Value != Entity.Null)
                    {
                        state.value = StateValues.Idle;
                    }

                    if (movement.t >= 0.99f)
                    {
                        // If we have finished moving and are still seeking, go back to idle status
                        state.value = StateValues.Idle;
                    }
                }
               
            }).WithReadOnly(foodTranslationData)
            .WithReadOnly(foodCarriedData)
            .WithReadOnly(foodEntityData)
            .WithName("ProcessBeeSeekingState")
            .ScheduleParallel();

        // Attacking state
        Entities.WithAll<BeeState>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state, ref PP_Movement movement,
                ref CarriedEntity carriedEntity, ref TargetedEntity targetedEntity, in Translation translation, in BeeTeam team) =>
            {
                //var random = Random.CreateFromIndex((uint)entityInQueryIndex + randomSeed);
                var random = Random.CreateFromIndex((uint)entityInQueryIndex + randomSeed);

        
                if (state.value == StateValues.Attacking)
                {
                    bool targetBeeStillExists = false;

                    for (int i = 0; i < beeCount; i++)
                    {
                        if (beeEntityData[i] == targetedEntity.Value)
                        {
                            targetBeeStillExists = true;

                            float translationDistance =
                                distancesq(translation.Value, beeTranslationData[i].Value);

                            if (translationDistance <= 1.25f)
                            {
                                if (beeCarriedEntityData[i] != Entity.Null)
                                {
                                    //if the enemy Bee is carrying something, set it to no longer be
                                    parallelWriter.AddComponent(entityInQueryIndex, beeCarriedEntityData[i],
                                            PP_Movement.Create(
                                                beeTranslationData[i].Value + float3(0f, -1f, 0f),
                                                beeTranslationData[i].Value.Floored()));

                                    parallelWriter.AddComponent(entityInQueryIndex, beeCarriedEntityData[i],
                                    new Food { isBeeingCarried = false });
                                }

                                //destroy enemy bee
                                parallelWriter.DestroyEntity(entityInQueryIndex, beeEntityData[i]);

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

                    if (!targetBeeStillExists)
                    {
                        state.value = StateValues.Idle;
                    }

                    if (movement.t >= 0.99f)
                    {
                        // If we have finished moving and are still seeking, go back to idle status
                        state.value = StateValues.Idle;
                    }
                }
   
            }).WithReadOnly(beeTranslationData)
            .WithReadOnly(beeEntityData)
            .WithReadOnly(beeCarriedEntityData)
            .WithDisposeOnCompletion(beeCarriedEntityData)
            .WithName("ProcessBeeAttackingState")
            .ScheduleParallel();


        // Idle state
        Entities.WithAll<BeeState>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state, ref PP_Movement movement,
                ref CarriedEntity carriedEntity, ref TargetedEntity targetedEntity, in Translation translation, in BeeTeam team) =>
            {
                var random = Random.CreateFromIndex((uint)entityInQueryIndex + randomSeed);

                if (state.value == StateValues.Idle)
                {
                    float foodOrEnemy = random.NextFloat();
                    if (foodOrEnemy > spawner.ChanceToAttack) // Try to choose a food
                    {
                        // Check if there actually is any food
                        if (foodCount > 0)
                        {
                            // Choose food at random here
                            int randomInt =
                                random.NextInt(foodTranslationData
                                    .Length); // Note: random int/uint values are non-inclusive of the maximum value
                            targetedEntity.Value = foodEntityData[randomInt];

                            Translation randomFoodTranslation = foodTranslationData[randomInt];
                            var x = randomFoodTranslation.Value.x;

                            if (!foodCarriedData[randomInt] &&
                                abs(foodTranslationData[randomInt].Value.x) < spawner.ArenaExtents.x)
                            {
                                if ((team.Value == TeamValue.Blue && x > -40) || (team.Value == TeamValue.Yellow && x < 40))
                                {
                                    movement.GoTo(translation.Value, randomFoodTranslation.Value);
                                    state.value = StateValues.Seeking;
                                }
                            }
                        }
                    }
                    else
                    {
                        float smallestDistance = float.MaxValue;
                        Entity nearestEntSoFar = Entity.Null;
                        float3 nearestTargetLocation = float3.zero;


                        for (int i = 0; i < beeCount; i++)
                        {
                            if (beeEntityData[i] != Entity.Null)
                            {
                                if (beeTeamData[i] != (int)team.Value)
                                {
                                    float dist = distancesq(translation.Value, beeTranslationData[i].Value);
                                    if (dist < smallestDistance)
                                    {
                                        smallestDistance = dist;
                                        nearestEntSoFar = beeEntityData[i];
                                        nearestTargetLocation = beeTranslationData[i].Value;
                                    }
                                }
                            }
                        }

                        if (nearestEntSoFar != Entity.Null)
                        {
                            targetedEntity.Value = nearestEntSoFar;
                            movement.GoTo(translation.Value, nearestTargetLocation);
                            state.value = StateValues.Attacking;
                            Debug.Log("Set to Attacking!");
                        }
                    }
                    
                    // If it's still idling after trying to attack or seek,
                    // go to a random location (the "wander" behavior)
                    if (state.value == StateValues.Idle)
                    {
                        float3 endLocation;
                        var beeRandomX = random.NextInt(-spawner.ArenaExtents.x, spawner.ArenaExtents.x);
                        var beeRandomY = random.NextInt(spawner.ArenaHeight / 4, spawner.ArenaHeight / 4 * 3);
                        var beeRandomZ = random.NextInt(-spawner.ArenaExtents.y, spawner.ArenaExtents.y);

                        endLocation = float3(beeRandomX, beeRandomY, beeRandomZ);

                        carriedEntity.Value = Entity.Null;
                        targetedEntity.Value = Entity.Null;

                        movement.GoTo(translation.Value, endLocation);
                        state.value = StateValues.Seeking;
                    }
                }
            }).WithReadOnly(beeTranslationData)
            .WithReadOnly(beeEntityData)
            .WithReadOnly(beeTeamData)
            .WithReadOnly(foodTranslationData)
            .WithReadOnly(foodCarriedData)
            .WithReadOnly(foodEntityData)
            .WithDisposeOnCompletion(beeTranslationData)
            .WithDisposeOnCompletion(beeEntityData)
            .WithDisposeOnCompletion(beeTeamData)
            .WithDisposeOnCompletion(foodTranslationData)
            .WithDisposeOnCompletion(foodCarriedData)
            .WithDisposeOnCompletion(foodEntityData)
            .WithName("ProcessBeeIdleState")
            .ScheduleParallel();


        commandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}