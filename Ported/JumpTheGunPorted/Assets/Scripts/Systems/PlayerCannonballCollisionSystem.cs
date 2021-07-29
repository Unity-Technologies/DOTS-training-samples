using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerCannonballCollisionSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate(GetEntityQuery(typeof(Player)));
        RequireForUpdate(GetEntityQuery(typeof(Cannonball)));
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var parallelWriter = ecb.AsParallelWriter();
        
        var player = GetSingletonEntity<Player>();
        var playerPos = GetComponent<Translation>(player);
        var spawnerArchetype = EntityManager.CreateArchetype(typeof(Spawner));

        var refs = this.GetSingleton<GameObjectRefs>();
        var config = refs.Config.Data;
        var invincible = config.Invincible;

        Entities
            .WithName("player_cannonball_collision_test")
            .ForEach((
                in Cannonball cannonball,
                in Translation translation) =>
            {
                if (!invincible)
                {
                    // TODO: Ensure we can never spawn multiple spawners in case
                    // multiple cannonballs hit the player at the same time
                    //if (HasSingleton<Spawner>())
                    //    return;

                    if (Vector3.Distance(playerPos.Value, translation.Value) <= (Cannonball.RADIUS * 2))
                    {
                        parallelWriter.CreateEntity(1, spawnerArchetype);
                    }
                }
            }).ScheduleParallel();
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}