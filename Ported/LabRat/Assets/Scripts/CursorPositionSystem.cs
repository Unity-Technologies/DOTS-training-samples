using System;
using Unity.Entities;
using UnityEngine;

public class CursorPositionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var camera = UnityEngine.Camera.main;
        if (camera == null)
            return;

        var canvas = PlayersCursors.canvas;
        if (canvas == null)
            return;

        Entities.WithoutBurst().ForEach((in PlayerTransform p, in Position position) =>
        {
            var point = new Vector3(position.Value.x, 0f, position.Value.y);
            var screenPosition = camera.WorldToScreenPoint(point);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, screenPosition, canvas.worldCamera, out var pos);
            PlayersCursors.players[p.Index].position = PlayersCursors.canvas.transform.TransformPoint(pos);
        }).Run();
    }
}