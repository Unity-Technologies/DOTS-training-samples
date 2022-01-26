using Unity.Collections;
using Unity.Entities;
using UnityColor = UnityEngine.Color;

[UpdateAfter(typeof(PlayerUpdateSystem))]
public partial class PlayerScoreUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var playersQuery = GetEntityQuery(
            ComponentType.ReadOnly<Player>(),
            ComponentType.ReadOnly<Color>(),
            ComponentType.ReadOnly<Score>(),
            ComponentType.ReadOnly<PlayerScoreDisplayIndex>());
        if (playersQuery.IsEmpty)
            return;

        var playerColors = playersQuery.ToComponentDataArray<Color>(Allocator.Temp);
        var playerScores = playersQuery.ToComponentDataArray<Score>(Allocator.Temp);
        var playerIndices = playersQuery.ToComponentDataArray<PlayerScoreDisplayIndex>(Allocator.Temp);
        var scoreDisplays = this.GetSingleton<GameObjectRefs>().ScoreDisplays;

        for (int i = 0; i < playerIndices.Length; ++i)
        {
            var idx = playerIndices[i].Index;
            if (idx >= scoreDisplays.Length)
                continue;
            var display = scoreDisplays[idx];
            display.text = playerScores[i].Value.ToString();
            var color = playerColors[i].Value;
            display.color = new UnityColor(color.x, color.y, color.z, color.w);
        }

        playerColors.Dispose();
        playerScores.Dispose();
        playerIndices.Dispose();
    }
}
