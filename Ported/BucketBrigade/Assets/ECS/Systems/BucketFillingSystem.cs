using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BucketFillingSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameConfigComponent>();
    }
    protected override void OnUpdate()
    {
        var ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbs.CreateCommandBuffer();
        var config = GetSingleton<GameConfigComponent>();
        var bucketFillRate = config.BucketFillRate;

        Entities.ForEach((ref BucketStartFill fill, ref BucketFullComponent full, ref BucketVolumeComponent bucketVolume, in WaterCapacityComponent bucketCap) =>
        {
            var water = fill.Water;
            if (water == Entity.Null)
            {
                return;
            }

            var waterVol = GetComponent<WaterVolumeComponent>(water);

            var bucketRemainSpace = bucketCap.Capacity - bucketVolume.Volume;

            var delta = math.min(bucketFillRate, math.min(bucketRemainSpace, waterVol.Volume));
            
            ecb.SetComponent(water, new WaterVolumeComponent(){Volume = waterVol.Volume- delta});
            bucketVolume.Volume += delta;

            if (bucketVolume.Volume >= bucketCap.Capacity)
            {
                fill.Water = Entity.Null;
                full.full = true;
            }



        }).Schedule();
        
        ecbs.AddJobHandleForProducer(Dependency);
    }
}
