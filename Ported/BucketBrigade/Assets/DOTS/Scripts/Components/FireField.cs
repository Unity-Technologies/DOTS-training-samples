using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct FireField : IComponentData
{
}

public struct FireHeat : IBufferElementData
{
    public static implicit operator float(FireHeat e) { return e.Value; }
    public static implicit operator FireHeat(float e) { return new FireHeat { Value = e }; }

    public float Value;
}
