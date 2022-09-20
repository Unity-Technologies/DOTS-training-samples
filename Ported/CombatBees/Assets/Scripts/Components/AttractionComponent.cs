using Unity.Entities;

public struct AttractionComponent : IComponentData {
    public Entity attracted;
    public Entity repelled;
}