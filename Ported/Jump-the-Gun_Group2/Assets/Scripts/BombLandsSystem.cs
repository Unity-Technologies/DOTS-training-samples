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
                ComponentType.ReadOnly<Position>(),
                ComponentType.ReadOnly<NormalisedMoveTime>()
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

        GameParams gameParams = GetSingleton<GameParams>();

        Entities
           .WithDeallocateOnJobCompletion(bombPosition)
           .WithDeallocateOnJobCompletion(bombTime)
           //for all the tiles
           .ForEach((ref Height height, ref Color c, in Position position) => {
               //for all the bombs
               for (int i = 0; i < bombPosition.Length; ++i) {
                   if (bombTime[i].Value > 1 && math.distance(bombPosition[i].Value, position.Value) < 0.1f) {
                       
                       height.Value = math.max(gameParams.TerrainMin, height.Value - 0.001f);

                       float range = (gameParams.TerrainMax - gameParams.TerrainMin);
                       float value = (height.Value - gameParams.TerrainMin) / range;
                       float4 color = math.lerp(gameParams.colorA, gameParams.colorB, value);
                       c.Value = color;
                   }
               }

           }).ScheduleParallel();

        Entities
           .WithNone<PlayerTag>()
           .ForEach((int entityInQueryIndex, Entity e, in NormalisedMoveTime n, in MovementParabola p) => {

               //if the bomb has finished moving
               if(n.Value > 1) {
                   //destroy the bomb
                   ecb.DestroyEntity(entityInQueryIndex, e);
               }

           }).ScheduleParallel();

        cbs.AddJobHandleForProducer(Dependency);
    }
}