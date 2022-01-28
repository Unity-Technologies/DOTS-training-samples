using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Utils;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(SpawnerSystem))]
public partial class UpdateStateSystem : SystemBase
{
    private EntityQuery yellowBeeQuery;
    private EntityQuery blueBeeQuery;
    private EntityQuery foodQuery;
    private EntityCommandBufferSystem commandBufferSystem;

    // Used to determine if two translations are "equal enough"
    const float squaredInteractionDistance = 1f;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        int yellowBeeCount = yellowBeeQuery.CalculateEntityCount();
        NativeArray<Entity> yellowBeeEntityData = new NativeArray<Entity>(yellowBeeCount, Allocator.TempJob);
        NativeArray<float3> yellowBeeTranslationData = new NativeArray<float3>(yellowBeeCount, Allocator.TempJob);
        NativeArray<Entity> yellowBeeCarriedEntityData = new NativeArray<Entity>(yellowBeeCount, Allocator.TempJob);

        int blueBeeCount = blueBeeQuery.CalculateEntityCount();
        NativeArray<Entity> blueBeeEntityData = new NativeArray<Entity>(blueBeeCount, Allocator.TempJob);
        NativeArray<float3> blueBeeTranslationData = new NativeArray<float3>(blueBeeCount, Allocator.TempJob);
        NativeArray<Entity> blueBeeCarriedEntityData = new NativeArray<Entity>(blueBeeCount, Allocator.TempJob);


 		Entities
            .WithStoreEntityQueryInField(ref yellowBeeQuery)
            .WithAll<YellowBeeTag>()
            .ForEach((Entity entity, int entityInQueryIndex, in PP_Movement movement, in Translation translation, in CarriedEntity carriedEntity) =>
            {
                yellowBeeTranslationData[entityInQueryIndex] =  movement.unperturbedPosition;
                yellowBeeEntityData[entityInQueryIndex] = entity;
                yellowBeeCarriedEntityData[entityInQueryIndex] = carriedEntity.Value;
            }).WithName("GetYellowBeeData")
            .ScheduleParallel();

        Entities
            .WithStoreEntityQueryInField(ref blueBeeQuery)
            .WithAll<BlueBeeTag>()
            .ForEach((Entity entity, int entityInQueryIndex, in PP_Movement movement, in Translation translation, in CarriedEntity carriedEntity) =>
            {
                blueBeeTranslationData[entityInQueryIndex] =  movement.unperturbedPosition;
                blueBeeEntityData[entityInQueryIndex] = entity;
                blueBeeCarriedEntityData[entityInQueryIndex] = carriedEntity.Value;
            }).WithName("GetBlueBeeData")
            .ScheduleParallel();


        // Get NativeArray data for food translation and entity data
        foodQuery = GetEntityQuery(typeof(Food));
        int foodCount = foodQuery.CalculateEntityCount();
        NativeArray<float3> foodTranslationData = new NativeArray<float3>(foodCount, Allocator.TempJob);
        NativeArray<bool> foodCarriedData = new NativeArray<bool>(foodCount, Allocator.TempJob);
        NativeArray<Entity> foodEntityData = new NativeArray<Entity>(foodCount, Allocator.TempJob);

        Entities
            .WithStoreEntityQueryInField(ref foodQuery)
            .WithAll<Food>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation, in Food food) =>
            {
                foodTranslationData[entityInQueryIndex] = translation.Value;
                foodCarriedData[entityInQueryIndex] = food.isBeeingCarried;
                foodEntityData[entityInQueryIndex] = entity;
            }).WithName("GetFoodData")
            .ScheduleParallel();



        // Parallel ECB for recording component add / remove
        // NOTE: Not necessary if only modifying pre-existing component values
        var randomSeed = (uint) max(1,
            DateTime.Now.Millisecond +
            DateTime.Now.Second +
            DateTime.Now.Minute +
            DateTime.Now.Day +
            DateTime.Now.Month +
            DateTime.Now.Year);

        var spawner = GetSingleton<Spawner>();

        // Parallel ECB for recording component add / remove
        // NOTE: Not necessary if only modifying pre-existing component values
        var ecb = commandBufferSystem.CreateCommandBuffer();
        var parallelWriter = ecb.AsParallelWriter();

        // YELLOW CARRY  state
        Entities.WithAll<YellowBeeTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state,
                ref CarriedEntity carriedEntity, in Translation translation) =>
            {
                carryCommon(entity,
                    entityInQueryIndex,
                    ref state,
                    TeamValue.Yellow,
                    carriedEntity,
                    translation,
                    parallelWriter,
                    randomSeed);

            }).WithName("ProcessYellowCarryingState")
            .ScheduleParallel();


        // BLUE CARRY  state
        Entities.WithAll<BlueBeeTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state,
                ref CarriedEntity carriedEntity, in Translation translation) =>
            {
                carryCommon(entity,
                    entityInQueryIndex,
                    ref state,
                    TeamValue.Blue,
                    carriedEntity,
                    translation,
                    parallelWriter,
                    randomSeed);

            }).WithName("ProcessBlueCarryingState")
            .ScheduleParallel();

        // Seeking/Wandering state
        Entities.WithAll<BeeState>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state, ref PP_Movement movement,
                ref CarriedEntity carriedEntity, ref TargetedEntity targetedEntity, in Translation translation, in BeeTeam team) =>
            {
                var random = Random.CreateFromIndex((uint) entityInQueryIndex + randomSeed);

                if (state.value == StateValues.Seeking || state.value == StateValues.Wandering)
                {
                    for (int i = 0; i < foodCount; i++)
                    {
                        if (state.value == StateValues.Seeking &&
                            foodEntityData[i] == targetedEntity.Value)
                        {
                            if (foodCarriedData[i] ||
                                abs(foodTranslationData[i].x) >= Spawner.ArenaExtents.x)
                            {
                                // If the food is already being carried,
                                // or if it's in a goal area,
                                // then give up on that food.
                                targetedEntity.Value = Entity.Null;
                                state.value = StateValues.Idle;
                                break;
                            }
                            movement.UpdateEndPosition(foodTranslationData[i]);
                        }

                        // Check if close enough to the food/wander-destination to pick it up or wander somewhere else
                        if (foodEntityData[i] == targetedEntity.Value)
                        {
                            float translationDistance = distancesq(translation.Value, foodTranslationData[i]);

                            if (translationDistance <= squaredInteractionDistance)
                            {
                                // If the food is not being carried already, pick it up
                                if (state.value == StateValues.Seeking &&
                                    foodCarriedData[i] == false)
                                {
                                    state.value = StateValues.Carrying;
                                    carriedEntity.Value = foodEntityData[i];

                                    // Set a random destination within the appropriate goal area
                                    var endLocation = SpawnerSystem.GetRandomGoalTarget(ref random, team.Value);
                                    movement.GoTo(translation.Value, endLocation);

                                    // Add updated movement information to food entity
                                    parallelWriter.AddComponent(entityInQueryIndex, foodEntityData[i],
                                        PP_Movement.Create(movement.startLocation + float3(0f, -1f, 0f),
                                            movement.endLocation + float3(0f, -1f, 0f)));

                                    parallelWriter.AddComponent(entityInQueryIndex, foodEntityData[i],
                                        new Food { isBeeingCarried = true });

                                    break;
                                }

                                // If the food is already being carried,
                                // or the bee done with this wander,
                                // then go to the idle state
                                state.value = StateValues.Idle;
                                targetedEntity.Value = Entity.Null;
                                break;
                            }
                        }

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

        // BLUE Attack state
        Entities.WithAll<BlueBeeTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state, ref PP_Movement movement,
                ref TargetedEntity targetedEntity, in Translation translation) =>
            {
                attackCommon( entity,
                    entityInQueryIndex,
                    ref state,
                    TeamValue.Blue,
                    yellowBeeCount,
                    yellowBeeEntityData,
                    yellowBeeTranslationData,
                    yellowBeeCarriedEntityData,
                    ref movement,
                    ref targetedEntity,
                    translation,
                    parallelWriter,
                    spawner
                );
            }).WithReadOnly(yellowBeeTranslationData)
            .WithReadOnly(yellowBeeEntityData)
            .WithReadOnly(yellowBeeCarriedEntityData)
            .WithDisposeOnCompletion(yellowBeeCarriedEntityData)
            .WithName("ProcessYellowBeeAttackingState")
            .ScheduleParallel();

        // YELLOW ATTACK state
        Entities.WithAll<YellowBeeTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state, ref PP_Movement movement,
                ref TargetedEntity targetedEntity, in Translation translation) =>
            {
                attackCommon( entity,
                    entityInQueryIndex,
                    ref state,
                    TeamValue.Yellow,
                    blueBeeCount,
                    blueBeeEntityData,
                    blueBeeTranslationData,
                    blueBeeCarriedEntityData,
                    ref movement,
                    ref targetedEntity,
                    translation,
                    parallelWriter,
                    spawner
                );

            }).WithReadOnly(blueBeeTranslationData)
            .WithReadOnly(blueBeeEntityData)
            .WithReadOnly(blueBeeCarriedEntityData)
            .WithDisposeOnCompletion(blueBeeCarriedEntityData)
            .WithName("ProcessBlueBeeAttackingState")
            .ScheduleParallel();




        // BLUE Idle state
        Entities.WithAll<BlueBeeTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state, ref PP_Movement movement,
                ref CarriedEntity carriedEntity, ref TargetedEntity targetedEntity, in Translation translation,
                in BeeTeam team) =>
            {


                idleCommon(entity,
                    entityInQueryIndex,
                    ref state,
                    TeamValue.Blue,
                    randomSeed,
                    foodEntityData,
                    foodTranslationData,
                    foodCarriedData,
                    ref movement,
                    ref targetedEntity,
                    ref carriedEntity,
                    translation,
                    parallelWriter,
                    spawner,
                    yellowBeeCount,
                    yellowBeeEntityData,
                    yellowBeeTranslationData
                );




            }).WithReadOnly(yellowBeeEntityData)
            .WithReadOnly(yellowBeeTranslationData)
            .WithReadOnly(foodTranslationData)
            .WithReadOnly(foodCarriedData)
            .WithReadOnly(foodEntityData)
            .WithDisposeOnCompletion(yellowBeeEntityData)
            .WithDisposeOnCompletion(yellowBeeTranslationData)
            .WithName("ProcessBLUEIdleState")
            .ScheduleParallel();


        // YELLOW Idle/Wander state
        Entities.WithAll<YellowBeeTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state, ref PP_Movement movement,
                ref CarriedEntity carriedEntity, ref TargetedEntity targetedEntity, in Translation translation,
                in BeeTeam team
                ) =>
            {
                idleCommon(entity,
                    entityInQueryIndex,
                    ref state,
                    TeamValue.Yellow,
                    randomSeed,
                    foodEntityData,
                    foodTranslationData,
                    foodCarriedData,
                    ref movement,
                    ref targetedEntity,
                    ref carriedEntity,
                    translation,
                    parallelWriter,
                    spawner,
                    blueBeeCount,
                    blueBeeEntityData,
                    blueBeeTranslationData
                );

            }).WithReadOnly(blueBeeEntityData)
            .WithReadOnly(blueBeeTranslationData)
            .WithReadOnly(foodTranslationData)
            .WithReadOnly(foodCarriedData)
            .WithReadOnly(foodEntityData)
            .WithDisposeOnCompletion(blueBeeEntityData)
            .WithDisposeOnCompletion(blueBeeTranslationData)
            .WithDisposeOnCompletion(foodTranslationData)
            .WithDisposeOnCompletion(foodCarriedData)
            .WithDisposeOnCompletion(foodEntityData)
            .WithName("ProcessYELLOWIdleState")
            .ScheduleParallel();



        commandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    private static void SetWanderLocation(
        ref Random random,
        ref CarriedEntity carriedEntity,
        ref TargetedEntity targetedEntity,
        ref PP_Movement movement,
        Translation translation)
    {
        float3 endLocation;
        var beeRandomX = random.NextInt(-Spawner.ArenaExtents.x, Spawner.ArenaExtents.x);
        var beeRandomY = random.NextInt(Spawner.ArenaHeight / 4, Spawner.ArenaHeight / 4 * 3);
        var beeRandomZ = random.NextInt(-Spawner.ArenaExtents.y, Spawner.ArenaExtents.y);

        endLocation = float3(beeRandomX, beeRandomY, beeRandomZ);

        carriedEntity.Value = Entity.Null;
        targetedEntity.Value = Entity.Null;

        movement.GoTo(translation.Value, endLocation);
    }

    static void carryCommon(Entity entity,
        int entityInQueryIndex,
        ref BeeState state,
        TeamValue team,
        CarriedEntity carriedEntity,
        in Translation translation,
        EntityCommandBuffer.ParallelWriter writer,
        uint seed)
    {
        if (state.value == StateValues.Carrying)
        {
            var random = Random.CreateFromIndex((uint)entityInQueryIndex + seed);

            // If this bee is within it's hives area, drop the food
            if (Spawner.isPositionInGoalArea(translation.Value, team))
            {
                // Add updated movement information to food entity
                writer.AddComponent(entityInQueryIndex, carriedEntity.Value,
                    PP_Movement.Create(translation.Value + float3(0f, -1f, 0f), translation.Value.Floored()));

                writer.AddComponent(entityInQueryIndex, carriedEntity.Value,
                    new Food { isBeeingCarried = false });

                state.value = StateValues.Idle;
            }
        }
    }


    static void attackCommon(Entity entity,
        int entityInQueryIndex,
        ref BeeState state,
        TeamValue team,
        int beeCount,
        NativeArray<Entity> beeEntityData,
        NativeArray<float3> beeTranslationData,
        NativeArray<Entity> beeCarriedEntityData,
        ref PP_Movement movement,
        ref TargetedEntity targetedEntity,
        Translation translation,
        EntityCommandBuffer.ParallelWriter writer,
        Spawner spawner
    )
    {
        if (state.value == StateValues.Attacking)
        {
            bool targetBeeStillExists = false;
            float3 targetBeeLocation = float3.zero;


            for (int i = 0; i < beeCount; i++)
            {
                if (beeEntityData[i] == targetedEntity.Value)
                {
                    targetBeeStillExists = true;
                    targetBeeLocation = beeTranslationData[i];

                    movement.UpdateEndPosition(beeTranslationData[i]);

                    float translationDistance =
                        distancesq(translation.Value, beeTranslationData[i]);

                    if (translationDistance <= squaredInteractionDistance)
                    {
                        if (beeCarriedEntityData[i] != Entity.Null)  //todo: is this correct? ~ajh
                        {
                            //if the enemy Bee is carrying something, set it to no longer be
                            writer.AddComponent(entityInQueryIndex, beeCarriedEntityData[i],
                                    PP_Movement.Create(
                                        beeTranslationData[i] + float3(0f, -1f, 0f),
                                        beeTranslationData[i].Floored()));

                            writer.AddComponent(entityInQueryIndex, beeCarriedEntityData[i],
                                new Food { isBeeingCarried = false });
                        }

                        //destroy enemy bee
                        writer.DestroyEntity(entityInQueryIndex, beeEntityData[i]);

                        //spawn a bee bit
                        var bitsEntity = writer.Instantiate(entityInQueryIndex,
                            spawner.BeeBitsPrefab);

                        writer.SetComponent(entityInQueryIndex, bitsEntity,
                            new Translation { Value = beeTranslationData[i] });

                        writer.SetComponent(entityInQueryIndex, bitsEntity,
                            PP_Movement.Create(beeTranslationData[i], beeTranslationData[i].Floored()));

                        state.value = StateValues.Idle;
                    }

                    break;
                }
            }

            if (!targetBeeStillExists)
            {
                state.value = StateValues.Idle;
            }
            else if (movement.t >= 0.99f)
            {
                // If we have finished moving and are still seeking, go back to idle status
                state.value = StateValues.Idle;
                movement.GoTo(translation.Value, targetBeeLocation);
            }
        }
    }


    static void idleCommon(Entity entity,
        int entityInQueryIndex,
        ref BeeState state,
        TeamValue team,
        uint randomSeed,
        NativeArray<Entity> foodEntityData,
        NativeArray<float3> foodTranslationData,
        NativeArray<bool> foodCarriedData,
        ref PP_Movement movement,
        ref TargetedEntity targetedEntity,
        ref CarriedEntity carriedEntity,
        Translation translation,
        EntityCommandBuffer.ParallelWriter writer,
        Spawner spawner,
        int beeCount,
        NativeArray<Entity> beeEntityData,
        NativeArray<float3> beeTranslationData
    )
    {
        if (state.value == StateValues.Idle || state.value == StateValues.Wandering)
        {
            var random = Random.CreateFromIndex((uint)entityInQueryIndex + randomSeed);

            float foodOrEnemy = random.NextFloat();
            if (foodOrEnemy > spawner.ChanceToAttack) // Try to choose a food
            {
                // Check if there actually is any food
                if (foodTranslationData.Length > 0)
                {
                    // Choose food at random here
                    int randomInt = random.NextInt(foodTranslationData.Length);

                    targetedEntity.Value = foodEntityData[randomInt];

                    float3 randomFoodTranslation = foodTranslationData[randomInt];
                    var x = randomFoodTranslation.x;

                    if (!foodCarriedData[randomInt] &&
                        abs(foodTranslationData[randomInt].x) < Spawner.ArenaExtents.x)
                    {
                        if ((team == TeamValue.Blue && x > -40) || (team == TeamValue.Yellow && x < 40))
                        {
                            movement.GoTo(translation.Value, randomFoodTranslation);
                            state.value = StateValues.Seeking;
                        }
                    }
                }
            }
            else
            {
                var enemyBeeIdx = random.NextInt(beeCount);

                targetedEntity.Value = beeEntityData[enemyBeeIdx];
                movement.GoTo(translation.Value, beeTranslationData[enemyBeeIdx]);
                state.value = StateValues.Attacking;
            }

            // If it's still idling after trying to attack or seek,
            // go to a random location (the "wander" behavior)
            if (state.value == StateValues.Idle)
            {
                state.value = StateValues.Wandering;

                SetWanderLocation(
                    ref random,
                    ref carriedEntity,
                    ref targetedEntity,
                    ref movement,
                    translation);
            }
        }
    }
}