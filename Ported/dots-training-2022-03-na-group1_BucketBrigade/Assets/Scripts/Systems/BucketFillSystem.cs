using Unity.Entities;

partial class BucketFillSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var currentTime = (float)Time.ElapsedTime;
        var manager = EntityManager;

        Entities
            .WithAll<Cooldown>()
            .ForEach((
                ref MyWorkerState workerState,
                in Cooldown cooldown,
                in BucketHeld bucketRef,
                in MyWaterPool poolRef) =>
            {
                // Not in filling state
                if (workerState.Value != WorkerState.FillingBucket) return;

                // Still waiting for the filling to be completed
                if (cooldown.Value < currentTime) return;

                // At this point, the cooldown is done.

                BucketHelper.SetState(manager, bucketRef.Value, BucketState.FullCarried);
                WaterPoolHelper.DecreaseVolume(manager, poolRef.Value);
                workerState.Value = WorkerState.Idle;
            })
            .Run();
    }
}
