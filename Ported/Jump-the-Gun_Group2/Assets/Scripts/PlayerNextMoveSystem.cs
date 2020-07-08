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
    }

    protected override void OnUpdate()
    {
        // Assume it is scaled later
        float2 cubeSize = math.float2(1, 1);

        Entities.ForEach((ref MovementParabola movement, ref NormalisedMoveTime normalisedMoveTime, in PlayerTag player, in Direction direction, in Position pos) =>
        {
            if(normalisedMoveTime.Value >= 1.0f)
            {
                movement.Origin = pos.Value;
                movement.Target = pos.Value + math.float3(cubeSize * direction.Value, 0);
                movement.Target = pos.Value + math.float3(cubeSize.x * direction.Value.x, 0, cubeSize.y * direction.Value.y);
                normalisedMoveTime.Value = 0.0f;
            }
        }).ScheduleParallel();

    }
}