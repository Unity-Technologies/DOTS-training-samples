using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerNextMoveSystem : SystemBase
{
    EntityQuery m_PlayerQuery;

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

                int xp = math.clamp((int)(direction.Value.x + movement.Origin.x),0,gp.TerrainDimensions.x);
                int yp = math.clamp((int)(direction.Value.y + movement.Origin.y),0,gp.TerrainDimensions.y);
                float height = gh[xp + yp * gp.TerrainDimensions.y].Height + 1;
                float3 newPos = new float3(xp, height, yp);

                movement.Target = newPos;
                normalisedMoveTime.Value = 0.0f;
            }
        }).ScheduleParallel();

    }
}