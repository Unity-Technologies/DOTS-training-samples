using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MouseInputSystem : SystemBase
{
    const float k_InputDeadZonePercentage = .1f;

    Camera m_Camera;
    EntityQuery m_PlayerQuery;

    float deadZone;

    protected override void OnCreate()
    {
        // To be used for having input relative to player
        m_PlayerQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<PlayerTag>(),
                ComponentType.ReadOnly<Direction>(),
                ComponentType.ReadOnly<Position>()
            }
        });

        RequireSingletonForUpdate<PlayerTag>();

        // use height as this is generally smaller than the width. We want the deadzone to be a circle.
        deadZone = Screen.height * k_InputDeadZonePercentage;

        m_Camera = Camera.main;
    }

    protected override void OnUpdate()
    {
        var playerEntity = GetSingletonEntity<PlayerTag>();
        var playerWorldPosition = EntityManager.GetComponentData<Position>(playerEntity);

        float2 inputDirection = ((float3)(Input.mousePosition - m_Camera.WorldToScreenPoint(playerWorldPosition.Value))).xy;
        float2 targetDir = new float2();

        if (math.lengthsq(inputDirection) > deadZone * deadZone)
        {
            if(math.abs(math.dot(inputDirection, new float2(1f, 0))) > deadZone)
                targetDir.y -= math.sign(inputDirection.x);
            if(math.abs(math.dot(inputDirection, new float2(0f, 1f))) > deadZone)
                targetDir.x += math.sign(inputDirection.y);
        }

        Entities.ForEach((ref Direction d) =>
        {
            d.Value = (int2)targetDir;
        }).Schedule();
    }
}