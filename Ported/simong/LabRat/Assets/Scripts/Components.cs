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

public struct ReachedBase : IComponentData
{
    public int PlayerID;
}

public struct ArrowRequest : IComponentData
{
    // TBC with ArrowSystem
}