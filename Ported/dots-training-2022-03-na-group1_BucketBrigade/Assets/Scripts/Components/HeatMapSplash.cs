using Unity.Entities;

[InternalBufferCapacity(128)]
public struct HeatMapSplash : IBufferElementData
{
    public static implicit operator int(HeatMapSplash e) { return e.value; }
    public static implicit operator HeatMapSplash(int e) { return new HeatMapSplash { value = e }; }

    public int value;
}