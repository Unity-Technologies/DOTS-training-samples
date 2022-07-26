using Unity.Entities;
using Unity.Mathematics;

// Just like we did with the turret, we create a tag component to identify the tank (cube).
struct FireFighterLine : IComponentData
{
    public int LineId;
    public float2 StartPosition;
    public float2 EndPosition;
    
}