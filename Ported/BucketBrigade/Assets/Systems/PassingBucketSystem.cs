using Unity.Entities;
public class PassingBucketSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer()
        /*Entities
            .ForEach((Entity entity, in PasserBot bot, in HoldingBucket bucket) =>
            {
                
            }).Schedule();*/
    }
}

