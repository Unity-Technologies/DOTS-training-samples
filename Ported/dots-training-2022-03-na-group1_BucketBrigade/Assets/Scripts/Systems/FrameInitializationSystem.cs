using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using static BucketBrigadeUtility;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
[UpdateAfter(typeof(SpawnerSystem))]
public partial class FrameInitializationSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        IncrementFrame();
        
        var waterPoolBufferEntity = GetSingletonEntity<WaterPoolInfo>();
        var waterPoolBuffer = EntityManager.GetBuffer<WaterPoolInfo>(waterPoolBufferEntity);
        
        waterPoolBuffer.Clear();
        
        var bucketBufferEntity = GetSingletonEntity<FreeBucketInfo>();
        var bucketBuffer = EntityManager.GetBuffer<FreeBucketInfo>(bucketBufferEntity);
        
        bucketBuffer.Clear();
    }
}
