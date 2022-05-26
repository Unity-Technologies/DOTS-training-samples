using Unity.Entities;
using Unity.Mathematics;

struct  Mover : IComponentData
{
    public bool HasDestination;
    
    public int2 DesiredLocation;

    public float Speed;

    public float YOffset;
}
