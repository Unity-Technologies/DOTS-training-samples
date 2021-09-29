using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public enum AntState : byte
{
    Searching,
    LineOfSight,
    ReturnHome
};

[GenerateAuthoringComponent]
public struct AntMovement : IComponentData
{
    public Quaternion Target; // old

    public float2 Direction;
    public float2 Position;
    
    public float FacingAngle;
    
    public AntState State;
}