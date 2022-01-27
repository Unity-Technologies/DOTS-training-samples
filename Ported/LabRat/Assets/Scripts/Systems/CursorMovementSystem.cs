using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(PlayerUpdateSystem))]
public partial class CursorMovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<Config>();
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<Config>();
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
                float2 offset = new float2(0.3f, 1.15f);
                offset *= math.max(config.MapWidth, config.MapHeight) / 30.0f;
                cursor.transform.position = camera.WorldToScreenPoint(new Vector3(pos.x+offset.x, 0, pos.y+offset.y));
            }).Run();
    }
}
