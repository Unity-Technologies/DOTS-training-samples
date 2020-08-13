using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct Bot : IComponentData
{
    public Translation targetTranslation;
}

public struct BucketTosser : IComponentData { }
