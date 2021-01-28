using Unity.Entities;

[GenerateAuthoringComponent]
public struct LinkedListLane: IComponentData
{
    public Entity Ahead;
    public Entity Behind;
}
