using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

class BloodSimulationSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameConfig>();
        RequireSingletonForUpdate<ShaderOverrideCenterSize>();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var parallelECB = ecb.AsParallelWriter();
        var gameConfig = GetSingleton<GameConfig>();

        float deltaTime = Time.DeltaTime;
        var elapsedTime = Time.ElapsedTime;
        var gravity = gameConfig.Gravity;
        var bloodLifeTime = gameConfig.BloodLifeTime;
        const float bloodSize = 0.02f;

        Entities
            .ForEach((Entity entity, ref Blood blood, ref Translation pos, ref NonUniformScale scale) =>
            {
                if (pos.Value.y > 0.0f)
                {
                    pos.Value.x = pos.Value.x + blood.Speed.x * deltaTime;
                    pos.Value.y = math.max(0.0f, pos.Value.y + blood.Speed.y * deltaTime);
                    pos.Value.z = pos.Value.z + blood.Speed.z * deltaTime;
                    blood.Speed.y -= gravity * deltaTime;
                }
                else
                {
                    var normalizedBleedingTime = (elapsedTime - blood.SpawnTime) / bloodLifeTime;
                    // shrink the blood splatters
                    scale.Value = new float3(bloodSize) * (float) (1d - normalizedBleedingTime);

                    // hit the ground
                    if (normalizedBleedingTime > 1d)
                    {
                        parallelECB.DestroyEntity(0, entity);
                    }
                }
            }).ScheduleParallel();
        Dependency.Complete();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
