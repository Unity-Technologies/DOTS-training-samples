using Unity.Entities;

[GenerateAuthoringComponent]
public struct HasBucket : IComponentData
{
    public bool PickingUpBucket;
    public bool PickedUp;
    public Entity Entity;
}