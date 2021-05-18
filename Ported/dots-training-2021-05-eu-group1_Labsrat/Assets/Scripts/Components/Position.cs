using Unity.Entities;
using Unity.Mathematics;

public struct Position : IComponentData
{
    public Position(int x, int y)
    {
        Value = new float2((float)x, (float)y);
    }
    
    public float2 Value;
}
