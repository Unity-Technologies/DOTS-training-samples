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

    public float RotationSpeed = 0.5f;

    public float RoundLength = 60f;
    public Vector2Int BoardDimensions = new Vector2Int(30, 30);
    public Vector2 CellSize = new Vector2(1f, 1f);

    public float ArrowLifeTime = 30f;
    public int MaxArrows = 3;

    // assume player 0 is human, others AI
    public int NumPlayers = 4;

    public Color[] PlayerColors = new Color[4]
    {
        new Color(0x00,0x1E,0xFF), 
        new Color(0xFF,0x00,0x00), 
        new Color(0x00,0xFF,0x06), 
        new Color(0xFF,0x00,0xCF)
    };

    public int MouseScoreAddition = 1;
    public float CatScoreMultiplier = 0.66f;
    
    public void Awake()
    {
        Instance = this;
        Debug.Log("constant data available");
    }
}

