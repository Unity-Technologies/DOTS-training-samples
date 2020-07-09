using Unity.Entities;
using Unity.Mathematics;

public class UpdateGridStateSystem : SystemBase
{
    EntityQuery m_Query;
    EntityQuery m_BufferQuery;

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Height>()
            }
        });
        m_Query.AddChangedVersionFilter(new ComponentType(typeof(Height)));

        m_BufferQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadWrite<GridHeight>()
            }
        });

        RequireSingletonForUpdate<GameParams>();
        RequireSingletonForUpdate<GridTag>();
    }

    protected override void OnUpdate()
    {
        var e = GetSingletonEntity<GridTag>();
        var gameParams = GetSingleton<GameParams>();
        var buffer = EntityManager.GetBuffer<GridHeight>(e).AsNativeArray();

        Entities
            .WithNativeDisableContainerSafetyRestriction(buffer)
            .WithNativeDisableParallelForRestriction(buffer)
            .ForEach((in Height height, in Position position) =>
        {
            var pos = (int2)position.Value.xz;
            buffer[GridFunctions.GetGridIndex(pos, gameParams.TerrainDimensions)] = new GridHeight { Height = height.Value };
        }).ScheduleParallel();
    }
}