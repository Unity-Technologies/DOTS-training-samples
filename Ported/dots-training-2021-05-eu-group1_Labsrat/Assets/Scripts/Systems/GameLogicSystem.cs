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


public struct GameLogic : IJobEntity
{
    [Singleton] Time time;
    [Singleton] GameConfig gameConfig;
    [Singleton] GameState gameState;
    
    float timer;
    int winner = 0;
    int winnerScore = 0;
    NativeArray<int> playerScores;
    NativeArray<float4> playerColors;

    // executed only the first time the job runs; 
    // Maybe this should be a singleton? 
    // But what about cases where we have separate job instances which each need their own set of arrays?
    // It's like we want a singleton private to the job instance?
    public void Init()
    {
        timer = gameConfig.RoundDuration;
        playerScores = UIDisplay.PlayerScores = new NativeArray<int>(gameConfig.NumOfAIPlayers + 1, Allocator.Persistent);
        playerColors = UIDisplay.PlayerColors = new NativeArray<float4>(gameConfig.NumOfAIPlayers + 1, Allocator.Persistent);
    }

    public void Execute(in Score score, in PlayerIndex playerIndex, in PlayerColor playerColor)
    {
        playerScores[playerIndex.Index] = score.Value;
        playerColors[playerIndex.Index] = playerColor.Color;
            
        if (score.Value > winnerScore)
        {
            winner = playerIndex.Index;
            winnerScore = score.Value;
        }
    }

    // run every time after Execute
    public void Post()
    {
        if (timer <= 0)
        {
            UIDisplay.EndGame = true;
            UIDisplay.Winner = winner;
            gameState = GameState.GameOver;
        }
        else
        {
            timer -= time.dt;
            UIDisplay.TimeLeft = math.ceil(timer);
        }
    }

    public void Teardown()
    {
        playerScores.Dispose();
        playerColors.Dispose();
    }
}