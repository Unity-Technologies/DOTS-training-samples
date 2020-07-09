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
        DynamicBuffer<GridOccupied> go = EntityManager.GetBuffer<GridOccupied>(gridtag);

        GameParams gp =  GetSingleton<GameParams>();

        Entities.
        WithAll<PlayerTag>().
        WithReadOnly(gh).
        WithReadOnly(go).
        ForEach((ref MovementParabola movement, ref NormalisedMoveTime normalisedMoveTime, in Direction direction, in Position pos) =>
        {
            if(normalisedMoveTime.Value >= 1.0f) 
            {
                float3 origin = (int3)(pos.Value+new float3(.5f));
                origin.y = gh[GridFunctions.GetGridIndex(origin.xz, gp.TerrainDimensions)].Height;
                movement.Origin = origin;

                float3 target = origin;
                target.x = math.clamp(target.x + direction.Value.x, 0f, gp.TerrainDimensions.x - 1);
                target.z = math.clamp(target.z + direction.Value.y, 0f, gp.TerrainDimensions.y - 1);
                target.y = gh[GridFunctions.GetGridIndex(target.xz, gp.TerrainDimensions)].Height;
                movement.Target = target;

                if(go[GridFunctions.GetGridIndex(target.xz, gp.TerrainDimensions)].Occupied)
                {
                    movement.Target = origin;
                    normalisedMoveTime.Value = 1.1f; // Allow querying another direction.
                }
                else
                {
                    normalisedMoveTime.Value = 0.0f;
                }
            }
        }).ScheduleParallel();

    }
}