
using Unity.Entities;

public struct FireCell : IComponentData
{
}

public struct FireTemperature : IBufferElementData
{
    public static implicit operator float(FireTemperature e) { return e.Value; }
    public static implicit operator FireTemperature(float e) { return new FireTemperature { Value = e }; }
    
    public float Value;

    public FireTemperature(float value = 0f)
    {
        Value = value;
    }
}