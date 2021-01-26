using Unity.Entities;

public struct CarMovement : IComponentData
{
    // Car offset is in 0-1 space where 0.5 means half-way down the track
    public float Offset;
    public float Lane;
    public float Velocity;
}
