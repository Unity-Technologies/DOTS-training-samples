using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

[AlwaysUpdateSystem]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ParticleSpawnSystem : JobComponentSystem
{
    EndInitializationEntityCommandBufferSystem m_Ecb;
    EntityArchetype m_ParticleArchetype;
    bool m_hasStarted = false;
    protected override void OnCreate()
    {
        base.OnCreate();
        m_ParticleArchetype = EntityManager.CreateArchetype(
            typeof(ParticlePosition),
            typeof(ParticleVelocity),
            typeof(ParticleColor),
            typeof(LocalToWorld));

        m_Ecb = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (m_hasStarted)
            return default;

        m_hasStarted = true;

        //var props = GetSingleton<EmitterPropetiers>();
        var spawnCount = 1000;

        using (var entities = new NativeArray<Entity>(spawnCount, Allocator.Temp))
        {
            EntityManager.CreateEntity(m_ParticleArchetype, entities);
        }

        var entityCommandBuffer = m_Ecb.CreateCommandBuffer();

        uint seed = 2; 
        const float sphereRadius = 5.0f;
        return Entities.ForEach((Entity entity, ref ParticlePosition position, ref ParticleVelocity velocity) =>
        {
            var rng = new Random(seed * (uint)(1 + entity.Index));
            position.value = sphereRadius * rng.NextFloat3Direction() * rng.NextFloat();
            velocity.value = float3.zero; 

        }).Schedule(inputDeps);
    }
}




