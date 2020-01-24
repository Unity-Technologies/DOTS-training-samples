using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

[AlwaysUpdateSystem]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ParticleSpawnSystem : JobComponentSystem
{
    EntityArchetype m_ParticleArchetype;
    bool m_HasStartedHack = false;
    protected override void OnCreate()
    {
        base.OnCreate();
        m_ParticleArchetype = EntityManager.CreateArchetype(
            typeof(ParticlePosition),
            typeof(ParticleVelocity),
            typeof(ParticleColor),
            typeof(LocalToWorld));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (m_HasStartedHack)
            return default;

        m_HasStartedHack = true;

        //var props = GetSingleton<EmitterPropetiers>();
        var spawnCount = 1000;

        using (var entities = new NativeArray<Entity>(spawnCount, Allocator.Temp))
        {
            EntityManager.CreateEntity(m_ParticleArchetype, entities);
        }

        uint seed = 2; 
        const float sphereRadius = 50.0f;
        return Entities.ForEach((Entity entity, ref ParticlePosition position, ref ParticleVelocity velocity) =>
        {
            var rng = new Random(1 + (seed * (uint)entity.Index));
            position.value = sphereRadius * rng.NextFloat3Direction() * rng.NextFloat();
            velocity.value = float3.zero; 
        }).Schedule(inputDeps);
    }
}




