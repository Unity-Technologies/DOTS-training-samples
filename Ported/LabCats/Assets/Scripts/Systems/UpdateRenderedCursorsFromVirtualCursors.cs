
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class UpdateRenderedCursorsFromVirtualCursors : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<GameObjectRefs>();
    }

    protected override void OnUpdate()
    {
        var gameObjectRefs = this.GetSingleton<GameObjectRefs>();
        var camera = gameObjectRefs.Camera;
        GameObject cursor;
        
        Entities.WithoutBurst().WithAll<Score, AITargetCell>().ForEach((in PlayerIndex playerIndex, in Translation translation) =>
        {
            if (playerIndex.Value == 0)
            {
                cursor = gameObjectRefs.Player1Cursor;
            }
            else if (playerIndex.Value == 1)
            {
                cursor = gameObjectRefs.Player2Cursor;
            }
            else if (playerIndex.Value == 2)
            {
                cursor = gameObjectRefs.Player3Cursor;
            }
            else
            {
                cursor = gameObjectRefs.Player4Cursor;
            }

            var vec = new Vector3(translation.Value.x, translation.Value.y, translation.Value.z);
            var targetPosition = camera.WorldToScreenPoint(vec);
            // gameObjectRefs.Player1Cursor.
            var transform = cursor.GetComponent<RectTransform>();
            transform.position = targetPosition;
        }).Run();
    }
}
