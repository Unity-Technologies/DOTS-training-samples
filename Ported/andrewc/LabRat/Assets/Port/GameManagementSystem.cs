using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using UnityEngine.SceneManagement;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class GameManagementSystem : ComponentSystem
{
    static readonly float k_NumSecondsPerGame = 30f;
    static readonly Color[] k_PlayerColors = { Color.black, Color.red, Color.green, Color.blue };
    static readonly double k_ReadyDisplayTimeInSeconds = 2f;
    static readonly double k_SetDisplayTimeInSeconds = 1f;
    static readonly double k_GoDisplayTimeInSeconds = 1f;
    static readonly double k_GameOverWaitTimeInSeconds = 4f;

    float m_numSecondsLeftInGame = k_NumSecondsPerGame;
    public float NumSecondsLeftInGame
    {
        get { return m_numSecondsLeftInGame; }
    }

    public int NumPlayers
    {
        get => 4;
    }

    public Color GetPlayerColor(int playerIndex)
    {
        return k_PlayerColors[playerIndex];
    }

    uint4 m_playerScore;
    public uint GetPlayerScore(int playerIndex)
    {
        return m_playerScore[playerIndex];
    }

    public enum GameState
    {
        DisplayReadyText,
        DisplaySetText,
        DisplayGoText,
        GamePlayInProgress,
        GameOver
    }

    GameState m_gameState = GameState.DisplayReadyText;
    public GameState TheGameState
    {
        get => m_gameState;
    }

    string m_introText = "Ready";
    public string IntroText
    {
        get => m_introText;
    }

    double m_lastTimestamp;

    protected override void OnCreate()
    {
        m_lastTimestamp = Time.ElapsedTime;
    }

    protected override void OnDestroy()
    {
    }

    void HandleTextChange(double timeToWait, string text, GameState nextState)
    {
        var timeSinceReady = Time.ElapsedTime - m_lastTimestamp;
        if (timeSinceReady > timeToWait)
        {
            m_lastTimestamp = Time.ElapsedTime;
            m_introText = text;
            m_gameState = nextState;
        }
    }

    void HandleReadyText()
    {
        HandleTextChange(k_ReadyDisplayTimeInSeconds, "Set", GameState.DisplaySetText);
    }

    void HandleSetText()
    {
        HandleTextChange(k_SetDisplayTimeInSeconds, "GO", GameState.DisplayGoText);
    }

    void HandleGoText()
    {
        HandleTextChange(k_GoDisplayTimeInSeconds, "", GameState.GamePlayInProgress);
    }

    void ResetGame()
    {
        m_lastTimestamp = Time.ElapsedTime;
        m_gameState = GameState.DisplayReadyText;
        m_introText = "Ready";

        m_numSecondsLeftInGame = k_NumSecondsPerGame;
    }

    void HandleGamePlay()
    {
        if (m_numSecondsLeftInGame > 0)
        {
            m_numSecondsLeftInGame -= Time.DeltaTime;
        }
        else
        {
            m_lastTimestamp = Time.ElapsedTime;
            m_gameState = GameState.GameOver;
        }
    }

    void HandleGameOver()
    {
        var timeSinceGameOver = Time.ElapsedTime - m_lastTimestamp;
        if (timeSinceGameOver > k_GameOverWaitTimeInSeconds)
        {
            ResetGame();
        }
    }

    protected override void OnUpdate()
    {
        switch (m_gameState)
        {
            case GameState.DisplayReadyText:
                {
                    HandleReadyText();
                    break;
                }
            case GameState.DisplaySetText:
                {
                    HandleSetText();
                    break;
                }
            case GameState.DisplayGoText:
                {
                    HandleGoText();
                    break;
                }
            case GameState.GamePlayInProgress:
                {
                    HandleGamePlay();
                    break;
                }
            case GameState.GameOver:
                {
                    HandleGameOver();
                    break;
                }
        }
    }
}
