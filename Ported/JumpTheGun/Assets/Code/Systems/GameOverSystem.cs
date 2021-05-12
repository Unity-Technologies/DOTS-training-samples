using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
public class GameOverSystem : SystemBase
{
    private EntityCommandBufferSystem ecbs_level;
    private EntityCommandBufferSystem ecbs_board;

    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(Player), typeof(WasHit));
        RequireForUpdate(query);

        ecbs_level = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        ecbs_board = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        /*Entity player = GetSingletonEntity<Player>();
        WasHit hit = GetComponent<WasHit>(player);

        if (hit.Count <= 0)
            return;

        Entities
            .WithAll<Player>()
            .ForEach((ref WasHit hit) =>
            {
                hit.Count = 0;
            }).Run();
        
        var ecbp = ecbs_level.CreateCommandBuffer();

        EntityCommandBuffer.ParallelWriter ecbl = ecbs_level.CreateCommandBuffer().AsParallelWriter();
        EntityCommandBuffer.ParallelWriter ecbb = ecbs_board.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithAll<CurrentLevel>()
            .ForEach((int entityInQueryIndex, Entity entity) =>
            {
                ecbl.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();
        
        Entities
            .WithAll<Board>()
            .ForEach((int entityInQueryIndex, Entity entity) =>
            {
                ecbb.AddComponent<BoardSpawnerTag>(entityInQueryIndex, entity);
            }).ScheduleParallel();
        
        ecbs_level.AddJobHandleForProducer(Dependency);
        ecbs_board.AddJobHandleForProducer(Dependency);*/
    }
}
