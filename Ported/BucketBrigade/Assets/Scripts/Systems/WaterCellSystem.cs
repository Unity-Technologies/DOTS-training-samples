using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial class WaterCellSystem : SystemBase
{
    
    
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        // TODO : use a Query instead
        Entities
            .WithAll<Volume, Position>()
            .WithNone<BucketId>().ForEach((ref NonUniformScale scale, ref Volume volume) =>
        {
            volume.Value = math.min(volume.Value + dt * 10, 100); // TODO : use a filling value
            scale.Value.x = volume.Value / 100.0f;
            scale.Value.z = volume.Value / 100.0f;
        }).ScheduleParallel();
    }
}

[BurstCompile]
partial struct WaterCellSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WaterCellConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<WaterCellConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var waterCell = CollectionHelper.CreateNativeArray<Entity>(config.CellCount, Allocator.Temp);

        
        ecb.Instantiate(config.WaterCellPrefab, waterCell);


        int i = 0;
        Unity.Mathematics.Random rand = new Unity.Mathematics.Random((uint)(config.CellSize * config.CellCount * config.GridSize));
        float offset = config.GridSize * config.CellSize * 0.5f + 0.5f;
        foreach (var cell in waterCell)
        {
            switch(i % 4)
            {
                case 0:
                    {
                        ecb.SetComponent(cell, new Translation { Value = new float3(rand.NextFloat(-offset, offset), 0, -offset) });
                    }
                    break;
                case 1:
                    {
                        ecb.SetComponent(cell, new Translation { Value = new float3(rand.NextFloat(-offset, offset), 0, offset) });
                    }
                    break;
                case 2:
                    {
                        ecb.SetComponent(cell, new Translation { Value = new float3(-offset, 0, rand.NextFloat(-offset, offset) ) });
                    }
                    break;
                case 3:
                default:
                    {
                        ecb.SetComponent(cell, new Translation { Value = new float3(offset, 0, rand.NextFloat(-offset, offset)) });
                    }
                    break;
            }
            ++i;
        }

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}
