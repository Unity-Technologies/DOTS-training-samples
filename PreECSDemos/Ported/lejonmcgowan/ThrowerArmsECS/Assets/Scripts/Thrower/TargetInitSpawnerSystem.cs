using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class TargetInitSpawnerSystem : JobComponentSystem
{
    private BeginInitializationEntityCommandBufferSystem m_spawnerECB;

    protected override void OnCreate()
    {
        m_spawnerECB = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<TargetSpawnerComponentData>();
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ecb = m_spawnerECB.CreateCommandBuffer();

        Entities
            //.WithBurst(FloatMode.Default,FloatPrecision.Standard,true)
            .WithoutBurst()
            .ForEach((Entity entity, int entityInQueryIndex, TargetSpawnerComponentData spawner) =>
            {
                Random rng = new Random();
                rng.InitState();

                for (int i = 0; i < spawner.initSpawn; i++)
                {
                    var spawnY = rng.NextFloat(spawner.yRange.x, spawner.yRange.y);
                    var spawnX = rng.NextFloat(spawner.xRange.x, spawner.xRange.y);

                    Entity target = ecb.Instantiate(spawner.prefab);
                    ecb.SetComponent(target, new Translation()
                    {
                        Value = new float3(spawnX, spawnY, spawner.spawnZ)
                    });

                    var data = new TargetComponentData()
                    {
                        velocityX = spawner.velocityX,
                        rangeXMin = spawner.xRange.x,
                        rangeXMax = spawner.xRange.y
                    };

                    ecb.AddComponent(target, data);
                }

                Enabled = false;

            }).Run();

        //m_spawnerECB.AddJobHandleForProducer(spawnJobHandle);

        return inputDeps;
    }
}
