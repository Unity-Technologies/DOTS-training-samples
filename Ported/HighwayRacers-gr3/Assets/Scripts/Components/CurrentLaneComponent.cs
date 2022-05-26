using Unity.Entities;


[GenerateAuthoringComponent]
public struct CurrentLaneComponent : IComponentData
{
    public Entity self;
    public int CurrentLaneNumber;
}
