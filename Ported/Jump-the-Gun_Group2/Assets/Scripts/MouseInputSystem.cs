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

    // ibufferelementdata  to define an element and then dynamic buffer --> getdynamicbuffer
    // fixedlist ? 
    // class component -> worse. NativeArray 

    protected override void OnUpdate()
    {        
        float2 threshold = 0.03f * math.float2(Screen.width, Screen.height);

        // For now we assume only one player. Need to convert this to a global bound center encompassing all players
        // if we move to that.
        var playerEntity = GetSingletonEntity<PlayerTag>();
        var playerLocation = EntityManager.GetComponentData<Position>(playerEntity);
        var playerScreenPos =  Camera.main.WorldToScreenPoint(playerLocation.Value);
        var mouseScreenPos = ((float3)UnityEngine.Input.mousePosition).xy;

        float2 playerToMouse = ((float3)UnityEngine.Input.mousePosition).xy - math.float2(playerScreenPos.x, playerScreenPos.y);
        float2 targetDir = math.float2(0.0f, 0.0f);

        if (math.abs(playerToMouse.x) > threshold.x)
        {
            targetDir.x += math.sign(playerToMouse.x);
        }
        if (math.abs(playerToMouse.y) > threshold.y)
        {
            targetDir.y += math.sign(playerToMouse.y);
        }

        Entities.ForEach((ref Direction d) => 
        {
            d.Value = targetDir;
        }).Schedule();
    }
}