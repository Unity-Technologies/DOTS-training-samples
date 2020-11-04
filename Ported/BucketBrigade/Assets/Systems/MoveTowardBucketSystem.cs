using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveTowardBucketSystem : SystemBase
{


    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach(Entity entity, ref Translation, ref MoveTowardBucket)
        {

        }
    }
}
