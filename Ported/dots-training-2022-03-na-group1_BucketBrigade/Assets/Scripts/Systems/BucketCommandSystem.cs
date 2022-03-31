using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using static BucketBrigadeUtility;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial class BucketCommandSystem : SystemBase
{
    void ProcessBufferDumpBucket(int frame)
    {
        var splashmapBuffer = BucketBrigadeUtility.GetSplashmapBuffer(this);
        var heatmapBuffer = BucketBrigadeUtility.GetHeatmapBuffer(this);
        
        Entities.WithName("DumpBucketCommand")
            .ForEach((ref DynamicBuffer<DumpBucketCommand> commandBuffer) =>
            {
                for (var iCmd = 0; iCmd < commandBuffer.Length; iCmd++)
                {
                    var worker = commandBuffer[iCmd].Worker;

                    var bucketHeld = GetComponent<BucketHeld>(worker);

                    if (bucketHeld.Value == Entity.Null || !bucketHeld.IsFull)
                        continue;
                    
                    SetComponent(worker, new BucketHeld(bucketHeld.Value, false));
                    SetComponent(bucketHeld.Value, new MyBucketState(BucketState.EmptyCarrried, frame));
                    SetComponent(worker, new Speed() { Value = EmptyBucketSpeed });
                    
                    // splash?
                    FireSuppressionSystem
                        .AddSplashByIndex(ref splashmapBuffer, heatmapBuffer.Length, commandBuffer[iCmd].fireTileIndex );
                }
                
                commandBuffer.Clear();
            }).Run();
    }

    protected override void OnUpdate()
    {
        var time = (float)Time.ElapsedTime;
        
        var frame = GetCurrentFrame();
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        ProcessBufferDumpBucket(frame);
        
        Entities.WithName("DropBucketCommand")
            .ForEach((ref DynamicBuffer<DropBucketCommand> commandBuffer) =>
            {
                for (var iCmd = 0; iCmd < commandBuffer.Length; iCmd++)
                {
                    var command = commandBuffer[iCmd];

                    var bucketHeld = GetComponent<BucketHeld>(command.Worker);

                    if (bucketHeld.Value == Entity.Null)
                        continue;

                    MyBucketState bucketState = GetComponent<MyBucketState>(bucketHeld.Value);

                    if (bucketState.Value == BucketState.FullCarried)
                    {
                        bucketState.Value = BucketState.FullOnGround;
                    }
                    else
                    {
                        bucketState.Value = BucketState.EmptyOnGround;
                    }

                    bucketState.FrameChanged = frame;

                    SetComponent(bucketHeld.Value, bucketState);
                    SetComponent(command.Worker, new Speed() { Value = FreeSpeed });
                    SetComponent(command.Worker, new BucketHeld() { Value = Entity.Null });
                }
                
                commandBuffer.Clear();
            }).Run();
        
        
        Entities.WithName("FillBucketCommand")
            .ForEach((ref DynamicBuffer<FillBucketCommand> commandBuffer) =>
            {
                for (var iCmd = 0; iCmd < commandBuffer.Length; iCmd++)
                {
                    var command = commandBuffer[iCmd];

                    var bucketHeld = GetComponent<BucketHeld>(command.Worker);

                    if (bucketHeld.Value == Entity.Null || bucketHeld.IsFull)
                        continue;

                    var WorkerPosition = GetComponent<Position>(command.Worker);
                    var waterPoolPosition = GetComponent<Position>(command.WaterPool);

                    if (!IsVeryClose(WorkerPosition.Value, waterPoolPosition.Value))
                        continue;
                    
                    SetComponent(command.Worker, Speed.WithFullBucket);
                    SetComponent(command.Worker, new MyWaterPool() { Value = command.WaterPool });
                    SetComponent(command.Worker, new Cooldown() { Value = time + FillDelay });
                    SetComponent(command.Worker, new BucketHeld(bucketHeld.Value, true));
                    SetComponent(command.Worker, new Speed() { Value = FullBucketSpeed });
                    SetComponent(bucketHeld.Value, new MyBucketState(BucketState.FullCarried, frame));
                    
                    SetComponent(command.Worker, new MyWorkerState(WorkerState.FillingBucket));
                }
            }).Run();
        
        Entities.WithName("PickupBucketCommand")
            .ForEach((Entity entity, ref DynamicBuffer<PickupBucketCommand> commandBuffer) =>
            {
                for (var iCmd = 0; iCmd < commandBuffer.Length; iCmd++)
                {
                    var command = commandBuffer[iCmd];

                    // delay 1 frame if delay exists
                    if (command.Delay > 0)
                    {
                        command.Delay -= 1;
                        ecb.AppendToBuffer(entity, command);
                        continue;
                    }

                    var bucketState = GetComponent<MyBucketState>(command.Bucket);

                    switch (bucketState.Value)
                    {
                        case BucketState.EmptyOnGround:
                            SetComponent(command.Bucket, new MyBucketState(BucketState.EmptyCarrried, frame));
                            SetComponent(command.Worker, new BucketHeld(command.Bucket, false));
                            SetComponent(command.Worker, Speed.WithEmptyBucket);
                            break;

                        case BucketState.FullOnGround:
                            SetComponent(command.Bucket, new MyBucketState(BucketState.FullCarried, frame));
                            SetComponent(command.Worker, new BucketHeld(command.Bucket, true));
                            SetComponent(command.Worker, Speed.WithFullBucket);
                            break;
                    }

                }
                
                commandBuffer.Clear();
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
