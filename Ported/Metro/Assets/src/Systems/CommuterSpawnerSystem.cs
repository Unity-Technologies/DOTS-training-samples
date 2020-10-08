using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CommuterSpawnerSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
    private Random m_Random;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        m_Random = new Random((uint)System.DateTime.Now.Ticks);
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        //var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithName("commuter_spawner")
            .WithStructuralChanges()
            .ForEach((Entity platform, in LocalToWorld localToWorld, in Translation translation, in CommuterSpawner spawner) =>
            {
                if (math.distancesq(localToWorld.Position, translation.Value) > 0.001f) return; // Matrix hasn't updated yet, try again next frame

                var commuters = EntityManager.Instantiate(spawner.Prefab, spawner.Count, Unity.Collections.Allocator.Temp);
                foreach (var commuter in commuters)
                {
                    float2 pos2D = m_Random.NextFloat2(spawner.Min, spawner.Max);
                    float4 pos = math.mul(localToWorld.Value, new float4(pos2D.x, spawner.Height, pos2D.y, 1));
                    SetComponent(commuter, new Translation
                    {
                        Value = new float3(pos.x, pos.y, pos.z)
                    });

                    EntityManager.AddComponentData(commuter, new NonUniformScale
                    {
                        Value = new float3(1, math.lerp(0.5f, 1.5f, m_Random.NextFloat()), 1)
                    });

                    EntityManager.AddComponentData(commuter, new CommuterOnPlatform
                    {
                        Value = platform
                    });
                }

                EntityManager.RemoveComponent<CommuterSpawner>(platform);
                commuters.Dispose();
            }).Run(); // TODO: switch to parallel?

        //m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
