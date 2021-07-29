using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerCannonballCollisionSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<Player>();
        RequireForUpdate(GetEntityQuery(typeof(Cannonball)));
    }

    protected override void OnUpdate()
    {
        var config = this.GetSingleton<GameObjectRefs>().Config.Data;
        if (config.Invincible)
            return;
        
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var parallelWriter = ecb.AsParallelWriter();
        
        var player = GetSingletonEntity<Player>();
        var playerPos = GetComponent<Translation>(player);
        var spawnerArchetype = EntityManager.CreateArchetype(typeof(Spawner));

        Entities
            .WithName("player_cannonball_collision_test")
            .ForEach((
                int entityInQueryIndex,
                in Cannonball cannonball,
                in Translation translation) =>
            {
                // TODO: Ensure we can never spawn multiple spawners in case
                // multiple cannonballs hit the player at the same time
                //if (HasSingleton<Spawner>())
                //    return;

                if (Vector3.Distance(playerPos.Value, translation.Value) <= (Cannonball.RADIUS * 2))
                {
                    parallelWriter.CreateEntity(entityInQueryIndex, spawnerArchetype);
                }
            }).ScheduleParallel();
        
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
