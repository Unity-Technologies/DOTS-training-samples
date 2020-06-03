using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantData : MonoBehaviour
{
    public static ConstantData Instance;
    
    public float[] Speed = {1f, 1f};
    public float[] Radius = {1f, 1f};
    
    public float RoundLength = 60f;
    public Vector2Int BoardDimensions = new Vector2Int(30, 30);
    public Vector2 CellSize = new Vector2(1f, 1f);

    public float ArrowLifeTime = 30f;
    public int MaxArrows = 3;

    // assume player 0 is human, others AI
    public int NumPlayers = 4;

    public void Awake()
    {
        Instance = this;
    }
}

