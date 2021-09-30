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
    public float FacingAngle;
    public float AntSpeed;

    public AntState State;
}