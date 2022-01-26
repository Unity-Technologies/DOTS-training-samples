using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateBefore(typeof(ArrowPlacerSystem))]
public partial class PlayerInputSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameRunning>();
    }

    protected override void OnUpdate()
    {
        var player = GetSingletonEntity<PlayerInputTag>();
        var config = GetSingleton<Config>();
        
        var mousePos = Input.mousePosition;
        SetComponent(player, new CursorPosition { Value = RaycastMapPos(Camera.main, mousePos) });
        
        if (Input.GetMouseButtonUp(0))
        {
            SetComponent(player, new PlayerSpawnArrow { Value = true });
        }
    }

    private static float2 RaycastMapPos(Camera camera, Vector2 screenPos)
    {
        var ray = camera.ScreenPointToRay(screenPos);

        float enter;

        var plane = new Plane(Vector3.up, new Vector3(0, 0, 0));

        if (!plane.Raycast(ray, out enter))
            return new int2(-1, -1);

        var worldPos = ray.GetPoint(enter);
        return new float2(worldPos.x, worldPos.z);
    }
}
