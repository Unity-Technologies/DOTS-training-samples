using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MouseInputSystem : SystemBase
{
    EntityQuery m_PlayerQuery;

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
    }

    protected override void OnUpdate()
    {
        float2 mousePos = ((float3)UnityEngine.Input.mousePosition).xy;

        mousePos.x /= Screen.width;
        mousePos.y /= Screen.height;

        mousePos -= 0.5f;
        mousePos *= 2;
        // 10 pixel away from center indicates a direction shift.
        float2 threshold =  10.0f * math.rcp(math.float2(Screen.width, Screen.height));

        Entities.ForEach((ref Direction d) => 
        {
            if (mousePos.x > threshold.x) d.Value.x = 1.0f;
            if (mousePos.x < threshold.x) d.Value.x = -1.0f;
            if (mousePos.y > threshold.y) d.Value.y = 1.0f;
            if (mousePos.y < threshold.y) d.Value.y = -1.0f;

        }).Schedule();
    }
}