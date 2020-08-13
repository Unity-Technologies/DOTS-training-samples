using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;

public struct Owner : IComponentData
{
    public Entity Value;
}

public class BucketSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityCommandBuffer cb = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithAll<Bucket>()
            .WithNone<Color>()
            .ForEach((Entity e) =>
            {
                cb.AddComponent<Color>(e);
            }).Run();
        cb.Playback(EntityManager);
        cb.Dispose();
        // update the color of the bucket based on the fill amount
        Entities
            .ForEach((Entity e, ref Color c, in Water w, in Bucket b) =>
            {
                c.Value = math.lerp(b.emptyColor, b.fullColor, w.volume / w.capacity);
            }).Schedule();
        
        var translations = GetComponentDataFromEntity<Translation>(true);
        var carried = GetComponentDataFromEntity<CarriedBucket>(true);
        
        Entities
            .WithReadOnly(translations)
            .WithReadOnly(carried)
            .WithNativeDisableContainerSafetyRestriction(translations)
            .WithAll<Bucket>()
            .ForEach((Entity e, ref Translation translation, in Owner owner) =>
            {
                if(carried.HasComponent(owner.Value))
                    translation.Value = translations[owner.Value].Value + new float3(0, 0.5f, 0);
            }).Schedule();
    }
}
