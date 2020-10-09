using Unity.Entities;

[GenerateAuthoringComponent]
public struct CarriageDoors : IComponentData
{
    public Entity DoorL_Geo;
    public Entity DoorL_Closed;
    public Entity DoorL_Open;
    public Entity DoorR_Geo;
    public Entity DoorR_Closed;
    public Entity DoorR_Open;
}
