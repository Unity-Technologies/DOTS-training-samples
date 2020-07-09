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

        RequireSingletonForUpdate<PlayerTag>();
    }

    protected override void OnUpdate()
    {        
        float threshold = 0.5f;

        // For now we assume only one player. Need to convert this to a global bound center encompassing all players
        // if we move to that.
        var playerEntity = GetSingletonEntity<PlayerTag>();
        var playerLocation = EntityManager.GetComponentData<Position>(playerEntity);

        var gp = GetSingleton<GameParams>();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float t = ((gp.TerrainMax + gp.TerrainMin) * 0.5f - ray.origin.y) / ray.direction.y;
        float3 mouseWorldPos = math.float3(0, 1, 0);
        float3 pointOnRay = ray.GetPoint(t);
        mouseWorldPos.x = pointOnRay.x;
        mouseWorldPos.z = pointOnRay.z;
        var mouseGridSpace = mouseWorldPos;

        float2 playerToMouse = (mouseGridSpace.xz - playerLocation.Value.xz);
        float2 targetDir = math.float2(0.0f, 0.0f);

        if ((math.abs(playerToMouse.x) > threshold) || (math.abs(playerToMouse.y) > threshold))
        {
            targetDir.x += math.sign(playerToMouse.x);
            targetDir.y += math.sign(playerToMouse.y);
        }

        Entities.ForEach((ref Direction d) => 
        {
            d.Value = (int2)targetDir;
        }).Schedule();
    }
}