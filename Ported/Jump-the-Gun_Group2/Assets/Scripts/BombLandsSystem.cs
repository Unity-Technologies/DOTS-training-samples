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

        var bombTime = bombQuery.ToComponentDataArrayAsync<NormalisedMoveTime>(Allocator.TempJob, out var bombTimeHandle);
        var bombPara = bombQuery.ToComponentDataArrayAsync<MovementParabola>(Allocator.TempJob, out var bombParaHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, bombTimeHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, bombParaHandle);

        GameParams gameParams = GetSingleton<GameParams>();

        Entities
           .WithDeallocateOnJobCompletion(bombTime)
           .WithDeallocateOnJobCompletion(bombPara)
           //for all the tiles
           .ForEach((ref Height height, ref Color c, ref Position pos) => {
               //for all the bombs
               for (int i = 0; i < bombTime.Length; ++i) {
                   if (bombTime[i].Value > 1 && math.distance(bombPara[i].Target.xz, pos.Value.xz) < 0.5f) {
                       
                       height.Value = math.max(gameParams.TerrainMin, height.Value - 0.3f);

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