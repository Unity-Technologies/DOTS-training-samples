using Unity.Entities;
public struct CarConfigComponent : IComponentData
{
    public int CarCount;
    public Entity CarPrefab;
    public float CarLength;

    public float CruisingSpeedMin;
    public float CruisingSpeedMax;

    public float OvertakeDurationMin;
    public float OvertakeDurationMax;

    public float OvertakeSpeedMin;
    public float OvertakeSpeedMax;

    public float OvertakeDistanceMin;
    public float OvertakeDistanceMax;
}
