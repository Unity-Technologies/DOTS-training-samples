using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
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

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        int yellowBeeCount = yellowBeeQuery.CalculateEntityCount();
        NativeArray<Entity> yellowBeeEntityData = new NativeArray<Entity>(yellowBeeCount, Allocator.TempJob);
        NativeArray<Translation> yellowBeeTranslationData = new NativeArray<float3>(yellowBeeCount, Allocator.TempJob);
        NativeArray<Entity> yellowBeeCarriedEntityData = new NativeArray<Entity>(yellowBeeCount, Allocator.TempJob);

        int blueBeeCount = blueBeeQuery.CalculateEntityCount();
        NativeArray<Entity> blueBeeEntityData = new NativeArray<Entity>(blueBeeCount, Allocator.TempJob);
        NativeArray<Translation> blueBeeTranslationData = new NativeArray<float3>(blueBeeCount, Allocator.TempJob);
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
        const float squaredInteractionDistance = 1f;

        // Parallel ECB for recording component add / remove
        // NOTE: Not necessary if only modifying pre-existing component values
        var randomSeed = (uint) max(1,
            DateTime.Now.Millisecond +
            DateTime.Now.Second +
            DateTime.Now.Minute +
            DateTime.Now.Day +
            DateTime.Now.Month +
            DateTime.Now.Year);
        // var random = new Random(randomSeed);


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
                //carryCommon(entity, entityInQueryIndex, ref state, ref carriedEntity, in translation, in spawner, ref parallelWriter, randomSeed);

                if (state.value == StateValues.Carrying)
                {
                    var random = Random.CreateFromIndex((uint)entityInQueryIndex + randomSeed);

                    // If this bee is within it's hives area, drop the food
                    if (translation.Value.x > spawner.ArenaExtents.x + 0.5f)
                    {
                        // Add updated movement information to food entity
                        parallelWriter.AddComponent(entityInQueryIndex, carriedEntity.Value,
                            PP_Movement.Create(translation.Value + float3(0f, -1f, 0f), translation.Value.Floored()));

                        parallelWriter.AddComponent(entityInQueryIndex, carriedEntity.Value,
                            new Food { isBeeingCarried = false });

                        state.value = StateValues.Idle;
                    }
                }


            }).WithName("ProcessYellowCarryingState")
            .ScheduleParallel();



        // BLUE CARRY  state
        Entities.WithAll<BlueBeeTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state,
                ref CarriedEntity carriedEntity, in Translation translation) =>
            {
                if (state.value == StateValues.Carrying)
                {
                    var random = Random.CreateFromIndex((uint)entityInQueryIndex + randomSeed);

                    // If this bee is within it's hives area, drop the food
                    if (translation.Value.x < -spawner.ArenaExtents.x - 0.5f)
                    {
                        // Add updated movement information to food entity
                        parallelWriter.AddComponent(entityInQueryIndex, carriedEntity.Value,
                            PP_Movement.Create(translation.Value + float3(0f, -1f, 0f), translation.Value.Floored()));

                        parallelWriter.AddComponent(entityInQueryIndex, carriedEntity.Value,
                            new Food {isBeeingCarried = false});

                        state.value = StateValues.Idle;
                    }
                }
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
                                abs(foodTranslationData[i].Value.x) >= Spawner.ArenaExtents.x)
                            {
                                // If the food is already being carried,
                                // or if it's in a goal area,
                                // then give up on that food.
                                targetedEntity.Value = Entity.Null;
                                state.value = StateValues.Idle;
                                break;
                            }
                            movement.UpdateEndPosition(foodTranslationData[i].Value);
                        }

                        // Check if close enough to the food/wander-destination to pick it up or wander somewhere else
                        float translationDistance = distancesq(translation.Value, foodTranslationData[i].Value);
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
                                    new Food {isBeeingCarried = true});

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

        // BLUE ATTACK state
        Entities.WithAll<BlueBeeTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state, ref PP_Movement movement,
                ref TargetedEntity targetedEntity, in Translation translation) =>
            {
                if (state.value == StateValues.Attacking)
                {
                    bool targetBeeStillExists = false;
                    float3 targetBeeLocation = float3.zero;

                    for (int i = 0; i < yellowBeeCount; i++)
                    {
                        if (yellowBeeEntityData[i] == targetedEntity.Value)
                        {
                            targetBeeStillExists = true;
                            targetBeeLocation = yellowBeeTranslationData[i].Value;

                            float translationDistance =
                                distancesq(translation.Value, yellowBeeTranslationData[i].Value);

                            if (translationDistance <= 1.25f)
                            {
                                if (yellowBeeCarriedEntityData[i] != Entity.Null)
                                {
                                    //if the enemy Bee is carrying something, set it to no longer be
                                    parallelWriter.AddComponent(entityInQueryIndex, yellowBeeCarriedEntityData[i],
                                            PP_Movement.Create(
                                                yellowBeeTranslationData[i].Value + float3(0f, -1f, 0f),
                                                yellowBeeTranslationData[i].Value.Floored()));

                                    parallelWriter.AddComponent(entityInQueryIndex, yellowBeeCarriedEntityData[i],
                                        new Food { isBeeingCarried = false });
                                }

                                //destroy enemy bee
                                parallelWriter.DestroyEntity(entityInQueryIndex, yellowBeeEntityData[i]);

                                //spawn a bee bit
                                var bitsEntity = parallelWriter.Instantiate(entityInQueryIndex,
                                    spawner.BeeBitsPrefab);

                                parallelWriter.SetComponent(entityInQueryIndex, bitsEntity,
                                    new Translation { Value = yellowBeeTranslationData[i].Value });

                                parallelWriter.SetComponent(entityInQueryIndex, bitsEntity,
                                    PP_Movement.Create(
                                        yellowBeeTranslationData[i].Value, yellowBeeTranslationData[i].Value.Floored()));

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
                if (state.value == StateValues.Attacking)
                {
                    bool targetBeeStillExists = false;
                    float3 targetBeeLocation = float3.zero;

                    for (int i = 0; i < blueBeeCount; i++)
                    {
                        movement.UpdateEndPosition(beeTranslationData[i]);
                        
                        if (blueBeeEntityData[i] == targetedEntity.Value)
                        {
                            targetBeeStillExists = true;
                            targetBeeLocation = blueBeeTranslationData[i].Value;

                            float translationDistance = distancesq(translation.Value, blueBeeTranslationData[i].Value);

                            if (translationDistance <= squaredInteractionDistance)
                            {
                                if (blueBeeCarriedEntityData[i] != Entity.Null)
                                {
                                    //if the enemy Bee is carrying something, set it to no longer be
                                    parallelWriter.AddComponent(entityInQueryIndex, blueBeeCarriedEntityData[i],
                                            PP_Movement.Create(
                                                blueBeeTranslationData[i].Value + float3(0f, -1f, 0f),
                                                blueBeeTranslationData[i].Value.Floored()));

                                    parallelWriter.AddComponent(entityInQueryIndex, blueBeeCarriedEntityData[i],
                                        new Food { isBeeingCarried = false });
                                }

                                //destroy enemy bee
                                parallelWriter.DestroyEntity(entityInQueryIndex, blueBeeEntityData[i]);

                                //spawn a bee bit
                                var bitsEntity = parallelWriter.Instantiate(entityInQueryIndex,
                                    spawner.BeeBitsPrefab);

                                parallelWriter.SetComponent(entityInQueryIndex, bitsEntity,
                                    new Translation {Value = blueBeeTranslationData[i]});

                                parallelWriter.SetComponent(entityInQueryIndex, bitsEntity,
                                    PP_Movement.Create(
                                        blueBeeTranslationData[i], blueBeeTranslationData[i].Floored()));

                                state.value = StateValues.Idle;

                                break;
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

            }).WithReadOnly(blueBeeTranslationData)
            .WithReadOnly(blueBeeEntityData)
            .WithReadOnly(blueBeeCarriedEntityData)
            .WithDisposeOnCompletion(blueBeeCarriedEntityData)
            .WithName("ProcessBlueBeeAttackingState")
            .ScheduleParallel();




        // BLUE Idle state
        Entities.WithAll<BlueBeeTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state, ref PP_Movement movement,
                ref CarriedEntity carriedEntity, ref TargetedEntity targetedEntity, in Translation translation, in BeeTeam team) =>
            {
                if (state.value == StateValues.Idle || state.value == StateValues.Wandering)
                {
                    var random = Random.CreateFromIndex((uint)entityInQueryIndex + randomSeed);

                    float foodOrEnemy = random.NextFloat();
                    if (foodOrEnemy > spawner.ChanceToAttack) // Try to choose a food
                    {
                        // Check if there actually is any food
                        if (foodCount > 0)
                        {
                            // Choose food at random here
                            int randomInt = random.NextInt(foodCount); // Note: random int/uint values are non-inclusive of the maximum value
                            targetedEntity.Value = foodEntityData[randomInt];

                            Translation randomFoodTranslation = foodTranslationData[randomInt];
                            var x = randomFoodTranslation.Value.x;

                            if (!foodCarriedData[randomInt] &&
                                abs(foodTranslationData[randomInt].Value.x) < Spawner.ArenaExtents.x)
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
                        var enemyBeeIdx = random.NextInt(yellowBeeCount);

                        targetedEntity.Value = yellowBeeEntityData[enemyBeeIdx];
                        movement.GoTo(translation.Value, yellowBeeTranslationData[enemyBeeIdx].Value);
                        state.value = StateValues.Attacking;
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
            }).WithReadOnly(yellowBeeEntityData)
            .WithReadOnly(yellowBeeTranslationData)
            .WithReadOnly(foodTranslationData)
            .WithReadOnly(foodCarriedData)
            .WithReadOnly(foodEntityData)
            .WithDisposeOnCompletion(yellowBeeEntityData)
            .WithDisposeOnCompletion(yellowBeeTranslationData)
            .WithName("ProcessBLUEIdleState")
            .ScheduleParallel();


        // YELLOW Idle state
        Entities.WithAll<YellowBeeTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref BeeState state, ref PP_Movement movement,
                ref CarriedEntity carriedEntity, ref TargetedEntity targetedEntity, in Translation translation, in BeeTeam team) =>
            {
                if (state.value == StateValues.Idle)
                {
                    var random = Random.CreateFromIndex((uint)entityInQueryIndex + randomSeed);

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
                        var enemyBeeIdx = random.NextInt(blueBeeCount);

                        targetedEntity.Value = blueBeeEntityData[enemyBeeIdx];
                        movement.GoTo(translation.Value, blueBeeTranslationData[enemyBeeIdx].Value);
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
    static void carryCommon(Entity entity, int entityInQueryIndex, ref BeeState state,
        ref CarriedEntity carriedEntity, in Translation translation, in Spawner spawner, ref EntityCommandBuffer.ParallelWriter writer, uint seed)
    {
        if (state.value == StateValues.Carrying)
        {
            var random = Random.CreateFromIndex((uint)entityInQueryIndex + seed);

            // If this bee is within it's hives area, drop the food
            if (translation.Value.x > spawner.ArenaExtents.x + 0.5f)
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
}