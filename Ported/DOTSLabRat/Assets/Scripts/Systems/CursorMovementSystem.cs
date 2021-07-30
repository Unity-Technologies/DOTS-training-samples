using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

namespace DOTSRATS
{
    public class CursorMovementSystem : SystemBase
    {
        NativeArray<float3> velocities;

        protected override void OnCreate()
        {
            velocities = new NativeArray<float3>(3, Allocator.Persistent);
        }

        protected override void OnUpdate()
        {
            var goRefs = this.GetSingleton<GameObjectRefs>();
            var uiCanvas = goRefs.uiCanvas;
            var camera = goRefs.camera;
            
            var canvasRect = uiCanvas.transform as RectTransform;
            var timeDelta = Time.DeltaTime;

            Entities
                .WithoutBurst()
                .WithAll<InPlay, AIPlayer>()
                .ForEach((in Player player) =>
                {
                    var playerCursor = goRefs.playerCursors[player.playerNumber];
                    var cellWorldPos = new Vector3(player.arrowToPlace.x, 0f, player.arrowToPlace.y);
                    var cellScreenPos = camera.WorldToScreenPoint(cellWorldPos);

                    var currentCursorPos = playerCursor.rectTransform.anchoredPosition;
                    var targetCursorPos = Utils.WorldToCanvas(canvasRect, cellWorldPos, camera);

                    var cursorVelocity = (Vector3)velocities[player.playerNumber - 1];
                    currentCursorPos = Vector3.SmoothDamp(currentCursorPos, targetCursorPos, ref cursorVelocity, 0.5f, 1000f, timeDelta);
                    velocities[player.playerNumber - 1] = cursorVelocity;

                    playerCursor.rectTransform.anchoredPosition3D = currentCursorPos;
                }).Run();
        }

        protected override void OnDestroy()
        {
            velocities.Dispose();
        }
    }
}
