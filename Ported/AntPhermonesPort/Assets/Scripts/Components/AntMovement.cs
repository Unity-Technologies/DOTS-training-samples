using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public enum AntState : byte
{
    Searching,
    LineOfSight,
    ReturnHome
};

[GenerateAuthoringComponent]
public struct AntMovement : IComponentData
{
    public float2 Direction;
    public float2 Position;
    public AntState State;
}