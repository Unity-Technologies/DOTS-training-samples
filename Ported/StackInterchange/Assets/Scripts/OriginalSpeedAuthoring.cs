using Unity.Entities;
 
[GenerateAuthoringComponent]
public struct OriginalSpeed : IComponentData
{
    public byte Value;
    private static readonly int kMaxValue = 255;

    public void SetSpeed(float value)
    {
        Value = (byte)(value * (float)kMaxValue);
    }

    public float GetSpeed()
    {
        var t = Value / (float)kMaxValue;
        return 1.0f + t;
    }
}
