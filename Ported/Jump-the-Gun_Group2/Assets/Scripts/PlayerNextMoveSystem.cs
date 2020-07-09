using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerNextMoveSystem : SystemBase
{
    public const float kYOffset = .3f;
    public const float kBounceHeight = 2;
    public const float kPlayerSpeed = 3.0f;

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

    public static void InitPlayerPosition(ref MovementParabola movement, float3 pos, DynamicBuffer<GridHeight> gh, GameParams gp)
    {
        int2 originTile = new int2((int2)pos.xz);
        movement.Origin = new float3(originTile.x + 0.5f, gh[GridFunctions.GetGridIndex(originTile, gp.TerrainDimensions)].Height + PlayerNextMoveSystem.kYOffset, originTile.y + 0.5f);

        movement.Target = movement.Origin;

        movement.Parabola.x = 0.0f;
        movement.Parabola.y = kBounceHeight;
        movement.Parabola.z = 0.0f;

        movement.Speed = kPlayerSpeed;
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
            if (normalisedMoveTime.Value >= 1.0f) 
            {
                InitPlayerPosition(ref movement, pos.Value, gh, gp);

                int2 targetTile = new int2((int2)pos.Value.xz);
                targetTile = math.clamp(targetTile + direction.Value, 0, gp.TerrainDimensions - 1);

                // Don't allow to move to target if it is occupied. Keep current position (setup in InitPlayerPosition)
                if (!go[GridFunctions.GetGridIndex(targetTile, gp.TerrainDimensions)].Occupied)
                {
                    movement.Target = new float3(targetTile.x + 0.5f, gh[GridFunctions.GetGridIndex(targetTile, gp.TerrainDimensions)].Height + PlayerNextMoveSystem.kYOffset, targetTile.y + 0.5f);
                }

                if (math.all(movement.Origin == movement.Target))
                {
                    movement.Parabola.x = 0.0f;
                    movement.Parabola.y = movement.Origin.y + kBounceHeight;
                    movement.Parabola.z = 0.0f;
                }
                else
                {
                    // Solving parabola path
                    float height = Mathf.Max(movement.Origin.y, movement.Target.y);
                    // TODO: from original game
                    // make height max of adjacent boxes when moving diagonally
                 //   if (startBox.col != endBox.col && startBox.row != endBox.row)
                  //  {
                   //     height = Mathf.Max(height, TerrainArea.instance.GetBox(startBox.col, endBox.row).top, TerrainArea.instance.GetBox(endBox.col, startBox.row).top);
                //    }
                    height += kBounceHeight;

                    ParabolaMath.Create(movement.Origin.y, height, movement.Target.y, out movement.Parabola.x, out movement.Parabola.y, out movement.Parabola.z);
                }

                normalisedMoveTime.Value = 0.0f;
            }

        }).ScheduleParallel();

    }
}