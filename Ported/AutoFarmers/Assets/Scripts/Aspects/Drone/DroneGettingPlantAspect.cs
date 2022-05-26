using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

readonly partial struct DroneGettingPlantAspect : IAspect<DroneGettingPlantAspect>
{
    public readonly Entity Self;

    private readonly MovementAspect Movement;


    private readonly RefRW<DroneAquirePlantIntent> DroneAquirePlantIntent;

    public bool AtDesiredLocation
    {
        get => Movement.AtDesiredLocation;
    }

    public int2 DesiredLocation
    {
        set => Movement.DesiredLocation = value;
    }

    public Entity Plant
    {
        get => DroneAquirePlantIntent.ValueRO.Plant;
        set => DroneAquirePlantIntent.ValueRW.Plant = value;
    }

    public bool HasPlant
    {
        get => DroneAquirePlantIntent.ValueRO.Plant != Entity.Null;
    }


}
