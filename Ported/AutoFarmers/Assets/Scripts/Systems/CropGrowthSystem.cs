using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public class CropGrowthSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var deltaTime = Time.DeltaTime;

        Entities
            .ForEach((Entity entity, ref Crop crop, ref Scale scale) =>
            {
                scale.Value += crop.GrowthRate * deltaTime;
                if (scale.Value >= crop.FullGrouthValue)
                {
                    ecb.RemoveComponent<Crop>(entity);
                    ecb.AddComponent<Plant>(entity);
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}
