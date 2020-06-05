using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    CAT,
    MOUSE
}

public class ConstantData : MonoBehaviour
{
    public static ConstantData Instance;

    public float[] Radius = { 1f, 1f };

    public float FallingSpeed = 2f;
    public float FallingKillY = -5f;

    public float RotationSpeed = 5f;

    public float RoundLength = 60f;
    public Vector2Int BoardDimensions = new Vector2Int(30, 30);
    public Vector2 CellSize = new Vector2(1f, 1f);
    
    public Vector2Int UICanvasDimensions = new Vector2Int(1024, 800);

    public float ArrowLifeTime = 30f;
    public int MaxArrows = 3;

    // assume player 0 is human, others AI
    public int NumPlayers = 4;

    public Color[] PlayerColors = new Color[4]
    {
        new Color(0x00 / 255.0f,0x1E / 255.0f,0xFF / 255.0f), 
        new Color(0xFF / 255.0f,0x00 / 255.0f,0x00 / 255.0f), 
        new Color(0x00 / 255.0f,0xFF / 255.0f,0x06 / 255.0f), 
        new Color(0xFF / 255.0f,0x00 / 255.0f,0xCF / 255.0f)
    };

    public int MouseScoreAddition = 1;
    public float CatScoreMultiplier = 0.66f;

    public float SpawnerFrequencyMultiplier = 1f;

    public bool CatsKillMice = true;

    public void Awake()
    {
        Instance = this;
        Debug.Log("constant data available");
    }
}

