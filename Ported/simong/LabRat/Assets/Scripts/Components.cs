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
public struct MouseTag : IComponentData
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