using Unity.Entities;

public struct PlayerOwned : IComponentData
{
    public Entity Owner;
}
