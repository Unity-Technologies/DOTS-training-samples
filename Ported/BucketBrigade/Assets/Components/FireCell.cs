using Unity.Entities;

[GenerateAuthoringComponent]
public struct FireCell : IComponentData
{
    public float Temperature;
}

public struct OnFire : IComponentData
{
}


