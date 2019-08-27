using Unity.Entities;

public struct LbCatMap : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator int(LbCatMap e) { return e.Value; }
    public static implicit operator LbCatMap(int e) { return new LbCatMap { Value = e }; }

    public int Value;
}