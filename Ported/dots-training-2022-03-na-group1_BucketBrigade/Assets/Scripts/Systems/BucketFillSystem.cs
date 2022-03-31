using Unity.Entities;
using static BucketBrigadeUtility;
using Unity.Mathematics;

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
                if (cooldown.Value > currentTime)
                {
                    var t = math.clamp(1f - (cooldown.Value - currentTime) / FillDelay, 0f, 1f);
                    var singleScale = math.lerp(EmptyWaterSize, FullWaterSize, t);
                    SetComponent(bucketRef.Value, new Scale() { Value = new float3(singleScale, singleScale, singleScale) });
                }
                else
                {
                    // At this point, the cooldown is done.

                    SetComponent(bucketRef.Value, new Scale() { Value = new float3(FullWaterSize, FullWaterSize, FullWaterSize) });
                    BucketHelper.SetState(manager, bucketRef.Value, BucketState.FullCarried);
                    WaterPoolHelper.DecreaseVolume(manager, poolRef.Value);
                    workerState.Value = WorkerState.Idle;
                }
            })
            .Run();
    }
}
