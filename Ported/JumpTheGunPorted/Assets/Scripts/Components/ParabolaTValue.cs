using Unity.Entities;

// When a component doesn't require any special handling during conversion,
// the attribute GenerateAuthoringComponent can be used to automatically
// generate an authoring component with the same fields and a conversion
// function that simply copies the fields from one to the other.
[GenerateAuthoringComponent]
public struct ParabolaTValue : IComponentData
{
    float value;
}