using Unity.Entities;

public struct PlayerHunterDelay : IComponentData
{
    public int currentDelay;
    public int maxDelay;
}
