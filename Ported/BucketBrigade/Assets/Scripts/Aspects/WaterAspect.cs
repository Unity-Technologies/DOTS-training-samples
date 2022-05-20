using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct WaterAspect : IAspect<WaterAspect>
{
    public readonly Entity Self;

    private readonly RefRW<Water> Water;

    private readonly TransformAspect Transform;

    public float WaterLevel
    {
        get => Water.ValueRW.WaterLevel;
        set => Water.ValueRW.WaterLevel = value;
    }

    public float3 Scale
    {
        get => Water.ValueRW.Scale;
        set => Water.ValueRW.Scale = value;
    }

    public float3 Position
    {
        get => Transform.Position;
        set => Transform.Position = value;
    }

}

