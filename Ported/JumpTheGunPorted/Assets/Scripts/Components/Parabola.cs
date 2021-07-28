using Unity.Entities;
using Unity.Mathematics;

// When a component doesn't require any special handling during conversion,
// the attribute GenerateAuthoringComponent can be used to automatically
// generate an authoring component with the same fields and a conversion
// function that simply copies the fields from one to the other.
[GenerateAuthoringComponent]
public struct Parabola : IComponentData
{
    public float StartY;
    public float Height;
    public float EndY;

    public float A;
    public float B;
    public float C;

    public float Duration;
    public float3 Forward;
}