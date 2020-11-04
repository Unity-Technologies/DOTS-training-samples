using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MazeGenerator : SystemBase
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

        var jobHandle = Entities.ForEach((Entity entity, in MazeSpawner mazeSpawner, in Spawner spawner) => {
            int halfWidth = (int)spawner.MazeSize.x / 2;
            int halfHeight = (int)spawner.MazeSize.y / 2;
            
            // top and bottom walls
            for (var i = 0; i < spawner.MazeSize.x; i++)
            {
                var spawnedTile = ecb.Instantiate(spawner.Prefab);
                ecb.SetComponent(spawnedTile, new Translation {Value = new float3(i - halfWidth, 0,  -halfHeight - 0.5f)});
                ecb.SetComponent(spawnedTile, new Rotation{Value = quaternion.Euler(0,math.PI, 0 )});
                
                spawnedTile = ecb.Instantiate(spawner.Prefab);
                ecb.SetComponent(spawnedTile, new Translation {Value = new float3(i - halfWidth, 0,  halfHeight - 0.5f)});
            }
            
            // side walls
            for (var i = 0; i < spawner.MazeSize.y; i++)
            {
                var spawnedTile = ecb.Instantiate(spawner.Prefab);
                ecb.SetComponent(spawnedTile, new Translation {Value = new float3(-halfWidth - 0.5f, 0,  i-halfHeight)});
                ecb.SetComponent(spawnedTile, new Rotation{Value = quaternion.Euler(0,-math.PI/2, 0 )});
                
                spawnedTile = ecb.Instantiate(spawner.Prefab);
                ecb.SetComponent(spawnedTile, new Translation {Value = new float3(halfWidth - 0.5f, 0,  i-halfHeight)});
                ecb.SetComponent(spawnedTile, new Rotation{Value = quaternion.Euler(0,math.PI/2 , 0)});
            }

            ecb.DestroyEntity(entity);
        }).Schedule(Dependency);
        
        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
    }
}
