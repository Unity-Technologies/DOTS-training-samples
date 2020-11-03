using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class TileSpawnerSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        var jobHandle = Entities.ForEach((Entity entity, in TileSpawner spawner) =>
        {
            var halfWidth = spawner.TileSize.x / 2;
            var halfHeight = spawner.TileSize.y / 2;
            
            for (int i = 0; i < spawner.TileSize.x; i++)
            for (int j = 0; j < spawner.TileSize.y; j++)
            {
                var spawnedTile = ecb.Instantiate(spawner.TilePrefab);
                ecb.SetComponent(spawnedTile, new Translation {Value = new float3(i - halfWidth, 0, j - halfHeight)});
            }

            ecb.DestroyEntity(entity);
        }).Schedule(Dependency);
        
        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        
    }
}
