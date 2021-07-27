using Unity.Entities;
using Unity.Mathematics;

// When a component doesn't require any special handling during conversion,
// the attribute GenerateAuthoringComponent can be used to automatically
// generate an authoring component with the same fields and a conversion
// function that simply copies the fields from one to the other.
[GenerateAuthoringComponent]
public struct Parabola : IComponentData
{
    float startTime;
    float heightStart;
    float heightMax;

    // TODO: don't we need these to calculate position per frame?
    float2 startPosition;
    float2 endPosition;
}