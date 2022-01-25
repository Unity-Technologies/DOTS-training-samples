using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

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
        
        // Get "closest" Food based on distance calculation
        Entities.WithAll<BeeTag>()
            .ForEach((Entity entity, in Translation translation) =>
            {
                for (int i = 0; i < foodCount; i++)
                {
                    float translationDistance = abs(distance(translation.Value, foodTranslationData[i].Value));
                    if (translationDistance <= distanceDelta && foodEntityData[i] != Entity.Null)
                    {
                        Debug.Log("Found Match");
                        ecb.AddComponent(entity, new CarriedEntity {Value = foodEntityData[i]});
                        break;
                    }
                }
            }).WithDisposeOnCompletion(foodTranslationData)
            .WithDisposeOnCompletion(foodEntityData)
            .Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}