using Unity.Entities;

public struct NextPlatformPosition : IComponentData
{
    public float Value;
    
    public static implicit operator float(NextPlatformPosition v) => v.Value;
    public static implicit operator NextPlatformPosition(float v) => new NextPlatformPosition { Value = v };
}
