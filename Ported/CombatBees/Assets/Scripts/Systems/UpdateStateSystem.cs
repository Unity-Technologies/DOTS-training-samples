using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using static Unity.Mathematics.math;

public partial class UpdateStateSystem : SystemBase
{
    private EntityQuery foodQuery;
    protected override void OnUpdate()
    {
        // Get NativeArray data for food translation and entity data
        foodQuery = GetEntityQuery(typeof(FoodTag));
        int foodCount = foodQuery.CalculateEntityCount();
        NativeArray<Translation> foodTranslationData = new NativeArray<Translation>(foodCount, Allocator.Temp);
        NativeArray<Entity> foodEntityData = new NativeArray<Entity>(foodCount, Allocator.Temp);
        Entities
            .WithStoreEntityQueryInField(ref foodQuery)
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
            {
                foodEntityData[entityInQueryIndex] = entity;
                foodTranslationData[entityInQueryIndex] = translation;
            })
            .ScheduleParallel();

        // Used to determine if two translations are "equal enough"
        const float distanceDelta = 0.1f;
        
        // ECB for recording component add / remove
        // NOTE: Not necessary if only modifying pre-existing component values
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Get "closest" Food based on distance calculation
        Entities.WithAll<BeeTag>()
            .ForEach((Entity entity, in Translation translation) =>
            {
                for (int i = 0; i < foodCount; i++)
                {
                    float translationDistance = abs(distance(translation.Value, foodTranslationData[i].Value));
                    if (translationDistance <= distanceDelta)
                    {
                        ecb.AddComponent(entity, new CarriedEntity {Value = foodEntityData[i]});
                        break;
                    }
                }
            }).ScheduleParallel();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}