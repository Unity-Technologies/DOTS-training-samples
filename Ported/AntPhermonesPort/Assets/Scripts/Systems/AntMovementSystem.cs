using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class AntMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .ForEach((ref Translation translation, in AntMovement ant) =>
            {
                translation.Value.x = (float)((time + 2.0f) % 100) - 50f;
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
