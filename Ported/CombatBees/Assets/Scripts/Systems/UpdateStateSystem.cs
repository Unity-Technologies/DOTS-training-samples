using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(SpawnerSystem))]
public partial class UpdateStateSystem : SystemBase
{
    private EntityQuery foodQuery;
    private EntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // Get NativeArray data for food translation and entity data
        foodQuery = GetEntityQuery(typeof(FoodTag));
        int foodCount = foodQuery.CalculateEntityCount();
        NativeArray<Translation> foodTranslationData = new NativeArray<Translation>(foodCount, Allocator.TempJob);
        NativeArray<Entity> foodEntityData = new NativeArray<Entity>(foodCount, Allocator.TempJob);
        Entities
            .WithStoreEntityQueryInField(ref foodQuery)
            .WithAll<FoodTag>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
            {
                foodTranslationData[entityInQueryIndex] = translation;
                foodEntityData[entityInQueryIndex] = entity;
            }).WithName("GetFoodData")
            .ScheduleParallel();

        // Used to determine if two translations are "equal enough"
        const float distanceDelta = 0.1f;
        
        // ECB for recording component add / remove
        // NOTE: Not necessary if only modifying pre-existing component values
        var ecb = commandBufferSystem.CreateCommandBuffer();
        
        Random random = new Random(1234);
        var goalDepth = 10;
        var arenaExtents = new int2(40, 15);
        var halfArenaHeight = 10;
        
        // Get "close enough" Food based on distance calculation
        Entities.WithAll<BeeTag>()
            .ForEach((Entity entity, ref State state, ref PP_Movement movement, in Translation translation, in BeeTeam team) =>
            {
                // If Bee is carrying -> continue
                //           seeking -> check for attack option then check for carry option
                //           attacking -> check for carry option then check for seeking
                if (state.value == StateValues.Carrying)
                {
                    return;
                }

                if (state.value == StateValues.Seeking)
                {
                    // Also potentially check for attack here
                    for (int i = 0; i < foodCount; i++)
                    {
                        float translationDistance = distance(translation.Value, foodTranslationData[i].Value);
                        if (translationDistance <= distanceDelta && foodEntityData[i] != Entity.Null)
                        {
                            state.value = StateValues.Carrying;
                            ecb.AddComponent(entity, new CarriedEntity {Value = foodEntityData[i]});
                            
                            // Calculate end location based on team value;
                            float3 endLocation;
                            if (team.Value == TeamValue.Yellow)
                            {
                                var randomX = random.NextInt(arenaExtents.x + 1, arenaExtents.x + goalDepth - 1);
                                var randomY = random.NextInt(halfArenaHeight + halfArenaHeight);
                                var randomZ = random.NextInt(-arenaExtents.y + 1, arenaExtents.y - 1);
                                endLocation = float3(randomX, randomY, randomZ);
                            }
                            else
                            {
                                var randomX = random.NextInt(-arenaExtents.x - goalDepth + 1, -arenaExtents.x - 1);
                                var randomY = random.NextInt(halfArenaHeight + halfArenaHeight);
                                var randomZ = random.NextInt(-arenaExtents.y + 1, arenaExtents.y - 1);
                                endLocation = float3(randomX, randomY, randomZ);
                            }
                            
                            movement.endLocation = endLocation;
                            movement.startLocation = translation.Value;
                            movement.t = 0.0f;
                            // Add updated movement information to food entity;
                            ecb.AddComponent(foodEntityData[i], new PP_Movement{endLocation = endLocation, startLocation = foodTranslationData[i].Value, t = 0.0f});
                            break;
                        }
                    }

                    return;
                }
                // Fall through to seeking
                // Choose food at random here
                int randomInt = random.NextInt(0, foodTranslationData.Length - 1);
                Translation randomFoodTranslation = foodTranslationData[randomInt];
                
                movement.endLocation = randomFoodTranslation.Value;
                movement.startLocation = translation.Value;
                movement.t = 0.0f;
                state.value = StateValues.Seeking;

            }).WithDisposeOnCompletion(foodTranslationData)
            .WithDisposeOnCompletion(foodEntityData)
            .WithName("ProcessBeeState")
            .Schedule();
        
        commandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}