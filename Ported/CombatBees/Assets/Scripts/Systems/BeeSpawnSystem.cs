using System.Security.Cryptography.X509Certificates;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct BeeSpawnSystem : ISystem
{
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var beeSizeRange = config.maximumBeeSize - config.minimumBeeSize;
        var beeSizeMiddle = config.minimumBeeSize + beeSizeRange * .5f;
        foreach(var (hive, team) in SystemAPI.Query<RefRO<Hive>, Team>())
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var bees = new NativeArray<Entity>(config.startBeeCount, Allocator.Temp);
            ecb.Instantiate(config.beePrefab, bees);
            
            ecb.SetSharedComponent(bees, new Team()
            {
                number = team.number
            });
            var hiveValue = hive.ValueRO;
            var color = new URPMaterialPropertyBaseColor { Value = (Vector4)hiveValue.color };

            foreach (var bee in bees)
            {
                ecb.SetComponent(bee, color);
                var pos = hiveValue.boundsPosition;
                pos.y = bee.Index;
                var position = hiveValue.boundsPosition;
                position.x += noise.cnoise(pos / 10f) * hiveValue.boundsExtents.x;
                position.y += noise.cnoise(pos / 11f) * hiveValue.boundsExtents.y;
                position.z += noise.cnoise(pos / 12f) * hiveValue.boundsExtents.z;
                ecb.SetComponent(bee, new LocalTransform
                {
                    Position = position,
                    Scale = math.clamp(noise.cnoise(pos / 13f) * 2f * beeSizeRange, config.minimumBeeSize, config.maximumBeeSize) + beeSizeMiddle
                });
            }
        }

        state.Enabled = false;
    }
}