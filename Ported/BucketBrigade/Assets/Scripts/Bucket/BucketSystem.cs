using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

public struct Owner : IComponentData
{
    public Entity Value;
}
public class BucketSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<Bucket>()
            .WithNone<Color>()
            .WithStructuralChanges()
            .ForEach((Entity e) =>
            {
                EntityManager.AddComponent<Color>(e);
            }).Run();
        // update the color of the bucket based on the fill amount
        Entities
            .ForEach((Entity e, ref Color c, in Water w, in Bucket b) =>
            {
                c.Value = math.lerp(b.emptyColor, b.fullColor, w.volume / w.capacity);
            }).ScheduleParallel();
        
        var translations = GetComponentDataFromEntity<Translation>(true);
        var carried = GetComponentDataFromEntity<CarriedBucket>(true);
        
        Entities
            .WithReadOnly(translations)
            .WithReadOnly(carried)
            .WithNativeDisableContainerSafetyRestriction(translations)
            .WithAll<Bucket>()
            .ForEach((Entity e, ref Translation translation, in Owner owner) =>
            {
                if (carried.HasComponent(owner.Value))
                    translation.Value = translations[owner.Value].Value + new float3(0, .4f, 0);
                else
                    translation.Value.y = .1f;
            }).ScheduleParallel();
   
        Entities
            .WithAll<Bucket>()
            .WithNone<Owner>()
            .ForEach((Entity e, ref Translation translation) =>
            {
                translation.Value.y = .1f;
            }).ScheduleParallel();
    }
}
