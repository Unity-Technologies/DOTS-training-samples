using Unity.Collections;
using Unity.Entities;

public class PercentCompleteSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        //GetSingleton<LaneInfo>()
        Entities.ForEach((ref PercentComplete percentComplete, in Speed speed) =>
        {

        });
    }
}
