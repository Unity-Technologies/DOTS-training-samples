
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
public class PlayerCollisionSystem : SystemBase
{
    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(Bullet));
        RequireForUpdate(query);
    }

    protected override void OnUpdate()
    {
        Entity player = GetSingletonEntity<Player>();
        float3 playerPos = GetComponent<Translation>(player).Value;

        Entity board = GetSingletonEntity<Board>();
        float radius = GetComponent<Radius>(board).Value;

        NativeArray<int> hitCount = new NativeArray<int>(GetEntityQuery(typeof(Bullet)).CalculateEntityCount(), Allocator.TempJob);

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
                    hitCount[entityInQueryIndex] = 1;
                }
            })
            .Run();

        Entities
            .WithAll<Player>()
            .WithDisposeOnCompletion(hitCount)
            .ForEach((ref WasHit hit) =>
            {
                for (int i = 0; i < hitCount.Length; i++)
                {
                    if (hitCount[i] == 1)
                    {
                        hit.Count = 1;
                        break;
                    }
                }
            }).Run();

    }
}
