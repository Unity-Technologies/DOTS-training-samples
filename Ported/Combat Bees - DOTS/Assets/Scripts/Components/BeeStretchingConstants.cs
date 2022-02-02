using Unity.Entities;

public struct BeeStretchingConstants : IComponentData
{
    public float SpeedStretchSensitivity;
    public float MinStretch;
    public float MaxStretch;
    public float XStretchMultiplier;
    public float YStretchMultiplier;
}
