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

[UpdateInGroup(typeof(ChuChuRocketUpdateGroup))]
[AlwaysUpdateSystem]
public class GameLogicSystem : SystemBase
{
    private float timer;
    private bool start = true;
    
    private int winner = 0;
    private int winnerScore = 0;

    protected override void OnCreate()
    {
        base.OnCreate();
        start = true;
        timer = float.PositiveInfinity;
    }

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
            playerScores[playerIndex.Index] = score.Value;
            playerColors[playerIndex.Index] = playerColor.Color;
            
            if (score.Value > winnerScore)
            {
                winner = playerIndex.Index;
                winnerScore = score.Value;
            }
            
        }).Run();

        if (timer <= 0)
        {
            UIDisplay.EndGame = true;
            UIDisplay.Winner = winner;
            World.GetExistingSystem<MovementSystem>().Enabled = false;
            World.GetExistingSystem<InputSystem>().Enabled = false;
            World.GetExistingSystem<AnimalSpawnerSystem>().Enabled = false;
        }
        else
        {
            timer -= time;
            UIDisplay.TimeLeft = math.ceil(timer);
        }
    }

    protected override void OnDestroy()
    {
        UIDisplay.PlayerScores.Dispose();
        UIDisplay.PlayerColors.Dispose();
    }
}
