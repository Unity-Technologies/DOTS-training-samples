using Components;
using Unity.Entities;

public partial class BucketFillSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var currentTime = UnityEngine.Time.realtimeSinceStartup;
        var manager = EntityManager;

        Entities
            .WithAll<BucketFill>()
            .ForEach((
                ref MyWorkerState workerState,
                in BucketFill fillStatus,
                in BucketHeld bucketRef,
                in MyWaterPool poolRef) =>
            {
                // Not in filling state
                if (workerState.Value != WorkerState.FillingBucket) return;

                // Still waiting for the filling to be completed
                if (fillStatus.cooldown < currentTime) return;

                // At this point, the cooldown is done.

                BucketHelper.SetState(manager, bucketRef.Value, BucketState.Full);
                WaterPoolHelper.DecreaseVolume(manager, poolRef.Value);
                workerState.Value = WorkerState.Idle;
            })
            .Run();
    }
}
