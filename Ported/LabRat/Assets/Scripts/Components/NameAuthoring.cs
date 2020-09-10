using Unity.Collections;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Name : IComponentData
{
    public FixedString32 Value;
}
