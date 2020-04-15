using Unity.Collections;
using Unity.Entities;

public class PercentCompleteSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(LaneInfo)));
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        LaneInfo laneInfo = GetSingleton<LaneInfo>();
        float totalDistance = laneInfo.EndXZ.y - laneInfo.StartXZ.y;
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref PercentComplete percentComplete, in Speed speed) =>
        {
            float relativeStartPosition = totalDistance * percentComplete.Value;
            var distance = speed.Value * deltaTime;
            percentComplete.Value = (relativeStartPosition + distance) / totalDistance;

        }).Schedule();

        ecb.Playback(EntityManager);
    }
}
