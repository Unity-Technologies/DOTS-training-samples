using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class GameManagementSystem : ComponentSystem
{
    static readonly float k_NumSecondsPerGame = 30f;
    static readonly Color[] k_PlayerColors = { Color.black, Color.red, Color.green, Color.blue };

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

    protected override void OnCreate()
    {
        
    }

    protected override void OnDestroy()
    {
    }

    protected override void OnUpdate()
    {
        if (m_numSecondsLeftInGame > 0)
            m_numSecondsLeftInGame -= Time.DeltaTime;
    }
}
