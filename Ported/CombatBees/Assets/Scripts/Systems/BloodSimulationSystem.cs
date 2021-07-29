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
        var gravity = gameConfig.Gravity;

        Entities
            .ForEach((Entity entity, ref Blood blood, ref Translation pos) =>
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
                    // hit the ground

                }
            }).ScheduleParallel();
        Dependency.Complete();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
