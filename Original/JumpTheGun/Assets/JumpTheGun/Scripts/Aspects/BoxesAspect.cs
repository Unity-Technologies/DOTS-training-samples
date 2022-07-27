using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct BoxesAspect  : IAspect<BoxesAspect>
{
    public readonly Entity Self;
    readonly TransformAspect Transform;
    readonly RefRW<Boxes> boxes;

    public float3 Position
    {
        get => Transform.Position;
        set => Transform.Position = value;
    }

    public float spacing
    {
        get => boxes.ValueRO.spacing;
        set => boxes.ValueRW.spacing = value;
    }

    public float boxHeight
    {
        get => boxes.ValueRO.boxHeight;
        set => boxes.ValueRW.boxHeight = value;
    }

    public int boxHeightDamage
    {
        get => boxes.ValueRO.boxHeightDamage;
        set => boxes.ValueRW.boxHeightDamage = value;
    }

    public float top
    {
        get => boxes.ValueRO.top;
        set => boxes.ValueRW.top = value;
    }


    
}
