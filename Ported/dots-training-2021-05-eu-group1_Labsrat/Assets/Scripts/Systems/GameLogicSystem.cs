using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

[AlwaysUpdateSystem]
public class GameLogicSystem : SystemBase
{
    private float timer;
    private bool start = true;
    
    private int winner = 0;
    private int winnerScore = 0;

    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;

        if (start && TryGetSingleton(out GameConfig gameConfig))
        {
            timer = gameConfig.RoundDuration;
            UIDisplay.PlayerScores = new NativeArray<int>(gameConfig.NumOfAIPlayers + 1, Allocator.Persistent);
            UIDisplay.PlayerColors = new NativeArray<float4>(gameConfig.NumOfAIPlayers + 1, Allocator.Persistent);
            
            start = false;
        }
        
        NativeArray<int> playerScores = UIDisplay.PlayerScores;
        NativeArray<float4> playerColors = UIDisplay.PlayerColors;

        Entities
            .WithoutBurst()
            .ForEach((in Score score, in PlayerIndex playerIndex, in PlayerColor playerColor) =>
        {
            // TODO: Change scoreValue to score.Value
            var scoreValue = playerIndex.Index;
            
            playerScores[playerIndex.Index] = scoreValue;
            playerColors[playerIndex.Index] = playerColor.Color;
            
            if (scoreValue > winnerScore)
            {
                winner = playerIndex.Index;
                winnerScore = scoreValue;
            }
            
        }).Run();

        if (timer <= 0)
        {
            UIDisplay.EndGame = true;
            UIDisplay.Winner = winner;
        }

        else
        {
            timer -= time;
            UIDisplay.TimeLeft = math.ceil(timer);
        }
    }
}
