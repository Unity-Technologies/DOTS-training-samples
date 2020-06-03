using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum CharacterType
{
    CAT,
    MOUSE
}

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
}

/*[GenerateAuthoringComponent]
public struct Position2D : IComponentData
{
    public float2 Value;
}

public enum GridDirection
{
    NORTH = (1<<0),
    EAST = (1<<1),
    SOUTH = (1<<2),
    WEST = (1<<3)
}

// Contents of CellInfo
// - bottom 4 bits GridDirection
// - is hole -> 1 bit
// - is base -> 1 bit
// - above that is owning player id for bases -> which player owns a base

//[GenerateAuthoringComponent]
public struct Direction2D : IComponentData
{
    public GridDirection Value;
}

//[GenerateAuthoringComponent]
public struct Rotation2D : IComponentData
{
    public float Value;
}*/

//[GenerateAuthoringComponent]
public struct SpawnerInfo : IComponentData
{
    public Entity Prefab;
    public float Frequency;
}

//[GenerateAuthoringComponent]
public struct SpawnerInstance : IComponentData
{
    public float Time;
}

//[GenerateAuthoringComponent]
public struct MouseTag : IComponentData
{
}

//[GenerateAuthoringComponent]
public struct CatTag : IComponentData
{
}

public struct ReachedBase : IComponentData
{
    public int PlayerID;
}

public struct ArrowRequest : IComponentData
{
    // TBC with ArrowSystem
}