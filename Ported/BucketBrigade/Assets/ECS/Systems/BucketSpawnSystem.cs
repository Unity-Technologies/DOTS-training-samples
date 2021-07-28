using System.Security.Cryptography;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BucketSpawnSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameConfigComponent>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var config = GetSingleton<GameConfigComponent>();
        var bucketCount = config.BucketCount;

        var rand = new Unity.Mathematics.Random();
        rand.InitState();
        for (var i = 0; i < bucketCount; ++i)
        {
            var bucket = ecb.Instantiate(config.BucketPrefab);
            var pos = rand.NextFloat3();
            pos.y = 0F;
            pos *= config.SimulationSize;
            var t = new Translation()
            {
                Value = pos
            };
            ecb.SetComponent(bucket, t);
            ecb.AddComponent<BucketStartFill>(bucket);
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        // Run just once, unless reset elsewhere.
        Enabled = false;
    }
}
