using Unity.Entities;

[GenerateAuthoringComponent]
public struct TargetCandidates : IComponentData
{
    public Entity TeamRed;
    public Entity TeamBlue;
    public Entity Food;
}

