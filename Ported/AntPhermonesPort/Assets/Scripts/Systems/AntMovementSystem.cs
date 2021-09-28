using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial class AntMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .ForEach((Entity entity, in AntMovement ant) =>
            {
                Debug.Log("I am an ant moving!");
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
