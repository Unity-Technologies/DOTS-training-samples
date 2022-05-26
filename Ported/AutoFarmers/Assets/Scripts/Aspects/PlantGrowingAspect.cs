using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

readonly partial struct PlantGrowingAspect : IAspect<PlantGrowingAspect>
{
    public readonly Entity Self;

    private readonly RefRW<PlantHealth> PlantHealth;

    private readonly RefRW<Scale> Scale;

    public float Health
    {
        get => PlantHealth.ValueRO.Health;
        set {
            var clamped = math.clamp(value, 0, 1);
            PlantHealth.ValueRW.Health = clamped;
            Scale.ValueRW.Value = clamped * 0.6f;
        }
    }

    public bool GrowingComplete
    {
        get => PlantHealth.ValueRO.Health == 1f;
    }
}
