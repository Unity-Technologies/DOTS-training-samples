using Unity.Entities;
using Unity.Mathematics;

public struct QueuePosition : IBufferElementData
{
    public float3 Value;
    
    public static implicit operator float3(QueuePosition e)
    {
        return e.Value;
    }
    public static implicit operator QueuePosition(float3 e)
    {
        return new QueuePosition {Value = e};
    }
}