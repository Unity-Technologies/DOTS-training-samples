using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class AntMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .ForEach((ref Translation translation, in AntMovement ant) =>
            {
                //translation.Value += 1.0f;
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
