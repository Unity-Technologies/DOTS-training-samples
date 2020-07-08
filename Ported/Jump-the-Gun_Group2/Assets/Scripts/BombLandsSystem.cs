using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

public class BombLandsSystem : SystemBase
{

    private EntityCommandBufferSystem cbs;

    private EntityQuery bombQuery;

    protected override void OnCreate() {

        bombQuery = GetEntityQuery(new EntityQueryDesc {
            All = new[]
            {
                ComponentType.ReadOnly<MovementParabola>(),
            },
            None = new[]
            {
                ComponentType.ReadOnly<PlayerTag>(),
            }
        });

        cbs = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = cbs.CreateCommandBuffer().ToConcurrent();

        var bombPosition = bombQuery.ToComponentDataArrayAsync<Position>(Allocator.TempJob, out var bombPositionHandle);
        var bombTime = bombQuery.ToComponentDataArrayAsync<NormalisedMoveTime>(Allocator.TempJob, out var bombTimeHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, bombPositionHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, bombTimeHandle);

        Entities
           .WithDeallocateOnJobCompletion(bombPosition)
           .WithDeallocateOnJobCompletion(bombTime)
           //for all the tiles
           .ForEach((ref Height height, in Position position, in Color c) => {
               //for all the bombs
               for (int i = 0; i < bombPosition.Length; ++i) {
                   if (bombTime[i].Value > 1 && math.distance(bombPosition[i].Value, position.Value) < 0.01f) {
                       height.Value -= 0.3f;
                   }
               }

           }).ScheduleParallel();

        Entities
           .WithNone<PlayerTag>()
           .ForEach((int entityInQueryIndex, Entity e, ref NormalisedMoveTime n, in MovementParabola p) => {

               //if the bomb has finished moving
               if(n.Value > 1) {
                   //destroy the bomb
                   ecb.DestroyEntity(entityInQueryIndex, e);
               }

           }).ScheduleParallel();

        cbs.AddJobHandleForProducer(Dependency);
    }
}