using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

readonly partial struct FarmerFindUntilledGroundAspect : IAspect<FarmerFindUntilledGroundAspect>
{
    public readonly Entity Self;
    private readonly MovementAspect Movement;

    public bool AtDesiredLocation
    {
        get => Movement.AtDesiredLocation;
    }

    public int2 DesiredLocation
    {
        set => Movement.DesiredLocation = value;
    }
}
