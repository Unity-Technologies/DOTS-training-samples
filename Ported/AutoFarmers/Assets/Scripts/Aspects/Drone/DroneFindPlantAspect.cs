using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


readonly partial struct DroneFindPlantAspect : IAspect<DroneFindPlantAspect>
{
    public readonly Entity Self;
    private readonly RefRW<Mover> Mover;
    private readonly RefRO<DroneFindPlantIntent> Intent;

    public int2 DesiredLocation
    {
        set => Mover.ValueRW.DesiredLocation = value;
    }

}
