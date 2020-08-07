using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Offset : IComponentData
{
    private byte Value;

    private static readonly float kOffset = byte.MaxValue / 2;

    public void SetOffset(float value)
    {
        var n = (value + 1.0f) * 0.5f;
        Value = (byte)(n * byte.MaxValue);
    }

    public static implicit operator float(Offset d)
    {
        var v = (float)d.Value;
        v = (v - kOffset) / kOffset;
        return v;
    }
}
