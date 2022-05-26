using Unity.Entities;

public class CarConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject CarPrefab;
    public int CarCount;
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

public class CarConfigBaker : Baker<CarConfigAuthoring>
{
    public override void Bake(CarConfigAuthoring authoring)
    {
        AddComponent(new CarConfigComponent
        {
            CarPrefab = GetEntity(authoring.CarPrefab),
            CarCount = authoring.CarCount,
            CarLength = authoring.CarLength,
            CruisingSpeedMin = authoring.CruisingSpeedMin,
            CruisingSpeedMax = authoring.CruisingSpeedMax,
            OvertakeDurationMin = authoring.OvertakeDurationMin,
            OvertakeDurationMax = authoring.OvertakeDurationMax,
            OvertakeSpeedMin = authoring.OvertakeSpeedMin,
            OvertakeSpeedMax = authoring.OvertakeSpeedMax,
            OvertakeDistanceMin = authoring.OvertakeDistanceMin,
            OvertakeDistanceMax = authoring.OvertakeDistanceMax,
        });
    }
}
