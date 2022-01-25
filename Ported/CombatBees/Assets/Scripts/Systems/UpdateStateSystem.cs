using System.Net;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(SpawnerSystem))]
public partial class UpdateStateSystem : SystemBase
{
    private EntityQuery foodQuery;
    protected override void OnUpdate()
    {
        // Get NativeArray data for food translation and entity data
        foodQuery = GetEntityQuery(typeof(FoodTag));
        int foodCount = foodQuery.CalculateEntityCount();
        NativeArray<Translation> foodTranslationData = new NativeArray<Translation>(foodCount, Allocator.TempJob);
        NativeArray<Entity> foodEntityData = new NativeArray<Entity>(foodCount, Allocator.TempJob);
        Dependency = Entities
            .WithStoreEntityQueryInField(ref foodQuery)
            .WithAll<FoodTag>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
            {
                foodTranslationData[entityInQueryIndex] = translation;
                foodEntityData[entityInQueryIndex] = entity;
            }).ScheduleParallel(Dependency);

        // Used to determine if two translations are "equal enough"
        const float distanceDelta = 0.1f;
        
        // ECB for recording component add / remove
        // NOTE: Not necessary if only modifying pre-existing component values
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        // Get "close enough" Food based on distance calculation
        Dependency = Entities.WithAll<BeeTag>()
            .ForEach((Entity entity, ref State state, ref PP_Movement movement, in Translation translation) =>
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
                            break;
                        }
                    }

                    return;
                }
                // Fall through to seeking
                // Choose food at random here
                
                Random r = new Random((uint)entity.Index);
                int randomInt = r.NextInt(0, foodTranslationData.Length - 1);
                Translation randomFoodTranslation = foodTranslationData[randomInt];

                movement.endLocation = randomFoodTranslation.Value;
                movement.startLocation = translation.Value;

            }).WithDisposeOnCompletion(foodTranslationData)
            .WithDisposeOnCompletion(foodEntityData)
            .Schedule(Dependency);
        
        Dependency.Complete();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}