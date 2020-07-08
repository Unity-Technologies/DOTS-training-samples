using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BombPlayerIntersectSystem : SystemBase {

    private EntityCommandBufferSystem cbs;

    protected override void OnCreate() {
        cbs = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {

        var ecb = cbs.CreateCommandBuffer().ToConcurrent();

        Entity playerEntity = GetSingletonEntity<PlayerTag>();
        Position pos = GetComponent<Position>(playerEntity);
        float3 playerPosition = pos.Value;

        Entities
          .WithNone<PlayerTag>()
          .WithAll<MovementParabola>()
          .ForEach((int entityInQueryIndex, in Position p) => {
              if (math.distance(p.Value, playerPosition) < 1) {
                  Entity e = ecb.CreateEntity(entityInQueryIndex);
                  ecb.AddComponent<GameOverTag>(entityInQueryIndex, e);
              }
          }).ScheduleParallel();

        cbs.AddJobHandleForProducer(Dependency);
    }
}