using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityColor = UnityEngine.Color;

[UpdateAfter(typeof(PlayerUpdateSystem))]
public partial class CursorMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var playersQuery = GetEntityQuery(
            ComponentType.ReadOnly<CursorPosition>(),
            ComponentType.ReadOnly<Color>(),
            ComponentType.ReadOnly<PlayerUIIndex>());
        if (playersQuery.IsEmpty)
            return;

        var playerIndices = playersQuery.ToComponentDataArray<PlayerUIIndex>(Allocator.Temp);
        var playerColors = playersQuery.ToComponentDataArray<Color>(Allocator.Temp);
        var playerPositions = playersQuery.ToComponentDataArray<CursorPosition>(Allocator.Temp);
        var cursors = this.GetSingleton<GameObjectRefs>().Cursors;

        for (int i = 0; i < playerIndices.Length; ++i)
        {
            var idx = playerIndices[i].Index;
            if (idx >= cursors.Length)
                continue;
            var cursor = cursors[idx];

            var color = playerColors[i].Value;
            cursor.color = new UnityEngine.Color(color.x, color.y, color.z, color.w);
            var pos = playerPositions[i].Value;
            cursor.transform.position = Camera.main.WorldToScreenPoint(new Vector3(pos.x+0.3f, 0, pos.y+1.15f));
        }

        playerIndices.Dispose();
        playerColors.Dispose();
        playerPositions.Dispose();
    }
}
