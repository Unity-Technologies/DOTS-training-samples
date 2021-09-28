using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public partial class AntMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .ForEach((ref Translation translation, in AntMovement ant, in LocalToWorld ltw) =>
            {
                translation.Value += ltw.Forward * (float)((time + Config.MoveSpeed) % 100);
                //translation.Value.x = (float)((time + Config.MoveSpeed) % 100) - 50f;
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
