using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MazeGenerator))]
public class CapsuleSpawnerSystem : SystemBase
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

        var jobHandle = Entities.ForEach((Entity entity, ref Random random, in CapsuleSpawner capsuleSpawner, in Spawner spawner) =>
        {
            for (int i = 0; i < capsuleSpawner.NumCapsules; i++)
            {
                var position = math.round(random.Value.NextFloat2() * spawner.MazeSize) - spawner.MazeSize / 2;
                var capsuleEntity = ecb.Instantiate(spawner.Prefab);
                ecb.SetComponent(capsuleEntity, new Position { Value = position });
                ecb.SetComponent(capsuleEntity, new Random { Value = new Unity.Mathematics.Random(((uint)i + 1) * 4391) });
            }
            ecb.DestroyEntity(entity);
        }).Schedule(Dependency);

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        Dependency = jobHandle;
    }
}
