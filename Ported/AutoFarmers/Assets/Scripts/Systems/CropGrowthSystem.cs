using UnityEngine;
using Unity.Collections;
using Unity.Entities;

public class CropGrowthSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, ref CropGrowth crop) =>
            {
                Debug.Log("In Crop Growth Loop");
                if (crop.GrowthValue < 25)
                {
                    // make bigger
                }
                crop.GrowthValue++;
            }).Run();

        ecb.Playback(EntityManager);
    }
}
