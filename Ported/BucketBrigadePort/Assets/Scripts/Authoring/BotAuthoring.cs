using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct Bot : IComponentData
{
    public Translation targetTranslation;
}

public struct LineId : IComponentData
{
    public int Value;
}

public struct BucketTosser : IComponentData
{
    public Entity BucketFiller;
}

public struct BucketFiller : IComponentData { }
