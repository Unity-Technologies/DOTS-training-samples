using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(PlayerUpdateSystem))]
public partial class CursorMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var cursors = this.GetSingleton<GameObjectRefs>().Cursors;
        var camera = this.GetSingleton<GameObjectRefs>().Camera;

        Entities
            .WithoutBurst()
            .ForEach((ref PlayerUIIndex playerIndex, ref Color playerColor, ref CursorPosition playerPosition) =>
            {
                var idx = playerIndex.Index;
                if (idx >= cursors.Length)
                    return;
                var cursor = cursors[idx];
                var color = playerColor.Value;
                cursor.color = new UnityEngine.Color(color.x, color.y, color.z, color.w);
                var pos = playerPosition.Value;
                cursor.transform.position = camera.WorldToScreenPoint(new Vector3(pos.x+0.3f, 0, pos.y+1.15f));
            }).Run();
    }
}
