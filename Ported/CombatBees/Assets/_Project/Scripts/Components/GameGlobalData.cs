using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Serialization;

[Serializable]
[GenerateAuthoringComponent]
public struct GameGlobalData : IComponentData
{
    public Entity BeePrefab;
    public Entity ResourcePrefab;
    public Entity LevelCubePrefab;
    
    public UnityEngine.Color FloorColor;
    public UnityEngine.Color TeamAColor;
    public UnityEngine.Color TeamBColor;
    
    public float Gravity;
    public float GridCellSize;
    public float ResourceHeight;
    public float ResourceSnapTime;
    public float ResourceCarryStiffness;
    public float ResourceCarryOffset;
    public int BeeSpawnCountOnScore;
}
