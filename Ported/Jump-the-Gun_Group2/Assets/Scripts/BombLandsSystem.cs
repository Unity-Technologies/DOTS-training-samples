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

        RequireSingletonForUpdate<GridTag>();
        RequireSingletonForUpdate<GameParams>();
        cbs = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = cbs.CreateCommandBuffer().ToConcurrent();

        GameParams gameParams = GetSingleton<GameParams>();
        var gridEntity = GetSingletonEntity<GridTag>();
        var heightsBuffer = EntityManager.GetBuffer<GridHeight>(gridEntity).AsNativeArray();

        Entities
            .WithNativeDisableContainerSafetyRestriction(heightsBuffer)
           //for all the bombs

           .WithNone<PlayerTag>()
           .ForEach((int entityInQueryIndex, Entity bomb, in NormalisedMoveTime time, in MovementParabola parabola) =>
           {
               if (time.Value >= 1.0f)
               {
                   // We reached destination. 
                   var gridIndex = GridFunctions.GetGridIndex(parabola.Target.xz, gameParams.TerrainDimensions);
                   var oldHeight = heightsBuffer[gridIndex];
                   var newHeight = new GridHeight { Height = math.max(oldHeight.Height - 0.3f, gameParams.TerrainMin) };
                   heightsBuffer[gridIndex] = newHeight;
                   ecb.DestroyEntity(entityInQueryIndex, bomb);

               }
           }).ScheduleParallel();

        cbs.AddJobHandleForProducer(Dependency);
    }
}