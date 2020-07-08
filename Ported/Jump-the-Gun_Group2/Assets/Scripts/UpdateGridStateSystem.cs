using Unity.Entities;
using Unity.Mathematics;

public class UpdateGridStateSystem : SystemBase
{
    EntityQuery m_Query;

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

        RequireSingletonForUpdate<GameParams>();
        RequireSingletonForUpdate<GridTag>();
    }

    protected override void OnUpdate()
    {
        var e = GetSingletonEntity<GridTag>();
        var gameParams = GetSingleton<GameParams>();
        var buffer = EntityManager.GetBuffer<GridHeight>(e);

        Entities.ForEach((in Height height, in Position position) =>
        {
            var pos = (int2)position.Value.xz;
            buffer[pos.y * gameParams.TerrainDimensions.x + pos.x] = new GridHeight { Height = height.Value };
        }).Schedule();
    }
}