using Unity.Entities;

[GenerateAuthoringComponent]
public struct Bot : IComponentData
{
    Entity nextBot;
}

[GenerateAuthoringComponent]
public struct ThrowerBot : IComponentData
{
}

[GenerateAuthoringComponent]
public struct FillerBot : IComponentData
{
}