using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MouseInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float2 mousePos = ((float3)UnityEngine.Input.mousePosition).xy;

        mousePos.x /= Screen.width;
        mousePos.y /= Screen.height;

        mousePos -= 0.5f;
        mousePos *= 2;

        Entities.ForEach((ref Direction d) => {
            d.Value = mousePos;
        }).Schedule();
    }
}