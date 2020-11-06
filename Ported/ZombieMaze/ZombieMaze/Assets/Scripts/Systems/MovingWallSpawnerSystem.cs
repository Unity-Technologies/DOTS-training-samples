using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(MazeGenerator))]
public class MovingWallSpawnerSystem : SystemBase
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

        var jobHandle = Entities.ForEach((Entity entity, ref Random random, in MovingWallSpawner wallSpawner, in Spawner spawner) =>
        {
            for (int i = 0; i < wallSpawner.NumWalls; i++)
            {
                var position = random.Value.NextFloat2() * spawner.MazeSize - spawner.MazeSize / 2;
                var dir = new int2(random.Value.NextBool() ? 1 : -1, 0);
                var speed = (random.Value.NextFloat() - 0.5f) * 50 + 100;
                var mw = new MovingWall { Index = position, Range = 5, Direction = dir, Speed = (int)speed };

                var wallEntity = ecb.Instantiate(spawner.Prefab);
                ecb.SetComponent(wallEntity, mw);
                ecb.SetComponent(wallEntity, new Position { Value = position });
                ecb.SetComponent(wallEntity, new Speed(1));
                ecb.SetComponent(wallEntity, random);
            }
            ecb.DestroyEntity(entity);
        }).Schedule(Dependency);

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        Dependency = jobHandle;
    }
}
    
