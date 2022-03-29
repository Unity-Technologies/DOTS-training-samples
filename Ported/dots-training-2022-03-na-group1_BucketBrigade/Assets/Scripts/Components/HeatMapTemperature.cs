using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(256)]
public struct HeatMapTemperature : IBufferElementData
{
    public static implicit operator float(HeatMapTemperature e) { return e.value; }
    public static implicit operator HeatMapTemperature(float e) { return new HeatMapTemperature { value = e }; }

    public float value;
}