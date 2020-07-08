using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerNextMoveSystem : SystemBase
{
    EntityQuery m_PlayerQuery;
    EntityQuery m_BufferQuery;

    protected override void OnCreate()
    {
        m_PlayerQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<PlayerTag>(),
                ComponentType.ReadOnly<Direction>()
            }
        });

        RequireSingletonForUpdate<GridTag>();
        RequireSingletonForUpdate<GameParams>();

        m_BufferQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new []
            {
                ComponentType.ReadOnly<GridHeight>()
            }
        });
    }

    protected override void OnUpdate()
    {
        // Assume it is scaled later
        float2 cubeSize = math.float2(1, 1);

        var gridtag = GetSingletonEntity<GridTag>();

        DynamicBuffer<GridHeight> gh = EntityManager.GetBuffer<GridHeight>(gridtag);

        GameParams gp =  GetSingleton<GameParams>();

        Entities.
        WithAll<PlayerTag>().
        WithReadOnly(gh).
        ForEach((ref MovementParabola movement, ref NormalisedMoveTime normalisedMoveTime, in Direction direction, in Position pos) =>
        {
            if(normalisedMoveTime.Value >= 1.0f)
            {
                movement.Origin = pos.Value;

                float3 newPos = new float3(math.clamp((direction.Value.x + pos.Value.x), 0, gp.TerrainDimensions.x), 0, math.clamp((direction.Value.y + pos.Value.z), 0, gp.TerrainDimensions.y));
                newPos.y = gh[(int)newPos.x + (int)newPos.y * gp.TerrainDimensions.x].Height + 1;
                movement.Target = newPos;
                normalisedMoveTime.Value = 0.0f;
            }
        }).ScheduleParallel();

    }
}