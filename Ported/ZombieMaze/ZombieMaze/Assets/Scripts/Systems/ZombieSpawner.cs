using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(MazeGenerator))]
public class ZombieSpawnerSystem : SystemBase
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

        Entities.ForEach((Entity entity, ref Random random, in ZombieSpawner zombieSpawner, in Spawner spawner) =>
        {
            for (int i = 0; i < zombieSpawner.NumZombies; i++)
            {
                var position = math.round(random.Value.NextFloat2() * spawner.MazeSize - spawner.MazeSize / 2);
                var zombieEntity = ecb.Instantiate(spawner.Prefab);
                ecb.SetComponent(zombieEntity, new Position { Value = position });
                ecb.SetComponent(zombieEntity, new Random { Value = new Unity.Mathematics.Random(((uint)i + 1) * 4391) });
                ecb.SetComponent(zombieEntity,
                    new URPMaterialPropertyBaseColor { Value = new float4(0, random.Value.NextFloat(), 0, 1) });
            }
            ecb.DestroyEntity(entity);
        }).Schedule();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
