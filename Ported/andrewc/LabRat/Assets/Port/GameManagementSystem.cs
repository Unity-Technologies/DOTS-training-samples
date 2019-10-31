using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using UnityEngine.SceneManagement;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class GameManagementSystem : ComponentSystem
{
    static readonly float k_NumSecondsPerGame = 30f;
    static readonly int k_NumPlayers = 4;
    static readonly Color[] k_PlayerColors = { Color.black, Color.red, Color.green, Color.blue };
    static readonly double k_ReadyDisplayTimeInSeconds = 2f;
    static readonly double k_SetDisplayTimeInSeconds = 1f;
    static readonly double k_GoDisplayTimeInSeconds = 1f;
    static readonly double k_GameOverWaitTimeInSeconds = 4f;
    static readonly float k_OverheadFactor = 1.5f;

    float m_numSecondsLeftInGame = k_NumSecondsPerGame;
    public float NumSecondsLeftInGame
    {
        get { return m_numSecondsLeftInGame; }
    }

    public int NumPlayers
    {
        get => k_NumPlayers;
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

    Vector3[] m_playerCursorScreenPos = new Vector3[k_NumPlayers];
    public Vector3 GetPlayerCursorScreenPos(uint playerIndex)
    {
        return m_playerCursorScreenPos[playerIndex];
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
    bool m_isCameraInitialized;
    BoardSystem m_boardSystem;
    Board m_board;

    protected override void OnCreate()
    {
        m_boardSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BoardSystem>();
        m_board = m_boardSystem.Board;

        ResetGame();
    }

    void UpdateCameraIfNeeded()
    {
        if (m_isCameraInitialized)
            return;

        Debug.Log("UpdateCamera");
        var cam = Camera.main;
        cam.orthographic = true;

        float maxSize = Mathf.Max(m_board.Size.x, m_board.Size.y);

        float maxCellSize = Mathf.Max(m_board.CellSize.x, m_board.CellSize.y);

        cam.orthographicSize = maxSize * maxCellSize * .65f;
        var posXZ = Vector2.Scale(new Vector2(m_board.Size.x, m_board.Size.y) * 0.5f, m_board.CellSize);

        cam.transform.position = new Vector3(0, maxSize * maxCellSize * k_OverheadFactor, 0);
        cam.transform.LookAt(new Vector3(posXZ.x, 0f, posXZ.y));

        m_isCameraInitialized = true;
        Debug.Log("UpdateCamera DONE");
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

        UpdateCursors();
    }

    void UpdateCursors()
    {
        m_playerCursorScreenPos[0] = Input.mousePosition;

        // TMP: 
        //m_playerCursorScreenPos[1] = new Vector3(UnityEngine.Random.Range(0f, Screen.width), UnityEngine.Random.Range(0f, Screen.height), UnityEngine.Random.Range(0f, 1f));
        //m_playerCursorScreenPos[2] = new Vector3(UnityEngine.Random.Range(0f, Screen.width), UnityEngine.Random.Range(0f, Screen.height), UnityEngine.Random.Range(0f, 1f));
        //m_playerCursorScreenPos[3] = new Vector3(UnityEngine.Random.Range(0f, Screen.width), UnityEngine.Random.Range(0f, Screen.height), UnityEngine.Random.Range(0f, 1f));
        // TMP
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
        UpdateCameraIfNeeded();
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
