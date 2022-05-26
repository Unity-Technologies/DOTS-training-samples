using Unity.Entities;
using Unity.Mathematics;

readonly partial struct DroneDepositingPlantAspect : IAspect<DroneDepositingPlantAspect>
{
    public readonly Entity Self;

    private readonly MovementAspect Movement;

    private readonly RefRO<DroneDepositPlantIntent> DroneDepositPlantIntent;

    public bool AtDesiredLocation
    {
        get => Movement.AtDesiredLocation;
    }

    public int2 DesiredLocation
    {
        set => Movement.DesiredLocation = value;
        get => Movement.DesiredLocation;
    }
    public Entity Plant
    {
        get => DroneDepositPlantIntent.ValueRO.Plant;
    }

}
