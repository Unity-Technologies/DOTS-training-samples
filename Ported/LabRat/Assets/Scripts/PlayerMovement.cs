using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(CursorPositionSystem))]
public class PlayerMovement : SystemBase
{
    protected override void OnUpdate()
    {
        var camera = UnityEngine.Camera.main;
        if (camera == null)
            return;
        
        var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        new UnityEngine.Plane(UnityEngine.Vector3.up, 0).Raycast(ray, out var enter);
        var hit = (float3)ray.GetPoint(enter);
        
        Entities.WithAll<Player>().ForEach((ref Position position) =>
        {
            position.Value = new float2(hit.x, hit.z);
        }).ScheduleParallel();
    }
}
