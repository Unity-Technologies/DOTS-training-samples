using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class PlayerInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var player = GetSingletonEntity<PlayerInputTag>();
        var mousePos = Input.mousePosition;
        SetComponent(player, new Position { Value = new float2(mousePos.x, mousePos.y) });
        if (Input.GetMouseButtonDown(0))
        {
            SetComponent(player, new PlayerSpawnArrow { Value = true });
        }
    }
}
