using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateBefore(typeof(CursorToUISystem))]
public class PlayerMovement : SystemBase
{
    protected override void OnUpdate()
    {
        var camera = UnityEngine.Camera.main;
        if (camera == null)
            return;
        
        var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        new UnityEngine.Plane(UnityEngine.Vector3.up, -0.5f).Raycast(ray, out var enter);
        var hit = (float3)ray.GetPoint(enter);
        
        Entities.WithAll<PlayerCursor>().ForEach((ref Position position) =>
        {
            position.Value = new float2(hit.x, hit.z);
        }).ScheduleParallel();
    }
}