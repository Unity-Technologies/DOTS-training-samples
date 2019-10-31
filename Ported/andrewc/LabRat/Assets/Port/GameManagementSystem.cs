using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class GameManagementSystem : ComponentSystem
{
    static readonly float k_NumSecondsPerGame = 30f;
    static readonly Color[] k_PlayerColors = { Color.black, Color.red, Color.green, Color.blue };
    static readonly double k_ReadyDisplayTimeInSeconds = 2f;
    static readonly double k_SetDisplayTimeInSeconds = 1f;
    static readonly double k_GoDisplayTimeInSeconds = 1f;

    float m_numSecondsLeftInGame = k_NumSecondsPerGame;
    public float NumSecodsLeftInGame
    {
        get { return m_numSecondsLeftInGame; }
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
        GamePlayInProgress
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

    double m_introStartTimestamp;

    protected override void OnCreate()
    {
        m_introStartTimestamp = Time.ElapsedTime;
    }

    protected override void OnDestroy()
    {
    }

    void HandleTextChange(double timeToWait, string text, GameState nextState)
    {
        var timeSinceReady = Time.ElapsedTime - m_introStartTimestamp;
        if (timeSinceReady > timeToWait)
        {
            m_introStartTimestamp = Time.ElapsedTime;
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
        HandleTextChange(k_SetDisplayTimeInSeconds, "", GameState.GamePlayInProgress);
    }

    public void ResetGame()
    {
        m_introStartTimestamp = Time.ElapsedTime;
        m_introText = "Ready";
    }

    void HandleGamePlay()
    {
        if (m_numSecondsLeftInGame > 0)
            m_numSecondsLeftInGame -= Time.DeltaTime;
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
        }
    }
}
