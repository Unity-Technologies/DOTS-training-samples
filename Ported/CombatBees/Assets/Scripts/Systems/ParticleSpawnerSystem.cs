using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public partial class ParticleSpawnerSystem : SystemBase
{
    private static readonly float3 up = new float3(0.0f, 1.0f, 0.0f);
    private Random random;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        random = new Random((uint)System.DateTime.Now.Ticks);
    }
    
    protected override void OnUpdate()
    {
        var system = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = system.CreateCommandBuffer().AsParallelWriter();
        uint seed = random.NextUInt();

        Entities
            .ForEach((Entity entity, int entityInQueryIndex, in ParticleSpawner spawner) =>
            {
                Random random = new Random(seed + (uint)entityInQueryIndex);
                
                ecb.DestroyEntity(entityInQueryIndex, entity);

                for (int i = 0; i < spawner.Count; ++i)
                {
                    var instance = ecb.Instantiate(entityInQueryIndex, spawner.Prefab);

                    var lifetime = new LifeTime
                    {
                        TimeRemaining = spawner.Lifetime,
                        TotalTime = spawner.Lifetime,
                    };
                    ecb.SetComponent(entityInQueryIndex, instance, lifetime);
                    
                    var translationComponent = new Translation { Value = spawner.Position };
                    ecb.SetComponent(entityInQueryIndex, instance, translationComponent);

                    float3 direction = up;
                    direction = math.rotate(quaternion.EulerXYZ(0.785398f, 0.0f, 0.0f), direction);
                    direction = math.rotate(quaternion.RotateY(random.NextFloat(0.0f, 6.28f)), direction);
                    float3 velocity = direction * spawner.Speed;
                    var velocityComponent = new Velocity { Value = velocity };
                    ecb.SetComponent(entityInQueryIndex, instance, velocityComponent);
                    
                    ecb.AddComponent<URPMaterialPropertyBaseColor>(entityInQueryIndex, instance);
                }
            }).ScheduleParallel();

        system.AddJobHandleForProducer(Dependency);
    }
}