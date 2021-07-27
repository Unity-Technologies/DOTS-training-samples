using System.Numerics;
using System.Security.Cryptography;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class FlameCellSpawnSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var config = GetSingleton<GameConfigComponent>();
        var grid = config.SimulationSize;
        var fireCount = config.startingFireCount;
        var flashpoint = config.FlashPoint;
        Entity e = EntityManager.CreateEntity(typeof(HeatMapElement));
        var buffer = EntityManager.GetBuffer<HeatMapElement>(e);

        buffer.ResizeUninitialized(grid * grid);

        
        for (int j = 0; j < grid; j++)
        {
            for (var i = 0; i < grid; ++i)
            {
                var flame = ecb.Instantiate(config.FlameCellPrefab);
                var pos = new float3(i,0f,j);
                float4 color = new float4(config.FlameDefaultColor.r, config.FlameDefaultColor.g, config.FlameDefaultColor.b, 1f );
                   
               
                var t = new Translation()
                {
                    Value = pos
                };
                var index = j * grid + i;
                ecb.SetComponent(flame, t);
                ecb.SetComponent(flame,new HeatMapIndex(){index = index});
                ecb.SetComponent(flame, new URPMaterialPropertyBaseColor(){Value = color});

                var temp_t = buffer[index];
                temp_t.temperature = 0;
                buffer[index] = temp_t;
            }
        }

        var rand = new Unity.Mathematics.Random();
        rand.InitState();
        for (int i = 0; i < fireCount; i++)
        {
            var index = rand.NextInt(0, grid * grid);
            buffer[index] = new HeatMapElement(){temperature = rand.NextFloat(flashpoint, 1)};
            

        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        // Run just once, unless reset elsewhere.
        Enabled = false;
    }
}
