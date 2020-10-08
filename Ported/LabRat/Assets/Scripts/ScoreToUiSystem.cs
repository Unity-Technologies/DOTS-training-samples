using Unity.Entities;
using UnityEngine;

public class ScoreToUiSystem : SystemBase
{
    // Start is called before the first frame update
    protected override void OnUpdate()
    {
        Entities
            .WithoutBurst()
            .ForEach((in HomeBase homebase) =>
            {
                PlayerScores.staticPlayerScores[homebase.playerIndex].text = homebase.playerScore.ToString();
            }).Run();
    }
}
