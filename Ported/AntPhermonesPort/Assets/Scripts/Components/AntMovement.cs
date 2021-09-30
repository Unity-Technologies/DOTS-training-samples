using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public enum AntState : byte
{
    Searching,          // Can't see food yet
    LineOfSightToFood,  // Can see the food
    ReturnToNest,
    ReturnToNestWithLineOfSight
};

[GenerateAuthoringComponent]
public struct AntMovement : IComponentData
{
    public float FacingAngle;
    public float AntSpeed;

    public AntState State;
}