
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PlayerCollisionSystem : SystemBase
{
    private EntityCommandBufferSystem ecbs;

    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(Player));
        RequireForUpdate(query);

        ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        Entity player = GetSingletonEntity<Player>();
        float3 playerPos = GetComponent<Translation>(player).Value;

        Entity board = GetSingletonEntity<Board>();
        float radius = GetComponent<Radius>(board).Value;

        EntityCommandBuffer.ParallelWriter ecb = ecbs.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithAll<Bullet>()
            .ForEach((int entityInQueryIndex, Entity entity, in Translation translation) =>
            {
                float3 pos = translation.Value;

                if (pos[0] - radius < playerPos[0]
                    && pos[0] + radius > playerPos[0]
                    //&& pos[1] - radius < playerPos[1]
                    //&& pos[1] + radius > playerPos[1]
                    && pos[2] - radius < playerPos[2]
                    && pos[2] + radius > playerPos[2])
                {
                    ecb.AddComponent(entityInQueryIndex, player, new WasHit { Count = 1 });
                }
            })
            .ScheduleParallel();

        ecbs.AddJobHandleForProducer(Dependency);
    }
}
