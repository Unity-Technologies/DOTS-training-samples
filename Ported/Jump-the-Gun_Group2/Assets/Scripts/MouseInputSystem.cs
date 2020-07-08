using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MouseInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float2 mousePos2 = ((float3)UnityEngine.Input.mousePosition).xy;

        Entities.ForEach((ref Direction d) => {
            d.Value = mousePos2;
        }).Schedule();
    }
}